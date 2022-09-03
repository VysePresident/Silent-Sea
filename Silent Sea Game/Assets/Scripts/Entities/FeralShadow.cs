using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeralShadow : Enemy
{
    public FeralShadowMoveSet feralShadowMoveSet;
    public Phases currentPhase = Phases.Patrolling;

    public Animator animator;

    // Movement pattern and attack timing variables
    public float minBackupTime = 3f;
    public float maxBackupTime = 6f;
    public float backupTimeLeft;

    public float duckingTime = 2.5f;
    protected float duckTimeLeft = -1;

    public float lungePrepTime = 0.3f;
    public float rushedLungePrepTime = 0.1f;
    protected float lungePrepTimeLeft;
    [HideInInspector]
    public float dirLunge;

    [HideInInspector]
    public FeralShadow_Damage_Controller damageController;

    public float lashOutRange;
    public float meleeRange;
    public float meleeTestTime = 0.3f;
    public float meleeTestTimeLeft;

    protected override void Start()
    {
        base.Start();

        moveSet = feralShadowMoveSet = GetComponent<FeralShadowMoveSet>();
        damageController = GetComponent<FeralShadow_Damage_Controller>();
        enemyType = Player.PlayerForm.Feral_Shadow;
    }

    protected virtual void Update()
    {
        if (!isDead)
        {
            DoUpdate();
        }
    }

    protected virtual void DoUpdate()
    {
        switch(currentPhase)
        {
            case Phases.Patrolling:
                animator.SetTrigger("Walk");
                if (PlayerInAgroRange())
                {
                    EnterBackingUpPhase();
                } // can add more else-if statements here to include ducking in to swipe at the player
                else
                {
                    Patrol();
                }
                break;
            case Phases.BackingUp:
                FlipTowardsPlayer();
                animator.SetTrigger("Walk");
                Backup();
                break;
            case Phases.PreppingLowLunge:
            case Phases.PreppingHighLunge:
            case Phases.RushedPreppingLowLunge:
            case Phases.RushedPreppingHighLunge:
                FlipTowardsPlayer();
                if (lungePrepTimeLeft <= 0)
                {
                    animator.SetTrigger("Snarl");
                    EnterLungingPhase();
                }
                else if (DistanceToPlayer() < lashOutRange)
                {
                    currentPhase = Phases.DuckingTowardsPlayer;
                }
                else
                {
                    animator.SetTrigger("Snarl");
                    PrepareToLunge();
                }
                break;
            case Phases.LungingHigh:
                animator.SetTrigger("Jump");
                LungeAttack();
                break;
            case Phases.LungingLow:
                animator.SetTrigger("Jump");
                LungeAttack();
                break;
            case Phases.DuckingTowardsPlayer:
                FlipTowardsPlayer();
                DuckTowardsPlayer();
                break;
            case Phases.MeleeAttacking:
                animator.SetTrigger("Swipe");
                MeleeAttack();
                break;
            case Phases.JumpingAway:
                animator.SetTrigger("Backflip");
                JumpAway();
                break;
        }
    }

    protected void EnterBackingUpPhase()
    {
        jump = false;

        FlipTowardsPlayer();

        backupTimeLeft = Random.Range(minBackupTime, maxBackupTime);

        currentPhase = Phases.BackingUp;
    }

    protected void EnterLungePreppingPhase()
    {
        move.x = 0;
        move.y = 0;

        // Randomly determine which type of lunge to perform
        int lowOrHigh = Random.Range(0, 10);
        if (lowOrHigh < 5)
        {
            currentPhase = Phases.PreppingLowLunge;
        }
        else
        {
            currentPhase = Phases.PreppingHighLunge;
        }

        dirLunge = player.transform.position.x - transform.position.x;

        lungePrepTimeLeft = lungePrepTime;
    }

    protected void EnterRushedLungePreppingPhase()
    {
        // Re-use some code
        EnterLungePreppingPhase();

        // Change things to the rushed versions
        currentPhase = (currentPhase == Phases.PreppingHighLunge) ? Phases.RushedPreppingHighLunge : Phases.RushedPreppingLowLunge;
        lungePrepTimeLeft = rushedLungePrepTime;
    }

    protected void EnterLungingPhase()
    {
        currentPhase = (currentPhase == Phases.PreppingHighLunge) ? Phases.LungingHigh : Phases.LungingLow;
        jump = true;
        move.x = (facingRight) ? 1 : -1;
    }

    protected void EnterMeleeAttackingPhase()
    {
        currentPhase = Phases.MeleeAttacking;
        meleeTestTimeLeft = meleeTestTime;
    }

    protected void Backup()
    {
        if ((facingRight && feralShadowMoveSet.controller.collisions.left || !facingRight && feralShadowMoveSet.controller.collisions.right) &&
            !feralShadowMoveSet.controller.collisions.climbingSlope)
        {
            EnterRushedLungePreppingPhase();
        }
        else if (DistanceToPlayer() < 3)
        {
            EnterMeleeAttackingPhase();
        }
        else if (backupTimeLeft <= 0 && !(DistanceToPlayer() >= Camera.current.orthographicSize * 2))
        {
            EnterLungePreppingPhase();
        }
        else if (DistanceToPlayer() >= Camera.current.orthographicSize * 2)
        {
            move.x = Mathf.Sign(player.transform.position.x - transform.position.x) * 0.5f; // Move away from the player

            backupTimeLeft -= Time.deltaTime;
        }
        else
        {
            move.x = Mathf.Sign(transform.position.x - player.transform.position.x) * 0.3f; // Move away from the player

            backupTimeLeft -= Time.deltaTime;
        }
    }

    protected void DuckTowardsPlayer()
    {
        if (duckTimeLeft == -1)
        {
            duckTimeLeft = duckingTime;
        }

        if (duckingTime <= 0)
        {
            duckTimeLeft = -1;
            EnterBackingUpPhase();
        }
        else
        {
            Vector2 meleeAttackSpawnPos = transform.position;
            meleeAttackSpawnPos.x += damageController.attackHitBoxOffset.x * ((facingRight) ? 1 : -1);
            meleeAttackSpawnPos.y += damageController.attackHitBoxOffset.y;

            if (Vector2.Distance(player.transform.position, meleeAttackSpawnPos) < 1.5f)
            {
                EnterMeleeAttackingPhase();
                duckTimeLeft = -1;
            }
            else
            {
                move.x = Mathf.Sign(player.transform.position.x - damageController.GetMeleeOrigin().x) * 1f; // Move towards the player
                duckTimeLeft -= Time.deltaTime;
            }
        }
    }

    protected void JumpAway()
    {
        move.x = Mathf.Sign(transform.position.x - player.transform.position.x) * 1.25f; // Move away from the player

        if (jump)
        {
            if (feralShadowMoveSet.velocity.y < 0)
            {
                jump = false;
            }
        }
        else
        {
            if (feralShadowMoveSet.controller.collisions.below)
            {
                EnterBackingUpPhase();
            }
        }
    }

    protected void LandingFromJumpingAway()
    {
        if (jump)
        {
            if (feralShadowMoveSet.velocity.y < 0)
            {
                jump = false;
            }
        }
        else
        {
            if (feralShadowMoveSet.controller.collisions.below)
            {
                EnterBackingUpPhase();
            }
        }
    }

    protected void MeleeAttack()
    {
        if (meleeTestTimeLeft <= 0)
        {
            MeleeAttackOver();
        }
        else
        {
            damageController.PerformMeleeAttack();
            meleeTestTimeLeft -= Time.deltaTime;
        }
    }

    public void MeleeAttackOver()
    {
        damageController.ResetAttack();
        currentPhase = Phases.JumpingAway;
        jump = true;
    }

    protected void PrepareToLunge()
    {
        lungePrepTimeLeft -= Time.deltaTime;
    }

    protected void LungeAttack()
    {
        damageController.PerformingLungeAttack();
    }

    public void LandFromLunge()
    {
        damageController.ResetAttack();
        jump = false;
        move.x = 0;

        if (DistanceToPlayer() < lashOutRange)
        {
            currentPhase = Phases.DuckingTowardsPlayer;
        }
        else
        {
            EnterBackingUpPhase();
        }
    }

    public enum Phases
    {
        Patrolling, //Attach Walking Animation
        BackingUp, //Attach Backwards Walk Animation
        PreppingLowLunge, //Attach Crouch Animation
        PreppingHighLunge, //Attach Crouch Animation (Variation)
        RushedPreppingLowLunge, //Attach Crouch Animation (fast)
        RushedPreppingHighLunge, //Attach Crouch Animation (Variation) (fast)
        LungingHigh, //Attach Jumping Animation
        LungingLow, //Attach Jumping Animation
        DuckingTowardsPlayer, //Attach Small Jump (Fast)
        MeleeAttacking, //Attach Swiping Animation
        JumpingAway //Attach Backflip Animation
    }
}