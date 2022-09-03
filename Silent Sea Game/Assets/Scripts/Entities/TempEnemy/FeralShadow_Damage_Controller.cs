using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeralShadow_Damage_Controller : MonoBehaviour
{
    public FeralShadow feralShadow;
    protected MovementController movementController;

    public FSMeleeAttack meleeAttack;
    public FSLungeAttack lungeAttack;
    public Vector2 attackHitBoxOffset;

    [HideInInspector]
    public AttackHitBox currentAttack;

    public int meleeHitDamage = 3;
    public int lungeHitDamage = 5;

    protected bool alreadyHitPlayer = false;

    public LayerMask attackMask;

    public Vector2 meleeOriginOffset;
    public float meleeRayLength = 1.0f;
    public float meleeRaySpacing = 0.5f;
    public int numMeleeRays = 3;

    public float lungeRayLength = 0.3f;
    public float lungeRaySpacing = 0.5f;
    public int numLungeRays = 3;

    protected Player_Health_Controller playerHealthController;
    protected MovementController playerMovementController;

    // Start is called before the first frame update
    void Start()
    {
        movementController = GetComponent<MovementController>();
    }

    public void ResetAttack()
    {
        alreadyHitPlayer = false;

        if (currentAttack != null)
        {
            Destroy(currentAttack.gameObject);
        }
    }

    public void PerformMeleeAttack()
    {
        if (currentAttack != null)
        {
            if (currentAttack.AttackOver)
            {
                Destroy(currentAttack.gameObject);
            }
        }
        else
        {
            Vector2 pos = transform.position;
            pos.x += attackHitBoxOffset.x * ((feralShadow.facingRight) ? 1 : -1);
            pos.y += attackHitBoxOffset.y;

            currentAttack = Instantiate(meleeAttack, pos, Quaternion.identity);
            (currentAttack as FSMeleeAttack).parent = feralShadow;
        }
    }

    public void PerformingLungeAttack()
    {
        if (currentAttack != null)
        {
            //currentAttack.controller.Move(feralShadow.feralShadowMoveSet.velocity * Time.deltaTime);
        }
        else
        {
            Vector2 pos = transform.position;
            pos.x += attackHitBoxOffset.x * ((feralShadow.facingRight) ? 1 : -1);
            pos.y += attackHitBoxOffset.y;

            currentAttack = Instantiate(lungeAttack, pos, Quaternion.identity);
        }
    }

    protected Transform CheckForMeleeHit()
    {
        float dirX = (feralShadow.facingRight) ? 1.0f : -1.0f;
        Vector2 meleeOrigin = GetMeleeOrigin();

        for (int i = 0; i < numMeleeRays; i++)
        {
            Vector2 rayOrigin = meleeOrigin + (Vector2.up * (meleeRaySpacing * i)); // Origin of melee hit detection ray

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * dirX, meleeRayLength, attackMask);

            Debug.DrawRay(rayOrigin, Vector2.right * dirX * meleeRayLength, Color.blue);

            if (hit) // If it hits the player
            {
                return (hit.transform);
            }
        }

        return (null);
    }

    protected Transform CheckForLungeHit()
    {
        float dirX = (feralShadow.facingRight) ? 1.0f : -1.0f;
        Vector2 lungeOrigin = (feralShadow.facingRight) ? movementController.GetRaycastOrigins().bottomRight : movementController.GetRaycastOrigins().bottomLeft;
        lungeOrigin += Vector2.right * -dirX * (lungeRayLength / 2); // Set lunch hit detection inside of the feral shadow

        for (int i = 0; i < numLungeRays; i++)
        {
            Vector2 rayOrigin = lungeOrigin + (Vector2.up * (lungeRaySpacing * i)); // Origin of lunge hit detection ray

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * dirX, lungeRayLength, attackMask);

            Debug.DrawRay(rayOrigin, Vector2.right * dirX * lungeRayLength, Color.green);

            if (hit) // If it hits the player
            {
                return (hit.transform);
            }
        }

        return (null);
    }

    public Vector2 GetMeleeOrigin()
    {
        float dirX = (feralShadow.facingRight) ? 1.0f : -1.0f;
        Vector2 offset = meleeOriginOffset * Vector2.right * dirX;
        Vector2 meleeOrigin = (Vector2)transform.position + offset;

        return meleeOrigin;
    }

    protected void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        float size = 0.2f;

        Vector2 pos = (Vector2)transform.position + attackHitBoxOffset * (Vector2.right * ((feralShadow.facingRight) ? 1.0f : -1.0f)) + attackHitBoxOffset * Vector2.up;

        Gizmos.DrawSphere(pos, size);
    }
}
