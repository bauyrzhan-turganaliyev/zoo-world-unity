using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public sealed class FrogMovement : MonoBehaviour
{
    [Header("Jump")]
    [SerializeField, Min(0.1f)] private float jumpDistance = 2.5f;
    [SerializeField, Min(0.1f)] private float jumpHeight = 0.8f;
    [SerializeField, Min(0f)] private float jumpInterval = 1.25f;

    [Header("Arena")]
    [SerializeField, Range(0f, 0.25f)] private float viewportMargin = 0.08f;
    private Camera _arenaCamera;
    
    private Rigidbody _body;
    private bool _grounded;
    private float _nextJumpTime;

    private void Awake()
    {
        _body = GetComponent<Rigidbody>();
        if (_arenaCamera == null)
            _arenaCamera = Camera.main;
    }

    private void FixedUpdate()
    {
        if (_grounded && Time.time >= _nextJumpTime)
            Jump();
    }

    private void OnCollisionEnter(Collision collision) => CheckLanding(collision);
    private void OnCollisionStay(Collision collision) => CheckLanding(collision);

    private void CheckLanding(Collision collision)
    {
        if (_grounded || _body.linearVelocity.y > 0.05f)
            return;

        for (int i = 0; i < collision.contactCount; i++)
        {
            if (collision.GetContact(i).normal.y < 0.65f)
                continue;

            _grounded = true;
            _nextJumpTime = Time.time + jumpInterval;

            Vector3 velocity = _body.linearVelocity;
            _body.linearVelocity = new Vector3(0f, Mathf.Min(velocity.y, 0f), 0f);
            return;
        }
    }

    private void Jump()
    {
        float gravity = -Physics.gravity.y;
        if (gravity <= 0.01f)
            return;

        Vector3 direction = ScreenMovement.RandomDirection();
        direction = ScreenMovement.KeepOnScreen(
            _arenaCamera, _body.position, direction, jumpDistance, viewportMargin);

        float verticalSpeed = Mathf.Sqrt(2f * gravity * jumpHeight);
        float flightTime = 2f * verticalSpeed / gravity;
        float horizontalSpeed = jumpDistance / flightTime;

        _grounded = false;
        _body.linearVelocity = direction * horizontalSpeed + Vector3.up * verticalSpeed;
    }
}