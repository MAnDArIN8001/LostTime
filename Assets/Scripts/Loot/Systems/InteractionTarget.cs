using System;
using Gameplay.Interaction.Core;
using UnityEngine;
using UnityEngine.Events;

namespace Loot.Systems
{
    public class InteractionTarget : MonoBehaviour, IInteractable
    {
        [SerializeField] private string _interactionPrompt = "Interact";
        [SerializeField] private bool _singleUse = true;
        [SerializeField] private bool _consumeOnInteract = true;
        [SerializeField] private UnityEvent _onInteracted;

        private bool _isConsumed;

        public string InteractionPrompt => _interactionPrompt;
        public bool CanInteract => !_singleUse || !_isConsumed;

        public event Action<IInteractable, GameObject> Interacted;

        public void ResetInteractionState()
        {
            _isConsumed = false;
        }

        public void Interact(GameObject interactor)
        {
            if (!CanInteract)
            {
                InteractionDebugLog.Log(this, $"InteractionTarget '{name}' rejected interaction because it is consumed.");
                return;
            }

            if (_consumeOnInteract)
            {
                _isConsumed = true;
            }

            InteractionDebugLog.Log(this, $"InteractionTarget '{name}' invoked by '{(interactor != null ? interactor.name : "null")}'. singleUse={_singleUse}, consumeOnInteract={_consumeOnInteract}.");
            _onInteracted?.Invoke();
            Interacted?.Invoke(this, interactor);
        }
    }
}
