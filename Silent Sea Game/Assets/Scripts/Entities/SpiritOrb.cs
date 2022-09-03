using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiritOrb : Entity
{
    public bool testing;

    public Player.PlayerForm form;
    public bool closestToPlayer = false;

    public Enemy parentEnemy;

    public void UsedToTransform()
    {
        if (!testing)
        {
            GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    public void TransformedBack()
    {
        if (!testing)
        {
            parentEnemy.OnRespawn();

            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<Player>().PlayerCanTransformInto(transform);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<Player>().PlayerNoLongerInRange(transform);
        }
    }
}
