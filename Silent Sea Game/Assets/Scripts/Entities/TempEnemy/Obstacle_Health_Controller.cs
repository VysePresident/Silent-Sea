using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle_Health_Controller : Enemy_Health_Controller
{
    public bool stay_destroyed;

    public void Update()
    {
        if (Hitpoints <= 0 && stay_destroyed == true)
        {

        }
    }
    
}
