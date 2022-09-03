using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

public class Player_Health_Controller : Health_Controller
{
    // Heart images
    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite halfHeart;
    public Sprite emptyHeart;

    // ISAAC BURNS UPDATE:
    // Checkpoint data implemented.  May require some simplifications later.
    public static Vector3 DefaultSpawn;             // Where the player will spawn after a transition or death
    public static Vector3 RespawnPosition;          // When the player falls on spikes, this is the position of the checkpoint they will teleport to
    private GameObject playerObj = null;            // Player
    private static Vector3 StartPos;                // Where the player is set before the game is run
    public bool DemoStart;                          // Set to true if you want the player to spawn wherever you placed the Player object in the scene whenever you play the scene
    public static bool demo = true;                // Modified when DemoStart is true
    public static bool firstOpen = true;           // Modified when the game is run the first time
    public static int HP_transfer;                  // HP transferred from one scene to another
    public static int Coin_transfer;                  // HP transferred from one scene to another

    public float flashOnHitDuration = 0.4f;
    public static bool isInvincible = false;        // Status of invincibility to enemy attacks

    // ISAAC BURNS UPDATE 2: Death animation implemented
    // ISAAC BURNS UPDATE 3: Coin transfer implemented
    public Animator animator;

    // ISAAC BURNS UPDATE 4: Interact tooltip
    public GameObject Interact_E;
    private bool interactContact = false;

    // ISAAC BURNS UPDATE 5: Healing Mechanics added
    public static bool healingStationUsed = false;

    // ISAAC BURNS UPDATE 6: Save Station Mechanics added
    public static bool saveStationUsed = false;
    public static bool load = false;
    public static PlayerData data = null;
    // and Ability Unlock Variables
    public static bool transform_power = false;
    public static bool charge_attack_power = false;
    public static bool dash_power = false;
    public bool hasTransformPower = false;
    public bool hasChargeAttackPower = false;
    public bool hasDashPower = false;

    // ISAAC BURNS UPDATE 7: Minor updates to prevent game freezing when demo start is disabled
    public static bool scene_select_mode = false;

    void Awake()
    {
        if (demo == true && DemoStart == true)
        {
            firstOpen = true;
        }
    }
    // Re-added Start function to ensure that the player would get the right data for spawning
    public override void Start()
    {
        base.Start();
        // Find the player
        if (playerObj == null)
            playerObj = GameObject.FindGameObjectWithTag("Player");

        // DemoStart gets set to true, they will start at the position the user left them in the scene
        if (demo == true && DemoStart == true)
        {
            StartPos = playerObj.transform.position;
            RespawnPosition = StartPos;
            demo = false;
        }

        // Code used when the game is first run from wherever you left off
        if (firstOpen == true && load == false && scene_select_mode == false)
        {
            print("Hit Points are full");
            HP_transfer = MaxHitPoints;
            Coin_transfer = 0;
            transform_power = hasTransformPower;
            charge_attack_power = hasChargeAttackPower;
            dash_power = hasDashPower;
            StartPos = playerObj.transform.position;
            RespawnPosition = StartPos;
            firstOpen = false;
        }
        if (load == true)
        {
            LoadPlayer(data);
            load = false;
            StartPos = playerObj.transform.position;
        }
        if (scene_select_mode == true)
        {
            print("Hit Points are full");
            HP_transfer = MaxHitPoints;
            scene_select_mode = false;
            firstOpen = false;
        }

        transform.position = RespawnPosition;
        Hitpoints = HP_transfer;
        Player.Currency = Coin_transfer;
        load = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Hitpoints > MaxHitPoints)
        {
            Hitpoints = MaxHitPoints;
        }

        if (Hitpoints % 2 == 1)
        {
            hearts[Hitpoints / 2].sprite = halfHeart;
            hearts[Hitpoints / 2].enabled = true;
            for (int i = 0; i < hearts.Length; i++)
            {
                if (i < Hitpoints / 2)
                {
                    hearts[i].sprite = fullHeart;
                    hearts[i].enabled = true;
                }
                else if (i > Hitpoints / 2)
                {
                    hearts[i].sprite = emptyHeart;
                    hearts[i].enabled = false;
                }
            }
        }
        else
        {
            for (int i = 0; i < hearts.Length; i++)
            {
                if (i < Hitpoints / 2)
                {
                    hearts[i].sprite = fullHeart;
                    hearts[i].enabled = true;
                }
                else
                {
                    hearts[i].sprite = emptyHeart;
                    hearts[i].enabled = false;
                }
            }
        }

        // Interact tooltip
        if (DialogueManager.DialogueActive == false && interactContact == true)
        {
            Interact_E.SetActive(true);
            // print("Interact_E present");
        }
        else
        {
            Interact_E.SetActive(false);
            //print("No interaction");
        }

