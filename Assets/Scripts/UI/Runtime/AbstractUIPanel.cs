using UnityEngine;

namespace UI.Runtime
{
    public abstract class AbstractUIPanel : MonoBehaviour, IUIPanel
    {
        [Header("Panel Config")]
        [SerializeField] private bool _isModal;
        [SerializeField] private UICachePolicy _cachePolicy = UICachePolicy.DestroyOnClose;
        [SerializeField] private bool _hideOnEnable = true;
        [SerializeField] private GameObject _panelRoot;

        public PanelId Id { get; private set; }
        public UIPanelConfig Config { get; private set; }
        public bool IsVisible { get; private set; }

        protected virtual void Awake()
        {
            Id = PanelId.From(GetType());
            Config = new UIPanelConfig(_isModal, _cachePolicy);
            EnsurePanelRoot();
        }

        protected virtual void OnEnable()
        {
            if (_hideOnEnable)
            {
                HideImmediate();
            }
        }

        public void Show()
        {
            if (IsVisible)
            {
                return;
            }

            EnsurePanelRoot();
            OnBeforeShow();

            if (_panelRoot != null)
            {
                _panelRoot.SetActive(true);
            }

            IsVisible = true;
            OnAfterShow();
        }

        public void Hide()
        {
            if (!IsVisible)
            {
                return;
            }

            OnBeforeHide();

            if (_panelRoot != null)
            {
                _panelRoot.SetActive(false);
            }

            IsVisible = false;
            OnAfterHide();
        }

        public void Close(UIPanelCloseReason reason)
        {
            if (IsVisible)
            {
                Hide();
            }

            OnClosed(reason);
        }

        protected virtual void OnBeforeShow()
        {
        }

        protected virtual void OnAfterShow()
        {
        }

        protected virtual void OnBeforeHide()
        {
        }

        protected virtual void OnAfterHide()
        {
        }

        protected virtual void OnClosed(UIPanelCloseReason reason)
        {
        }

        protected GameObject GetPanelRoot()
        {
            EnsurePanelRoot();
            return _panelRoot;
        }

        protected void SetPanelRoot(GameObject panelRoot)
        {
            _panelRoot = panelRoot;
        }

        private void HideImmediate()
        {
            EnsurePanelRoot();
            if (_panelRoot != null)
            {
                _panelRoot.SetActive(false);
            }

            IsVisible = false;
        }

        private void EnsurePanelRoot()
        {
            if (_panelRoot == null)
            {
                _panelRoot = gameObject;
            }
        }
    }
}
