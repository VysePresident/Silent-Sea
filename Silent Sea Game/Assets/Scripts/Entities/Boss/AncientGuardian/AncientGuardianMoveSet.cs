using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AncientGuardianMoveSet : BasicMoveSet
{
    public MovementSettings phase2Settings;

    protected override void Start()
    {
        base.Start();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    protected override void CalculateHorizontalMovement()
    {
        base.CalculateHorizontalMovement();

        // Instant stopping
        if (entity.move.x == 0 && (entity as AncientGuardian).stop)
        {
            velocity.x = 0;
        }
    }

    public void BeginPhase2()
    {
        settings = phase2Settings;
        CalculateGravityAndJumpVelocity();
    }
}
