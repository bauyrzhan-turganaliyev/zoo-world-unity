using UnityEngine;

namespace ZooWorld.Arena
{
    public interface IArenaBounds
    {
        bool TryGetRandomPoint(float height, out Vector3 point);
        
        Vector3 KeepInside(Vector3 position, Vector3 direction, float lookAheadDistance);
    }
}
