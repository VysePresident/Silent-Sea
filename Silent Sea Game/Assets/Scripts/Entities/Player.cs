using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : Entity
{
    public BasicMoveSet normalMoveSet;
    List<MoveSet> alternateMoveSets;
    int numMoveSets = 2;
    private MovementController movementPlayer;

    // ANDREW BARRETT UPDATE: Currency variable
    // ISAAC BURNS UPDATE: Currency now transitions between scenes
    public static int Currency = 0;
    public int coins;
    public Text coinText;

    // ANDREW BARRETT UPDATE: Treasure chest
    public GameObject coin;

    //ALEX BURNS UPDATE: Create animator
    public Animator animator;

    // ALEX ANDREWS: Sprite positioning changes affecting physics interactions and camera
    // I changed the "center" of the sprite and the positioning of the sprite and hitbox
    // We can now use the SpriteRenderer and simply flip the sprite
    public SpriteRenderer spriteRenderer;


    //NATHAN B UPDATE:
    //Adding melee attacks to the player entity.
    //These public variables 
    public hero_Melee_Attack meleePrefab;
    public hero_Melee_Attack meleeRedBlinkPrefab;
    public Transform attackPosRight;
    public Transform attackPosLeft;
    public Transform attackPosUp;
    public Transform attackPosDown;
    public float attackRange;
    private float cooldownTimeForAttack = .4f; //set this for cooldown time for attack
    private bool canAttack = true;

    // ALEX A UPDATE:
    // Added a timer for the transformation mechanic
    public float transformationTimeRemaining = 0f;  // time in seconds left before transforming back to normal
    public float transformationDuration = 30f;
    public float spiritOrbDetectionRadius = 20f;
    public CircleCollider2D orbDetector;

    // ALEX A UPDATE: Spirit Orb tracking and transformation updates
    private Dictionary<Transform, SpiritOrb> spiritOrbsInRange = new Dictionary<Transform, SpiritOrb>();
    private Transform closestOrbKey = null;
    private SpiritOrb usedOrb;

    public PlayerForm currentForm = PlayerForm.Normal;
    public PlayerForm canTransformInto;

    public int attackDamage = 1;
    [HideInInspector]
    public Player_Health_Controller healthController;

    public bool isTryingToWallClimb = false;

    // HUD Current Form sprites
    public Text transformTimeDisplay;
    public Text blankTimeDisplay;
    private float emptyTime;
    public Image transformation;
    public Sprite NoTransformation;
    public Sprite SkullbugTransformation;
    public Sprite ReptoadTransformation;
    public Sprite NormalForm;
    public Sprite ReptoadForm;
    public Sprite SkullbugForm;
    public Sprite Feral_ShadowForm;
    public Sprite Boss_AncientGuardianForm;
    public SpriteRenderer formRenderer;

    // Save update trigger boolean
    public static bool save = false;
    // Transition boolean (used for locking velocity at transitions and preventing jump at transitions)
    public static bool transition;

    // Variables used for bouncing movement after successful downward strike
    public float bounceHeight;
    public static bool downStrike = false;
    public static bool bounce = false;

    // Variables used for the dash power
    public float dashVelocity = 20f;
    public float dashTimer = 0.4f;
    private bool isDashing;
    public static bool hasDash = true;
    private bool dashingRight = false;
    private bool dashButtonDown = false;

    protected void Start()
    {
        facingRight = true;
        movementPlayer = GetComponent<MovementController>();
        moveSet = normalMoveSet = GetComponent<BasicMoveSet>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        healthController = GetComponent<Player_Health_Controller>();
        transition = false;

        InitializeAlternateMoveSets();
    }

    private void InitializeAlternateMoveSets()
    {
        alternateMoveSets = new List<MoveSet>();
        alternateMoveSets.Add(GetComponent<ReptoadMoveSet>());
        alternateMoveSets.Add(GetComponent<PlayerSkullBugMoveSet>());

        foreach (MoveSet alternateMoveSet in alternateMoveSets)
        {
            alternateMoveSet.DisableMoveset();
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (save == true)
        {
            SavePlayer();
            save = false;
        }

        coins = Currency;
        if (healthController.Hitpoints > 0)
        {
            // Handles dash movements
            if (isDashing == true)
            {
                moveSet.velocity.y = 0;
                if (dashingRight == true)
                {
                    moveSet.velocity.x = dashVelocity;
                }
                else
                {
                    moveSet.velocity.x = -dashVelocity;
                }
            }

            // Handles movement while not dashing
            else
            {
                move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
                jump = Input.GetButton("Jump");

                if (move.y == 1)
                {
                    facingUp = true;
                }
                else
                {
                    facingUp = false;
                }

                //Alex Burns - Jump/Fall Animation Triggers
                if (moveSet.velocity.y < -1.26f || jump && transition == false)
                {
                    animator.SetBool("isJumping", true);
                }
                if (movementPlayer.collisions.below)
                {
                    animator.SetBool("isJumping", false);
                }

                animator.SetFloat("Speed", Mathf.Abs(/*Input.GetAxisRaw("Horizontal")*/moveSet.velocity.x));

                if (currentForm == PlayerForm.Skullbug && Input.GetKey(KeyCode.LeftShift))
                {
                    isTryingToWallClimb = true;
                }
                else
                {
                    isTryingToWallClimb = false;
                }

                // If statement for handling the bounce from down attacks
                if (bounce == true)
                {
                    moveSet.TakeKB(new Vector2(moveSet.velocity.x, bounceHeight));
                    bounce = false;
                }

                // If statement for handling dash
                if (hasDash == true && Input.GetKeyDown(KeyCode.LeftControl) && Player_Health_Controller.dash_power == true && dashButtonDown == false)
                {
                    moveSet.ignoreGravity = true;
                    isDashing = true;
                    hasDash = false;
                    if (facingRight == true)
                    {
                        dashingRight = true;
                    }
                    else
                    {
                        dashingRight = false;
                    }
                    StartCoroutine(Dash());
                }



                CheckSpiritOrbsInRange();

                if (Input.GetButtonDown("Fire2") && Player_Health_Controller.transform_power == true)
                {
                    print("l pushed");
                    if (currentForm == PlayerForm.Normal && canTransformInto != PlayerForm.Normal)
                    {
                        ChangeForm();
                    }
                    else
                    {
                        RevertToNormal();
                    }

                }

                if (move.x != 0)
                {
                    if (Mathf.Sign(move.x) == 1 && !facingRight)
                    {
                        FlipFacing();
                    }
                    else if (Mathf.Sign(move.x) == -1 && facingRight)
                    {
                        FlipFacing();
                    }
                }

                if (Input.GetButtonDown("Fire1") && canAttack == true && !(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S)))
                {
                    if (facingRight)
                    {
                        canAttack = false;
                        hero_Melee_Attack attack = Instantiate(meleeRedBlinkPrefab, attackPosRight.position, transform.rotation);
                        attack.SetKnockBackAndDamage(attackDamage, attackKnockBack);
                        StartCoroutine(Cooldown());
                    }
                    if (!facingRight)
                    {
                        canAttack = false;
                        hero_Melee_Attack attack = Instantiate(meleeRedBlinkPrefab, attackPosLeft.position, transform.rotation);
                        attack.SetKnockBackAndDamage(attackDamage, attackKnockBack);
                        StartCoroutine(Cooldown());
                    }
                    //ALEX BURNS UPDATE: TO-MELEE TRANSITION
                    animator.SetBool("isAttacking", true); //Attack Animation Trigger
                }
                else if (Input.GetKey(KeyCode.W) && Input.GetButtonDown("Fire1") && canAttack == true)
                {
                    canAttack = false;
                    Instantiate(meleePrefab, attackPosUp.position, transform.rotation);
                    hero_Melee_Attack attack = Instantiate(meleeRedBlinkPrefab, attackPosUp.position, transform.rotation);
                    attack.SetKnockBackAndDamage(attackDamage, attackKnockBack);
                    animator.SetBool("isAttackingUp", true);
                    StartCoroutine(Cooldown());
                }
                else if (Input.GetKey(KeyCode.S) && Input.GetButtonDown("Fire1") && canAttack == true)
                {
                    canAttack = false;
                    downStrike = true;
                    Instantiate(meleePrefab, attackPosDown.position, transform.rotation);
                    hero_Melee_Attack attack = Instantiate(meleeRedBlinkPrefab, attackPosDown.position, transform.rotation);
                    attack.SetKnockBackAndDamage(attackDamage, attackKnockBack);
                    animator.SetBool("isAttackingDown", true);
                    StartCoroutine(Cooldown());
                }
                else
                {
                    animator.SetBool("isAttacking", false); //Ends Attack Animation Trigger
                    animator.SetBool("isAttackingUp", false);
                    animator.SetBool("isAttackingDown", false);
                }
            }
        }
        else
        {
            move.x = move.y = 0;
            jump = false;
        }


        // ISAAC BURNS UPDATE: Coin text will update without the coins triggering that update.
        coinText.text = Currency.ToString();
        // ISAAC BURNS UPDATE: If transitioning, velocity will be locked
        if (transition == true)
        {
            normalMoveSet.LockVelocity();
        }

        // Reset dash after landing on the ground
        if (isDashing == false && animator.GetBool("isJumping") == false && dashButtonDown == false)
        {
            hasDash = true;
        }

        if (currentForm == PlayerForm.Normal)
        {
            transformation.sprite = NoTransformation; //in normal form there is no image on screen
        }
        // If the player is currently transformed (not the normal form)
        if (currentForm != PlayerForm.Normal)
        {
            transformationTimeRemaining -= Time.deltaTime;
            transformTimeDisplay.text = transformationTimeRemaining.ToString(); //puts countdown timer on screen

            // If time is up
            if (transformationTimeRemaining <= 0)
            {
                transformTimeDisplay.text = blankTimeDisplay.text; //removes time from screen
                transformation.sprite = NoTransformation; //removes transition image from screen
                                                          // Transform back
                RevertToNormal();
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            dashButtonDown = true;
        }
        else if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            dashButtonDown = false;
        }
    }

    void FlipFacing()
    {
        facingRight = !facingRight;
        //transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
        spriteRenderer.flipX = !spriteRenderer.flipX;
    }

    void ChangeForm()
    {
        if (canTransformInto != PlayerForm.Normal)
        {
            Vector2 oldVelocity = moveSet.DisableMoveset();

            currentForm = canTransformInto;
            animator.SetTrigger("Transform");
            switch (currentForm)
            {
                case PlayerForm.Reptoad:
                    moveSet = alternateMoveSets[0];
                    transformation.sprite = ReptoadTransformation; //shows icon of reptoad on screen during transform
                    // Code to activeate reptoad attacks
                    break;
                case PlayerForm.Skullbug:
                    moveSet = alternateMoveSets[1];
                    transformation.sprite = SkullbugTransformation; //shows icon of skullbug on screen during transform
                    //isTryingToWallClimb = true;
                    (moveSet as PlayerSkullBugMoveSet).isWallClimbing = true;
                    break;
            }

            moveSet.EnableMoveset(oldVelocity);

            transformationTimeRemaining = transformationDuration;



            //TeleportToOrb();

            usedOrb = spiritOrbsInRange[closestOrbKey];
            usedOrb.UsedToTransform();
        }
        updateFormUI();
    }

    void TeleportToOrb()
    {
        (moveSet as BasicMoveSet).controller.Teleport(spiritOrbsInRange[closestOrbKey].transform.position);
    }

    void RevertToNormal()
    {
        if (currentForm != PlayerForm.Normal)
        {
            animator.SetTrigger("Revert");
            Vector2 oldVelocity = moveSet.DisableMoveset();
            // Code to deactivate any other attacks/abilities from transformation

            currentForm = PlayerForm.Normal;
            moveSet = normalMoveSet;
            moveSet.EnableMoveset(oldVelocity);
            transformTimeDisplay.text = blankTimeDisplay.text; //removes time left for transform from screen

            usedOrb.TransformedBack();
            usedOrb = null;
        }
        updateFormUI();
    }

    private void updateFormUI()
    {
        Sprite formSprite = NormalForm;
        //transformation[0].sprite = NoTransformation;
        switch (currentForm)
        {
            case PlayerForm.Boss_AncientGuardian:
                formSprite = Boss_AncientGuardianForm;
                break;
            case PlayerForm.Feral_Shadow:
                formSprite = Feral_ShadowForm;
                break;
            case PlayerForm.Reptoad:
                formSprite = ReptoadForm;
                //transformation[2].sprite = ReptoadTransformation;
                break;
            case PlayerForm.Skullbug:
                formSprite = SkullbugForm;
                //transformation[1].sprite = SkullbugTransformation;
                break;
        }
        formRenderer.sprite = formSprite;
    }

    private void CheckSpiritOrbsInRange()
    {
        if (currentForm == PlayerForm.Normal && spiritOrbsInRange.Count > 0)
        {
            Transform newClosestOrbKey = FindClosestSpiritOrb();
            if (closestOrbKey == null)
            {
                closestOrbKey = newClosestOrbKey;
            }
            else if (closestOrbKey != newClosestOrbKey)
            {
                spiritOrbsInRange[closestOrbKey].closestToPlayer = false; // Old orb no longer closest
                closestOrbKey = newClosestOrbKey;
            }

            canTransformInto = spiritOrbsInRange[closestOrbKey].form;
            spiritOrbsInRange[closestOrbKey].closestToPlayer = true;
        }
        else
        {
            canTransformInto = PlayerForm.Normal;
        }
    }

    private Transform FindClosestSpiritOrb()
    {
        Transform closest = null;
        float distToClosest = float.MaxValue;

        foreach (var orb in spiritOrbsInRange)
        {
            float dist = Vector3.Distance(orb.Key.position, transform.position); // Calculate the distance between the orb and the player
            if (dist < distToClosest)
            {
                distToClosest = dist;
                closest = orb.Key;
            }
        }

        return closest;
    }

    public void PlayerCanTransformInto(Transform transform)
    {
        if (!spiritOrbsInRange.ContainsKey(transform))
        {
            spiritOrbsInRange.Add(transform, transform.GetComponent<SpiritOrb>());
        }
    }

    public void PlayerNoLongerInRange(Transform transform)
    {
        if (spiritOrbsInRange.ContainsKey(transform))
        {
            spiritOrbsInRange[transform].closestToPlayer = false;
            spiritOrbsInRange.Remove(transform);
            if (transform == closestOrbKey)
            {
                closestOrbKey = null;
            }
        }
    }

    // When a Spirit Orb comes within range of the player's orb-detection radius, add it to the list of orbs in range
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Coin"))
        {
            // add to currency count
            Currency++;
            // ISAAC BURNS UPDATE: Moved coin text update to Update() function.
            // remove coin
            Destroy(collision.gameObject);
        }
        else if (collision.CompareTag("TreasureChest"))
        {
            // entered treasure chest
            for (int i = 0; i < 10; i++)
            {
                Instantiate(coin, collision.gameObject.transform.position, collision.gameObject.transform.rotation);
            }
            Debug.Log("entered chest");
            Destroy(collision.gameObject);
        }
    }

    // When a Spirit Orb becomes too far away, remove it from the list of Spirit Orbs in range
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("SpiritOrb"))
        {
            if (spiritOrbsInRange.ContainsKey(collision.transform))
            {
                spiritOrbsInRange[collision.transform].closestToPlayer = false;
                spiritOrbsInRange.Remove(collision.transform);
                if (collision.transform == closestOrbKey)
                {
                    closestOrbKey = null;
                }
            }
        }
    }

    public void SavePlayer()
    {
        SaveSystem.SavePlayer(this);
    }

    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(cooldownTimeForAttack);
        canAttack = true;
        downStrike = false;
    }

    IEnumerator Dash()
    {
        yield return new WaitForSeconds(dashTimer);
        isDashing = false;
        moveSet.ignoreGravity = false;
    }

    private void OnDrawGizmosSelected()
    { //to see the area of the attack positions
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPosRight.position, attackRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPosLeft.position, attackRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPosUp.position, attackRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPosDown.position, attackRange);
    }

    public enum PlayerForm
    {
        Normal,
        Reptoad,
        Skullbug,
        Feral_Shadow,
        Boss_AncientGuardian,
        Deadbones
    }
}