using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity
{
    protected NodeMoveSet nodes;
    protected Vector2 targetNode;

    public List<Transform> points;

    public int nextID = 0;

    public float speed = 8;

    [SerializeField]
    protected Transform player;
    [SerializeField]
    protected float agroRange;

    public Player.PlayerForm enemyType;
    protected Enemy_Health_Controller healthController;

    protected Vector2 startingLocation;
    protected Vector2[] startingNodes;
    protected int numNodes;

    public bool isDead;

    // Spirit orb to be dropped upon death
    // Set in the Unity Editor
    public GameObject SpiritOrbPrefab;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        nodes = GetComponent<NodeMoveSet>();
        targetNode = nodes.GetNextNode();

        healthController = GetComponent<Enemy_Health_Controller>();

        InitializeStartingInfo();
    }

    protected void InitializeStartingInfo()
    {
        startingLocation = (Vector2)transform.position;
        numNodes = (nodes.useLocalNodes) ? nodes.localNodes.Length : nodes.globalNodes.Length;
        startingNodes = new Vector2[numNodes];

        for (int i = 0; i < numNodes; i++)
        {
            startingNodes[i] = nodes.globalNodes[i];
        }
    }

    protected void Patrol()
    {
        if (Mathf.Abs(targetNode.x - transform.position.x) < 0.2)
        {
            nodes.NextNode();
            targetNode = nodes.GetNextNode();
        }

        move.x = Mathf.Sign((targetNode - (Vector2)transform.position).x) * 0.6f;

        if (facingRight && move.x < 0) // started moving left
        {
            Flip();
        }
        else if (!facingRight && move.x > 0) // started moving right
        {
            Flip();
        }
    }

    public virtual bool PlayerInAgroRange()
    {
        return DistanceToPlayer() <= agroRange;
    }

    public float DistanceToPlayer()
    {
        float dist = Vector2.Distance((Vector2)player.position, (Vector2)transform.position);
        // Debug.Log("Distance to player: " + dist);
        return Vector2.Distance((Vector2)player.position, (Vector2)transform.position);
    }

    public virtual void OnDeath()
    {
        isDead = true;
        moveSet.DisableMoveset();
        (moveSet as BasicMoveSet).controller.main_collider.enabled = false;
        GetComponent<SpriteRenderer>().enabled = false;
    }

    public virtual void OnRespawn()
    {
        isDead = false;
        moveSet.EnableMoveset();
        (moveSet as BasicMoveSet).controller.main_collider.enabled = true;

        transform.position = startingLocation;

        for (int i = 0; i < numNodes; i++)
        {
            nodes.globalNodes[i] = startingNodes[i];
        }
        nodes.ResetNumbers();

        healthController.ResetHealth();

        GetComponent<SpriteRenderer>().enabled = true;
    }

    protected void DropSpiritOrb()
    {
        SpiritOrb droppedOrb = Instantiate(SpiritOrbPrefab, transform.position, Quaternion.identity).GetComponent<SpiritOrb>();
        droppedOrb.parentEnemy = this;
    }

    protected void FlipTowardsPlayer()
    {
        float playerPosition = player.position.x - transform.position.x;
        if (playerPosition < 0 && facingRight)
        {
            Flip();
        }
        else if (playerPosition > 0 && !facingRight)
        {
            Flip();
        }
    }

    protected void Flip()
    {
        facingRight = !facingRight;
        transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
    }
}
