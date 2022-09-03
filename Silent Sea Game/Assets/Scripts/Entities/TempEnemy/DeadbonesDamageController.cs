using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadbonesDamageController : MonoBehaviour
{
    public Player player;
    public int damage = 1;
    public bool enabled = false;

    bool playerIsInBox = false;

    public void doMeleeDamage()
    {
        if (playerIsInBox)
        {
            player.healthController.TakeHit(damage);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            playerIsInBox = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerIsInBox = false;
        }
    }
}
