using System;
using UnityEngine;

namespace ZooWorld.Arena
{
    public sealed class CameraArenaBounds : IArenaBounds
    {
        private readonly Camera _camera;
        private readonly float _viewportMargin;

        public CameraArenaBounds(Camera camera, float viewportMargin)
        {
            _camera = camera != null ? camera : throw new ArgumentNullException(nameof(camera));
            _viewportMargin = Mathf.Clamp(viewportMargin, 0f, 0.45f);
        }

        public bool TryGetRandomPoint(float height, out Vector3 point)
        {
            float x = UnityEngine.Random.Range(_viewportMargin, 1f - _viewportMargin);
            float y = UnityEngine.Random.Range(_viewportMargin, 1f - _viewportMargin);
            return TryProject(new Vector3(x, y, 0f), height, out point);
        }

        public Vector3 KeepInside(Vector3 position, Vector3 direction, float lookAheadDistance)
        {
            direction.y = 0f;
            if (direction.sqrMagnitude < 0.0001f)
                direction = Vector3.forward;
            direction.Normalize();

            Vector3 future = position + direction * lookAheadDistance;
            if (IsInside(future))
                return direction;

            if (!TryProject(new Vector3(0.5f, 0.5f, 0f), position.y, out Vector3 center))
                return -direction;

            Vector3 towardCenter = center - position;
            towardCenter.y = 0f;
            return towardCenter.sqrMagnitude > 0.0001f
                ? towardCenter.normalized
                : -direction;
        }

        private bool IsInside(Vector3 worldPoint)
        {
            Vector3 viewport = _camera.WorldToViewportPoint(worldPoint);
            return viewport.z > 0f &&
                   viewport.x >= _viewportMargin &&
                   viewport.x <= 1f - _viewportMargin &&
                   viewport.y >= _viewportMargin &&
                   viewport.y <= 1f - _viewportMargin;
        }

        private bool TryProject(Vector3 viewportPoint, float height, out Vector3 point)
        {
            Ray ray = _camera.ViewportPointToRay(viewportPoint);
            Plane plane = new Plane(Vector3.up, new Vector3(0f, height, 0f));

            if (plane.Raycast(ray, out float distance))
            {
                point = ray.GetPoint(distance);
                return true;
            }

            point = default;
            return false;
        }
    }
}
