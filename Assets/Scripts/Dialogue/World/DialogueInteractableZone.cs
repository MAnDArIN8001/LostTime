using System;
using Dialogue.Authoring;
using Dialogue.Runtime;
using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.Events;
using Loot.Systems;

namespace Dialogue.World
{
    public sealed class DialogueInteractableZone : MonoBehaviour, IInteractable
    {
        [SerializeField] private string _interactionPrompt = "Talk";
        [SerializeField] private bool _singleUse;
        [SerializeField] private bool _consumeOnCompleteOnly = true;
        [SerializeField] private Transform _graphicsTarget;
        [SerializeField] private CinemachineCamera _dialogueCamera;
        [SerializeField] private DialogueDefinition _dialogueDefinition;
        [SerializeField] private DialogueOrchestrator _orchestrator;
        [SerializeField] private UnityEvent _onStartRejected;

        private bool _isConsumed;
        private bool _hasDialogueCameraCache;
        private PrioritySettings _cachedDialogueCameraPriority;
        private Transform _cachedDialogueCameraFollow;
        private Transform _cachedDialogueCameraLookAt;

        public string InteractionPrompt => _interactionPrompt;
        public bool CanInteract => !_singleUse || !_isConsumed;
        public Transform GraphicsTarget => _graphicsTarget != null ? _graphicsTarget : transform;
        public DialogueDefinition Definition => _dialogueDefinition;
        public bool ConsumeOnCompleteOnly => _consumeOnCompleteOnly;

        public event Action<IInteractable, GameObject> Interacted;
        public event Action<DialogueInteractableZone, GameObject> StartRequested;

        public void Interact(GameObject interactor)
        {
            if (!CanInteract)
            {
                return;
            }

            Interacted?.Invoke(this, interactor);
            StartRequested?.Invoke(this, interactor);
            if (_orchestrator != null && !_orchestrator.TryStart(this))
            {
                NotifyStartRejected();
            }
        }

        public void MarkConsumed()
        {
            _isConsumed = true;
        }

        public void NotifyStartRejected()
        {
            _onStartRejected?.Invoke();
        }

        public bool TryActivateCamera()
        {
            if (_dialogueCamera == null)
            {
                return false;
            }

            _cachedDialogueCameraPriority = _dialogueCamera.Priority;
            _cachedDialogueCameraFollow = _dialogueCamera.Follow;
            _cachedDialogueCameraLookAt = _dialogueCamera.LookAt;
            _hasDialogueCameraCache = true;

            var priority = _dialogueCamera.Priority;
            priority.Enabled = true;
            priority.Value = Mathf.Max(1, _cachedDialogueCameraPriority.Value + 1);
            _dialogueCamera.Priority = priority;
            return true;
        }

        public void RestoreCamera()
        {
            if (!_hasDialogueCameraCache || _dialogueCamera == null)
            {
                return;
            }

            _dialogueCamera.Priority = _cachedDialogueCameraPriority;
            _dialogueCamera.Follow = _cachedDialogueCameraFollow;
            _dialogueCamera.LookAt = _cachedDialogueCameraLookAt;

            _cachedDialogueCameraFollow = null;
            _cachedDialogueCameraLookAt = null;
            _hasDialogueCameraCache = false;
        }
    }
}
