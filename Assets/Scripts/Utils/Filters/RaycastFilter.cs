using System;
using System.Linq;
using UnityEngine;
using Utils.Physics.Raycaster;

namespace Utils.Filters
{
    public class RaycastFilter : IRaycastFilter
    {
        public event Action<RaycastHit[]> OnHitProcessed;

        private readonly Func<RaycastHit, bool> _filter;

        private readonly DirectionalRaycaster _directionalRaycaster;

        public RaycastFilter(DirectionalRaycaster directionalRaycaster, Func<RaycastHit, bool> filter)
        {
            _filter = filter;
            _directionalRaycaster = directionalRaycaster;

            _directionalRaycaster.OnRayCollide += Process;
        }

        private void Process(RaycastHit[] hitsInfo)
        {
            var filterResult = hitsInfo.Where(_filter);
            
            OnHitProcessed?.Invoke(filterResult.ToArray());
        }

        public void Dispose()
        {
            _directionalRaycaster.OnRayCollide -= Process;
        }
    }
}