using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicController : RaycastController
{
    public CollisionInfo collisions;

    float maxIncline, maxDecline = 0;

    public Vector2 oldVelocity;

    protected override void Start()
    {
        base.Start();
    }

    public virtual void Move(Vector2 velocity, bool standingOnPlatform = false)
    {
        UpdateRaycastOrigins();
        collisions.Reset();
        collisions.velocityOld = velocity;

        if (velocity.y < 0 && velocity.x != 0)
        {
            DescendSlope(ref velocity);
        }
        if (velocity.x != 0)
        {
            HorizontalCollisions(ref velocity);
        }
        if (velocity.y != 0)
        {
            VerticalCollisions(ref velocity);
        }

        transform.Translate(velocity);

        if (standingOnPlatform)
        {
            collisions.below = true;
        }

        oldVelocity = velocity;
    }

    #region Collision Detection

    void VerticalCollisions(ref Vector2 velocity)
    {
        float dirY = Mathf.Sign(velocity.y);
        float rayLength = Mathf.Abs(velocity.y) + skinWidth;

        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin = (dirY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * dirY, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.up * dirY * rayLength, Color.red);

            if (hit)
            {
                velocity.y = (hit.distance - skinWidth) * dirY;
                rayLength = hit.distance;

                // Prevents jittering when hitting an object from below while climbing a slope
                if (collisions.climbingSlope)
                {
                    velocity.x = velocity.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x);
                }

                collisions.below = dirY == -1;
                collisions.above = dirY == 1;
            }
        }

        // Makes transitions from one slope to another smoother
        if (collisions.climbingSlope)
        {
            float dirX = Mathf.Sign(velocity.x);
            rayLength = Mathf.Abs(velocity.x) + skinWidth;
            Vector2 rayOrigin = ((dirX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) + Vector2.up * velocity.y;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * dirX, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.up * dirY * rayLength, Color.red);

            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                if (slopeAngle != collisions.slopeAngle)
                {
                    velocity.x = (hit.distance - skinWidth) * dirX;
                    collisions.slopeAngle = slopeAngle;
                }
            }
        }
    }

    void HorizontalCollisions(ref Vector2 velocity)
    {
        float dirX = Mathf.Sign(velocity.x);
        float rayLength = Mathf.Abs(velocity.x) + skinWidth;

        for (int i = 0; i < horizontalRayCount; i++)
        {
            Vector2 rayOrigin = (dirX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * (horizontalRaySpacing * i);

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * dirX, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.right * dirX * rayLength, Color.red);

            if (hit)
            {
                // prevents abnormal horizontal movement when inside of another object
                if (hit.distance == 0)
                {
                    continue;
                }

                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                if (i == 0 && slopeAngle <= maxIncline) // if the bottom-most ray hits a slope that can be climbed
                {
                    float distanceToSlopeStart = 0;

                    // Prevents slowing down transissioning from descending to ascending
                    if (collisions.descendingSlope)
                    {
                        collisions.descendingSlope = false;
                        velocity = collisions.velocityOld;
                    }

                    // ensures the entity gets all the way to the slope
                    if (slopeAngle != collisions.slopeAngleOld)
                    {
                        distanceToSlopeStart = hit.distance - skinWidth;
                        velocity.x -= distanceToSlopeStart * dirX;
                    }

                    ClimbSlope(ref velocity, slopeAngle);

                    // ensures the entity gets all the way to the slope
                    velocity.x += distanceToSlopeStart * dirX;
                }

                if (!collisions.climbingSlope || slopeAngle > maxIncline)
                {
                    velocity.x = (hit.distance - skinWidth) * dirX;
                    rayLength = hit.distance;

                    // prevents jittering when colliding with an object in front of an entity while climbing
                    if (collisions.climbingSlope)
                    {
                        velocity.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x);
                    }

                    collisions.left = dirX == -1;
                    collisions.right = dirX == 1;
                }
            }
        }
    }

    void ClimbSlope(ref Vector2 velocity, float slopeAngle)
    {
        float moveDistance = Mathf.Abs(velocity.x);
        float climbVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;

        if (velocity.y <= climbVelocityY)
        {
            velocity.y = climbVelocityY;
            velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);

            collisions.below = true;
            collisions.climbingSlope = true;
            collisions.slopeAngle = slopeAngle;
        }
    }

    void DescendSlope(ref Vector2 velocity)
    {
        float dirX = Mathf.Sign(velocity.x);
        Vector2 rayOrigin = (dirX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;

        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, Mathf.Infinity, collisionMask);

        if (hit)
        {
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            if (slopeAngle != 0 && slopeAngle <= maxDecline)
            {
                if (Mathf.Sign(hit.normal.x) == dirX)
                {
                    if (hit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x))
                    {
                        float moveDistance = Mathf.Abs(velocity.x);
                        float descendVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
                        velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
                        velocity.y -= descendVelocityY;

                        collisions.slopeAngle = slopeAngle;
                        collisions.descendingSlope = true;
                    }
                }
            }
        }
    }

    #endregion

    public struct CollisionInfo
    {
        public bool above, below;
        public bool left, right;

        public bool climbingSlope, descendingSlope;
        public float slopeAngle, slopeAngleOld;

        public Vector2 velocityOld;

        public void Reset()
        {
            above = below = false;
            left = right = false;
            climbingSlope = false;
            descendingSlope = false;

            slopeAngleOld = slopeAngle;
            slopeAngle = 0;
        }
    }
}