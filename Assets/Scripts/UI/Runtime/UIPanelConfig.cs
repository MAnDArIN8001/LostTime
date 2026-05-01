namespace UI.Runtime
{
    public readonly struct UIPanelConfig
    {
        public bool IsModal { get; }
        public UICachePolicy CachePolicy { get; }

        public UIPanelConfig(bool isModal, UICachePolicy cachePolicy)
        {
            IsModal = isModal;
            CachePolicy = cachePolicy;
        }

        public static UIPanelConfig Default => new UIPanelConfig(false, UICachePolicy.DestroyOnClose);
        public static UIPanelConfig Modal => new UIPanelConfig(true, UICachePolicy.DestroyOnClose);
        public static UIPanelConfig ModalKeepAlive => new UIPanelConfig(true, UICachePolicy.KeepAlive);
        public static UIPanelConfig NonModalKeepAlive => new UIPanelConfig(false, UICachePolicy.KeepAlive);
    }
}
