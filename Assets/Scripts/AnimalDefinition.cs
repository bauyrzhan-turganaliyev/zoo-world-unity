using UnityEngine;

[CreateAssetMenu(fileName = "AnimalDefinition", menuName = "Zoo World/Animal Definition")]
public sealed class AnimalDefinition : ScriptableObject
{
    public enum FoodRole
    {
        Prey,
        Predator
    }

    [SerializeField] private string speciesName;
    [SerializeField] private FoodRole foodRole;
    [SerializeField] private Animal prefab;

    public string SpeciesName => speciesName;
    public FoodRole Role => foodRole;
    public Animal Prefab => prefab;
}