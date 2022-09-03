using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    private float length, startpos_x, startpos_y;
    public GameObject cam;
    public float parallaxEffect;


    void Start()
    {
        startpos_x = transform.position.x;
        startpos_y = transform.position.y;
    }

    void Update()
    {
        float distance_x = (cam.transform.position.x * parallaxEffect);
        float distance_y = (cam.transform.position.y * parallaxEffect);

        transform.position = new Vector3(startpos_x + distance_x, startpos_y + distance_y, transform.position.z);
    }
}
