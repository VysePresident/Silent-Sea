using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : Enemy
{
    private Obstacle_Health_Controller hp;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        hp = GetComponent<Obstacle_Health_Controller>();
    }
    private void Update()
    {
        print(hp.Hitpoints);
        if (hp.Hitpoints <= 0)
        {
            Destroy(this);
        }
    }
}
