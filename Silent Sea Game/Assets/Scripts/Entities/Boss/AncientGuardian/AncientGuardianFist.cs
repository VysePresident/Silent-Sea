using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AncientGuardianFist : AttackHitBox
{
    public AncientGuardianCeilingIceHandler iceHandler;

    public float IceDropDelay; // Time in s between finishing the attack and dropping ice from the ceiling

    protected float iceDropDelayTimeLeft; // Timer

    public bool IceDropped; // Attack is really over now

    protected string hazardCode = "fist";

    protected override void Start()
    {
        base.Start();

        TypeOfAttack = AttackType.SingleHit;
    }

    protected override void Update()
    {
        DoUpdate();
    }

    protected virtual void DoUpdate()
    {
        CheckAttackEnding();

        WaitAndDropIceIfAttackOver();
    }

    // If the Ancient Guardian's fist encounters a floor/wall before completing its move, end the attack
    void CheckAttackEnding()
    {
        if (!AttackOver)
        {
            if (controller.collisions.below || controller.collisions.left || controller.collisions.right)
            {
                OnAttackOver();
            }
        }
    }

    protected void WaitAndDropIceIfAttackOver()
    {
        if (AttackOver && !IceDropped)
        {
            if (iceDropDelayTimeLeft <= 0)
            {
                iceHandler.TriggerHazard(hazardCode);
                IceDropped = true;
            }
            else
            {
                iceDropDelayTimeLeft -= Time.deltaTime;
            }
        }
    }

    protected override void OnAttackOver()
    {
        iceDropDelayTimeLeft = IceDropDelay;

        base.OnAttackOver();
    }
}