using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTriggers : MonoBehaviour
{
    public Enemy_Health_Controller Boss;
    public GameObject[] ExitObjects;
    public AudioSource BossMusic;
    public AudioSource VictoryMusic;
    public int EscapeMethod = 0;    // Leave at 0 for Feral Shadow Boss escape method

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
            if (EscapeMethod == 0 && BossDefeated == true)
            {
                for (int i=0;i<ExitObjects.Length;i++)
                {
                    ExitObjects[i].SetActive(true);
                }
            }
        }
    }

    public new void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player" && BossFightStarted == false && BossDefeated == false)
        {
            print("Cue the boss music");
            BossMusic.Play();
            BossFightStarted = true;
        }
    }
}
