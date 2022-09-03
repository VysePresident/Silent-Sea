using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeralShadow_Health_Controller : Enemy_Health_Controller
{
    public GameObject skullbug;
    void Update()
    {
        if (Hitpoints < 1)
        {
            skullbug.SetActive(true);
        }
    }
}
