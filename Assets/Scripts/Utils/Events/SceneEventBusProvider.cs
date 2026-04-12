using UnityEngine;

namespace Utils.Events
{
    [DisallowMultipleComponent]
    public sealed class SceneEventBusProvider : MonoBehaviour
    {
        [SerializeField] private bool _dontDestroyOnLoad;

        private static SceneEventBusProvider _instance;
        private EventBus _eventBus;

        public static SceneEventBusProvider Instance => _instance;
        public EventBus EventBus => _eventBus;

        public static bool TryGetEventBus(out EventBus eventBus)
        {
            eventBus = _instance?._eventBus;
            return eventBus != null;
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Debug.LogWarning($"Duplicate {nameof(SceneEventBusProvider)} found on '{name}'. Destroying duplicate.");
                Destroy(this);
                return;
            }

            _instance = this;
            _eventBus ??= new EventBus();

            if (_dontDestroyOnLoad)
            {
                DontDestroyOnLoad(gameObject);
            }
        }

        private void OnDestroy()
        {
            if (_instance != this)
            {
                return;
            }

            _eventBus?.Dispose();
            _eventBus = null;
            _instance = null;
        }
    }
}
