using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shadeling_Damage_Controller : MonoBehaviour
{
    public int damage = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<Player_Health_Controller>().TakeHit(damage);
        }
    }
}
