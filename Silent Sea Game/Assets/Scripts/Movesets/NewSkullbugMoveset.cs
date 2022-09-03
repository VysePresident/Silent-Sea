using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewSkullbugMoveset : BasicMoveSet
{
    protected float velocityYSmoothing;

    Skullbug skullbug;

    protected override void Start()
    {
        base.Start();

        skullbug = GetComponent<Skullbug>();
    }

    void FixedUpdate()
    {
        if (isEnabled)
        {
            UpdateVerticalCollisions();

            CalculateHorizontalMovement();

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

            if (!(skullbug.isClimbingOrDescending || skullbug.isClimbingOnCeiling))
            {
                AddGravity();
            }

            if (skullbug.isClimbingOrDescending || skullbug.isClimbingOnCeiling)
            {
                CalculateVerticalMovement();
            }

            controller.Move(velocity * Time.deltaTime);
        }

    }

    void CalculateVerticalMovement()
    {
        float targetYVelocity = entity.move.y * settings.verticalMoveSpeed;

        velocity.y = Mathf.SmoothDamp(velocity.y, targetYVelocity, ref velocityYSmoothing, (controller.collisions.below) ? settings.groundedTimeToSpeed : settings.airbornTimeToSpeed);
    }

    bool isClimbing(bool isClimbing)
    {
        return (controller.collisions.above || controller.collisions.left || controller.collisions.right);
    }
}
