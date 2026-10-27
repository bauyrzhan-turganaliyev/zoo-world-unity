using System;
using UnityEngine;
using ZooWorld.Animals;
using ZooWorld.Statistics;

namespace ZooWorld.Spawning
{
    public sealed class AnimalFactory
    {
        private readonly AnimalPool _pool;
        private readonly AnimalStatistics _statistics;

        public AnimalFactory(AnimalPool pool, AnimalStatistics statistics)
        {
            _pool = pool;
            _statistics = statistics;
        }

        public Animal Spawn(AnimalDefinition definition, Vector3 position, Quaternion rotation)
        {
            if (definition == null)
                throw new ArgumentNullException(nameof(definition));

            Animal animal = _pool.Rent(definition);
            animal.transform.SetPositionAndRotation(position, rotation);
            animal.Initialize(definition, _pool.Return);
            _statistics.Track(animal);
            animal.gameObject.SetActive(true);

            if (animal.TryGetComponent(out Rigidbody body) && !body.isKinematic)
            {
                body.linearVelocity = Vector3.zero;
                body.angularVelocity = Vector3.zero;
                body.WakeUp();
            }

            return animal;
        }
    }
}
