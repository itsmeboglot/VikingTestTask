using Game.Player;
using UnityEngine;
using Zenject;

namespace Game.Mutant
{
    public class MutantController
    {
        public Mutant View { get; }

        public bool Spawned;

        public MutantController(Mutant view)
        {
            View = view;
        }

        public void Initialize(SignalBus signalBus, IPlayerInfo playerInfo)
        {
            View.Initialize(playerInfo, signalBus);
        }
        
        public void SetData(int maxHealth)
        {
            View.Respawn(maxHealth);
        }

        private void Attack()
        {
            Debug.Log("Attacking the target!");
        }
    }
}
