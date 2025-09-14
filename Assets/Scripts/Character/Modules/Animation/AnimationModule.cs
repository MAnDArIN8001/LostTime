using UnityEngine;

namespace Character.Modules.Animation
{
    public class AnimationModule : MonoBehaviour
    {
        [Header("Animation Keys")] 
        [SerializeField] private string _inputKey;
        [SerializeField] private string _runKey;
        
        [Space, SerializeField] protected Animator _animator;

        public void PlayIdle()
        {
            
        }

        public void SetRunState(bool state)
        {
            _animator.SetFloat(_runKey, state ? 1 : 2);
        }

        public void SetInput(float input)
        {
            _animator.SetFloat(_runKey, input);
        }
    }
}