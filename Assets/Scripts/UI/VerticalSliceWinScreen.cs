using Quest;
using UI.Runtime;
using UnityEngine;
using Zenject;

namespace UI
{
    public sealed class VerticalSliceWinScreen : MonoBehaviour
    {
        [SerializeField] private VerticalSliceQuestProgression _questProgression;
        [InjectOptional] private IUIService _uiService;

        private bool _shown;

        private void OnEnable()
        {
            if (_questProgression == null)
            {
                return;
            }

            _questProgression.Completed += OnQuestFinished;
            _questProgression.QuestCompleted += OnQuestFinished;

            if (_questProgression.CurrentStep == VerticalSliceQuestProgression.QuestStep.Completed)
            {
                Show();
            }
        }

        private void OnDisable()
        {
            if (_questProgression == null)
            {
                return;
            }

            _questProgression.Completed -= OnQuestFinished;
            _questProgression.QuestCompleted -= OnQuestFinished;
        }

        private void OnQuestFinished()
        {
            Show();
        }

        private void Show()
        {
            if (_shown)
            {
                return;
            }

            _shown = true;
            if (_uiService == null)
            {
                Debug.LogWarning("[VerticalSliceWinScreen] IUIService is not available.");
                return;
            }

            _uiService.Open<VerticalSliceWinPanel>();
        }
    }
}
