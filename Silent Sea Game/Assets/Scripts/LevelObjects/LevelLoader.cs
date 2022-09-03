/*
 * Set up using the help of Brackeys's tutorial on scene transitions:
 * https://www.youtube.com/watch?v=CE9VOZivb3I
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LevelLoader : MonoBehaviour
{
    // Scene Name (name of the scene the player is going to)
    public string Scene;

    // Get Animator Object
    public Animator transition;
    // Set a transition time (default is 1 second)
    public float transitionTime = 1f;
    // Set the position in the next scene that the player will start in
    public Vector3 RespawnPosition;  // When the player falls on spikes or transitions from one scene to another, this will determine where the player will go
    public bool EndMusic = false;


    // If a player's collision enters the transition zone, the scene will transition
    public new void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (EndMusic == true)
            {
                GameObject[] musics = GameObject.FindGameObjectsWithTag("Music");
                foreach (GameObject music in musics)
                {
                    AudioSource audio = music.GetComponent<AudioSource>();
                    StartCoroutine(MusicManager.FadeOut(audio, 2f));
                    //GameObject.Destroy(music);
                }
            }
            LoadNextLevel();
        }
    }

    // Test Case: Click the left mouse button and the scene will transition
    /*
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            LoadNextLevel();
        }
    }
    */

    // Start the coroutine for loading the next scene
    public void LoadNextLevel()
    {
        Player_Health_Controller.DefaultSpawn = RespawnPosition;
        Player_Health_Controller.RespawnPosition = RespawnPosition;
        print("Target Respawn Position: " + RespawnPosition + "\nActual Position: " + Player_Health_Controller.DefaultSpawn);
        Player.transition = true;
        StartCoroutine(LoadLevel(Scene));
    }

    // Function used for loading the level
    IEnumerator LoadLevel(string scene)
    {
        // Play Animation
        transition.SetTrigger("Start");
        // Wait
        yield return new WaitForSeconds(transitionTime);
        // Load Scene
        SceneManager.LoadScene(scene);
    }
}
