using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DBMeleeAttack : AttackHitBox
{
    public Deadbones parent;

    protected override void DoKnockBackHit(GameObject playerObject)
    {
        Vector2 kb = knockBack;

        if (controller.oldVelocity.x != 0)
        {
            float xDir = Mathf.Sign(controller.oldVelocity.x);
            kb.x *= xDir;
        }
        else
        {
            kb.x *= (parent.facingRight) ? 1 : -1;
        }

        playerObject.GetComponent<Health_Controller>().TakeHit(AttackDamage, kb);
    }
}