        if (healingStationUsed == true)
        {
            if (Hitpoints < MaxHitPoints && Player.Currency >= 10)
            {
                Hitpoints = MaxHitPoints;
                Player.Currency -= 10;
            }
            healingStationUsed = false;
        }
        if (saveStationUsed == true)
        {
            Player.save = true;
            saveStationUsed = false;
        }
    }

    public override void TakeHit(int damage)
    {
        if (!isInvincible)
        {
            Hitpoints -= damage;

            if (Hitpoints <= 0)
            {
                RespawnPosition = DefaultSpawn;     // Added to set respawn back to the default spawn, so that the player cannot respawn at a checkpoint when they die
                HP_transfer = MaxHitPoints;         // When the player respawns, they should return to full HP
                Coin_transfer = Player.Currency / 2;
                StartCoroutine(PlayerDies());       // Play death animation and reload scene
            }
            else
            {
                isInvincible = true;
                StartCoroutine(FlashAlpha());
            }
        }
    }

    public override void TakeHit(int damage, Vector2 knockBack)
    {
        if (!isInvincible)
        {
            base.TakeHit(damage, knockBack);
        }
    }

    public new void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Enemy" && (other.gameObject.GetComponent<Enemy>().enemyType != Player.PlayerForm.Feral_Shadow) && isInvincible == false)
        {
            Enemy enemy = other.gameObject.GetComponent<Enemy>();
            TakeHit(1);
            MoveSet moveset = playerObj.gameObject.GetComponent<MoveSet>();
            if (takesKB == true)
            {
                base.controller.Move(new Vector2(0, .5f));
                if (enemy.facingRight == true)
                {
                    //base.controller.Move(enemy.attackKnockBack);
                    moveset.TakeKB(enemy.attackKnockBack);
                }
                else
                {
                    //base.controller.Move(new Vector2 (-enemy.attackKnockBack.x, enemy.attackKnockBack.y));
                    moveset.TakeKB(new Vector2(-enemy.attackKnockBack.x, enemy.attackKnockBack.y));
                }
            }


            // ALEX A UPDATE: Moved invincibility trigger to the TakeHit function
        }

        // Set spike tag to Respawn for the spikes to teleport the player back to the checkpoint they were last at
        if (other.gameObject.tag == "Respawn")
        {
            StartCoroutine(FlashAlpha());
            //TakeHit(1);
            // Teleports player to their checkpoint only if they have HP remaining (does not currently work until )
            if (Hitpoints - 1 > 0)
            {
                transform.position = RespawnPosition;
            }
        }

        // Data transfer block of code for stats like HP and stuff between scenes
        if (other.gameObject.tag == "Transition")
        {
            HP_transfer = Hitpoints;
            Coin_transfer = Player.Currency;
        }

        // Data transfer block of code for stats like HP and stuff between scenes
        if (other.gameObject.tag == "HealthPowerup" && Hitpoints < MaxHitPoints)
        {
            Hitpoints += 2;
            Destroy(other.gameObject);
        }

        // Get the Interact tooltip to appear
        if (other.CompareTag("SplashScreen") || other.CompareTag("HealStation"))
        {
            interactContact = true;
            // print("I am here");
        }

        // Item Pickup
        if (other.CompareTag("Powerup"))
        {
            Powerup powerup = other.GetComponent<Powerup>();
            powerup.GetPickupPower();
            Animator anim = playerObj.GetComponent<Animator>();
            anim.SetTrigger("Revert");
            SpriteRenderer orb = other.GetComponent<SpriteRenderer>();
            Destroy(orb);
            Destroy(other);
        }
    }

    public new void OnTriggerExit2D(Collider2D collision)
    {
        // Get the Interact tooltip to disappear
        if (collision.CompareTag("SplashScreen") || collision.CompareTag("HealStation"))
        {
            interactContact = false;
        }

        if (collision.CompareTag("Enemy"))
        {

        }
    }

    public IEnumerator FlashAlpha()
    {
        for (float ft = 1f; ft >= 0; ft -= 0.1f)
        {
            Color c = GetComponent<Renderer>().material.color;
            c.a = ft;
            GetComponent<Renderer>().material.color = c;
            yield return new WaitForSeconds(flashOnHitDuration / 20);
        }

        for (float ft = 0f; ft <= 1.2; ft += 0.1f)
        {
            Color c = GetComponent<Renderer>().material.color;
            c.a = ft;
            GetComponent<Renderer>().material.color = c;
            yield return new WaitForSeconds(flashOnHitDuration / 20);
        }

        isInvincible = false;
    }
    public IEnumerator PlayerDies()
    {
        animator.SetTrigger("death");
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadPlayer(PlayerData data)
    {
        //PlayerData data = SaveSystem.LoadPlayer();
        HP_transfer = data.health;
        print(HP_transfer);
        MaxHitPoints = data.max_health;
        Coin_transfer = data.coins;
        transform_power = data.transform_power;
        charge_attack_power = data.charge_attack_power;
        dash_power = data.dash_power;

        Vector3 position;
        position.x = data.position[0];
        position.y = data.position[1];
        position.z = data.position[2];

        transform.position = position;
    }
}
