using System;

namespace ZooWorld.FoodChain
{
    public sealed class FoodChainLevelRule : IFoodChainRule
    {
        public EatResult Resolve(IFoodChainMember first, IFoodChainMember second)
        {
            if (first == null)
                throw new ArgumentNullException(nameof(first));
            if (second == null)
                throw new ArgumentNullException(nameof(second));

            int firstLevel = first.FoodChainLevel;
            int secondLevel = second.FoodChainLevel;

            if (firstLevel > secondLevel)
                return EatResult.FirstEatsSecond;
            if (secondLevel > firstLevel)
                return EatResult.SecondEatsFirst;

            return FoodChainLevels.IsPredator(firstLevel)
                ? EatResult.FirstEatsSecond
                : EatResult.None;
        }
    }
}
