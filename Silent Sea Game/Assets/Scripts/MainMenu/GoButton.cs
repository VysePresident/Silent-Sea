using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GoButton : MonoBehaviour
{
    public string sceneName;
    public Vector3 RespawnPosition;
    public bool transformPower;
    public bool dashPower;
    public void EnterScene()
    {
        Player_Health_Controller.firstOpen = true;

        // End menu music
        GameObject[] musics = GameObject.FindGameObjectsWithTag("Music");
        foreach (GameObject music in musics)
        {
            GameObject.Destroy(music);
        }

        // Give the player the necessary powers
        Player_Health_Controller.transform_power = transformPower;
        Player_Health_Controller.dash_power = dashPower;
        Player_Health_Controller.scene_select_mode = true;

        // Spawn the player in the appropriate place
        Player_Health_Controller.DefaultSpawn = RespawnPosition;
        Player_Health_Controller.RespawnPosition = RespawnPosition;
        SceneManager.LoadScene(sceneName);
    }
}
