using Zenject;

public sealed class ZooInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<FoodChainResolver>().AsSingle();
        Container.Bind<AnimalStatistics>().AsSingle();
        Container.Bind<AnimalPool>().AsSingle();
        Container.Bind<AnimalFactory>().AsSingle();
    }
}
