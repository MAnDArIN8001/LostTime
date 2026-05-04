using Character;
using Dialogue.Authoring;
using Dialogue.Core;
using Dialogue.UI;
using Dialogue.World;
using Quest;
using UI.Runtime;
using UnityEngine;
using Utils.Events;
using Zenject;

namespace Dialogue.Runtime
{
    public sealed class DialogueOrchestrator : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private Character.Character _character;
        [SerializeField] private VerticalSliceQuestProgression _questProgression;
        [SerializeField] private string _historyPlayerPrefsKey = "dialogue.history.v1";
        [SerializeField] private bool _usePersistentHistory = true;

        private IUIService _uiService;
        private EventBus _eventBus;
        private IDialogueHistoryRepository _historyRepository;
        private DialogueHistory _history;
        private DialogueSession _session;
        private DialogueInteractableZone _activeZone;
        private bool _isDialogueOpen;
        private DialoguePresenter _presenter;

        private readonly QuestStateQueryAdapter _questQuery = new();

        public bool IsDialogueActive => _session != null && _session.Status == DialogueStatus.Running;

        private void Awake()
        {
            _historyRepository = _usePersistentHistory
                ? new PlayerPrefsDialogueHistoryRepository(_historyPlayerPrefsKey)
                : new InMemoryDialogueHistoryRepository();
            _history = _historyRepository.Load();

            _questQuery.Bind(_questProgression);
        }

        [Inject]
        public void Construct(IUIService uiService)
        {
            _uiService = uiService;
        }

        private void Start()
        {
            if (SceneEventBusProvider.TryGetEventBus(out var eventBus))
            {
                _eventBus = eventBus;
            }
        }

        public bool TryStart(DialogueInteractableZone zone)
        {
            if (zone == null || IsDialogueActive || _uiService == null)
            {
                return false;
            }

            var definitionAsset = zone.Definition;
            if (definitionAsset == null)
            {
                zone.NotifyStartRejected();
                return false;
            }

            var definition = definitionAsset.ToData();
            var session = new DialogueSession(definition, _history, _questQuery);
            if (!session.Start())
            {
                zone.NotifyStartRejected();
                return false;
            }

            _activeZone = zone;
            _session = session;
            _session.StepShown += OnStepShown;
            _session.OptionSelected += OnOptionSelected;
            _session.StepCompleted += OnStepCompleted;
            _session.Completed += OnCompleted;
            _session.Broken += OnBroken;

            zone.TryActivateCamera();

            _character?.EnterCommunicationState();
            var panel = OpenDialoguePanel();
            _presenter = panel != null ? new DialoguePresenter(_session, panel) : null;

            _eventBus?.Publish(new DialogueStartedEvent(definition.DialogueId, session.CurrentNode?.NodeId ?? string.Empty));
            OnStepShown(session.CurrentNode);
            return true;
        }

        public void BreakActiveDialogue()
        {
            _session?.Break();
        }

        private void OnStepShown(DialogueNodeData node)
        {
            if (node == null || _session == null)
            {
                return;
            }

            var panel = OpenDialoguePanel();
            if (panel == null)
            {
                return;
            }

            _presenter ??= new DialoguePresenter(_session, panel);
            _presenter.Render(node);
            _eventBus?.Publish(new DialogueStepShownEvent(_session.DialogueId, node.NodeId));
        }

        private void OnOptionSelected(string nodeId, string optionId)
        {
            if (_session == null)
            {
                return;
            }

            _historyRepository.Save(_history);
            _eventBus?.Publish(new DialogueOptionSelectedEvent(_session.DialogueId, nodeId, optionId));
        }

        private void OnStepCompleted(string nodeId)
        {
            if (_session == null || string.IsNullOrWhiteSpace(nodeId))
            {
                return;
            }

            _eventBus?.Publish(new DialogueStepCompletedEvent(_session.DialogueId, nodeId));
        }

        private void OnCompleted()
        {
            if (_activeZone != null && _activeZone.ConsumeOnCompleteOnly)
            {
                _activeZone.MarkConsumed();
            }

            if (_session != null)
            {
                _eventBus?.Publish(new DialogueCompletedEvent(_session.DialogueId));
            }

            CleanupSession();
        }

        private void OnBroken()
        {
            if (_session != null)
            {
                _eventBus?.Publish(new DialogueBreakEvent(_session.DialogueId));
            }

            CleanupSession();
        }

        private void CleanupSession()
        {
            if (_session != null)
            {
                _session.StepShown -= OnStepShown;
                _session.OptionSelected -= OnOptionSelected;
                _session.StepCompleted -= OnStepCompleted;
                _session.Completed -= OnCompleted;
                _session.Broken -= OnBroken;
            }

            _historyRepository.Save(_history);
            _session = null;
            _presenter = null;
            CloseDialoguePanel();
            _character?.ExitCommunicationState();

            _activeZone?.RestoreCamera();
            _activeZone = null;
        }

        private DialoguePanel OpenDialoguePanel()
        {
            var panel = _uiService.Open<DialoguePanel>();
            if (panel == null)
            {
                return null;
            }

            if (_isDialogueOpen)
            {
                return panel;
            }

            panel.ContinueRequested += OnContinueRequested;
            panel.OptionRequested += OnOptionRequested;
            panel.CancelRequested += OnCancelRequested;
            _isDialogueOpen = true;
            return panel;
        }

        private void CloseDialoguePanel()
        {
            if (_uiService == null || !_isDialogueOpen)
            {
                return;
            }

            if (_uiService.TryGet<DialoguePanel>(out var panel) && panel != null)
            {
                panel.ContinueRequested -= OnContinueRequested;
                panel.OptionRequested -= OnOptionRequested;
                panel.CancelRequested -= OnCancelRequested;
            }

            _uiService.Close<DialoguePanel>(UIPanelCloseReason.Service);
            _isDialogueOpen = false;
        }

        private void OnContinueRequested()
        {
            _session?.Continue();
        }

        private void OnOptionRequested(string optionId)
        {
            if (string.IsNullOrWhiteSpace(optionId))
            {
                return;
            }

            _session?.SelectOption(optionId);
        }

        private void OnCancelRequested()
        {
            BreakActiveDialogue();
        }

        private sealed class QuestStateQueryAdapter : IQuestStateQuery
        {
            private VerticalSliceQuestProgression _questProgression;

            public void Bind(VerticalSliceQuestProgression questProgression)
            {
                _questProgression = questProgression;
            }

            public bool IsQuestCompleted(string questId)
            {
                if (_questProgression == null)
                {
                    return false;
                }

                return _questProgression.CurrentStep == VerticalSliceQuestProgression.QuestStep.Completed;
            }
        }
    }
}
