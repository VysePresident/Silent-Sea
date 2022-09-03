using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    public bool transform_power;
    public bool dash_power;

    // If we want stuff to spawn on pickup
    public bool spawn_objects;
    public GameObject[] spawnObjects;

    public void GetPickupPower()
    {
        if (transform_power == true)
        {
            Player_Health_Controller.transform_power = true;
        }
        if (dash_power == true)
        {
            Player_Health_Controller.dash_power = true;
        }
        if (spawn_objects == true)
        {
            foreach (GameObject obj in spawnObjects)
            {
                obj.SetActive(true);
            }
        }
    }
}
