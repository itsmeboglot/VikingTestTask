using Game.Mutant;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class MutantInstaller : MonoInstaller<MutantInstaller>
    {
        [SerializeField] private Mutant enemyPrefab;

        public override void InstallBindings()
        {
            Container.BindInterfacesTo<MutantFactory>().AsSingle().WithArguments(enemyPrefab);
        }
    }
}