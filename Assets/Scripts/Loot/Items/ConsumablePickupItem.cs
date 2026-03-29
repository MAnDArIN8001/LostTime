using Combat;
using Loot.Systems;
using UnityEngine;

namespace Loot.Items
{
    public class ConsumablePickupItem : MonoBehaviour, ICollectible
    {
        [Header("Flow")]
        [SerializeField] private bool _collectOnTrigger;

        [Header("Payload")]
        [SerializeField, Min(0f)] private float _healthRestore;
        [SerializeField, Min(0f)] private float _manaRestore;
        [SerializeField, Min(0)] private int _coinReward;

        private ITakable _takable;

        private void Awake()
        {
            _takable = GetComponent<ITakable>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!_collectOnTrigger)
            {
                return;
            }

            TryCollect(other.gameObject);
        }

        public bool TryCollect(GameObject interactor)
        {
            if (_takable == null || interactor == null)
            {
                return false;
            }

            if (interactor.GetComponentInParent<Character.Character>() == null)
            {
                return false;
            }

            ApplyPayload(interactor);
            _takable.Take();
            return true;
        }

        private void ApplyPayload(GameObject interactor)
        {
            if (_healthRestore > 0f)
            {
                var vitals = interactor.GetComponentInParent<CharacterVitals>();
                vitals?.RestoreHealth(_healthRestore);
            }

            if (_manaRestore > 0f)
            {
                var mana = interactor.GetComponentInParent<CharacterMana>();
                mana?.Restore(_manaRestore);
            }

            if (_coinReward > 0)
            {
                Debug.Log($"Collected coin reward: +{_coinReward}");
            }
        }
    }
}
