using System;
using System.Collections.Generic;
using Core.Signals;
using Game.Heal.Interfaces;
using UnityEngine;
using Zenject;

namespace Game.Heal
{
    public class HealHandler : MonoBehaviour, IHealHandler
    {
        [SerializeField] private Heal healPrefab;

        [Inject] private SignalBus _signalBus;
        
        private readonly List<Heal> _healsPool = new ();

        private void Start()
        {
            _signalBus.Subscribe<EnemyDeathSignal>(HandleMutantDeath);
            _signalBus.Subscribe<HealPickUpSignal>(HandleHealPickUp);
        }

        private void OnDestroy()
        {
            _signalBus.Unsubscribe<EnemyDeathSignal>(HandleMutantDeath);
            _signalBus.Unsubscribe<HealPickUpSignal>(HandleHealPickUp);
        }
        
        private void HandleHealPickUp(HealPickUpSignal signal)
        {
            signal.HealGameObject.SetActive(false);
        }
        
        private void HandleMutantDeath(EnemyDeathSignal signal)
        {
            CreateHeal(signal.Mutant.transform);
        }

        public Heal CreateHeal(Transform point)
        {
            foreach (var heal in _healsPool)
            {
                if (!heal.gameObject.activeSelf)
                {
                    heal.Spawn(point);
                    return heal;
                }
            }

            var newHeal = Instantiate(healPrefab);
            newHeal.Spawn(point);
            _healsPool.Add(newHeal);

            return newHeal;
        }
    }
}
