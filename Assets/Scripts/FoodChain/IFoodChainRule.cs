namespace ZooWorld.FoodChain
{
    public interface IFoodChainRule
    {
        EatResult Resolve(IFoodChainMember first, IFoodChainMember second);
    }
}
