using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackHitBox : MonoBehaviour
{
    public BasicController controller;

    public int AttackDamage;

    public AttackType TypeOfAttack;

    public float AttackDuration;

    public bool AttackOver = false;

    public Vector2 knockBack;

    protected float attackTimeLeft;

    // Single-hit variables
    public bool AlreadyHit { protected set; get; } = false;

    // Overtime-hit variables
    public float DamageProcCooldown { protected set; get; } = 1f;

    protected float timeToNextDamageProc = -1f;

    protected virtual void Start()
    {
        controller = GetComponent<BasicController>();

        attackTimeLeft = AttackDuration;
    }

    protected virtual void Update()
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
    }

    protected virtual void OnAttackOver()
    {
        AttackOver = true;
    }

    protected virtual void OnPlayerHit(GameObject playerObject)
    {
        switch(TypeOfAttack)
        {
            case AttackType.OvertimeHit:
                DoOvertimeHit(playerObject);
                break;
            case AttackType.SingleHit:
            default:
                DoSingleHit(playerObject);
                break;
        }
    }

    protected virtual void DoOvertimeHit(GameObject playerObject)
    {
        if (timeToNextDamageProc <= 0)
        {
            playerObject.GetComponent<Health_Controller>().TakeHit(AttackDamage);
            timeToNextDamageProc = DamageProcCooldown;
        }
        else
        {
            timeToNextDamageProc -= Time.deltaTime;
        }
    }

    protected virtual void DoSingleHit(GameObject playerObject)
    {
        if (!AlreadyHit)
        {
            if (knockBack != Vector2.zero)
            {
                DoKnockBackHit(playerObject);
            }
            else
            {
                playerObject.GetComponent<Health_Controller>().TakeHit(AttackDamage);
            }

            AlreadyHit = true;
        }
    }

    protected virtual void DoKnockBackHit(GameObject playerObject)
    {
        if (controller.oldVelocity.x != 0)
        {
            float xDir = Mathf.Sign(controller.oldVelocity.x);
            Vector2 kb = knockBack;
            kb.x *= xDir;

            playerObject.GetComponent<Health_Controller>().TakeHit(AttackDamage, kb);

        }
        else
        {
            playerObject.GetComponent<Health_Controller>().TakeHit(AttackDamage);

        }
    }

    public void Move(Vector2 displacement)
    {
        controller.Move(displacement);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            OnPlayerHit(collision.gameObject);
        }
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (TypeOfAttack == AttackType.OvertimeHit)
            {
                DoOvertimeHit(collision.gameObject);
            }
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (TypeOfAttack == AttackType.OvertimeHit)
            {
                timeToNextDamageProc = -1f;
            }
        }
    }

    public enum AttackType
    {
        SingleHit,
        OvertimeHit
    }
}
