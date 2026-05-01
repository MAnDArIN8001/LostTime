namespace UI.Runtime
{
    public struct UIPanelConfig
    {
        public bool IsModal { get; private set; }
        public UICachePolicy CachePolicy { get; private set; }
        public bool BlocksGameplayInput { get; private set; }

        public UIPanelConfig(bool isModal, UICachePolicy cachePolicy, bool blocksGameplayInput = false)
        {
            IsModal = isModal;
            CachePolicy = cachePolicy;
            BlocksGameplayInput = blocksGameplayInput;
        }

        public static UIPanelConfig Default => new UIPanelConfig(false, UICachePolicy.DestroyOnClose, false);
        public static UIPanelConfig Modal => new UIPanelConfig(true, UICachePolicy.DestroyOnClose, true);
        public static UIPanelConfig ModalKeepAlive => new UIPanelConfig(true, UICachePolicy.KeepAlive, true);
        public static UIPanelConfig NonModalKeepAlive => new UIPanelConfig(false, UICachePolicy.KeepAlive, false);
    }
}
