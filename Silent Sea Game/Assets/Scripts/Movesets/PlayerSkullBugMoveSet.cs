using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkullBugMoveSet : BasicMoveSet
{
    public bool printTestStatement;

    //[HideInInspector]
    public bool isWallClimbing = false;
    public StickingTo stickingTo;
    public bool cornering;
    protected Vector2 corneringDisplacement;
    protected int postCornerWaitFrames = 5;
    protected int postCornerWaitFramesLeft = 0;

    public bool justTransformed = false;
    private bool initialStickSet = false;
    private bool initialStickToLeft = false;
    private bool initialStickToRight = false;

    protected float velocityYSmoothing;

    protected Vector2 clampedMoveInput;

    public float climbSpeedMultiplier = 0.6f;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        clampedMoveInput = new Vector2();

        ignoreGravity = false;
    }

    // Update is called once per frame
    protected override void FixedUpdate()
    {
        if (isEnabled)
        {
            clampedMoveInput.x = entity.move.x;
            clampedMoveInput.y = entity.move.y;
            if (justTransformed && !initialStickSet)
            {
                initialStickToLeft = controller.collisions.left;
                initialStickToRight = controller.collisions.right;
                initialStickSet = true;
            }

            if (justTransformed)
            {
                controller.collisions.left = initialStickToLeft;
                controller.collisions.right = initialStickToRight;
            }

            if ((CollidingWithWall() && ((entity as Player).isTryingToWallClimb || justTransformed))
                || (cornering && (entity as Player).isTryingToWallClimb))
            {
                isWallClimbing = true;
                stickingTo = GetStuckTo();
            }
            else if (postCornerWaitFramesLeft > 0)
            {
                postCornerWaitFramesLeft--;
            }
            else
            {
                isWallClimbing = false;
                stickingTo = StickingTo.nothing;
            }

            if (justTransformed && ((entity as Player).isTryingToWallClimb || entity.move.x != 0 || entity.move.y != 0))
            {
                justTransformed = false;
            }

            if (cornering) // Ensure the entity doesn't move away from corners
            {
                switch(stickingTo)
                {
                    case StickingTo.topLeftCorner:
                        if (clampedMoveInput.x <= 0) { clampedMoveInput.x = 0; } else { stickingTo = StickingTo.floor; }
                        if (clampedMoveInput.y >= 0) { clampedMoveInput.y = 0; } else { stickingTo = StickingTo.rightWall; }
                        if (velocity.x <= 0) { velocity.x = 0; }
                        if (velocity.y >= 0) { velocity.y = 0; }
                        break;
                    case StickingTo.topRightCorner:
                        if (clampedMoveInput.x >= 0) { clampedMoveInput.x = 0; } else { stickingTo = StickingTo.floor; }
                        if (clampedMoveInput.y >= 0) { clampedMoveInput.y = 0; } else { stickingTo = StickingTo.leftWall; }
                        if (velocity.x >= 0) { velocity.x = 0; }
                        if (velocity.y >= 0) { velocity.y = 0; }
                        break;
                    case StickingTo.bottomLeftCorner:
                        if (clampedMoveInput.x <= 0) { clampedMoveInput.x = 0; } else { stickingTo = StickingTo.ceiling; controller.collisions.above = true; }
                        if (clampedMoveInput.y <= 0) { clampedMoveInput.y = 0; } else { stickingTo = StickingTo.rightWall; }
                        if (velocity.x <= 0) { velocity.x = 0; }
                        if (velocity.y <= 0) { velocity.y = 0; }
                        break;
                    case StickingTo.bottomRightCorner:
                        if (clampedMoveInput.x >= 0) { clampedMoveInput.x = 0; } else { stickingTo = StickingTo.ceiling; controller.collisions.above = true; }
                        if (clampedMoveInput.y <= 0) { clampedMoveInput.y = 0; } else { stickingTo = StickingTo.leftWall; }
                        if (velocity.x >= 0) { velocity.x = 0; }
                        if (velocity.y <= 0) { velocity.y = 0; }
                        break;
                }

                if (stickingTo != StickingTo.topLeftCorner && stickingTo != StickingTo.topRightCorner &&
                    stickingTo != StickingTo.bottomLeftCorner && stickingTo != StickingTo.bottomRightCorner)
                {
                    cornering = false;
                    postCornerWaitFramesLeft = postCornerWaitFrames;
                }
            }

            UpdateVerticalCollisions();

            CalculateHorizontalMovement();

            if (isWallClimbing)
            {
                if (entity.jump && !jumpAlreadyPressed)
                {
                    jumpAlreadyPressed = true;

                    if (controller.collisions.left || controller.collisions.right)
                    {
                        JumpAwayFromWall();
                    }
                    else // controller.collisions.above == true
                    {
                        JumpDownFromCeiling();
                    }
                }
                else if (!entity.jump && jumpAlreadyPressed)
                {
                    jumpAlreadyPressed = false;

                    if (velocity.y > minJumpVelocity)
                    {
                        DoMinJump();
                    }
                }
                else if (!entity.jump && controller.collisions.above)
                {
                    velocity.y = 0.001f;
                }

                CalculateVerticalMovement();

                if (!cornering)
                {
                    entity.ignoreNewLedgeDetection = false;
                    CheckForLedgeWhileClimbing(velocity * Time.deltaTime);
                    entity.ignoreNewLedgeDetection = true;
                }

                if (stickingTo == StickingTo.floor)
                {
                    if (ignoreGravity == false)
                    {
                        AddGravity();
                    }
                }

                CheckForCornering();
            }
            else
            {
                if (entity.jump && controller.collisions.below && !jumpAlreadyPressed)
                {
                    jumpAlreadyPressed = true;
                    DoJump();
                    //animator.SetBool("isJumping", true); //Test starting jump animation
                }
                else if (!entity.jump && jumpAlreadyPressed)
                {
                    jumpAlreadyPressed = false;
                    //animator.SetBool("isJumping", false); //Test ending jump animation

                    if (velocity.y > minJumpVelocity)
                    {
                        DoMinJump();
                    }
                }

                if (ignoreGravity == false)
                {
                    AddGravity();
                }

                entity.ignoreNewLedgeDetection = false;
            }

            if (cornering)
            {
                controller.Move(corneringDisplacement);
            }
            else
            {
                controller.Move(velocity * Time.deltaTime);
            }
        }
    }

    protected virtual void CheckForLedgeWhileClimbing(Vector2 velocity)
    {
        switch (stickingTo)
        {
            case StickingTo.floor:
                controller.CheckForLedge(ref velocity, "down");
                break;
            case StickingTo.leftWall:
                controller.CheckForLedge(ref velocity, "left");
                break;
            case StickingTo.rightWall:
                controller.CheckForLedge(ref velocity, "right");
                break;
            case StickingTo.ceiling:
                controller.CheckForLedge(ref velocity, "up");
                break;
            default:
                break;
        }
    }

    protected virtual void CheckForCornering()
    {
        if (entity.isStandingOnLedge)
        {
            if (cornering)
            {
                corneringDisplacement.x = corneringDisplacement.y = 0;
            }
            else
            {
                switch (stickingTo)
                {
                    case StickingTo.floor:
                        if (entity.facingRight) // If the entity would move past the top right corner of a ledge
                        {
                            Vector2 bottomLeftCorner = controller.main_collider.bounds.min;
                            
                            if ((bottomLeftCorner.x + (velocity * Time.deltaTime).x) >= entity.ledgeEdge.x)
                            {
                                corneringDisplacement = new Vector2(entity.ledgeEdge.x - bottomLeftCorner.x, 0);
                                stickingTo = StickingTo.topRightCorner;
                                cornering = true;
                            }
                        }
                        else // If the entity would move past the top left corner of a ledge
                        {
                            Vector2 bottomRightCorner = controller.main_collider.bounds.min;
                            bottomRightCorner.x += controller.main_collider.bounds.size.x;

                            if ((bottomRightCorner.x + (velocity * Time.deltaTime).x) <= entity.ledgeEdge.x)
                            {
                                corneringDisplacement = new Vector2(entity.ledgeEdge.x - bottomRightCorner.x, 0);
                                stickingTo = StickingTo.topLeftCorner;
                                cornering = true;
                            }
                        }
                        break;
                    case StickingTo.leftWall:
                        if (entity.facingUp) // If the entity would move past the top right corner of a ledge
                        {
                            Vector2 bottomLeftCorner = controller.main_collider.bounds.min;
                            
                            if ((bottomLeftCorner.y + (velocity * Time.deltaTime).y) >= entity.ledgeEdge.y)
                            {
                                corneringDisplacement = new Vector2(0, entity.ledgeEdge.y - bottomLeftCorner.y);
                                stickingTo = StickingTo.topRightCorner;
                                cornering = true;
                            }
                        }
                        else // If the entity would move past the bottom right corner of a ledge
                        {
                            Vector2 topLeftCorner = controller.main_collider.bounds.max;
                            topLeftCorner.x -= controller.main_collider.bounds.size.x;
                            
                            if ((topLeftCorner.y + (velocity * Time.deltaTime).y) <= entity.ledgeEdge.y)
                            {
                                corneringDisplacement = new Vector2(0, entity.ledgeEdge.y - topLeftCorner.y);
                                stickingTo = StickingTo.bottomRightCorner;
                                cornering = true;
                            }
                        }
                        break;
                    case StickingTo.rightWall:
                        if (entity.facingUp) // If the entity would move past the top left corner of a ledge
                        {
                            Vector2 bottomRightCorner = controller.main_collider.bounds.min;
                            bottomRightCorner.x += controller.main_collider.bounds.size.x;

                            if ((bottomRightCorner.y + (velocity * Time.deltaTime).y) >= entity.ledgeEdge.y)
                            {
                                corneringDisplacement = new Vector2(0, entity.ledgeEdge.y - bottomRightCorner.y);
                                stickingTo = StickingTo.topLeftCorner;
                                cornering = true;
                            }
                        }
                        else // If the entity would move past the bottom left corner of a ledge
                        {
                            Vector2 topRightCorner = controller.main_collider.bounds.max;
                            
                            if ((topRightCorner.y + (velocity * Time.deltaTime).y) <= entity.ledgeEdge.y)
                            {
                                corneringDisplacement = new Vector2(0, entity.ledgeEdge.y - topRightCorner.y);
                                stickingTo = StickingTo.bottomLeftCorner;
                                cornering = true;
                            }
                        }
                        break;
                    case StickingTo.ceiling:
                        if (entity.facingRight) // If the entity would move past the bottom right corner of a ledge
                        {
                            Vector2 topLeftCorner = controller.main_collider.bounds.max;
                            topLeftCorner.x -= controller.main_collider.bounds.size.x;

                            if ((topLeftCorner.x + (velocity * Time.deltaTime).x) >= entity.ledgeEdge.x)
                            {
                                corneringDisplacement = new Vector2(entity.ledgeEdge.x - topLeftCorner.x, 0);
                                stickingTo = StickingTo.bottomRightCorner;
                                cornering = true;
                            }
                        }
                        else // If the entity would move past the bottom left corner of a ledge
                        {
                            Vector2 topRightCorner = controller.main_collider.bounds.max;
                            
                            if ((topRightCorner.x + (velocity * Time.deltaTime).x) <= entity.ledgeEdge.x)
                            {
                                corneringDisplacement = new Vector2(entity.ledgeEdge.x - topRightCorner.x, 0);
                                stickingTo = StickingTo.bottomLeftCorner;
                                cornering = true;
                            }
                        }
                        break;
                }
            }
        }
        else if (cornering)
        {
            corneringDisplacement.x = corneringDisplacement.y = 0;
        }
    }

    protected virtual StickingTo GetStuckTo()
    {
        if (cornering)
        {
            // Determine which corner we're stuck to
            return stickingTo;
        }
        else if (controller.collisions.left && !controller.collisions.above) // Stuck to left wall only
        {
            return StickingTo.leftWall;
        }
        else if (controller.collisions.right && !controller.collisions.above) // Stuck to left wall only
        {
            return StickingTo.rightWall;
        }
        else if (controller.collisions.above && !(controller.collisions.left || controller.collisions.right)) // Stuck to ceiling only
        {
            return StickingTo.ceiling;
        }
        else if (!controller.collisions.below) // Not on the floor
        {
            if (controller.collisions.above && entity.move.y > 0)
            {
                return StickingTo.ceiling;
            }
            else
            {
                return (controller.collisions.left) ? StickingTo.leftWall : StickingTo.rightWall;
            }
        }   
        else
        {
            return StickingTo.floor;
        }
    }

    protected override void CalculateHorizontalMovement()
    {
        float targetXVelocity = clampedMoveInput.x * settings.horizontalMoveSpeed * ((isWallClimbing) ? climbSpeedMultiplier : 1);

        if (cornering && targetXVelocity == 0)
        {
            velocity.x = 0;
        }
        else
        {
            velocity.x = Mathf.SmoothDamp(velocity.x, targetXVelocity, ref velocityXSmoothing, (controller.collisions.below) ? settings.groundedTimeToSpeed : settings.airbornTimeToSpeed);
        }
    }

    protected virtual void CalculateVerticalMovement()
    {
        float targetYVelocity;

        if ((controller.collisions.left && clampedMoveInput.x == -1) || (controller.collisions.right && clampedMoveInput.x == 1)) // Move up a wall if player moving into it wall wall climbing
        {
            targetYVelocity = settings.horizontalMoveSpeed * climbSpeedMultiplier;
        }
        else
        {
            targetYVelocity = clampedMoveInput.y * settings.horizontalMoveSpeed * ((isWallClimbing) ? climbSpeedMultiplier : 1);
        }

        if (cornering && targetYVelocity == 0)
        {
            velocity.y = 0;
        }
        else
        {
            velocity.y = Mathf.SmoothDamp(velocity.y, targetYVelocity, ref velocityYSmoothing, settings.groundedTimeToSpeed);
        }
    }

    protected void JumpDownFromCeiling()
    {
        DoMinJump();
        velocity.y *= -1;
    }

    protected void JumpAwayFromWall()
    {
        DoJump();

        velocity.x = maxJumpVelocity * ((controller.collisions.right) ? -1 : 1);
    }

    protected bool CollidingWithWall()
    {
        return (controller.collisions.above || controller.collisions.left || controller.collisions.right || controller.collisions.below) && 
            !(controller.collisions.climbingSlope || controller.collisions.descendingSlope);
    }

    public override void EnableMoveset(Vector2 vel)
    {
        base.EnableMoveset(vel);

        justTransformed = true;
        initialStickSet = false;
    }

    public enum StickingTo
    {
        nothing,
        floor,
        leftWall,
        rightWall,
        ceiling,
        bottomLeftCorner,
        bottomRightCorner,
        topLeftCorner,
        topRightCorner
    }
}
