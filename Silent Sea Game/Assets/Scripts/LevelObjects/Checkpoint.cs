using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public Vector3 SpawnPosition;
    public new void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            // Indicate on console that the checkpoint has been interacted with
            print("Checkpoint Reached!");
            // Set player's respawn position to the checkpoint's specified position
            Player_Health_Controller.RespawnPosition = SpawnPosition;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
