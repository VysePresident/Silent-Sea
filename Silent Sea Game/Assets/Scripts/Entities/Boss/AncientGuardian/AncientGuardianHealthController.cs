using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AncientGuardianHealthController : BossHealthController
{
    // Ancient Guardian does not take knockback
    public override void TakeHit(int damage, Vector2 knockBack)
    {
        base.TakeHit(damage);
    }
}
