using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SecretWall : MonoBehaviour
{
    private float length, prescale_x, prescale_y;
    public GameObject secret;

    void Start()
    {
        float prescale_x = transform.localScale.x;
        float prescale_y = transform.localScale.y;
    }

    public new void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            transform.localScale = new Vector3(0, 0, transform.localScale.z);
        }
        else
        {
            transform.localScale = new Vector3(prescale_x, prescale_y, transform.localScale.z);
        }
    }

    void Update()
    {
        
    }
}
