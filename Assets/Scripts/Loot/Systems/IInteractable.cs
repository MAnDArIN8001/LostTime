using System;
using UnityEngine;

namespace Loot.Systems
{
    public interface IInteractable
    {
        public string InteractionPrompt { get; }
        public bool CanInteract { get; }

        public event Action<IInteractable, GameObject> Interacted;

        public void Interact(GameObject interactor);
    }
}
