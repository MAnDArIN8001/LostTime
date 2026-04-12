namespace Gameplay.Input
{
    public interface IPointerControlSignalSource
    {
        bool TryReadControlAxis(out float axisValue);
    }
}
