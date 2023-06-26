using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Signals;
using Game.Mutant.Data;
using Game.Mutant.Interfaces;
using Game.Player;
using UnityEngine;
using Zenject;

namespace Game.Mutant
{
    public class MutantsHandler : MonoBehaviour, IMutantHandler
    {
        [SerializeField] private Transform[] spawnPoints;
        [SerializeField] private int spawnCount = 10;
        [SerializeField] private float respawnDelay = 4f;
        [SerializeField] private float respawnRadius = 10f;
        
        private IMutantFactory _mutantFactory;
        private SignalBus _signalBus;
        private IPlayerInfo _playerInfo;
        private WaitForSeconds _respawnWaiter;
        
        private readonly Dictionary<int, EnemySpawnInfo> _spawnInfos = new ();

        [Inject]
        private void Construct(IMutantFactory mutantFactory, SignalBus signalBus, IPlayerInfo playerInfo)
        {
            _mutantFactory = mutantFactory;
            _signalBus = signalBus;
            _playerInfo = playerInfo;
        }

        private void Awake()
        {
            if (spawnCount > spawnPoints.Length)
                spawnCount = spawnPoints.Length;

            _respawnWaiter = new WaitForSeconds(respawnDelay);
        }

        private void Start()
        {
            SpawnEnemies();
            
            _signalBus.Subscribe<EnemyDeathSignal>(HandeEnemyDeath);
        }

        private void OnDestroy()
        {
            _signalBus.Unsubscribe<EnemyDeathSignal>(HandeEnemyDeath);
        }

        private void HandeEnemyDeath(EnemyDeathSignal signal)
        {
            StartCoroutine(RespawnEnemy(signal.Mutant.GetInstanceID()));
        }

        private IEnumerator RespawnEnemy(int instanceId)
        {
            yield return _respawnWaiter;
            
            Vector3 randomPosition = Random.insideUnitSphere * respawnRadius;
            randomPosition += _playerInfo.Transform.position;
            
            var spawnInfo = _spawnInfos[instanceId];
            spawnInfo.Health += 1;

            var enemy = SpawnEnemy(instanceId, randomPosition, 
                Quaternion.Euler(randomPosition - _playerInfo.Transform.position));
            enemy.SetData(spawnInfo.Health);
        }

        private void SpawnEnemies()
        {
            var spawns = spawnPoints.ToList();
            
            for (int i = 0; i < spawnCount; i++)
            {
                int rndIndex = Random.Range(0, spawns.Count);
                var enemy = SpawnEnemy(spawns[rndIndex]);
                enemy.Initialize(_signalBus, _playerInfo);
                _spawnInfos.Add(enemy.View.GetInstanceID(), new EnemySpawnInfo());
                
                var spawnInfo = _spawnInfos[enemy.View.GetInstanceID()];
                enemy.SetData(spawnInfo.Health);
                
                spawns.RemoveAt(rndIndex);
            }
        }

        public void RespawnEnemies()
        {
            var spawns = spawnPoints.ToList();
            
            foreach (var spawnInfo in _spawnInfos)
            {
                int rndIndex = Random.Range(0, spawns.Count);
                var enemy = SpawnEnemy(spawnInfo.Key, spawns[rndIndex].position, spawns[rndIndex].rotation);
                spawnInfo.Value.Health = 1;

                enemy.SetData(spawnInfo.Value.Health);
                spawns.RemoveAt(rndIndex);
            }
        }

        private MutantController SpawnEnemy(int instanceId, Vector3 position, Quaternion rotation)
        {
            MutantController enemy = _mutantFactory.GetEnemyFromPool(instanceId);
            enemy.View.transform.position = position;
            enemy.View.transform.rotation = rotation;

            return enemy;
        }
        
        private MutantController SpawnEnemy(Transform spawnPoint)
        {
            MutantController enemy = _mutantFactory.GetEnemyFromPool();
            enemy.View.transform.position = spawnPoint.position;
            enemy.View.transform.rotation = spawnPoint.rotation;

            enemy.Spawned = true;
            
            return enemy;
        }
    }
}
