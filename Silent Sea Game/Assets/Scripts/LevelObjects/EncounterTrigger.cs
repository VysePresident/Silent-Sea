using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterTrigger : MonoBehaviour
{
    public GameObject[] enemies;
    public GameObject[] exitObjects;
    public GameObject[] gates;
    private bool fightStarted = false;
    private bool victory = false;
    public AudioSource EncounterMusic;
    public AudioSource VictoryMusic;
    public bool isBoss = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (fightStarted == true && victory == false)
        {
            for (int i = 0; i < enemies.Length; i++)
            {
                if ((enemies[i] && isBoss == false) || (isBoss == true && enemies[i].GetComponent<Health_Controller>().Hitpoints > 0))
                {
                    break;
                }
                if ((i + 1 == enemies.Length && isBoss == false) || (isBoss == true && enemies[i].GetComponent<Health_Controller>().Hitpoints <= 0))
                {
                    victory = true;
                    print("I think that's it...");
                    EncounterMusic.Stop();
                    VictoryMusic.Play();
                    foreach (GameObject obj in gates)
                    {
                        obj.SetActive(false);
                    }
                    foreach (GameObject obj in exitObjects)
                    {
                        obj.SetActive(true);
                    }
                }
            }
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player" && fightStarted == false && victory == false)
        {
            fightStarted = true;
            foreach (GameObject obj in enemies)
            {
                obj.SetActive(true);
            }
            foreach (GameObject obj in gates)
            {
                obj.SetActive(true);
            }
            print("Cue the boss music");
            EncounterMusic.Play();
        }
    }
}
