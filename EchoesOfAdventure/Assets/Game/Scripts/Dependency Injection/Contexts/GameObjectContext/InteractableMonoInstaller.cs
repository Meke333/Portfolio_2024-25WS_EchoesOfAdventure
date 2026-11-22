using Echo;
using Interactable;
using ObjectPool;
using UnityEngine;
using Zenject;

namespace DependencyInjection
{
    public class InteractableMonoInstaller : MonoInstaller
    {

        public override void InstallBindings()
        {
            Container.Bind<PoolManagerData>()
                .AsSingle()
                .NonLazy();

            Container.Bind<EchoEvents>()
                .AsSingle()
                .NonLazy();

            Container.Bind<InteractableEvents>()
                .AsSingle()
                .NonLazy();
        }

    }
}
