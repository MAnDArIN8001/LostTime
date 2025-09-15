using UnityEngine;

namespace Character.Modules.Animation
{
    public class AnimationModule : MonoBehaviour
    {
        [Header("Animation Keys")] 
        [SerializeField] private string _inputKey;
        
        [Space, SerializeField] protected Animator _animator;
        
        public void SetMovement(float input, int myltiplyer)
        {
            _animator.SetFloat(_inputKey, input * myltiplyer);
        }
    }
}