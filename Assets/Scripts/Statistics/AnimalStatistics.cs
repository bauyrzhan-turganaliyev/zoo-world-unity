using System;
using ZooWorld.Animals;

namespace ZooWorld.Statistics
{
    public sealed class AnimalStatistics
    {
        public int DeadPrey { get; private set; }
        public int DeadPredators { get; private set; }

        public event Action Changed;

        public void Track(Animal animal)
        {
            if (animal == null)
                throw new ArgumentNullException(nameof(animal));

            if (animal.Definition == null)
                throw new InvalidOperationException("Initialize the animal before tracking it");

            if (!animal.IsAlive)
                return;

            animal.Died -= OnAnimalDied;
            animal.Died += OnAnimalDied;
        }

        private void OnAnimalDied(Animal animal)
        {
            animal.Died -= OnAnimalDied;

            if (animal.Definition.IsPredator)
                DeadPredators++;
            else
                DeadPrey++;

            Changed?.Invoke();
        }
    }
}
