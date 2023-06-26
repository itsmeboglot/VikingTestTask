using UnityEngine;

namespace Game.Heal.Interfaces
{
    public interface IHealHandler
    {
        Heal CreateHeal(Transform point);
    }
}