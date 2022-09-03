using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeralShadowMoveSet : BasicMoveSet
{
    public float lowLungeJumpHeight;
    public float highLungeJumpHeight;
    public float horizLungeVelocity;
    public float timeToLungeSpeed = 0.05f;

    protected float cameraSize = 11f;
    protected float lowLungeJumpVelocity;
    protected float highLungeJumpVelocity;
    protected float lowLungeGravity;
    protected float highLungeGravity;

    protected FeralShadow feralShadow;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        feralShadow = (FeralShadow)entity;

        controller = (MovementController)raycastController;

        CalculateGravityAndJumpVelocity();
        CalculateLungeJumpVelocities();
    }

    private void FixedUpdate()
    {
        if (isEnabled)
        {
            if (!Mathf.Approximately(cameraSize, Camera.current.orthographicSize * 2))
            {
                cameraSize = Camera.current.orthographicSize * 2;
                CalculateLungeJumpVelocities();
            }

            UpdateVerticalCollisions();

            // If not lunging or jumping away
            if (feralShadow.currentPhase != FeralShadow.Phases.LungingHigh &&
                feralShadow.currentPhase != FeralShadow.Phases.LungingLow)
            {
                CalculateHorizontalMovement();

                if (entity.jump && (controller.collisions.below || feralShadow.currentPhase == FeralShadow.Phases.JumpingAway) && !jumpAlreadyPressed)
                {
                    jumpAlreadyPressed = true;
                    DoJump();
                }
                else if (!entity.jump && jumpAlreadyPressed)
                {
                    jumpAlreadyPressed = false;

                    if (velocity.y > minJumpVelocity)
                    {
                        DoMinJump();
                    }
                }
            }
            else // if lunging
            {
                if (!jumpAlreadyPressed)
                {
                    jumpAlreadyPressed = true;

                    CalculateLungeHorizontalMovement();

                    if (feralShadow.currentPhase == FeralShadow.Phases.LungingHigh)
                    {
                        DoHighLunge();
                    }
                    else if (feralShadow.currentPhase == FeralShadow.Phases.LungingLow)
                    {
                        DoLowLunge();
                    }
                }
                else if (controller.collisions.below ||
                         (Mathf.Sign(feralShadow.dirLunge) < 0 && controller.collisions.left) ||
                         (Mathf.Sign(feralShadow.dirLunge) > 0 && controller.collisions.right))
                {
                    jumpAlreadyPressed = false;
                    feralShadow.LandFromLunge();
                }
                else
                {
                    CalculateLungeHorizontalMovement();
                }
            }

            AddGravity();

            controller.Move(velocity * Time.deltaTime);

            if (feralShadow.currentPhase == FeralShadow.Phases.LungingHigh ||
                feralShadow.currentPhase == FeralShadow.Phases.LungingLow)
            {
                feralShadow.damageController.currentAttack.controller.Move(velocity * Time.deltaTime);
            }
        }
    }

    protected void CalculateLungeHorizontalMovement()
    {
        float targetXVelocity = entity.move.x * horizLungeVelocity;

        velocity.x = Mathf.SmoothDamp(velocity.x, targetXVelocity, ref velocityXSmoothing, timeToLungeSpeed);
    }

    private void DoLowLunge()
    {
        velocity.y = lowLungeJumpVelocity;
    }

    private void DoHighLunge()
    {
        velocity.y = highLungeJumpVelocity;
    }

    protected override void AddGravity()
    {
        switch (feralShadow.currentPhase)
        {
            case FeralShadow.Phases.LungingLow:
                velocity.y += lowLungeGravity * Time.deltaTime;
                break;
            case FeralShadow.Phases.LungingHigh:
                velocity.y += highLungeGravity * Time.deltaTime;
                break;
            default:
                base.AddGravity();
                break;
        }
    }

    protected void CalculateLungeJumpVelocities()
    {
        //lowLungeJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) + lowLungeJumpHeight);
        //highLungeJumpVelocity = maxJumpVelocity;

        float timeToOppositeSideOfScreen = cameraSize / horizLungeVelocity;
        float timeToLungeJumpApex = timeToOppositeSideOfScreen / 2;

        lowLungeGravity = -(2 * lowLungeJumpHeight) / Mathf.Pow(timeToLungeJumpApex, 2);
        lowLungeJumpVelocity = Mathf.Abs(lowLungeGravity) * timeToLungeJumpApex;

        highLungeGravity = -(2 * highLungeJumpHeight) / Mathf.Pow(timeToLungeJumpApex, 2);
        highLungeJumpVelocity = Mathf.Abs(highLungeGravity) * timeToLungeJumpApex;
    }
}
