using UnityEngine;
using Zenject;
using ZooWorld.FoodChain;

namespace ZooWorld.Animals
{
    [RequireComponent(typeof(Animal), typeof(Collider))]
    public sealed class AnimalCollision : MonoBehaviour
    {
        private Animal _animal;
        private FoodChainResolver _foodChain;

        private void Awake()
        {
            _animal = GetComponent<Animal>();
        }

        [Inject]
        private void Construct(FoodChainResolver foodChain)
        {
            _foodChain = foodChain;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (_foodChain == null || !_animal.IsAlive)
                return;

            Animal other = collision.collider.GetComponentInParent<Animal>();
            if (other == null || other == _animal)
                return;

            _foodChain.TryResolve(_animal, other, out _);
        }
    }
}
