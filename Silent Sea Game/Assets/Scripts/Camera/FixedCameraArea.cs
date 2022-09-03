using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedCameraArea : MonoBehaviour
{
    public float size;
    public CameraController controller;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            controller.ChangeToFixedCamera(new Vector2(transform.position.x, transform.position.y), size);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            controller.GetComponent<CameraController>().ChangeToFollowCamera();
        }
    }
}
