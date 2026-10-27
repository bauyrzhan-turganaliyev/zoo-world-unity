using System;

public sealed class FoodChainResolver
{

    public bool TryResolve(Animal first, Animal second, out Animal eater)
    {
        eater = null;

        if (first == null || second == null || first == second ||
            !first.IsAlive || !second.IsAlive ||
            first.Definition == null || second.Definition == null)
            return false;

        bool firstPredator = first.Definition.Role == AnimalDefinition.FoodRole.Predator;
        bool secondPredator = second.Definition.Role == AnimalDefinition.FoodRole.Predator;

        if (!firstPredator && !secondPredator)
            return false;

        Animal winner;
        Animal victim;

        if (firstPredator != secondPredator)
        {
            winner = firstPredator ? first : second;
            victim = firstPredator ? second : first;
        }
        else
        {
            winner = first.GetInstanceID() < second.GetInstanceID() ? first : second;
            victim = winner == first ? second : first;
        }

        if (!victim.TryDie())
            return false;

        eater = winner;
        winner.NotifyAte();
        return true;
    }
}
