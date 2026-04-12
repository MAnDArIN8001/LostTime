using UnityEngine;

namespace Gameplay.Interaction.Core
{
    public interface IFocusDiscovery<TFocus>
    {
        bool TryDiscover(RaycastHit[] hits, out TFocus focus);
    }
}
