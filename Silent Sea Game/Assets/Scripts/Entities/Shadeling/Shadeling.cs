using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shadeling : Enemy
{
    //Here is where 
    public ShadelingMoveSet shadelingMoveset;

    Rigidbody2D enemyRB;

    public SpriteRenderer spriteRenderer;

    private Enemy_Health_Controller enemyHP;

    private Animator enemyAnim;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        shadelingMoveset = GetComponent<ShadelingMoveSet>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        enemyHP = GetComponent<Enemy_Health_Controller>();
        enemyRB = GetComponent<Rigidbody2D>();
        //enemyAnim = GetComponent<Animator>();
        facingRight = false;
    }

    private void FixedUpdate()
    {
        if (!isDead)
        {
            DoFixedUpdate();
        }
        else
        {
            Destroy(this.gameObject); // Awkward solution to the problem, but I suppose it might work for now
        }
        print(isDead);
    }

    // Update is called once per frame
    void DoFixedUpdate()
    {
        if (base.player != null && base.player.transform.position.x < this.transform.position.x && facingRight)
        {
            FlipFacing();
        }
        else if (base.player != null && base.player.transform.position.x > this.transform.position.x && !facingRight)
        {
            FlipFacing();
        }
        
    }

    void FlipFacing() //just like the player that Alex changed this, I went ahead and change it here too
    {
        facingRight = !facingRight;
        spriteRenderer.flipX = !spriteRenderer.flipX;
    }

    public override void OnDeath()
    {
        //DropSpiritOrb();    // Called first to ensure the function actually works
        //base.OnDeath();
        isDead = true;
        if (enemyHP.Hitpoints <= 0)
        {
            Destroy(this.gameObject); // Awkward solution to the problem, but I suppose it might work for now
        }
    }

    /*protected bool PlayerInAgroRange()
    {
        return DistanceToPlayer() <= agroRange;
    }

    public float DistanceToPlayer()
    {
        return Vector2.Distance((Vector2)player.position, (Vector2)transform.position);
    }*/
}
