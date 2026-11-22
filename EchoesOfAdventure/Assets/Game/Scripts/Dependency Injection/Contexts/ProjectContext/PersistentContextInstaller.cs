using Events;
using Helper;
using Interface;
using Sounds;
using UnityEngine;
using Zenject;

namespace DependencyInjection
{
    public class PersistentContextInstaller : MonoInstaller
    {
        [SerializeField] private SceneLoader.CoreSceneLoader sceneLoader;
        [SerializeField] private MusicAudioHandler audioHandler;
        public override void InstallBindings()
        {
            Container.Bind<ISceneLoader>().FromInstance(sceneLoader).AsSingle().NonLazy();
            Container.Bind<ProjectEvents>().AsSingle().NonLazy();
            Container.Bind<FactoryHelper>().AsSingle().NonLazy();
            Container.Bind<SoundManager>().AsSingle().NonLazy();
            Container.Bind<MusicAudioHandler>().FromInstance(audioHandler).AsSingle().NonLazy();

        }
    }
}