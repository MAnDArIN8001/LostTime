using System;
using Loot.Data;

namespace Loot.Systems
{
    public interface ITakable
    {
        public event Action<ITakable> OnItemTaken;
        
        public ItemSetup ItemSetup { get; }
        public string InteractionPrompt { get; }

        public void Take();
    }
}

