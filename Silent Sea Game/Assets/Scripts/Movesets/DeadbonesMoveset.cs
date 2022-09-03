using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadbonesMoveset : BasicMoveSet
{
    // [SerializeField] Transform target;
    // UnityEngine.AI.NavMeshAgent agent;

    // // This is the enemy that the moveset will work for (intended for Shadeling)
    // public Enemy enemy;
    // // This is the distance from the player, at which point the Deadbones will attack
    // public float stayBackDistance;

    // private bool isAttacking = false;
    // private bool ready = false;
    // public float chargeTime = 1f;
    // private float currentChargeTime;
    // public float attackTime = 2f;
    // private float currentAttackTime;
    // private Health_Controller hp_ctrl;

    // // Start is called before the first frame update
    // protected override void Start()
    // {
    //     base.Start();
    //     agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
    //     agent.updateRotation = false;
    //     agent.updateUpAxis = false;
    //     currentChargeTime = chargeTime;
    //     currentAttackTime = attackTime;
    //     //hp_ctrl = GetComponent<Shadeling_Health_Controller>();
    // }

    // // Update is called once per frame
    // protected override void FixedUpdate()
    // {
    //     base.FixedUpdate();

    //     base.gravity = 0;
    //     // While not attacking, the Deadbones should only hunt the player when they are within agro range.
    //     if (!isAttacking)
    //     {
    //         // If player in agro range, start moving towards the player
    //         if (enemy.PlayerInAgroRange() && enemy.DistanceToPlayer() > stayBackDistance)
    //         {
    //             agent.SetDestination(target.position);
    //         }
    //         // If Shadeling is within a certain distance of the player, begin attack
    //         else if (enemy.PlayerInAgroRange() && enemy.DistanceToPlayer() < stayBackDistance)
    //         {
    //             agent.SetDestination(this.transform.position);
    //             // Set up attack sequence
    //             isAttacking = !isAttacking;
    //             //StartCoroutine(hp_ctrl.FlashBlue());
    //             agent.speed = 15;
    //             agent.acceleration = 100;
    //         }
    //         // If player is not in agro range, remain idle
    //         else
    //         {
    //             agent.SetDestination(this.transform.position);
    //         }
    //     }
    //     // When attacking, the Shadeling should give a warning before charging through the player's last position.
    //     if (isAttacking)
    //     {
    //         if (currentChargeTime > 0 && ready == false)
    //         {
    //             currentChargeTime -= Time.deltaTime;
    //         }
    //         else if (currentChargeTime <= 0 && ready == false && currentAttackTime > 0)
    //         {
    //             ready = true;
    //             agent.SetDestination(GetDistanceBetweenPlayerAndDeadbones());
    //         }
    //         else if (currentChargeTime <= 0 && ready == true)
    //         {
    //             if (currentAttackTime > 0)
    //             {
    //                 currentAttackTime -= Time.deltaTime;
    //             }
    //             else if (currentAttackTime <= 0)
    //             {
    //                 agent.speed = 5;
    //                 agent.acceleration = 10;
    //                 ready = false;
    //                 currentChargeTime = chargeTime;
    //                 currentAttackTime = attackTime;
    //                 isAttacking = !isAttacking;
    //             }
    //         }
    //     }
    // }

    // private Vector2 GetDistanceBetweenPlayerAndDeadbones()
    // {
    //     // Variables for storing the total distance between player and shadeling
    //     float dX, dY;
    //     // Gather 
    //     dX = target.position.x + (target.position.x - this.transform.position.x);
    //     dY = target.position.y + (target.position.y - this.transform.position.y);
    //     print("dX = " + dX + ", dY = " + dY);
    //     return new Vector2(dX, dY);
    // }
}