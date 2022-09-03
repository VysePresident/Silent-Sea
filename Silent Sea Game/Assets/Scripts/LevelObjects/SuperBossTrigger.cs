using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperBossTrigger : MonoBehaviour
{
    public BossHealthController Boss;
    public GameObject ExitObject;
    public AudioSource BossMusic;
    public AudioSource VictoryMusic;
    public int EntryMethod = 0;    // Leave at 0 for Ancient Guardian Entry method
    public int EscapeMethod = 0;    // Leave at 0 for Feral Shadow Boss escape method (works for Ancient Guardian)
    public bool EmergencyExit = false; // Set true if the boss is still glitchy while dying

    private bool BossFightStarted = false;
    private bool BossDefeated = false;

    void Update()
    {
        if (Boss.Hitpoints <= 0 && BossFightStarted == true && BossDefeated == false)
        {
            print("The king is dead!  Long live the king!");
            BossMusic.Stop();
            VictoryMusic.Play();
            BossFightStarted = false;
            BossDefeated = true;
            if (EscapeMethod == 0 && BossDefeated == true )
            {
                ExitObject.SetActive(false);
                Boss.GetComponent<BoxCollider2D>().enabled = false;
            }
            /*if (EmergencyExit == true)
            {
                Destroy(Boss);
            }*/
        }
    }

    public new void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player" && BossFightStarted == false && BossDefeated == false)
        {
            if (EntryMethod == 0)
            {
                ExitObject.SetActive(true);
            }
            print("Cue the boss music");
            BossMusic.Play();
            BossFightStarted = true;
        }
    }
}
