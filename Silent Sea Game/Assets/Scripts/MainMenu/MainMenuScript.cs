/*
    https://www.youtube.com/watch?v=-GWjA6dixV4&ab_channel=BMo
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    public void PlayGame()
    {
        GameObject[] musics = GameObject.FindGameObjectsWithTag("Music");
        foreach (GameObject music in musics)
        {
            GameObject.Destroy(music);
        }
        Player_Health_Controller.firstOpen = true;
        Vector3 spawnPos = new Vector3(0.1f, 17.7f, 0f);
        Player_Health_Controller.DefaultSpawn = spawnPos;
        Player_Health_Controller.RespawnPosition = spawnPos;
        SceneManager.LoadScene("Section A1");
        //print("Respawn Point: " + Player_Health_Controller.RespawnPosition);
        //print("Default Spawn Point: " + Player_Health_Controller.DefaultSpawn);
    }
    public void LoadGame()
    {
        GameObject[] musics = GameObject.FindGameObjectsWithTag("Music");
        foreach (GameObject music in musics)
        {
            GameObject.Destroy(music);
        }
        PlayerData data = SaveSystem.LoadPlayer();
        if (data != null)
        {
            Player_Health_Controller.demo = false;
            Player_Health_Controller.load = true;
            Player_Health_Controller.data = data;
            Vector3 spawnPos = new Vector3(data.position[0], data.position[1], data.position[2]);
            Player_Health_Controller.DefaultSpawn = spawnPos;
            Player_Health_Controller.RespawnPosition = spawnPos;
            //print("Respawn Point: " + Player_Health_Controller.RespawnPosition);
            //print("Default Spawn Point: " + Player_Health_Controller.DefaultSpawn);
            SceneManager.LoadScene(data.scene);
        }
        else
        {
            print("Data could not be loaded.");
        }
    }
    public void GoTo()
    {
        SceneManager.LoadScene("SceneSelect");
    }
    public void ShowControls()
    {
        SceneManager.LoadScene("ControlsScreen");
    }
    public void Credits()
    {
        SceneManager.LoadScene("CreditsScreen");
    }
    public void QuitGame()
    {
        Application.Quit();
    }

    public void BackButton()
    {
        SceneManager.LoadScene("MainMenu");
    }

}
