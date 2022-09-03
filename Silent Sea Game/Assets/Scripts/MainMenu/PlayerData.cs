using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class PlayerData
{
    public int health, max_health;
    public int coins;
    public float[] position;
    public bool transform_power, charge_attack_power, dash_power;
    public string scene;

    public PlayerData(Player player)
    {
        health = player.healthController.Hitpoints;
        max_health = player.healthController.MaxHitPoints;
        coins = player.coins;
        transform_power = Player_Health_Controller.transform_power;
        charge_attack_power = Player_Health_Controller.charge_attack_power;
        dash_power = Player_Health_Controller.dash_power;

        Scene currentScene = SceneManager.GetActiveScene();
        string currentSceneName = currentScene.name;
        Debug.Log("The scene name is " + currentScene.name);

        position = new float[3];
        position[0] = player.transform.position.x;
        position[1] = player.transform.position.y;
        position[2] = player.transform.position.z;

        scene = currentSceneName;
    }
}
