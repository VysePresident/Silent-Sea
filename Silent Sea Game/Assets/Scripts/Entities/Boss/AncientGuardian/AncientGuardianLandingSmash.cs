using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AncientGuardianLandingSmash : AncientGuardianFist
{
    protected override void Start()
    {
        base.Start();

        hazardCode = "landing smash";
    }

    protected override void DoUpdate()
    {
        if (!AttackOver)
        {
            if (attackTimeLeft <= 0)
            {
                OnAttackOver();
            }
            else
            {
                attackTimeLeft -= Time.deltaTime;
            }
        }

        WaitAndDropIceIfAttackOver();
    }

    protected override void DoKnockBackHit(GameObject playerObject)
    {
        float xDir = Mathf.Sign(playerObject.transform.position.x - transform.position.x);
        Vector2 kb = new Vector2(knockBack.x, knockBack.y);

        kb.x *= xDir;

        playerObject.GetComponent<Health_Controller>().TakeHit(AttackDamage, kb);
    }
}
