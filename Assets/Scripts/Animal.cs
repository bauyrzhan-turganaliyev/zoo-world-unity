using System;
using UnityEngine;

public sealed class Animal : MonoBehaviour
{
    public AnimalDefinition Definition { get; private set; }
    public bool IsAlive { get; private set; } = true;

    public event Action<Animal> Died;

    public void Initialize(AnimalDefinition definition)
    {
        if (definition == null)
            throw new ArgumentNullException(nameof(definition));

        if (Definition != null)
            throw new InvalidOperationException("Already initialized");

        Definition = definition;
    }

    public bool TryDie()
    {
        if (Definition == null)
            throw new InvalidOperationException("Not initialized");

        if (!IsAlive)
            return false;

        IsAlive = false;
        Died?.Invoke(this);
        Destroy(gameObject);
        return true;
    }
}