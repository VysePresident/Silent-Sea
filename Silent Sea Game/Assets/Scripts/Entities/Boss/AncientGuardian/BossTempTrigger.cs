using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTempTrigger : MonoBehaviour
{
    public AncientGuardian boss;
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            // Get the ice spike attack going
            boss.status = AncientGuardian.Status.MovingToIceSpikeAttackPosition;
        }
    }
}
