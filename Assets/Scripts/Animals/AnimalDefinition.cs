using UnityEngine;
using ZooWorld.FoodChain;

namespace ZooWorld.Animals
{
    [CreateAssetMenu(fileName = "AnimalDefinition", menuName = "Zoo World/Animal Definition")]
    public sealed class AnimalDefinition : ScriptableObject, IFoodChainMember
    {
        [SerializeField] private string speciesName;

        [Tooltip("0 = prey. A higher level eats a lower level; predators of the same level fight")]
        [SerializeField, Min(FoodChainLevels.Prey)] private int foodChainLevel;

        [SerializeField] private Animal prefab;

        public string SpeciesName => speciesName;
        public int FoodChainLevel => foodChainLevel;
        public bool IsPredator => FoodChainLevels.IsPredator(foodChainLevel);
        public Animal Prefab => prefab;
    }
}
