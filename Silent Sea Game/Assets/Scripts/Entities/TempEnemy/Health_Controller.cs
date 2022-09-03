/*
    Health:
        https://www.youtube.com/watch?v=7DPElwMtc9Y&ab_channel=DistortedPixelStudios
    Red Blinking:
        https://www.youtube.com/watch?v=veFcxTNsfZY&ab_channel=ThatBoopGuy
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health_Controller : MonoBehaviour
{
    protected bool isBlinking = true;
    public bool takesKB = true;
    public int Hitpoints;
    public int MaxHitPoints = 5;
    //public Skullbug_Healthbar Healthbar;
    public SpriteRenderer sprite;
    public Color originalColor;

    protected MovementController controller;

    // Start is called before the first frame update
    public virtual void Start()
    {
        Hitpoints = MaxHitPoints;
        sprite = GetComponent<SpriteRenderer>();
        //Healthbar.SetHealth(Hitpoints, MaxHitPoints);

        controller = GetComponent<MovementController>();
    }

    public virtual void TakeHit(int damage)
    {
        Hitpoints = Hitpoints - damage;
        //if(Hitpoints <= 0)
        //{
        //    Destroy(gameObject);
        //}
    }

    public virtual void TakeHit(int damage, Vector2 knockBack)
    {
        TakeHit(damage);

        if (takesKB == true)
        {
            controller.Move(knockBack);
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Attacked" && isBlinking)
        {
            isBlinking = false;
            StartCoroutine(FlashRed());
            hero_Melee_Attack heroAttack = other.GetComponent<hero_Melee_Attack>();
            if (heroAttack.attackKnockBack.magnitude != 0)
            {
                float dirKB = transform.position.x - heroAttack.transform.position.x;
                Vector2 knockBack = heroAttack.attackKnockBack;
                knockBack.x = knockBack.x * dirKB;

                TakeHit(heroAttack.attackDamage, knockBack);
            }
            else
            {
                TakeHit(heroAttack.attackDamage);
            }
        }
  
    }

    public void ResetHealth()
    {
        Hitpoints = MaxHitPoints;
    }

    public IEnumerator FlashRed()
    {
        originalColor = sprite.color;
        sprite.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        sprite.color = originalColor;
        isBlinking = true;
    }

    public IEnumerator FlashBlue()
    {
        originalColor = sprite.color;
        sprite.color = Color.blue;
        yield return new WaitForSeconds(0.1f);
        sprite.color = originalColor;
        isBlinking = true;
    }
}
