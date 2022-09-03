using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deadbones : Enemy
{
    public enum DeadbonesPhases
    {
        Patrolling, //Attach Walking Animation
        Agroed,
        Charging,
        Swinging,
        Cooldown
    }
    /* Phase Timers */
    private const float AGRO_DURATION = 0.8f;
    private float Agro_Timer = 0.0f;
    private float Melee_Range = 1.0f;
    private const float COOLDOWN_DURATION = 1.0f;
    private float Cooldown_Timer = 0.0f;
    private Vector2 attackTarget;

    public DeadbonesArea dbArea;

    /* Melee invisible boxes */
    MeleeBox leftBox;
    MeleeBox rightBox;

    private const float SWINGING_DURATION = 1.5F;
    private float Swinging_Timer = 0.0f;
    Rigidbody2D enemyRB;
    private bool shouldAttack = true;
    private bool running = false;
    private bool swinging = false;
    private bool shuffling = true;
    // private const float windupDuration = 2.0f;
    // private float windupTimeLeft = 0.0f;

    // private float attackDuration = 2.0f;
    // private float attackTimeLeft = 0.0f;

    public MoveSet deadbonesMoveSet;
    public DeadbonesPhases currentPhase = DeadbonesPhases.Patrolling;

    [HideInInspector]
    public DeadbonesDamageController damageController;
    public SpriteRenderer spriteRenderer;
    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        leftBox = transform.Find("MeleeLeft").gameObject.GetComponent<MeleeBox>();
        rightBox = transform.Find("MeleeRight").gameObject.GetComponent<MeleeBox>();
        moveSet = deadbonesMoveSet = GetComponent<MoveSet>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        damageController = GetComponent<DeadbonesDamageController>();
        enemyRB = GetComponent<Rigidbody2D>();
        enemyType = Player.PlayerForm.Deadbones;

        updateAnimations();
    }

    public override void OnDeath()
    {
        isDead = true;
        Destroy(this.gameObject); // Awkward solution to the problem, but I suppose it might work for now
    }

    private void FaceWalkingDirection()
    {
        // Make him face the direction he is moving
        if (move.x != 0)
        {
            if (Mathf.Sign(move.x) == 1 && !facingRight)
            {
                Flip();
            }
            else if (Mathf.Sign(move.x) == -1 && facingRight)
            {
                Flip();
            }
        }
    }

    protected virtual void Update()
    {
        if (!isDead)
        {
            // Face the direction he is walking
            FaceWalkingDirection();
            switch (currentPhase)
            {
                case DeadbonesPhases.Patrolling:
                    {
                        // Start the running animation
                        if (PlayerInAgroRange())
                        {
                            move.x = 0;
                            running = false;
                            shuffling = false;
                            updateAnimations();
                            if (DistanceToPlayer() < Melee_Range)
                            {
                                currentPhase = DeadbonesPhases.Swinging;
                            }
                            else
                            {
                                currentPhase = DeadbonesPhases.Agroed;
                                Agro_Timer = AGRO_DURATION;
                            }
                        }
                        else
                        {
                            running = false;
                            swinging = false;
                            shuffling = true;
                            updateAnimations();
                            Patrol();
                        }
                        break;
                    }
                case DeadbonesPhases.Agroed:
                    {
                        // Decrement timer
                        Agro_Timer -= Time.deltaTime;

                        // Face the player
                        FlipTowardsPlayer();

                        // If timer <= 0, go to attack phase
                        if (Agro_Timer < 0)
                        {
                            currentPhase = DeadbonesPhases.Charging;
                        }
                        break;
                    }
                case DeadbonesPhases.Charging:
                    {
                        Charge();

                        break;
                    }
                case DeadbonesPhases.Swinging:
                    {
                        Debug.Log("Swinging");
                        swinging = true;
                        running = false;
                        shuffling = false;
                        updateAnimations();
                        move.x = 0;
                        if (Swinging_Timer < 0.0f)
                        {
                            Cooldown_Timer = COOLDOWN_DURATION;
                            currentPhase = DeadbonesPhases.Cooldown;
                        }
                        else
                        {
                            Swing();
                            Swinging_Timer -= Time.deltaTime;
                        }
                        break;
                    }
                case DeadbonesPhases.Cooldown:
                    {
                        shuffling = true;
                        running = false;
                        swinging = false;
                        updateAnimations();
                        if (Cooldown_Timer < 0.0f)
                        {
                            // End cooldown phase
                            currentPhase = DeadbonesPhases.Patrolling;
                        }
                        else
                        {
                            // Mid cooldown phase
                            Cooldown_Timer -= Time.deltaTime;
                            move.x = 0;
                        }
                        break;
                    }
                    //Vector2 toPlayer = player.position - transform.position;
                    //float distToPlayer = Vector2.Distance(transform.position, player.position);
            }
        }
    }

    public void SwordDealsDamage()
    {
        leftBox.dealDamage();
        rightBox.dealDamage();
    }

    public override bool PlayerInAgroRange()
    {
        return DistanceToPlayer() <= agroRange && dbArea.playerInArea;
    }

    private void Patrol()
    {
        running = true;
        swinging = false;

        // If we stumble into the player, start swinging at him
        // if (DistanceToPlayer() < 0.5f)
        // {
        //     currentPhase = DeadbonesPhases.Swinging;
        //     return;
        // }
        // Make him walk back and forth while he is not in agro range of the player
        if (Mathf.Abs(targetNode.x - transform.position.x) < 0.2)
        {
            nodes.NextNode();
            targetNode = nodes.GetNextNode();
        }

        move.x = Mathf.Sign((targetNode - (Vector2)transform.position).x);
    }

    private void Charge()
    {

        if (!dbArea.playerInArea)
        {
            currentPhase = DeadbonesPhases.Patrolling;
            return;
        }

        // If the player is not on the same platform anymore, bail out

        Debug.Log("Charge!!!");
        attackTarget = player.position;
        Vector2 swinging_position = new Vector2(attackTarget.x, this.transform.position.y);
        if (attackTarget.x < this.transform.position.x)
        {
            // player on left of deadbones
            swinging_position.x += 2.0f;
        }
        else
        {
            // player on right of deadbones
            swinging_position.x -= 2.0f;
        }

        if (Mathf.Abs(swinging_position.x - this.transform.position.x) < 0.5f || Mathf.Abs(DistanceToPlayer()) < 0.5)
        {
            // We got to the swinging position, so now go to swinging
            Swinging_Timer = SWINGING_DURATION;
            currentPhase = DeadbonesPhases.Swinging;
            Debug.Log("Going to swinging");
        }
        else
        {
            speed = 10;
            running = true;
            shuffling = false;
            swinging = false;
            updateAnimations();
            transform.position = Vector2.MoveTowards(transform.position, swinging_position, speed * Time.deltaTime);
        }
    }

    private void Swing()
    {
        Debug.Log("Swinging");
        swinging = true;
        shuffling = false;
        running = false;

        updateAnimations();

        // If the player is in melee range, take hit
        if (DistanceToPlayer() <= 0.5)
        {
            // GameObject go = GameObject.Find("Player");
            // Player_Health_Controller other = (Player_Health_Controller)go.GetComponent(typeof(Player_Health_Controller));
            // other.TakeHit(1);
        }
    }

    private void updateAnimations()
    {
        animator.SetBool("Running", running);
        animator.SetBool("Swinging", swinging);
        animator.SetBool("Shuffling", shuffling);

        leftBox.setEnabled(swinging);
        rightBox.setEnabled(swinging);
    }
}
