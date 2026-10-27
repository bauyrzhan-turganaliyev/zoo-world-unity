using UnityEngine;
using Zenject;
using ZooWorld.Arena;
using ZooWorld.FoodChain;
using ZooWorld.Spawning;
using ZooWorld.Statistics;

namespace ZooWorld.Installers
{
    public sealed class ZooInstaller : MonoInstaller
    {
        [SerializeField] private Camera arenaCamera;
        [SerializeField, Range(0f, 0.45f)] private float arenaViewportMargin = 0.08f;

        public override void InstallBindings()
        {
            if (arenaCamera == null)
            {
                Debug.LogError("ZooInstaller: assign the top-down camera", this);
                return;
            }

            Container.Bind<Camera>().FromInstance(arenaCamera).AsSingle();
            Container.Bind<IArenaBounds>()
                .FromInstance(new CameraArenaBounds(arenaCamera, arenaViewportMargin))
                .AsSingle();

            Container.Bind<IFoodChainRule>().To<FoodChainLevelRule>().AsSingle();
            Container.Bind<FoodChainResolver>().AsSingle();
            Container.Bind<AnimalStatistics>().AsSingle();
            Container.Bind<AnimalPool>().AsSingle();
            Container.Bind<AnimalFactory>().AsSingle();
        }
    }
}
