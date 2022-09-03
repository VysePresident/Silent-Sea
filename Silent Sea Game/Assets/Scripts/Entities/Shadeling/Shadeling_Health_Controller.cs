using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shadeling_Health_Controller : Enemy_Health_Controller
{
    public bool isStunned = false;

    public override void TakeHit(int damage, Vector2 knockBack)
    {
        TakeHit(damage);

        if (takesKB == true)
        {
            controller.Move(knockBack);
        }
        isStunned = true;
    }

    public new void OnTriggerEnter2D(Collider2D other)
    {
        print("test successful");
        if (other.gameObject.tag == "Attacked" && isBlinking)
        {
            isBlinking = false;
            StartCoroutine(base.FlashRed());
            hero_Melee_Attack heroAttack = other.GetComponent<hero_Melee_Attack>();
            ShadelingMoveSet moveset = enemy.gameObject.GetComponent<ShadelingMoveSet>();
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
                        if (Player.downStrike == false)
                            moveset.agent.SetDestination(new Vector2(moveset.target.transform.position.x + moveset.evasionDistance.x, moveset.target.transform.position.y + moveset.evasionDistance.y));
                        else
                            moveset.agent.SetDestination(new Vector2(moveset.target.transform.position.x + moveset.evasionDistance.x, moveset.target.transform.position.y - moveset.evasionDistance.y));
                    }
                    else
                    {
                        if (Player.downStrike == false)
                            moveset.agent.SetDestination(new Vector2(moveset.target.transform.position.x - moveset.evasionDistance.x, moveset.target.transform.position.y + moveset.evasionDistance.y));
                        else
                            moveset.agent.SetDestination(new Vector2(moveset.target.transform.position.x - moveset.evasionDistance.x, moveset.target.transform.position.y - moveset.evasionDistance.y));
                    }
                }

                if (Player.downStrike == true)
                {
                    Player.bounce = true;
                }
            }
            else
            {
                TakeHit(heroAttack.attackDamage);
            }
        }

    }

}
