using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mist : MonoBehaviour
{
    public float X_posChange;
    public float right_boundary;
    public float left_boundary;

    /*void Start()
    {
        
    }*/

    void Update()
    {
        /* The mist will move a certain speed in a certain direction depending on the value of X_posChange.
         * A negative value goes left, and a positive value goes right.
         * A value of 1 is REALLY FAST... like watching a train speed by.
         * A value of 0.0001 is REALLY SLOW... kinda like watching clouds go by on a day that's not particularly windy.*/
        transform.position = new Vector3(transform.position.x + X_posChange, transform.position.y, transform.position.z);

        /* The mist works off a boundary system. For the texture being used, I recommend using multiples of (24*x_scale).
         * If the mist moves right and approaches the right boundary, the mist will reappear at the left boundary.
         * If the mist moves left and approaches the left boundary, the mist will reappear at the right boundary.
         * Recommended to avoid setting boundary locations within the player's sight.*/
        if (transform.position.x > right_boundary)
        {
            transform.position = new Vector3(left_boundary, transform.position.y, transform.position.z);
        }
        else if (transform.position.x < left_boundary)
        {
            transform.position = new Vector3(right_boundary, transform.position.y, transform.position.z);
        }
    }
}
