/*
    Derivative of ranged combat shown in this video:
    https://www.youtube.com/watch?v=8TqY6p-PRcs&t=15s&ab_channel=DistortedPixelStudios
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hero_Melee_Attack : MonoBehaviour
{
    public float durationOfAttack = 1.0f;
    public float MovementSpeed = 1;
    public float JumpForce = 1;
    private Rigidbody2D rigidBody;
    public int attackDamage = 1;

    public Vector2 attackKnockBack;

    public void SetKnockBackAndDamage(int damage, Vector2 knockBack)
    {
        attackDamage = damage;
        attackKnockBack = knockBack;
    }

    // Update is called once per frame
    void Update()
    {
        var movement = Input.GetAxis("Horizontal");
        transform.position = transform.position + new Vector3(movement, 0, 0) * Time.deltaTime * MovementSpeed;

        if(!Mathf.Approximately(0, movement))
        {
            transform.rotation = movement < 0 ? Quaternion.Euler(0, 180, 0) : Quaternion.identity;
        }

        if(Input.GetButtonDown("Jump") && Mathf.Abs(rigidBody.velocity.y) < 0.001f)
        {
            rigidBody.AddForce(new Vector2(0, JumpForce), ForceMode2D.Impulse);
        }


        Destroy(gameObject, durationOfAttack);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //var enemy = collision.gameObject.GetComponent<Skullbug_Controller>();
        //if(enemy)
        //{
        //    enemy.TakeHit(1);
        //}
    }

}