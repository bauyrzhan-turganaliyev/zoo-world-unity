using System;
using ZooWorld.Animals;

namespace ZooWorld.FoodChain
{
    public sealed class FoodChainResolver
    {
        private readonly IFoodChainRule _rule;

        public FoodChainResolver(IFoodChainRule rule)
        {
            _rule = rule ?? throw new ArgumentNullException(nameof(rule));
        }

        public bool TryResolve(Animal first, Animal second, out Animal eater)
        {
            eater = null;

            if (first == null || second == null || first == second ||
                !first.IsAlive || !second.IsAlive ||
                first.Definition == null || second.Definition == null)
                return false;
            
            if (first.GetInstanceID() > second.GetInstanceID())
            {
                (first, second) = (second, first);
            }

            EatResult result = _rule.Resolve(first.Definition, second.Definition);
            if (result == EatResult.None)
                return false;

            Animal winner = result == EatResult.FirstEatsSecond ? first : second;
            Animal victim = winner == first ? second : first;

            if (!victim.TryDie())
                return false;

            eater = winner;
            winner.NotifyAte();
            return true;
        }
    }
}
