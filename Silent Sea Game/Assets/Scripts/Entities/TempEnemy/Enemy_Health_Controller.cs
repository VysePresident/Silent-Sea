using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Health_Controller : Health_Controller
{
    protected Enemy enemy;
    [Range(0,1)]
    public float knockbackResistance;

    public override void Start()
    {
        base.Start();
        enemy = GetComponent<Enemy>();
    }

    public void Update()
    {

    }

    public new void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Attacked" && isBlinking)
        {
            isBlinking = false;
            StartCoroutine(FlashRed());
            hero_Melee_Attack heroAttack = other.GetComponent<hero_Melee_Attack>();
            MoveSet moveset = enemy.gameObject.GetComponent<MoveSet>();
            if (heroAttack.attackKnockBack.magnitude != 0)
            {
                float dirKB = transform.position.x - heroAttack.transform.position.x;
                Vector2 knockBack = heroAttack.attackKnockBack;
                knockBack.x = knockBack.x * dirKB;

                TakeHit(heroAttack.attackDamage);
                if (takesKB == true)
                {
                    base.controller.Move(new Vector2(0, .5f));
                    if (dirKB >= 0)
                    {
                        moveset.TakeKB(new Vector2(heroAttack.attackKnockBack.x - (knockbackResistance * heroAttack.attackKnockBack.x), moveset.velocity.y + (heroAttack.attackKnockBack.y - (knockbackResistance * heroAttack.attackKnockBack.x))));
                    }
                    else
                    {
                        moveset.TakeKB(new Vector2(-(heroAttack.attackKnockBack.x - (knockbackResistance * heroAttack.attackKnockBack.x)), moveset.velocity.y + (heroAttack.attackKnockBack.y - (knockbackResistance * heroAttack.attackKnockBack.x))));
                    }
                }

                if (Player.downStrike == true)
                {
                    Player.bounce = true;
                    Player.hasDash = true;
                }
            }
            else
            {
                TakeHit(heroAttack.attackDamage);
            }
        }

    }

    public override void TakeHit(int damage)
    {
        base.TakeHit(damage);

        if (Hitpoints <= 0)
        {
            enemy.OnDeath();
        }
    }
}
