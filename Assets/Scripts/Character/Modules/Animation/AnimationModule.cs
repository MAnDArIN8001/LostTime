using UnityEngine;
using Utils.Events;
using Character.Modules.Animation.Data;
using Character.Modules.Animation.Events;

namespace Character.Modules.Animation
{
    public class AnimationModule : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] protected Animator _animator;

        [Space, SerializeField] private AnimationParamsDataBase _animationParamsDataBase;

        private EventBus _animationEventBus;

        public void Initialize(EventBus animationEventBus)
        {
            _animationEventBus = animationEventBus;
            
            _animationEventBus.Subscribe<AnimationParamEvent>(HandleAnimationParamEvent);
        }

        private void OnDestroy()
        {
            _animationEventBus.Unsubscribe<AnimationParamEvent>(HandleAnimationParamEvent);
        }

        private void HandleAnimationParamEvent(AnimationParamEvent animationParamEvent)
        {
            if (!_animationParamsDataBase.AnimationParamSetups.TryGetValue(animationParamEvent.AnimationParamId, out var animationParamSetup))
            {
                Debug.LogWarning($"Warning: The Animation Params Data Base {_animationParamsDataBase} doesn't contains any param with id:{animationParamEvent.AnimationParamId}");
                
                return;
            }
            
            var paramType = animationParamSetup.Type;

            switch (paramType)
            {
                case ParamType.Bool:
                    _animator.SetBool(animationParamSetup.Name, (bool)animationParamEvent.Value);
                    break;
                
                case ParamType.Float:
                    _animator.SetFloat(animationParamSetup.Name, (float)animationParamEvent.Value);
                    break;
                
                case ParamType.Int:
                    _animator.SetInteger(animationParamSetup.Name, (int)animationParamEvent.Value);
                    break;
                
                case ParamType.Trigger:
                    _animator.SetTrigger(animationParamSetup.Name);
                    break;
                
                default:
                    Debug.LogWarning($"Warning: Strange ParamType: {paramType}");
                    break;
            }
        }
    }
}