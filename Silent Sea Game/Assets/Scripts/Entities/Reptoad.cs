using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reptoad : Enemy
{
    //Here is where 
    public ReptoadMoveSet reptoadMoveset;

    Rigidbody2D enemyRB;

    public SpriteRenderer spriteRenderer;

    private Animator enemyAnim;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        moveSet = reptoadMoveset = GetComponent<ReptoadMoveSet>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        enemyRB = GetComponent<Rigidbody2D>();
        enemyAnim = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        if (!isDead)
        {
            DoFixedUpdate();
        }
    }

    // Update is called once per frame
    void DoFixedUpdate()
    {
        Vector2 toPlayer = player.position - transform.position;
        float distToPlayer = Vector2.Distance(transform.position, player.position);

        //print(reptoadMoveset.settings.maxJumpHeight);
        //print(distToPlayer);

        AnimationController();

        if (!canSeePlayer() && isGrounded())
        {
            notAttacking();
        }
        /*else{
            attacking(); //took out this part and did it in animation, it let me control it to make it look like it's using its legs to jump
        }*/

        if (move.x != 0)
        {
            if (Mathf.Sign(move.x) == 1 && !facingRight)
            {
                FlipFacing();
            }
            else if (Mathf.Sign(move.x) == -1 && facingRight)
            {
                FlipFacing();
            }
        }
    }

    void FlipFacing() //just like the player that Alex changed this, I went ahead and change it here too
    {
        facingRight = !facingRight;
        spriteRenderer.flipX = !spriteRenderer.flipX;
    }

    bool canSeePlayer() //created this one to be able to use in animator
    {
        float distToPlayer = Vector2.Distance(transform.position, player.position);
        bool inMaxDistace = false;

        if (distToPlayer < agroRange)
        {
            inMaxDistace = true;
        }
        else
        {
            inMaxDistace = false;
        }
        return inMaxDistace;
    }

    void attacking() //created this to be able to use in animator
    {
        Vector2 toPlayer = player.position - transform.position;
        float distToPlayer = Vector2.Distance(transform.position, player.position);

        if (!jump)
        {
            jump = true;
        }
        else
        {
            if (isGrounded())
            {
                jump = false;
            }
        }
        move.x = (Mathf.Sign(toPlayer.x) == -1) ? -1 : 1; // TODO - make only use -1 or 1
        

        //by changing this to -1 to 1 Reptoad now easily transitions back into walking to the next point after losing sight of the player, but
        //still can't get it to have jumps be powerful enought to reach the player in one jump. I was messing with the
        //Reptoad Move Settings and I think if we can figure out the right combo of virtical move speed, airborn time, jump height and time to
        //jump apex, then I think we can get it right. Will work on more later.

        /*if (Mathf.Abs(distToPlayer) > 0.2 ) // took this out because it kept making reptoad rapidly go left and right when close to player
        {
            FlipFacing();
        }*/
    }

    void notAttacking()
    {
        if (Mathf.Abs(targetNode.x - transform.position.x) < 0.2)
        {
            nodes.NextNode();
            targetNode = nodes.GetNextNode();
        }

        move.x = Mathf.Sign((targetNode - (Vector2)transform.position).x);
    }

    bool isGrounded() //created these so that they appear in the animator to select from during transitions
    {
        bool grounded = false;
        if(reptoadMoveset.controller.collisions.below)
        {
            grounded = true;
        }
        else
        {
            grounded = false;
        }

        return grounded;
    }

    void jumpAttack()
    {
        float distToPlayer = Vector2.Distance(transform.position, player.position);
        enemyRB.AddForce(new Vector2 (distToPlayer,reptoadMoveset.settings.maxJumpHeight), ForceMode2D.Force);
    }

    void AnimationController()
    {
        enemyAnim.SetBool("canSeePlayer", canSeePlayer());
        enemyAnim.SetBool("isGrounded", isGrounded());
    }

    public override void OnDeath()
    {
        base.OnDeath();

        DropSpiritOrb();
    }
}
