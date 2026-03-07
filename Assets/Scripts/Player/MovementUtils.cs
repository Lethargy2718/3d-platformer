using UnityEngine;

public static class MovementUtils
{
    public static Vector3 ApplyHorizontal(Vector3 current, Vector3 targetDirection, float maxSpeed, float acceleration, float deceleration, float dt)
    {
        Vector3 xz = new Vector3(current.x, 0f, current.z);
        Vector3 targetXZ = targetDirection * maxSpeed;

        float force = targetDirection.sqrMagnitude > 0.01f ? acceleration : deceleration;
        xz = Vector3.MoveTowards(xz, targetXZ, force * dt);

        return new Vector3(xz.x, current.y, xz.z);
    }
}