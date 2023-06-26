using System.Collections.Generic;
using Game.Mutant.Interfaces;
using Zenject;

namespace Game.Mutant
{
    public class MutantFactory : IMutantFactory
    {
        private readonly DiContainer _container;
        private readonly Mutant _enemyPrefab;

        private readonly Dictionary<int, MutantController> _enemyPool = new ();

        public MutantFactory(DiContainer container, Mutant enemyPrefab)
        {
            _container = container;
            _enemyPrefab = enemyPrefab;
        }

        private MutantController CreateEnemy()
        {
            var view = _container.InstantiatePrefabForComponent<Mutant>(_enemyPrefab);
            var enemyController = new MutantController(view);
        
            _enemyPool.Add(view.GetInstanceID(), enemyController);
            return enemyController;
        }

        public MutantController GetEnemyFromPool()
        {
            foreach (var enemy in _enemyPool)
            {
                if (!enemy.Value.Spawned)
                {
                    return enemy.Value;
                }
            }

            return CreateEnemy();
        }
        
        public MutantController GetEnemyFromPool(int hashCode)
        {
            return _enemyPool.TryGetValue(hashCode, out var enemy) ? enemy : CreateEnemy();
        }
    }
}