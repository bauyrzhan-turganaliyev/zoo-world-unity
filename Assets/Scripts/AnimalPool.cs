using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public sealed class AnimalPool
{
    private sealed class Bucket
    {
        public AnimalDefinition Definition;
        public Transform Parent;
        public readonly Stack<Animal> Available = new Stack<Animal>();
        public readonly HashSet<Animal> Rented = new HashSet<Animal>();
        public int TotalCreated;
    }

    private readonly IInstantiator _instantiator;
    private readonly Dictionary<AnimalDefinition, Bucket> _buckets = new Dictionary<AnimalDefinition, Bucket>();
    private readonly Dictionary<Animal, Bucket> _owners = new Dictionary<Animal, Bucket>();

    public AnimalPool(IInstantiator instantiator)
    {
        _instantiator = instantiator;
    }

    public void Prewarm(AnimalDefinition definition, Transform parent, int initialSize)
    {
        if (definition == null || definition.Prefab == null)
            throw new ArgumentException("A valid animal definition with a prefab is required", nameof(definition));
        if (initialSize < 1)
            throw new ArgumentOutOfRangeException(nameof(initialSize));

        if (_buckets.ContainsKey(definition))
            return;

        var bucket = new Bucket { Definition = definition, Parent = parent };
        _buckets.Add(definition, bucket);
        Expand(bucket, initialSize);
    }

    public Animal Rent(AnimalDefinition definition)
    {
        if (!_buckets.TryGetValue(definition, out Bucket bucket))
            throw new InvalidOperationException("Prewarm the species before renting animals");

        if (bucket.Available.Count == 0)
        {
            int growth = Mathf.Max(4, Mathf.CeilToInt(bucket.TotalCreated * 0.5f));
            Expand(bucket, growth);
            Debug.Log($"AnimalPool: expanded {definition.SpeciesName} to {bucket.TotalCreated} instances");
        }

        Animal animal = bucket.Available.Pop();
        if (!bucket.Rented.Add(animal))
            throw new InvalidOperationException("Animal was rented twice");

        return animal;
    }

    public void Return(Animal animal)
    {
        if (animal == null || !_owners.TryGetValue(animal, out Bucket bucket))
            throw new InvalidOperationException("Animal does not belong to this pool");
        if (!bucket.Rented.Remove(animal))
            throw new InvalidOperationException("Animal was already returned to the pool");

        animal.ResetForPool();

        if (animal.TryGetComponent(out Rigidbody body) && !body.isKinematic)
        {
            body.linearVelocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
            body.Sleep();
        }

        animal.gameObject.SetActive(false);
        bucket.Available.Push(animal);
    }

    public void GetCounts(AnimalDefinition definition, out int created, out int active, out int available)
    {
        if (!_buckets.TryGetValue(definition, out Bucket bucket))
        {
            created = active = available = 0;
            return;
        }

        created = bucket.TotalCreated;
        active = bucket.Rented.Count;
        available = bucket.Available.Count;
    }

    private void Expand(Bucket bucket, int count)
    {
        for (int i = 0; i < count; i++)
        {
            Animal animal = _instantiator.InstantiatePrefabForComponent<Animal>(
                bucket.Definition.Prefab,
                bucket.Parent != null ? bucket.Parent.position : Vector3.zero,
                Quaternion.identity,
                bucket.Parent);

            if (animal == null)
                throw new InvalidOperationException("Prefab must contain an Animal component");

            animal.gameObject.SetActive(false);
            bucket.Available.Push(animal);
            _owners.Add(animal, bucket);
            bucket.TotalCreated++;
        }
    }
}
