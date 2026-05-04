using DG.Tweening;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Animations.IK
{
    public class IkTriggerZoneFollow : MonoBehaviour 
    {
        [SerializeField] private float _followSmoothnes;

        [Space, SerializeField] private Vector3 _followOffset;    

        private Transform _target;
        [Space, SerializeField] private Transform _followerPonit;

        [Space, SerializeField] private MultiAimConstraint _targetConstraint;

        private Tween _weightUpdateTween;

        private void Update() 
        {
            if (_target == null)
            {
                return;
            }     

            _followerPonit.position = _target.position;
        }

        private void OnTriggerEnter(Collider obj)
        {
            if (obj.TryGetComponent<Character.Character>(out var character)) 
            {
                _target = character.transform;

                _weightUpdateTween?.Kill();
                _weightUpdateTween = DOTween.To(() => _targetConstraint.weight, x => _targetConstraint.weight = x, 1, _followSmoothnes);

            }
        }

        private void OnTriggerExit(Collider obj) 
        {
            if (obj.TryGetComponent<Character.Character>(out var character)) 
            {
                _target = null;

                _weightUpdateTween?.Kill();
                _weightUpdateTween = DOTween.To(() => _targetConstraint.weight, x => _targetConstraint.weight = x, 0, _followSmoothnes);
            }
        }
    }
}