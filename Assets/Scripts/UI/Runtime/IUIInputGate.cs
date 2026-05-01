namespace UI.Runtime
{
    public interface IUIInputGate
    {
        public void BlockGameplayInput();
        public void RestoreGameplayInput();
    }
}
