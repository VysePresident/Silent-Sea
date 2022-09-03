using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadbonesArea : MonoBehaviour
{
    // Start is called before the first frame update
    public bool playerInArea;
    void Start()
    {
        playerInArea = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInArea = true;
            Debug.Log("Player is in db area");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player is out of db area");
            playerInArea = false;
        }
    }
}
