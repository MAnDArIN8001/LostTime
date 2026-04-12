using UnityEngine;

namespace Gameplay.Interaction.Core
{
    public readonly struct PointerTargetContext
    {
        public PointerTargetContext(
            GameObject target,
            Collider hitCollider,
            Vector3 hitPoint,
            Vector3 hitNormal,
            float distance,
            InteractionIntent intent = InteractionIntent.None)
        {
            Target = target;
            HitCollider = hitCollider;
            HitPoint = hitPoint;
            HitNormal = hitNormal;
            Distance = distance;
            Intent = intent;
        }

        public GameObject Target { get; }
        public Collider HitCollider { get; }
        public Vector3 HitPoint { get; }
        public Vector3 HitNormal { get; }
        public float Distance { get; }
        public InteractionIntent Intent { get; }

        public bool HasTarget => Target != null || HitCollider != null;

        public static PointerTargetContext FromRaycastHit(
            RaycastHit hit,
            InteractionIntent intent = InteractionIntent.None)
        {
            return new PointerTargetContext(
                hit.collider != null ? hit.collider.gameObject : null,
                hit.collider,
                hit.point,
                hit.normal,
                hit.distance,
                intent);
        }

        public PointerTargetContext WithIntent(InteractionIntent intent)
        {
            return new PointerTargetContext(Target, HitCollider, HitPoint, HitNormal, Distance, intent);
        }
    }
}
