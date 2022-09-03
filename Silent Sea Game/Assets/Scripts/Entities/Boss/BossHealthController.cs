using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHealthController : Health_Controller
{
    protected BossEnemy boss;

    protected int DamageLeftUntilNextPhase;

    protected bool LastPhase = false;

    public override void Start()
    {
        base.Start();
        boss = GetComponent<BossEnemy>();

        DamageLeftUntilNextPhase = MaxHitPoints / 2;
    }

    protected void Update()
    {
        /*
         * PUT STUFF HERE TO CONTROL THE BIG BOSS HEALTHBAR AND STUFF
         * DISPLAY IT'S CURRENT HEALTH AND IT'S NAME
         */
    }

    public override void TakeHit(int damage)
    {
        base.TakeHit(damage);

        DamageLeftUntilNextPhase -= damage;

        if (Hitpoints <= 0)
        {
            boss.OnDeath();
        }
        else if (!LastPhase && DamageLeftUntilNextPhase <= 0)
        {
            boss.StartNextPhase();
            
            if (boss.currentPhase >= boss.numPhases)
            {
                LastPhase = true;
            }
            else
            {
                DamageLeftUntilNextPhase = Hitpoints / 2;
            }
        }
    }
}
