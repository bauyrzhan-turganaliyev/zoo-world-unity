using UnityEngine;

internal static class ScreenMovement
{
    public static Vector3 RandomDirection()
    {
        Vector2 direction = Random.insideUnitCircle;
        if (direction.sqrMagnitude < 0.0001f)
            return Vector3.forward;

        direction.Normalize();
        return new Vector3(direction.x, 0f, direction.y);
    }

    public static Vector3 KeepOnScreen(
        Camera camera,
        Vector3 position,
        Vector3 direction,
        float lookAheadDistance,
        float viewportMargin)
    {
        direction.y = 0f;
        if (direction.sqrMagnitude < 0.0001f)
            direction = Vector3.forward;
        direction.Normalize();

        if (camera == null)
            return direction;

        Vector3 future = position + direction * lookAheadDistance;
        Vector3 viewport = camera.WorldToViewportPoint(future);

        bool visible = viewport.z > 0f &&
                       viewport.x >= viewportMargin &&
                       viewport.x <= 1f - viewportMargin &&
                       viewport.y >= viewportMargin &&
                       viewport.y <= 1f - viewportMargin;

        if (visible)
            return direction;

        Ray centerRay = camera.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        Plane movementPlane = new Plane(Vector3.up, position);
        if (!movementPlane.Raycast(centerRay, out float distance))
            return -direction;

        Vector3 towardCenter = centerRay.GetPoint(distance) - position;
        towardCenter.y = 0f;
        return towardCenter.sqrMagnitude > 0.0001f
            ? towardCenter.normalized
            : -direction;
    }
}