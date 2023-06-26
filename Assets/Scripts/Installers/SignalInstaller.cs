using Core.Signals;
using Zenject;

namespace Installers
{
    public class SignalInstaller : MonoInstaller<SignalInstaller>
    {
        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);
            Container.DeclareSignal<MovementSignal>();
            Container.DeclareSignal<LookSignal>();
            Container.DeclareSignal<AttackSignal>();
            Container.DeclareSignal<PlayerHitSignal>();
            Container.DeclareSignal<EnemyDeathSignal>();
            Container.DeclareSignal<HealPickUpSignal>();
        }
    }
}