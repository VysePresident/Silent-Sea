using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceSpike : AttackHitBox
{
    public float speed;
    public float launchVelocity;
    public float gravity;

    public bool affectedByGravity, launchStart;

    public Vector2 direction;

    public Status status;

    public float timeToReady = 0.5f;

    protected Vector2 velocity;

    public float animationTimeLeft = 0.5f;

    protected override void Start()
    {
        base.Start();
        status = Status.Growing;

        velocity = new Vector2(0, 0);
    }

    protected override void Update()
    {
        
    }

    protected void FixedUpdate()
    {

        switch (status)
        {
            case Status.Growing:
                if (timeToReady <= 0)
                {
                    status = Status.Ready;
                }
                else
                {
                    timeToReady -= Time.deltaTime;
                }
                break;
            case Status.Moving:
                if (controller.collisions.below || controller.collisions.left || controller.collisions.right)
                {
                    OnAttackOver();
                }
                else
                {
                    DoMovement();
                }
                break;
            case Status.Breaking:
                AlreadyHit = true;
                DoSmashingAnimation();
                break;
            case Status.Ready:
            default:
                break;
        }
    }

    public void Launch()
    {
        if (launchStart)
        {
            velocity.x = direction.normalized.x * launchVelocity;
            velocity.y = direction.normalized.y * launchVelocity;
        }
        else
        {
            velocity.x = direction.normalized.x * speed * Time.deltaTime;
            velocity.y = direction.normalized.y * speed * Time.deltaTime;
        }

        controller.Move(velocity);

        status = Status.Moving;
    }

    protected void DoMovement()
    {
        velocity.x = direction.normalized.x * speed;

        if (affectedByGravity)
        {
            velocity.y += gravity * Time.deltaTime;
        }
        else
        {
            velocity.y = direction.normalized.y * speed;
        }

        controller.Move(velocity * Time.deltaTime);
    }

    protected void DoSmashingAnimation()
    {
        if (animationTimeLeft <= 0)
        {
            Destroy(gameObject);
        }
        else
        {
            animationTimeLeft -= Time.deltaTime;
        }
    }

    protected override void DoKnockBackHit(GameObject playerObject)
    {
        float xDir;
        Vector2 kb = new Vector2(knockBack.x, knockBack.y);
        if (controller.oldVelocity.x != 0)
        {
            xDir = Mathf.Sign(controller.oldVelocity.x);
        }
        else
        {
            xDir = Mathf.Sign(playerObject.transform.position.x - transform.position.x);
        }

        kb.x *= xDir;

        playerObject.GetComponent<Health_Controller>().TakeHit(AttackDamage, kb);
    }

    protected override void OnAttackOver()
    {
        base.OnAttackOver();

        AlreadyHit = true;
        status = Status.Breaking;
    }

    public enum Status
    {
        Growing,
        Ready,
        Moving,
        Breaking
    }
}
