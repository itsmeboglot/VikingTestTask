using UnityEngine;

namespace Game.Player
{
    public interface IPlayerInfo
    {
        Transform Transform { get; }
        int MaxHealth { get; }
        bool IsDead { get; }
    }
}