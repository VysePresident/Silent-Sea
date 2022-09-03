using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AncientGuardian : BossEnemy
{
    public Animator animator;

    public float fightStartDistance;
    public float wakeUpTime;
    protected float wakeUpTimeLeft;

    public float maxMoveTimePhase1;
    public float maxMoveTimePhase2;
    protected float moveTimeLeft;

    protected float recoveryTimeLeft;

    public float fistSmashAttackChargeTime;
    public float fistSmashAttackRecoveryTime;
    protected float fistSmashAttackChargeTimeLeft;

    public float leapingSmashAttackChargeTime;
    public float leapingSmashAttackRecoveryTime;
    protected float leapingSmashAttackChargeTimeLeft;

    public float iceSpikeAttackChargeTime;
    public float iceSpikeAttackRecoveryTime;
    protected float iceSpikeAttackChargeTimeLeft;

    public float preferredDistance;

    public Status status;
    protected AttackType previousAttackType = AttackType.none;

    protected AncientGuardianMoveSet ancientGuardianMoveSet;

    public Vector2 fistSmashKB1, landingSmashKB1;
    public Vector2 fistSmashKB2, landingSmashKB2;

    public AncientGuardianFist fistSmashAttackPrefab;
    public Vector2 fistSmashAttackOffset;
    public float maxRushDistance;
    public float radius;
    public int smashSpeed = 1;
    public float fistWindUpTime;
    protected float fistWindUpTimeLeft;
    protected AncientGuardianFist currentFist;

    public AncientGuardianLandingSmash landingSmashAttackPrefab;
    public Vector2 landingSmashAttackOffset;
    protected AncientGuardianLandingSmash currentLandingSmash;

    public AncientGuardianIceSpikeAttack iceSpikeAttackPrefab;
    public Vector2 iceSpikeAttackOffset;
    protected AncientGuardianIceSpikeAttack currentIceSpikeAttack;

    public AncientGuardianCeilingIceHandler iceHandler;

    public Vector2 arenaLeftBound, arenaRightBound;
    public Vector2[] iceSpikeAttackLocations;

    protected bool attackFinished;
    protected bool attackCanceled;
    protected bool charging;

    protected bool doingPattern, startingPattern;
    protected int currentPattern; // 0 - leaps, 1 - smash & leap, 2 - smashes
    protected int attackPatternsSinceLastIceSpikeAttack;

    protected override void Start()
    {
        base.Start();
        enemyType = Player.PlayerForm.Boss_AncientGuardian;

        moveSet = GetComponent<BasicMoveSet>();
        ancientGuardianMoveSet = (AncientGuardianMoveSet)moveSet;

        status = Status.WaitingToStart;

        InitializeAttackPatterns();
    }

    protected void InitializeAttackPatterns()
    {
        leapsAttackPattern = new AttackPattern(leapsAttacks, leapsAttackPatternWait);
        smashLeapAttackPattern = new AttackPattern(smashLeapAttacks, smashLeapAttackPaternWait);
        smashesAttackPattern = new AttackPattern(smashesAttacks, smashesAttackPatternWait);
    }

    protected void FixedUpdate()
    {
        if (!isDead)
        {
            DoFixedUpdate();
        }
        else
        {
            ancientGuardianMoveSet.velocity.y = 0;
        }
    }

    protected void DoFixedUpdate()
    {
        switch (status)
        {
            case Status.WaitingToStart:
                WaitToStart();
                break;
            case Status.StartingFight:
                FlipTowardsPlayer();
                StartFight();
                break;
            case Status.DeterminingNextMove:
                DetermineNextMove();
                break;
            case Status.MovingToPreferredDistance:
                FlipTowardsPlayer();
                animator.SetTrigger("walk"); //Walk?
                MoveToPreferredDistance();
                break;
            case Status.ChargingSmashAttack:
                FlipTowardsPlayer();
                ChargeFistSmashAttack();
                break;
            case Status.Rushing:
                Rush();
                animator.SetTrigger("walk"); //Walk?
                break;
            case Status.Smashing:
                Smash();
                break;
            case Status.ChargingLeapingSmash:
                FlipTowardsPlayer();
                animator.SetTrigger("jump"); //Walk?
                ChargeLeapingSmashAttack();
                break;
            case Status.Leaping:
                Leap();
                animator.SetTrigger("jump"); //Walk?
                break;
            case Status.LandingSmashing:
                WaitForLandingSmashToFinish();
                break;
            case Status.MovingToIceSpikeAttackPosition:
                FlipTowardsPlayer();
                animator.SetTrigger("jump"); //Walk?
                MoveToIceSpikeAttackLocation();
                break;
            case Status.ChargingIceSpikeAttack:
                ChargeIceSpikeAttack();
                //Magic
                break;
            case Status.ShootingIceSpikes:
                WaitForIceSpikeAttackToFinish();
                //Magic
                break;
            case Status.RecoveringFromSmashAttack:
            case Status.RecoveringFromLandingSmashAttack:
            case Status.RecoveringFromIceSpikeAttack:
                RecoverFromAttacking();
                break;
            case Status.ChangingPhase:
                ChangePhaseWait();
                break;
        }
    }

    public override void StartNextPhase()
    {
        CancelCurrentAction();
        ancientGuardianMoveSet.BeginPhase2();

        fistSmashAttackChargeTime *= 0.66f;
        fistSmashAttackRecoveryTime *= 0.66f;
        chargeMultiplier *= 1.5f;
        fistWindUpTime *= 0.66f;

        leapingSmashAttackChargeTime *= 0.66f;
        leapingSmashAttackRecoveryTime *= 0.66f;

        iceSpikeAttackChargeTime *= 0.66f;
        iceSpikeAttackRecoveryTime *= 0.66f;

        status = Status.ChangingPhase;

        phaseChangeTimeLeft = phaseChangeDuration;

        currentPhase++;
    }

    public void ChangePhaseWait()
    {
        if (phaseChangeTimeLeft <= 0)
        {
            StartSmashLeapAttackPatern();
        }
        else
        {
            phaseChangeTimeLeft -= Time.deltaTime;
        }
    }

    protected void DetermineNextMove()
    {
        #region Determine next attack if starting/doing a pattern

        AttackType nextAttack;
        if (startingPattern)
        {
            switch (currentPattern)
            {
                case 0:
                    nextAttack = leapsAttackPattern.GetAttack();
                    break;
                case 2:
                    nextAttack = smashesAttackPattern.GetAttack();
                    break;
                case 1:
                default:
                    nextAttack = smashLeapAttackPattern.GetAttack();
                    break;
            }

            startingPattern = false;
        }
        else if (doingPattern)
        {
            switch(currentPattern)
            {
                case 0:
                    nextAttack = leapsAttackPattern.GetNextAttack();
                    break;
                case 2:
                    nextAttack = smashesAttackPattern.GetNextAttack();
                    break;
                case 1:
                default:
                    nextAttack = smashLeapAttackPattern.GetNextAttack();
                    break;
            }
        }
        else
        {
            nextAttack = AttackType.none; // This one will get overwritten before it matters, it's just here to make the compiler happy
        }

        #endregion

        if (doingPattern)
        {
            if (nextAttack == AttackType.none)  // ATTACK PATTERN COMPLETE
            {
                if (previousAttackType != AttackType.iceSpike && previousAttackType != AttackType.none)
                {
                    attackPatternsSinceLastIceSpikeAttack++;
                }

                ResetAttackPatterns();

                if (attackPatternsSinceLastIceSpikeAttack < 3)
                {
                    RandomlyDecideNextAttackPattern();
                }
                else
                {
                    // at least 3 completed attack patterns since the last ice spike attack
                    StartIceSpikeAttack();
                }
            }
            else if (nextAttack == AttackType.fistSmash && WouldBoxInPlayer())       // TOO CLOSE TO THE EDGE OF THE ARENA
            {
                ResetAttackPatterns();
                StartIceSpikeAttack();
            }
            else                                // VALID NEXT ATTACK IN PATTERN
            {
                switch (nextAttack)
                {
                    case AttackType.landingSmash:
                        StartLeapAttack();
                        break;
                    case AttackType.iceSpike:
                        StartIceSpikeAttack();
                        break;
                    case AttackType.fistSmash:
                    default:
                        StartSmashAttack();
                        break;
                }
            }
        }
        else
        {
            ResetAttackPatterns();
            RandomlyDecideNextAttackPattern();
        }
    }

    protected void RandomlyDecideNextAttackPattern()
    {
        currentPattern = Random.Range(0, 3); // Get a random int 0, 1, or 2 for the pattern

        switch (currentPattern)  // 0 - leaps, 1 - smash & leap, 2 - smashes
        {
            case 0:
                StartLeapsAttackPattern();
                break;
            case 2:
                StartSmashesAttackPattern();
                break;
            case 1:
            default:
                StartSmashLeapAttackPatern();
                break;
        }
    }

    #region Attack Pattern Starting Functions

    protected void StartSmashesAttackPattern()
    {
        currentPattern = 2;

        startingPattern = doingPattern = true;

        status = Status.MovingToPreferredDistance;
    }

    protected void StartSmashLeapAttackPatern()
    {
        currentPattern = 1;

        startingPattern = doingPattern = true;

        status = Status.MovingToPreferredDistance;
    }

    protected void StartLeapsAttackPattern()
    {
        currentPattern = 0;

        startingPattern = doingPattern = true;

        status = Status.MovingToPreferredDistance;
    }

    #endregion

    #region Individual Attack Starter Functions

    protected void StartSmashAttack()
    {
        status = Status.ChargingSmashAttack;
        rushDistTraveled = 0;
    }

    protected void StartLeapAttack()
    {
        status = Status.ChargingLeapingSmash;
    }

    protected void StartIceSpikeAttack()
    {
        status = Status.MovingToIceSpikeAttackPosition;

        attackPatternsSinceLastIceSpikeAttack = 0;

        doingPattern = startingPattern = false;
    }

    #endregion

    protected bool WouldBoxInPlayer()
    {
        float maxRushEndXPos = transform.position.x + ((facingRight) ? maxRushDistance - ancientGuardianMoveSet.controller.main_collider.bounds.extents.x : -maxRushDistance + ancientGuardianMoveSet.controller.main_collider.bounds.extents.x);

        if (facingRight && maxRushEndXPos > arenaRightBound.x - 2 ||
            !facingRight && maxRushEndXPos < arenaLeftBound.x + 2)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    protected void RecoverFromAttacking()
    {
        if (recoveryTimeLeft <= 0)
        {
            stop = false;
            status = Status.DeterminingNextMove;
        }
        else
        {
            recoveryTimeLeft -= Time.deltaTime;
        }
    }

    protected void CancelCurrentAction()
    {
        animator.SetTrigger("idle");
        move = Vector2.zero;
        jump = false;

        switch (status)
        {
            case Status.ChargingSmashAttack:
            case Status.ChargingLeapingSmash:
            case Status.ChargingIceSpikeAttack:
                charging = false;
                break;
            case Status.Smashing:
                Destroy(currentFist.gameObject);
                break;
            case Status.LandingSmashing:
                Destroy(currentLandingSmash.gameObject);
                break;
            case Status.ShootingIceSpikes:
                //AGIceSpikeAttack
                GameObject object_to_destroy = GameObject.Find("AGIceSpikeAttack(Clone)");
                Destroy(object_to_destroy);
                break;
            default:
                // Nothing!
                break;
        }

        ResetAttackPatterns();
    }

    protected void MoveToPreferredDistance()
    {
        if (Mathf.Abs(DistanceToPlayer() - preferredDistance) < 0.5 || moveTimeLeft <= 0 ||
            (ancientGuardianMoveSet.controller.collisions.left || ancientGuardianMoveSet.controller.collisions.right))
        {
            status = Status.DeterminingNextMove;
            DetermineNextMove();
        }
        else
        {
            move.x = (facingRight) ? -1 : 1; // BACK away from player

            moveTimeLeft -= Time.deltaTime;
        }
    }

    #region Beginning of Fight Functions

    protected void WaitToStart()
    {
        if (DistanceToPlayer() < fightStartDistance)
        {
            // LET'S FIGHT!
            // These really just exist for animations

            wakeUpTimeLeft = wakeUpTime;
            status = Status.StartingFight;
        }
    }

    protected void StartFight()
    {
        // Kinda just exists for animations

        if (wakeUpTimeLeft <= 0)
        {
            StartSmashLeapAttackPatern();
        }
        else
        {
            wakeUpTimeLeft -= Time.deltaTime;
        }
    }

    #endregion

    #region Fist Smash Attack Functions

    protected float rushDistTraveled = -1;
    protected int degrees;

    protected Vector2 startingPos;
    protected Vector2 fistDownVelocity;

    protected float chargeMultiplier = 1.75f;
    [HideInInspector]
    public bool stop;

    protected void ChargeFistSmashAttack()
    {
        if (!charging)
        {
            fistSmashAttackChargeTimeLeft = fistSmashAttackChargeTime;
            charging = true;
        }

        if (fistSmashAttackChargeTimeLeft <= 0)
        {
            charging = false;
            status = Status.Rushing;
        }
        else
        {
            fistSmashAttackChargeTimeLeft -= Time.deltaTime;
        }
    }

    protected void Rush()
    {
        float rushDistLeft = Mathf.Abs(player.transform.position.x - transform.position.x) - (ancientGuardianMoveSet.controller.main_collider.bounds.extents.x + 0.5f);

        move.x = chargeMultiplier * ((facingRight) ? 1 : -1);

        if (rushDistLeft > 1 && rushDistTraveled < maxRushDistance)
        {
            move.x = chargeMultiplier * ((facingRight) ? 1 : -1);

            rushDistTraveled += Mathf.Abs(ancientGuardianMoveSet.velocity.x) * Time.deltaTime;
        }
        else
        {
            animator.SetTrigger("windup");
            stop = true;
            move = Vector2.zero;
            rushDistTraveled = -1;
            previousAttackType = AttackType.fistSmash;

            InitializeFistAttack();
        }
    }

    protected void InitializeFistAttack()
    {
        Vector2 pos = transform.position;
        pos.x += fistSmashAttackOffset.x * ((facingRight) ? 1 : -1);
        pos.y += fistSmashAttackOffset.y;

        currentFist = Instantiate(fistSmashAttackPrefab, pos, Quaternion.identity);

        currentFist.iceHandler = iceHandler;
        currentFist.AttackDuration = 8;
        currentFist.knockBack = (currentPhase == 1) ? fistSmashKB1 : fistSmashKB2;

        startingPos = pos;
        degrees = 90;
        fistDownVelocity = Vector2.zero;
        fistWindUpTimeLeft = fistWindUpTime;

        status = Status.Smashing;
    }

    protected void Smash()
    {
        if (fistWindUpTimeLeft <= 0)
        {
            animator.SetTrigger("smash");
            if (degrees < 180)
            {
                Vector2 targetPos = new Vector2(
                    startingPos.x + ((Mathf.Cos(Mathf.PI * 2 * degrees / 360) * radius) * ((facingRight) ? -1f : 1f)),
                    startingPos.y + Mathf.Sin(Mathf.PI * 2 * degrees / 360) * radius
                    );

                currentFist.Move(targetPos - (Vector2)currentFist.transform.position);

                degrees += smashSpeed;
            }
            else
            {
                if (fistDownVelocity == Vector2.zero)
                {
                    fistDownVelocity = currentFist.controller.oldVelocity;
                }

                if (currentFist.IceDropped)
                {
                    Destroy(currentFist.gameObject);

                    recoveryTimeLeft = fistSmashAttackRecoveryTime;
                    status = Status.RecoveringFromSmashAttack;
                }
                else if (!currentFist.AttackOver)
                {
                    currentFist.Move(fistDownVelocity);
                }
            }
        }
        else
        {
            fistWindUpTimeLeft -= Time.deltaTime;
        }
    }

    #endregion

    #region Leaping Smash Attack Functions

    protected Vector2 leapTarget;

    protected void ChargeLeapingSmashAttack()
    {
        if (!charging)
        {
            leapingSmashAttackChargeTimeLeft = leapingSmashAttackChargeTime;
            charging = true;
        }

        if (leapingSmashAttackChargeTimeLeft <= 0)
        {
            leapTarget = player.transform.position;
            charging = false;
            jump = true;
            status = Status.Leaping;
        }
        else
        {
            leapingSmashAttackChargeTimeLeft -= Time.deltaTime;
        }
    }

    protected void Leap()
    {
        Vector2 toTarget = (Vector2)transform.position - leapTarget;

        if (Mathf.Abs(toTarget.x) < 0.2)
        {
            move.x = 0;
        }
        else if (facingRight)
        {
            move.x = 1.2f;
        }
        else
        {
            move.x = -1.2f;
        }
        
        if (jump)
        {
            if (ancientGuardianMoveSet.velocity.y < 0)
            {
                jump = false;
            }
        }
        else
        {
            if (ancientGuardianMoveSet.controller.collisions.below)
            {
                move.x = 0;
                previousAttackType = AttackType.landingSmash;

                InitializeLandingSmashAttack();
            }
        }
        
    }

    protected void InitializeLandingSmashAttack()
    {
        Vector2 pos = transform.position;
        pos.x += landingSmashAttackOffset.x * ((facingRight) ? 1 : -1);
        pos.y += landingSmashAttackOffset.y;

        currentLandingSmash = Instantiate(landingSmashAttackPrefab, pos, Quaternion.identity);

        currentLandingSmash.iceHandler = iceHandler;
        currentLandingSmash.knockBack = (currentPhase == 1) ? landingSmashKB1 : landingSmashKB2;

        status = Status.LandingSmashing;
    }

    protected void WaitForLandingSmashToFinish()
    {
        if (currentLandingSmash.IceDropped)
        {
            Destroy(currentLandingSmash.gameObject);

            recoveryTimeLeft = leapingSmashAttackRecoveryTime;
            status = Status.RecoveringFromLandingSmashAttack;
        }
    }

    #endregion

    #region Ice Spike Attack Functions

    protected int targetIceSpikeAttackLocation = -1;
    protected bool jumpedAway;

    protected void MoveToIceSpikeAttackLocation()
    {
        if (targetIceSpikeAttackLocation == -1)
        {
            targetIceSpikeAttackLocation = GetFurthestIceSpikeAttackLocationFromPlayer();
        }

        Vector2 toTarget = (Vector2) transform.position - iceSpikeAttackLocations[targetIceSpikeAttackLocation];

        if (Mathf.Abs(toTarget.x) < 0.2f) // If AG is w/in 0.2 units from the target Ice Spike Attack Location
        {
            stop = true;
            move = Vector2.zero;

            status = Status.ChargingIceSpikeAttack;
            targetIceSpikeAttackLocation = -1;
        }
        else
        {
            if (transform.position.x < iceSpikeAttackLocations[targetIceSpikeAttackLocation].x) // move towards the target ice spike attack location
            {
                move.x = 1;
            }
            else
            {
                move.x = -1;
            }

            if (!jumpedAway)
            {
                jump = true;
                jumpedAway = true;
            }
            else if (moveSet.velocity.y < 0) // if falling (reached apex of jump)
            {
                jump = false;
            }
        }
    }

    protected int GetFurthestIceSpikeAttackLocationFromPlayer()
    {
        if (Vector2.Distance(player.transform.position, iceSpikeAttackLocations[0]) > Vector2.Distance(player.transform.position, iceSpikeAttackLocations[1]))
        {
            // iceSpikeAttackLocations[0] furthest from player
            return 0;
        }
        else
        {
            // iceSpikeAttackLocations[1] furthest from player
            return 1;
        }
    }

    protected void ChargeIceSpikeAttack()
    {
        if (!charging)
        {
            iceSpikeAttackChargeTimeLeft = iceSpikeAttackChargeTime;
            charging = true;
            jumpedAway = false;
        }

        if (iceSpikeAttackChargeTimeLeft <= 0)
        {
            charging = false;
            previousAttackType = AttackType.iceSpike;

            InitializeIceSpikeAttack();
        }
        else if (!attackCanceled)
        {
            iceSpikeAttackChargeTimeLeft -= Time.deltaTime;
        }
        else // If the attack is canceled
        {
            charging = false;
        }
    }

    protected void InitializeIceSpikeAttack()
    {
        Vector2 pos = transform.position;
        pos.x += iceSpikeAttackOffset.x * ((facingRight) ? 1 : -1);
        pos.y += iceSpikeAttackOffset.y;

        currentIceSpikeAttack = Instantiate(iceSpikeAttackPrefab, pos, Quaternion.identity);

        currentIceSpikeAttack.boss = this;

        status = Status.ShootingIceSpikes;
    }

    protected void WaitForIceSpikeAttackToFinish()
    {
        if (attackCanceled)
        {
            Destroy(currentIceSpikeAttack.gameObject);
        }
        else if (currentIceSpikeAttack.finished)
        {
            Destroy(currentIceSpikeAttack.gameObject);

            recoveryTimeLeft = iceSpikeAttackRecoveryTime;
            status = Status.RecoveringFromIceSpikeAttack;
        }
    }

    #endregion

    public override void OnDeath()
    {
        CancelCurrentAction();
        animator.SetTrigger("death");
        (moveSet as BasicMoveSet).controller.main_collider.enabled = false;
        ancientGuardianMoveSet.enabled = false;
        isDead = true;
    }

    public enum Status
    {
        WaitingToStart,
        StartingFight,

        DeterminingNextMove,

        MovingToPreferredDistance,
        MovingToIceSpikeAttackPosition,

        ChargingSmashAttack,
        Rushing,
        Smashing,
        RecoveringFromSmashAttack,

        ChargingLeapingSmash,
        Leaping,
        LandingSmashing,
        RecoveringFromLandingSmashAttack,

        ChargingIceSpikeAttack,
        ShootingIceSpikes,
        RecoveringFromIceSpikeAttack,

        ChangingPhase,
        Dead
    }

    public enum AttackType
    {
        fistSmash,
        landingSmash,
        iceSpike,
        none
    }

    protected struct AttackPattern
    {
        AttackType[] attacks;
        int current;
        float waitDuration;

        public AttackPattern(AttackType[] atks, float waitDur)
        {
            attacks = atks;
            current = 0;
            waitDuration = waitDur;
        }

        public AttackType GetNextAttack()
        {
            current++;
            return GetAttack();
        }

        public AttackType GetAttack()
        {
            if (current >= 0 && current < attacks.Length)
            {
                return attacks[current];
            }
            else
            {
                return AttackType.none;
            }
        }

        public void Reset()
        {
            current = 0;
        }

        public void Reset(float waitDur)
        {
            waitDuration = waitDur;
            current = 0;
        }
    }

    public AttackType[] leapsAttacks =
    {
        AttackType.landingSmash, AttackType.landingSmash, AttackType.landingSmash
    };
    public float leapsAttackPatternWait;
    protected AttackPattern leapsAttackPattern;

    public AttackType[] smashLeapAttacks =
    {
        AttackType.fistSmash, AttackType.landingSmash
    };
    public float smashLeapAttackPaternWait;
    protected AttackPattern smashLeapAttackPattern;

    public AttackType[] smashesAttacks =
    {
        AttackType.fistSmash, AttackType.fistSmash, AttackType.fistSmash
    };
    public float smashesAttackPatternWait;
    protected AttackPattern smashesAttackPattern;

    protected void ResetAttackPatterns()
    {
        leapsAttackPattern.Reset();
        smashLeapAttackPattern.Reset();
        smashesAttackPattern.Reset();
    }

    protected void ResetAttackPatterns(float waitDur)
    {
        leapsAttackPattern.Reset(waitDur);
        smashLeapAttackPattern.Reset(waitDur);
        smashesAttackPattern.Reset(waitDur);
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        // Draw arena bounds
        Gizmos.color = Color.red;

        Gizmos.DrawRay(arenaLeftBound, Vector2.up * 10);
        Gizmos.DrawRay(arenaRightBound, Vector2.up * 10);

        // Draw attack spawn locations
        Gizmos.color = Color.blue;              // Fist is blue
        Vector2 attackPos = transform.position;
        attackPos.x += fistSmashAttackOffset.x * ((facingRight) ? 1 : -1);
        attackPos.y += fistSmashAttackOffset.y;
        Gizmos.DrawSphere(attackPos, 0.2f);

        Gizmos.color = Color.green;             // Landing smash is green
        attackPos = transform.position;
        attackPos.x += landingSmashAttackOffset.x * ((facingRight) ? 1 : -1);
        attackPos.y += landingSmashAttackOffset.y;
        Gizmos.DrawSphere(attackPos, 0.2f);

        Gizmos.color = Color.yellow;            // Ice spike is yellow
        attackPos = transform.position;
        attackPos.x += iceSpikeAttackOffset.x * ((facingRight) ? 1 : -1);
        attackPos.y += iceSpikeAttackOffset.y;
        Gizmos.DrawSphere(attackPos, 0.2f);

        // Draw Ice Spike Attack locations
        foreach (Vector2 pos in iceSpikeAttackLocations)
        {
            Gizmos.DrawCube(pos, new Vector3(0.5f, 0.5f, 0.3f));
        }
    }
}