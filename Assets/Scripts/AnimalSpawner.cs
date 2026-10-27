using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public sealed class AnimalSpawner : MonoBehaviour
{
    [SerializeField] private Camera spawnCamera;
    [SerializeField] private AnimalDefinition[] species;
    [SerializeField, Min(0.1f)] private float minSpawnInterval = 1f;
    [SerializeField, Min(0.1f)] private float maxSpawnInterval = 2f;
    [SerializeField, Range(0f, 0.45f)] private float viewportMargin = 0.1f;
    [SerializeField] private float spawnHeight = 1f;

    private readonly List<AnimalDefinition> _validSpecies = new List<AnimalDefinition>();
    private IInstantiator _instantiator;
    private AnimalStatistics _statistics;

    [Inject]
    private void Construct(IInstantiator instantiator, AnimalStatistics statistics)
    {
        _instantiator = instantiator;
        _statistics = statistics;
    }

    private IEnumerator Start()
    {
        if (spawnCamera == null)
        {
            Debug.LogError("AnimalSpawner: assign the top-down camera", this);
            yield break;
        }

        if (species != null)
        {
            foreach (AnimalDefinition definition in species)
            {
                if (definition == null || definition.Prefab == null)
                {
                    Debug.LogWarning("AnimalSpawner: skipping a missing definition or prefab", this);
                    continue;
                }

                _validSpecies.Add(definition);
            }
        }

        if (_validSpecies.Count == 0)
        {
            Debug.LogError("AnimalSpawner: assign at least one valid animal definition", this);
            yield break;
        }

        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minSpawnInterval, maxSpawnInterval));
            SpawnAnimal();
        }
    }

    private void SpawnAnimal()
    {
        AnimalDefinition definition = _validSpecies[Random.Range(0, _validSpecies.Count)];
        float x = Random.Range(viewportMargin, 1f - viewportMargin);
        float y = Random.Range(viewportMargin, 1f - viewportMargin);
        Ray ray = spawnCamera.ViewportPointToRay(new Vector3(x, y, 0f));
        Plane spawnPlane = new Plane(Vector3.up, new Vector3(0f, spawnHeight, 0f));

        if (!spawnPlane.Raycast(ray, out float distance))
        {
            Debug.LogError("AnimalSpawner: camera ray does not intersect the spawn plane", this);
            return;
        }

        Animal animal = _instantiator.InstantiatePrefabForComponent<Animal>(
            definition.Prefab,
            ray.GetPoint(distance),
            Quaternion.identity,
            transform);

        animal.Initialize(definition);
        _statistics.Track(animal);
    }

    private void OnValidate()
    {
        minSpawnInterval = Mathf.Max(0.1f, minSpawnInterval);
        maxSpawnInterval = Mathf.Max(minSpawnInterval, maxSpawnInterval);
    }
}