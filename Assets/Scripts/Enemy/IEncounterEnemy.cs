using System;

namespace Enemy
{
    public interface IEncounterEnemy
    {
        bool IsDead { get; }
        event Action<IEncounterEnemy> Died;
    }
}
