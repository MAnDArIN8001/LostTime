using System;
using UnityEngine;

namespace Utils.Filters
{
    public interface IRaycastFilter : IDisposable
    {
        public event Action<RaycastHit[]> OnHitProcessed;
    }
}