using System;
using Gameplay.Interaction.Authoring;
using Gameplay.Interaction.Core;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.Interaction.World
{
    public class PressableWorldObject : MonoBehaviour, IPressable
    {
        [SerializeField] private string _pressPrompt = "Press";
        [SerializeField] private bool _singleUse = true;
        [SerializeField] private bool _consumeOnPress = true;
        [SerializeField] private bool _requirePointerTarget = false;
        [SerializeField] private UnityEvent _onPressed;

        private bool _isConsumed;

        public string PressPrompt => _pressPrompt;

        public bool IsConsumed => _isConsumed;

        public event Action<IPressable, GameObject, PointerTargetContext> Pressed;

        public void ResetPressState()
        {
            _isConsumed = false;
        }

        public bool CanPress(GameObject interactor, in PointerTargetContext pointerContext)
        {
            if (interactor == null)
            {
                return false;
            }

            if (_singleUse && _isConsumed)
            {
                return false;
            }

            if (_requirePointerTarget && !pointerContext.HasTarget)
            {
                return false;
            }

            return true;
        }

        public void Press(GameObject interactor, in PointerTargetContext pointerContext)
        {
            if (!CanPress(interactor, pointerContext))
            {
                InteractionDebugLog.Log(this, $"Pressable '{name}' rejected press. interactor='{(interactor != null ? interactor.name : "null")}', hasPointerTarget={pointerContext.HasTarget}, consumed={_isConsumed}.");
                return;
            }

            if (_consumeOnPress)
            {
                _isConsumed = true;
            }

            InteractionDebugLog.Log(this, $"Pressable '{name}' pressed by '{interactor.name}'. hitPoint={pointerContext.HitPoint}, consumed={_isConsumed}.");
            _onPressed?.Invoke();
            Pressed?.Invoke(this, interactor, pointerContext);
        }

        private void OnValidate()
        {
            _pressPrompt = InteractionAuthoringGuards.NormalizePrompt(_pressPrompt, "Press");

            if (!_singleUse && _consumeOnPress)
            {
                _consumeOnPress = false;
#if UNITY_EDITOR
                Debug.LogWarning(
                    $"{nameof(PressableWorldObject)}: consume-on-press disabled because single-use is turned off.",
                    this);
#endif
            }

            if (_requirePointerTarget && !InteractionAuthoringGuards.HasPointerColliderBinding(transform))
            {
                InteractionAuthoringGuards.WarnMissingPointerBinding(this, "Require Pointer Target");
            }
        }
    }
}
