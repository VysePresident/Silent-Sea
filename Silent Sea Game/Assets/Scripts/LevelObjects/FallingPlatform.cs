/**
 * 
 * Scripted by Isaac Burns using code from this tutorial:
 * https://www.youtube.com/watch?v=ovsCoNLP05w
 * 
 **/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    Rigidbody2D rb;

    public float fallTimer;
    public float refreshTimer;

    private Vector3 origin;

    private bool falling;
    // Start is called before the first frame update
    void Start()
    {
        origin = transform.position;
        rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
    }

    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && falling == false)
        {
            falling = true;
            StartCoroutine(DropPlatform());
        }
    }

    public IEnumerator DropPlatform()
    {
        yield return new WaitForSeconds(fallTimer);
        rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        yield return new WaitForSeconds(refreshTimer);
        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        transform.position = origin;
        falling = false;
    }
}
