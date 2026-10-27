using UnityEngine;
using Zenject;
using ZooWorld.Arena;

namespace ZooWorld.Movement
{
    [RequireComponent(typeof(Rigidbody), typeof(Collider))]
    public sealed class ContinuousMovement : MonoBehaviour
    {
        [SerializeField, Min(0.1f)] private float speed = 2.5f;
        [SerializeField, Min(0.1f)] private float maxAcceleration = 18f;
        [SerializeField, Min(1f)] private float turnSpeed = 360f;
        [SerializeField, Min(0.1f)] private float minDirectionInterval = 2f;
        [SerializeField, Min(0.1f)] private float maxDirectionInterval = 4f;
        [SerializeField, Min(0.1f)] private float lookAheadDistance = 1.5f;

        private IArenaBounds _arena;
        private Rigidbody _body;
        private Vector3 _direction;
        private float _nextDirectionTime;

        [Inject]
        private void Construct(IArenaBounds arena)
        {
            _arena = arena;
        }

        private void Awake()
        {
            _body = GetComponent<Rigidbody>();
        }

        private void OnEnable()
        {
            ChooseDirection();
        }

        private void FixedUpdate()
        {
            if (Time.time >= _nextDirectionTime)
                ChooseDirection();

            if (_arena != null)
                _direction = _arena.KeepInside(_body.position, _direction, lookAheadDistance);

            Vector3 currentVelocity = _body.linearVelocity;
            Vector3 horizontalVelocity = new Vector3(currentVelocity.x, 0f, currentVelocity.z);
            Vector3 desiredVelocity = _direction * speed;
            Vector3 acceleration = Vector3.ClampMagnitude(
                (desiredVelocity - horizontalVelocity) / Time.fixedDeltaTime, maxAcceleration);
            _body.AddForce(acceleration, ForceMode.Acceleration);

            Quaternion targetRotation = Quaternion.LookRotation(_direction, Vector3.up);
            Quaternion rotation = Quaternion.RotateTowards(
                _body.rotation, targetRotation, turnSpeed * Time.fixedDeltaTime);
            _body.MoveRotation(rotation);
        }

        private void ChooseDirection()
        {
            _direction = Directions.RandomHorizontal();
            _nextDirectionTime = Time.time + Random.Range(minDirectionInterval, maxDirectionInterval);
        }

        private void OnValidate()
        {
            maxDirectionInterval = Mathf.Max(minDirectionInterval, maxDirectionInterval);
        }
    }
}
