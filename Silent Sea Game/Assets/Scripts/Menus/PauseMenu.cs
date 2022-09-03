/**
 * Pause menu screen script created from a Brackeys tutorial:
 * https://www.youtube.com/watch?v=JivuXdrIHK0
 **/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public GameObject PauseMenuUI;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused == true)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (GameIsPaused == true)
            {
                LoadMenu();
            }
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            if (GameIsPaused == true)
            {
                QuitGame();
            }
        }
    }

    public void Resume()
    {
        Debug.Log("Resuming...");
        PauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    void Pause()
    {
        Debug.Log("Pausing the game...");
        PauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void LoadMenu()
    {
        Debug.Log("Returning to menu...");
        Time.timeScale = 1f;
        GameIsPaused = false;
        GameObject[] musics = GameObject.FindGameObjectsWithTag("Music");

        // Deactivate player powers when they return to the main menu
        Player_Health_Controller.transform_power = false;
        Player_Health_Controller.dash_power = false;

        foreach (GameObject music in musics)
        {
            Destroy(music);
        }
        SceneManager.LoadScene("MainMenu");

    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}
