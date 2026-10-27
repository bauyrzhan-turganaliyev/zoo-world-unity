using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using ZooWorld.Animals;
using ZooWorld.Arena;

namespace ZooWorld.Spawning
{
    public sealed class AnimalSpawner : MonoBehaviour
    {
        [SerializeField] private AnimalDefinition[] species;
        [SerializeField, Min(0.1f)] private float minSpawnInterval = 1f;
        [SerializeField, Min(0.1f)] private float maxSpawnInterval = 2f;
        [SerializeField] private float spawnHeight = 1f;
        [SerializeField, Range(10, 15)] private int initialPoolSize = 12;

        [Tooltip("No other animal may be closer than this to a new spawn point")]
        [SerializeField, Min(0f)] private float spawnClearance = 0.75f;
        [SerializeField, Min(1)] private int maxSpawnAttempts = 8;

        private readonly Collider[] _overlapBuffer = new Collider[16];

        private readonly List<AnimalDefinition> _validSpecies = new List<AnimalDefinition>();
        private AnimalFactory _factory;
        private AnimalPool _pool;
        private IArenaBounds _arena;

        [Inject]
        private void Construct(AnimalFactory factory, AnimalPool pool, IArenaBounds arena)
        {
            _factory = factory;
            _pool = pool;
            _arena = arena;
        }

        private IEnumerator Start()
        {
            if (_factory == null || _pool == null || _arena == null)
            {
                Debug.LogError("AnimalSpawner: Extenject bindings are missing", this);
                yield break;
            }

            _validSpecies.Clear();
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

            foreach (AnimalDefinition definition in _validSpecies)
                _pool.Prewarm(definition, transform, initialPoolSize);

            while (true)
            {
                yield return new WaitForSeconds(Random.Range(minSpawnInterval, maxSpawnInterval));
                SpawnAnimal();
            }
        }

        private void SpawnAnimal()
        {
            if (!TryFindFreePoint(out Vector3 position))
                return;

            AnimalDefinition definition = _validSpecies[Random.Range(0, _validSpecies.Count)];
            _factory.Spawn(definition, position, Quaternion.identity);
        }

        private bool TryFindFreePoint(out Vector3 position)
        {
            for (int attempt = 0; attempt < maxSpawnAttempts; attempt++)
            {
                if (!_arena.TryGetRandomPoint(spawnHeight, out position))
                {
                    Debug.LogError("AnimalSpawner: camera ray does not intersect the spawn plane", this);
                    return false;
                }

                if (!IsOccupied(position))
                    return true;
            }

            position = default;
            return false;
        }

        private bool IsOccupied(Vector3 position)
        {
            int count = Physics.OverlapSphereNonAlloc(
                position, spawnClearance, _overlapBuffer, Physics.AllLayers, QueryTriggerInteraction.Ignore);

            for (int i = 0; i < count; i++)
            {
                if (_overlapBuffer[i].GetComponentInParent<Animal>() != null)
                    return true;
            }

            return false;
        }

        private void OnValidate()
        {
            minSpawnInterval = Mathf.Max(0.1f, minSpawnInterval);
            maxSpawnInterval = Mathf.Max(minSpawnInterval, maxSpawnInterval);
        }
    }
}
