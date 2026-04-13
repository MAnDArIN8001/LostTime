using Combat;
using Quest.Core;
using UnityEngine;
using Utils.Events;

namespace Quest.Integration
{
    [DisallowMultipleComponent]
    public sealed class CharacterSpellCastQuestEventPublisher : MonoBehaviour
    {
        [SerializeField] private CharacterSpellCaster _spellCaster;
        [SerializeField] private SceneEventBusProvider _eventBusProvider;
        [SerializeField] private string _sourceId = "player_spell";
        [SerializeField] private string _targetId = "spell_cast";

        private void OnEnable()
        {
            if (_spellCaster != null)
            {
                _spellCaster.SpellCast += OnSpellCast;
            }
        }

        private void OnDisable()
        {
            if (_spellCaster != null)
            {
                _spellCaster.SpellCast -= OnSpellCast;
            }
        }

        private void OnSpellCast(SpellCastInfo spellCastInfo)
        {
            if (!TryResolveEventBus(out var eventBus))
            {
                return;
            }

            eventBus.Publish(new QuestEventData(
                QuestEventIds.SpellCast,
                _sourceId,
                _targetId,
                1,
                _spellCaster != null ? _spellCaster.gameObject : null,
                spellCastInfo.Projectile != null ? spellCastInfo.Projectile.gameObject : null));
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
            if (_spellCaster == null)
            {
                _spellCaster = GetComponent<CharacterSpellCaster>();
            }
        }
    }
}
