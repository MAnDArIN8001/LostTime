using UnityEngine;

namespace Character.Modules.Animation
{
    public class AnimationModule : MonoBehaviour
    {
        private bool _isRunning;
        
        [Header("Animation Keys")] 
        [SerializeField] private string _inputKey;

        [Header("Animation Params")] 
        [SerializeField] private float _movementModifier;
        [SerializeField] private float _runModifier;

        private float _currentModifier = 1;

        [SerializeField] private float _smoothStep;
        
        [Space, SerializeField] protected Animator _animator;
        
        public void SetMovement(float input)
        {
            _currentModifier =
                Mathf.SmoothStep(_currentModifier, _isRunning ? _runModifier : _movementModifier, _smoothStep);
            
            _animator.SetFloat(_inputKey, input * _currentModifier);
        }

        public void SetRunning(bool isRunning)
        {
            _isRunning = isRunning;
        }
    }
}