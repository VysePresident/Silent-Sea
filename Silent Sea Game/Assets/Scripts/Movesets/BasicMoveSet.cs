using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMoveSet : MoveSet
{
    public float gravity;
    public float maxJumpVelocity;
    public float minJumpVelocity;
    public float jumpMultiplier = 1;
    [HideInInspector]
    public bool jumpAlreadyPressed;
    public Animator animator; //Animator instantiated.

    protected float velocityXSmoothing;

    private bool transition;

    [HideInInspector]
    public MovementController controller;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        controller = (MovementController)raycastController;

        transition = false;

        ignoreGravity = false;

        CalculateGravityAndJumpVelocity();
    }

    // Update is called once per frame
    protected virtual void FixedUpdate()
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

            if (ignoreGravity == false)
            {
                AddGravity();
            }
            controller.Move(velocity * Time.deltaTime);
        }
    }

    protected void UpdateVerticalCollisions()
    {
        if (controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0;
        }
    }

    protected virtual void CalculateHorizontalMovement()
    {
        float targetXVelocity = entity.move.x * settings.horizontalMoveSpeed;
        if (transition == false)
        {
            velocity.x = Mathf.SmoothDamp(velocity.x, targetXVelocity, ref velocityXSmoothing, (controller.collisions.below) ? settings.groundedTimeToSpeed : settings.airbornTimeToSpeed);
        }
    }

    protected void CalculateGravityAndJumpVelocity()
    {
        gravity = -(2 * settings.maxJumpHeight) / Mathf.Pow(settings.timeToJumpApex, 2);
        maxJumpVelocity = Mathf.Abs(gravity) * settings.timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) + settings.minJumpHeight);
    }

    protected void DoJump()
    {
        velocity.y = maxJumpVelocity;
    }

    protected void DoMinJump()
    {
        velocity.y = minJumpVelocity;
    }

    public void UpdateJumpMultiplier(float multiplier)
    {
        jumpMultiplier = multiplier;
        CalculateGravityAndJumpVelocity();
    }

    protected virtual void AddGravity()
    {
        velocity.y += gravity * Time.deltaTime;
    }

    public void LockVelocity()
    {
        float lockX = velocity.x;
        float lockY = velocity.y;
        velocity.x = lockX;
        velocity.y = lockY;
        transition = true;
    }

    public struct JumpStatus
    {
        public int maxJumps;
        public int jumpsLeft;
        public JumpState jumpState;
        public bool readyToJumpAgain;

        public void StartJump()
        {
            jumpState = JumpState.Jumping;
            jumpsLeft--;
            readyToJumpAgain = false;
        }

        public void Reset()
        {
            jumpState = JumpState.Grounded;
            jumpsLeft = maxJumps;
        }

        public JumpStatus(int jumps, JumpState state, bool ready)
        {
            jumpsLeft = maxJumps = jumps;
            jumpState = state;
            readyToJumpAgain = ready;
        }
    }

    public enum JumpState
    {
        Grounded,
        Jumping,    // NOTE: Indicates jump has not 
        Falling     // NOTE: An object can have a positive y velocity and still be considered "falling"
    }
}