using UnityEngine;

namespace ZooWorld.Movement
{
    internal static class Directions
    {
        public static Vector3 RandomHorizontal()
        {
            Vector2 direction = Random.insideUnitCircle;
            if (direction.sqrMagnitude < 0.0001f)
                return Vector3.forward;

            direction.Normalize();
            return new Vector3(direction.x, 0f, direction.y);
        }
    }
}
