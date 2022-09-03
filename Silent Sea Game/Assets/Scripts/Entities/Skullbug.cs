using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skullbug : Enemy
{
    public NewSkullbugMoveset newSkullbugMoveset;
    Rigidbody2D enemyRB;
    public SpriteRenderer spriteRenderer;
    private Animator enemyAnim;
    bool isDescending = false;
    bool isClimbing = false;
    public bool isClimbingOrDescending = false;
    public bool isClimbingOnCeiling = false;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        moveSet = newSkullbugMoveset = GetComponent<NewSkullbugMoveset>();
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
        AnimationController();
        notAttacking();

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

    void notAttacking()
    {
        if ((newSkullbugMoveset.controller.collisions.right || newSkullbugMoveset.controller.collisions.left) && !newSkullbugMoveset.controller.collisions.above)
        {
            if (Mathf.Abs(targetNode.y - transform.position.y) < 0.2)
            {
                print("Ready y node");
                nodes.NextNode();
                targetNode = nodes.GetNextNode();
            }
            //isClimbingOnCeiling = false;
        }
        else
        {
            if (newSkullbugMoveset.controller.collisions.above && !newSkullbugMoveset.controller.collisions.below)
            {
                isClimbingOnCeiling = true;
            }
            else
            {
                isClimbingOnCeiling = false;

            }

            if (Mathf.Abs(targetNode.x - transform.position.x) < 0.2)
            {
                // print("Ready x node");
                nodes.NextNode();
                targetNode = nodes.GetNextNode();
                //FlipFacing();
            }
        }

        move.y = Mathf.Sign((targetNode - (Vector2)transform.position).y);
        move.x = Mathf.Sign((targetNode - (Vector2)transform.position).x);

        if (newSkullbugMoveset.controller.collisions.right || newSkullbugMoveset.controller.collisions.left)
        {
            if (Mathf.Sign(move.y) == 1)
            {
                isDescending = false;
                isClimbing = true;
                isClimbingOrDescending = true;
            }
            else if (Mathf.Sign(move.y) == -1)
            {
                isDescending = true;
                isClimbing = false;
                isClimbingOrDescending = true;
            }
        }
        else
        {
            isDescending = false;
            isClimbing = false;
            isClimbingOrDescending = false;
        }

    }

    bool hitOnRightLeft()
    {
        bool hitleftright = false;

        if (newSkullbugMoveset.controller.collisions.right || newSkullbugMoveset.controller.collisions.left)
        {
            hitleftright = true;
        }
        else
        {
            hitleftright = false;
        }
        return hitleftright;
    }

    bool isCornerBottomLeft()
    {
        bool corner = false;
        if (newSkullbugMoveset.controller.collisions.below && hitOnRightLeft())
        {
            corner = true;
        }
        else
        {
            corner = false;
        }
        return corner;
    }

    bool isCornerTopLeft()
    {
        bool corner = false;
        if (newSkullbugMoveset.controller.collisions.above && hitOnRightLeft())
        {
            corner = true;
        }
        else
        {
            corner = false;
        }
        return corner;
    }

    void AnimationController()
    {
        enemyAnim.SetBool("isClimbing", isClimbing);
        enemyAnim.SetBool("isDescending", isDescending);
        enemyAnim.SetBool("hitleftright", hitOnRightLeft());
        enemyAnim.SetBool("isClimbingOnCeiling", isClimbingOnCeiling);
        enemyAnim.SetBool("isAbove", newSkullbugMoveset.controller.collisions.above);
        enemyAnim.SetBool("isCornerBottomLeft", isCornerBottomLeft());
        enemyAnim.SetBool("isCornerTopLeft", isCornerTopLeft());
        enemyAnim.SetBool("isGrounded", newSkullbugMoveset.controller.collisions.below);
    }

    public override void OnDeath()
    {
        base.OnDeath();
        if (Player_Health_Controller.transform_power == true)
        {
            DropSpiritOrb();
        }
    }
}
