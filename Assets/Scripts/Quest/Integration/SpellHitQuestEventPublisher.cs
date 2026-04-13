using Combat;
using Quest.Core;
using UnityEngine;
using Utils.Events;

namespace Quest.Integration
{
    [DisallowMultipleComponent]
    public sealed class SpellHitQuestEventPublisher : MonoBehaviour
    {
        [SerializeField] private SpellHitEventTrigger _spellHitTrigger;
        [SerializeField] private SceneEventBusProvider _eventBusProvider;
        [SerializeField] private string _sourceId = "player_spell";
        [SerializeField] private string _targetId = "target";

        private void OnEnable()
        {
            if (_spellHitTrigger != null)
            {
                _spellHitTrigger.SpellHit += OnSpellHit;
            }
        }

        private void OnDisable()
        {
            if (_spellHitTrigger != null)
            {
                _spellHitTrigger.SpellHit -= OnSpellHit;
            }
        }

        private void OnSpellHit(SpellProjectile projectile)
        {
            if (!TryResolveEventBus(out var eventBus))
            {
                return;
            }

            eventBus.Publish(new QuestEventData(
                QuestEventIds.TargetHit,
                _sourceId,
                _targetId,
                1,
                projectile != null ? projectile.Caster : null,
                _spellHitTrigger != null ? _spellHitTrigger.gameObject : null));
        }

        private bool TryResolveEventBus(out EventBus eventBus)
        {
            if (_eventBusProvider != null && _eventBusProvider.EventBus != null)
            {
                eventBus = _eventBusProvider.EventBus;
                return true;
            }

            return SceneEventBusProvider.TryGetEventBus(out eventBus);
        }

        private void Reset()
        {
            if (_spellHitTrigger == null)
            {
                _spellHitTrigger = GetComponent<SpellHitEventTrigger>();
            }
        }
    }
}
