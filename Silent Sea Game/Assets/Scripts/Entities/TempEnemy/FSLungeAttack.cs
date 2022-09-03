using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSLungeAttack : AttackHitBox
{
    protected MovementController playerMovementController;

    protected override void Update()
    {
        // Nothing!
    }

    protected override void DoOvertimeHit(GameObject playerObject)
    {
        if (!AlreadyHit)
        {
            playerObject.GetComponent<Health_Controller>().TakeHit(AttackDamage);

            AlreadyHit = true;
        }

        if (playerMovementController == null)
        {
            playerMovementController = playerObject.GetComponent<MovementController>();
        }

        playerMovementController.Move(controller.oldVelocity);
    }
}
