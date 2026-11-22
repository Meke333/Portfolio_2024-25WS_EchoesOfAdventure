using Echo;
using Player;
using UnityEngine;
using Zenject;
using Player.Statemachine;

namespace DependencyInjection
{
    public class PlayerMonoInstaller : MonoInstaller
    {
        [SerializeField] private PlayerBase _playerbase;
        
        public override void InstallBindings()
        {
            Container.Bind<PlayerBase>()
                .FromInstance(_playerbase)
                .AsSingle()
                .NonLazy();

            Container.Bind<PlayerEvents>()
                .AsSingle()
                .NonLazy();

            Container.Bind<PlayerStateData>()
                .FromInstance(new PlayerStateData())
                .AsSingle()
                .NonLazy();

            
            SetUpPlayerStatemachine();
        }

        private void SetUpPlayerStatemachine()
        {
            Container.Bind<Player.Statemachine.Idle>().AsTransient();
            Container.Bind<Player.Statemachine.Dead>().AsTransient();
            Container.Bind<Player.Statemachine.Jumping>().AsTransient();
            Container.Bind<Player.Statemachine.Falling>().AsTransient();
            Container.Bind<Player.Statemachine.Grabbing>().AsTransient();
            Container.Bind<Player.Statemachine.Climbing>().AsTransient();
        }
    }
}
