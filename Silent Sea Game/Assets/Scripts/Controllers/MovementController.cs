using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Entity))]
public class MovementController : RaycastController
{
    public CollisionInfo collisions;

    [HideInInspector]
    public Entity entity;

    protected Vector2 ledgeEdgeLocation;

    protected override void Start()
    {
        base.Start();

        entity = GetComponent<Entity>();
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
            VerticalCollisions(ref velocity, standingOnPlatform);
        }

        DetectLedges(ref velocity, standingOnPlatform);

        transform.Translate(velocity);

        if (standingOnPlatform)
        {
            collisions.below = true;
        }
    }

    #region Ledge Detection

    void DetectLedges(ref Vector2 velocity, bool standingOnPlatform)
    {
        if (collisions.below || standingOnPlatform)
        {
            CheckForLedge(ref velocity, "down");
        }
        else if (entity.isStandingOnLedge) // If the Entity was previously standing on a ledge but is no longer standing on anything
        {
            entity.OnMoveAwayFromLedge();
        }
    }

    public void CheckForLedge(ref Vector2 velocity, string downDirName)
    {
        RaycastHit2D[] ledgeCheckHits = new RaycastHit2D[verticalRayCount];
        float ledgeCheckRayLength = ((collisions.descendingSlope) ? Mathf.Abs(velocity.y) : 0f) + entity.maxDropOffDistance + skinWidth;
        Vector2 startingCorner;
        Vector2 nextDir;
        Vector2 downDir;
        float forwardSign;
        float raySpacing;
        int numRays;
        switch (downDirName)
        {
            case "up":
                startingCorner = raycastOrigins.topLeft;
                nextDir = Vector2.right;
                downDir = Vector2.up;
                forwardSign = (Mathf.Sign(velocity.x) != 0) ? Mathf.Sign(velocity.x) : (entity.facingRight) ? 1 : -1;
                raySpacing = verticalRaySpacing;
                numRays = verticalRayCount;
                break;
            case "right":
                startingCorner = raycastOrigins.bottomRight;
                nextDir = Vector2.up;
                downDir = Vector2.right;
                forwardSign = (Mathf.Sign(velocity.y) != 0) ? Mathf.Sign(velocity.y) : (entity.facingUp) ? 1 : -1;
                raySpacing = horizontalRaySpacing;
                numRays = horizontalRayCount;
                break;
            case "left":
                startingCorner = raycastOrigins.bottomLeft;
                nextDir = Vector2.up;
                downDir = Vector2.left;
                forwardSign = (Mathf.Sign(velocity.y) != 0) ? Mathf.Sign(velocity.y) : (entity.facingUp) ? 1 : -1;
                raySpacing = horizontalRaySpacing;
                numRays = horizontalRayCount;
                break;
            case "down":
            default:
                startingCorner = raycastOrigins.bottomLeft;
                nextDir = Vector2.right;
                downDir = Vector2.down;
                forwardSign = (Mathf.Sign(velocity.x) != 0) ? Mathf.Sign(velocity.x) : (entity.facingRight) ? 1 : -1;
                raySpacing = verticalRaySpacing;
                numRays = verticalRayCount;
                break;
        }

        // Cast rays downwards to detect what's being stood on
        for (int i = 0; i < numRays; i++)
        {
            Vector2 rayOrigin = startingCorner + (nextDir * (raySpacing * i));

            ledgeCheckHits[i] = Physics2D.Raycast(rayOrigin, downDir, ledgeCheckRayLength, collisionMask);
            if (entity.DrawLedgeDetectionDebugs) { Debug.DrawRay(rayOrigin, downDir * ledgeCheckRayLength, Color.green); }
        }

        // DETERMINE IF HANGING OFF A LEDGE
        int current = (forwardSign == -1) ? 0 : numRays - 1;
        if (!ledgeCheckHits[current]) // THERE'S NOT COLLISION ON THE FIRST RAY, SO THE ENTITY IS STANDING ON A LEDGE. IN HERE, WE'LL FIND IT
        {
            int toNext = (int)forwardSign * -1;
            int count = 1;
            bool ledgeFound = false;

            current += toNext;
            while (!ledgeFound && count < numRays)
            {
                if (ledgeCheckHits[current])
                {
                    ledgeFound = true;
                }
                else
                {
                    current += toNext;
                    count++; // Probably excessive, but eh
                }
            }

            if (ledgeFound) // This should always be true at this point, but who knows
            {
                Vector2 ledgeCheckRayOrigin = startingCorner;

                ledgeCheckRayOrigin += nextDir * ((current - toNext) * raySpacing + velocity.x);
                ledgeCheckRayOrigin += downDir * (((collisions.descendingSlope) ? ledgeCheckHits[current].distance : 0) + (skinWidth * 2));

                RaycastHit2D ledgeEdgeHit = Physics2D.Raycast(ledgeCheckRayOrigin, nextDir * toNext, raySpacing * 2, collisionMask);

                if (entity.DrawLedgeDetectionDebugs) { Debug.DrawRay(ledgeCheckRayOrigin, nextDir * toNext * raySpacing * 2, Color.green); }

                if (ledgeEdgeHit) // This should also always be true at this point
                {
                    entity.OnLedgeDectected(ledgeEdgeHit.point);
                }
            }
        }
        else if (entity.isStandingOnLedge) // NOT standing on a ledge anymore, but the Entity previously was standing on a ledge
        {
            entity.OnMoveAwayFromLedge();
        }
    }

    void CheckForLedge(ref Vector2 velocity)
    {
        RaycastHit2D[] ledgeCheckHits = new RaycastHit2D[verticalRayCount];
        float ledgeCheckRayLength = ((collisions.descendingSlope) ? Mathf.Abs(velocity.y) : 0f) + entity.maxDropOffDistance + skinWidth;

        // Cast rays downwards to detect what's being stood on
        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin = raycastOrigins.bottomLeft + (Vector2.right * (verticalRaySpacing * i));

            ledgeCheckHits[i] = Physics2D.Raycast(rayOrigin, Vector2.down, ledgeCheckRayLength, collisionMask);
            if (entity.DrawLedgeDetectionDebugs) { Debug.DrawRay(rayOrigin, Vector2.down * ledgeCheckRayLength, Color.green); }
        }

        // DETERMINE IF HANGING OFF A LEDGE
        int current = (Mathf.Sign(velocity.x) == -1) ? 0 : verticalRayCount - 1;
        if (!ledgeCheckHits[current]) // THERE'S NOT COLLISION ON THE FIRST RAY, SO THE ENTITY IS STANDING ON A LEDGE. IN HERE, WE'LL FIND IT
        {
            int toNext = (int)Mathf.Sign(velocity.x) * -1;
            int count = 1;
            bool ledgeFound = false;

            current += toNext;
            while (!ledgeFound && count < verticalRayCount)
            {
                if (ledgeCheckHits[current])
                {
                    ledgeFound = true;
                }

                current += toNext;
                count++; // Probably excessive, but eh
            }

            if (ledgeFound) // This should always be true at this point, but who knows
            {
                Vector2 ledgeCheckRayOrigin = raycastOrigins.bottomLeft;

                ledgeCheckRayOrigin += Vector2.right * ((current + (-1 * toNext)) * verticalRaySpacing + velocity.x);
                ledgeCheckRayOrigin += Vector2.down * (((collisions.descendingSlope) ? ledgeCheckHits[current].distance : 0) + (skinWidth * 2));

                RaycastHit2D ledgeEdgeHit = Physics2D.Raycast(ledgeCheckRayOrigin, Vector2.right * toNext, verticalRaySpacing * 2, collisionMask);

                if (entity.DrawLedgeDetectionDebugs) { Debug.DrawRay(ledgeCheckRayOrigin, Vector2.right * toNext * verticalRaySpacing * 2, Color.green); }

                if (ledgeEdgeHit) // This should also always be true at this point
                {
                    entity.OnLedgeDectected(ledgeEdgeHit.point);
                }
            }
        }
        else if (entity.isStandingOnLedge) // NOT standing on a ledge anymore, but the Entity previously was standing on a ledge
        {
            entity.OnMoveAwayFromLedge();
        }
    }

    #endregion

    #region Collision Detection

    void VerticalCollisions(ref Vector2 velocity, bool standingOnPlatform)
    {
        float dirY = Mathf.Sign(velocity.y);
        float rayLength = Mathf.Abs(velocity.y) + skinWidth;

        RaycastHit2D[] ledgeCheckHits = new RaycastHit2D[verticalRayCount];

        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin = (dirY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i);

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

            Debug.DrawRay(rayOrigin, Vector2.right * dirX * rayLength, Color.black);

            if (hit)
            {
                // prevents abnormal horizontal movement when inside of another object
                if (hit.distance == 0)
                {
                    continue;
                }

                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (i == 0 && slopeAngle <= entity.moveSet.settings.maxIncline) // if the bottom-most ray hits a slope that can be climbed
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

                if (!collisions.climbingSlope || slopeAngle > entity.moveSet.settings.maxIncline)
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

    #endregion

    #region Handle slopes

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
            if (slopeAngle != 0 && slopeAngle <= entity.moveSet.settings.maxDecline)
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

    #region Teleportation

    public void Teleport(Vector2 targetPos)
    {
        // Check whether the Entity will be inside any obstacles
        Vector2 extents = (Vector2)main_collider.bounds.extents;

        RaycastHit2D hitAbove = Physics2D.Raycast(targetPos, Vector2.up, extents.y, collisionMask);
        RaycastHit2D hitBelow = Physics2D.Raycast(targetPos, Vector2.down, extents.y, collisionMask);

        Debug.DrawRay(targetPos, Vector2.up * extents.y, Color.yellow);
        Debug.DrawRay(targetPos, Vector2.down * extents.y, Color.yellow);

        if (hitBelow)
        {
            targetPos.y = hitBelow.point.y + extents.y;
            collisions.below = true;
            collisions.above = false;
        }
        else if (hitAbove)
        {
            targetPos.y = hitAbove.point.y - extents.y;
            collisions.above = true;
            collisions.below = false;
        }
        else
        {
            collisions.above = false;
            collisions.below = false;
        }


        RaycastHit2D hitLeft = Physics2D.Raycast(targetPos, Vector2.left, extents.x, collisionMask);
        RaycastHit2D hitRight = Physics2D.Raycast(targetPos, Vector2.right, extents.x, collisionMask);

        Debug.DrawRay(targetPos, Vector2.left * extents.x, Color.yellow);
        Debug.DrawRay(targetPos, Vector2.right * extents.x, Color.yellow);

        if (hitLeft)
        {
            targetPos.x = hitLeft.point.x + extents.x;
            collisions.left = true;
            collisions.right = false;
        }
        else if (hitRight)
        {
            targetPos.x = hitRight.point.x - extents.x;
            collisions.right = true;
            collisions.left = false;
        }
        else
        {
            collisions.left = false;
            collisions.right = false;
        }

        transform.position = targetPos;
        Physics2D.SyncTransforms();
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
