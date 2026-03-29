using UnityEngine;

namespace Loot.Systems
{
    public interface ICollectible
    {
        bool TryCollect(GameObject interactor);
    }
}
