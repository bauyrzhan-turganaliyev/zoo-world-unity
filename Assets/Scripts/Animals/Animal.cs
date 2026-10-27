using System;
using UnityEngine;

namespace ZooWorld.Animals
{
    public sealed class Animal : MonoBehaviour
    {
        public AnimalDefinition Definition { get; private set; }
        public bool IsAlive { get; private set; }
        public uint SpawnVersion { get; private set; }

        public event Action<Animal> Died;
        public event Action Ate;

        private Action<Animal> _returnToPool;

        public void Initialize(AnimalDefinition definition, Action<Animal> returnToPool)
        {
            if (definition == null)
                throw new ArgumentNullException(nameof(definition));

            if (returnToPool == null)
                throw new ArgumentNullException(nameof(returnToPool));

            if (IsAlive || Definition != null)
                throw new InvalidOperationException("Animal is already initialized");

            Definition = definition;
            _returnToPool = returnToPool;
            IsAlive = true;
            SpawnVersion++;
        }

        internal void NotifyAte()
        {
            if (IsAlive)
                Ate?.Invoke();
        }

        public bool TryDie()
        {
            if (!IsAlive)
                return false;
            if (Definition == null)
                throw new InvalidOperationException("Animal has no definition");

            IsAlive = false;

            try
            {
                Died?.Invoke(this);
            }
            finally
            {
                _returnToPool(this);
            }
            return true;
        }

        public void ResetForPool()
        {
            if (IsAlive)
                throw new InvalidOperationException("Cannot pool a living animal");

            Died = null;
            Ate = null;
            Definition = null;
            _returnToPool = null;
        }
    }
}
