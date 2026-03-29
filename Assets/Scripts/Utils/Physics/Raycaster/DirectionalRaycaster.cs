using System;
using UnityEngine;

namespace Utils.Physics.Raycaster
{
    public abstract class DirectionalRaycaster : MonoBehaviour
    {
        public abstract event Action<RaycastHit[]> OnRayCollide;
        
        [Header("Constraints")] 
        [SerializeField] protected int _frameOffset;
        protected int _currentFrame;
        
        [Header("Ray Configuration")] 
        [SerializeField] protected float _raycastDistance;
    }
}