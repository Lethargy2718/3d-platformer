using UnityEngine;

public static class MovementUtils
{
    public static Vector3 ApplyHorizontal(
        Vector3 currentVelocity,
        Vector3 targetDirection,
        float maxSpeed,
        float acceleration,
        float friction,
        float dt)
    {
        Vector3 velocity = new Vector3(currentVelocity.x, 0f, currentVelocity.z);

        Vector3 targetVelocity = targetDirection * maxSpeed;

        bool hasInput = targetDirection.sqrMagnitude > 0.01f;

        if (hasInput)
        {
            // accelerate towards target velocity

            // accelerate faster if trying to move in the opposite direction
            float dot = Vector3.Dot(velocity.normalized, targetDirection);
            if (dot < 0f) // angle > 90
            {
                acceleration *= 4f; // TODO: add to ctx
            }
            velocity = Vector3.MoveTowards(velocity, targetVelocity, acceleration * dt);
        }
        else
        {
            // apply friction with exponential decay

            // -friction to keep the result between 0 and 1
            // subtract from 1 to increase t as friction increases
            float t = 1f - Mathf.Exp(-friction * dt); 
            velocity = Vector3.Lerp(velocity, Vector3.zero, t);
        }

        return new Vector3(velocity.x, currentVelocity.y, velocity.z);
    }
}