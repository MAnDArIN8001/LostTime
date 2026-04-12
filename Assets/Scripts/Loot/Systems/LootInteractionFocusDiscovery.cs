using Gameplay.Interaction.Core;
using UnityEngine;

namespace Loot.Systems
{
    public sealed class LootInteractionFocusDiscovery : IFocusDiscovery<LootInteractionFocus>
    {
        public bool TryDiscover(RaycastHit[] hits, out LootInteractionFocus focus)
        {
            focus = LootInteractionFocus.Empty;

            if (hits == null || hits.Length == 0)
            {
                InteractionDebugLog.LogVerbose(null, "Focus discovery skipped: no raycast hits.");
                return false;
            }

            var hasCandidate = false;
            var bestDistance = float.MaxValue;
            var bestFocus = LootInteractionFocus.Empty;

            for (var i = 0; i < hits.Length; i++)
            {
                var hit = hits[i];
                if (hit.collider == null)
                {
                    InteractionDebugLog.LogVerbose(null, $"Focus discovery hit[{i}] skipped: collider is null.");
                    continue;
                }

                if (!TryResolveTarget(hit.collider, out var markable, out var pressable, out var interactable, out var takable, out var controlable))
                {
                    InteractionDebugLog.LogVerbose(
                        hit.collider,
                        $"Focus discovery hit[{i}] ignored: '{hit.collider.name}' has no interaction contracts. distance={hit.distance:0.###}");
                    continue;
                }

                InteractionDebugLog.LogVerbose(
                    hit.collider,
                    $"Focus discovery candidate hit[{i}]: target='{hit.collider.name}', distance={hit.distance:0.###}, " +
                    $"markable={markable != null}, pressable={pressable != null}, interactable={interactable != null}, takable={takable != null}, controlable={controlable != null}");

                if (hasCandidate && hit.distance >= bestDistance)
                {
                    continue;
                }

                hasCandidate = true;
                bestDistance = hit.distance;
                bestFocus = new LootInteractionFocus(
                    markable,
                    pressable,
                    interactable,
                    takable,
                    controlable,
                    PointerTargetContext.FromRaycastHit(hit));
            }

            if (!hasCandidate)
            {
                InteractionDebugLog.LogVerbose(null, "Focus discovery finished: no valid interaction candidates.");
                return false;
            }

            var pointerContext = bestFocus.Context.PointerContext;
            var targetName = pointerContext.Target != null
                ? pointerContext.Target.name
                : pointerContext.HitCollider != null
                    ? pointerContext.HitCollider.name
                    : "null";
            InteractionDebugLog.LogVerbose(
                pointerContext.HitCollider,
                $"Focus discovery selected target='{targetName}', distance={pointerContext.Distance:0.###}.");
            focus = bestFocus;
            return true;
        }

        private static bool TryResolveTarget(
            Collider collider,
            out IMarkable markable,
            out IPressable pressable,
            out IInteractable interactable,
            out ITakable takable,
            out IControlable controlable)
        {
            markable = TryGetInterface<IMarkable>(collider);
            pressable = TryGetInterface<IPressable>(collider);
            interactable = TryGetInterface<IInteractable>(collider);
            takable = TryGetInterface<ITakable>(collider);
            controlable = TryGetInterface<IControlable>(collider);

            return markable != null || pressable != null || interactable != null || takable != null || controlable != null;
        }

        private static TInterface TryGetInterface<TInterface>(Component component)
            where TInterface : class
        {
            if (component == null)
            {
                return null;
            }

            if (component.TryGetComponent(typeof(TInterface), out var resolvedComponent))
            {
                return resolvedComponent as TInterface;
            }

            return component.GetComponentInParent(typeof(TInterface)) as TInterface;
        }
    }
}
