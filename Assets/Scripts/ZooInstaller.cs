using Zenject;

public sealed class ZooInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<FoodChainResolver>().AsSingle();
        Container.Bind<AnimalStatistics>().AsSingle();
    }
}