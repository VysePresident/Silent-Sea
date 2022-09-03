using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraController : MonoBehaviour
{
    public bool follow = true;

    public Entity target;
    Collider2D targetCollider;
    Camera cam;

    public float verticalOffset;
    [SerializeField] private float lookOffset;
    [SerializeField] private float lookTimer;       //Delay before looking
    private int timerCount = 0;                     //Track time before looking
    private float smoothFactor;                     //Velocity of Looking function
    private MovementController movementPlayer;  //To detect collisions - currently for Look

    public float lookAheadDistX;
    public float smoothTimeX;
    public float smoothTimeY;
    public float smoothResizeTime;
    public Vector2 focusAreaSize;

    FocusArea focusArea;

    float currentX;
    float currentY;
    float targetX;
    float targetY;
    float lookAheadDirX;
    bool lookAheadStopped;

    float smoothVelocityX;  // For smoothdamp
    float smoothVelocityY;  // For smoothdamp

    float targetSize;
    float defaultSize = 2;
    float smoothResize;

    private

    void Start()
    {
        targetCollider = target.gameObject.GetComponent<MovementController>().main_collider;
        focusArea = new FocusArea(targetCollider.bounds, focusAreaSize);
        cam = GetComponent<Camera>();
        defaultSize = cam.orthographicSize;
        lookOffset = 7.0f; //Distance to look
        smoothFactor = 0.4f; //Rate at which we look.  Currently unused.
        movementPlayer = GetComponent<MovementController>();
    }

    void LateUpdate()
    {
        focusArea.Update(targetCollider.bounds);


        //Look up. Replace with function eventually.
        /*if (Input.GetKey(KeyCode.W))
        {
            lookTimer += Time.deltaTime;

            if (lookTimer >= 2.0f && timerCount == 0)
            {
                //verticalOffset += lookOffset;
                verticalOffset += lookOffset;
                timerCount++;
            }
        }
        //Undo Up
        if (Input.GetKeyUp(KeyCode.W))
        {
            if (timerCount > 0)
            {
                verticalOffset -= lookOffset;
            }
            lookTimer = 0;
            timerCount = 0;
        }
        //Look Down
        if (Input.GetKey(KeyCode.S))
        {
            lookTimer += Time.deltaTime;

            if (lookTimer >= 2.0f && timerCount == 0)
            {
                verticalOffset -= lookOffset;
                timerCount++;
            }
        }
        //Undo Down
        if (Input.GetKeyUp(KeyCode.S))
        {
            if (timerCount > 0)
            {
                verticalOffset += lookOffset;
            }
            lookTimer = 0;
            timerCount = 0;
        }*/
        //End Look here.

        if (follow)
        {
            // Debug.Log("This is LateUpdate() when follow is true");

            Vector2 focusPosition = focusArea.center + Vector2.up * verticalOffset;

            if (focusArea.velocity.x != 0)
            {
                lookAheadDirX = Mathf.Sign(focusArea.velocity.x);
                if (Mathf.Sign(target.move.x) == Mathf.Sign(focusArea.velocity.x) && target.move.x != 0)
                {
                    lookAheadStopped = false;
                    targetX = lookAheadDirX * lookAheadDistX;
                }
                else
                {
                    if (!lookAheadStopped)
                    {
                        lookAheadStopped = true;
                        targetX = currentX + (lookAheadDirX * lookAheadDistX - currentX) / 4f;
                    }
                }
            }

            currentX = Mathf.SmoothDamp(currentX, targetX, ref smoothVelocityX, smoothTimeX);

            focusPosition.y = Mathf.SmoothDamp(transform.position.y, focusPosition.y, ref smoothVelocityY, smoothTimeY);
            focusPosition += Vector2.right * currentX;
            transform.position = (Vector3)focusPosition + Vector3.forward * -10;

            if (cam.orthographicSize != defaultSize)
            {
                cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, targetSize, ref smoothResize, smoothResizeTime);
            }
        }
        else
        {
            currentX = Mathf.SmoothDamp(transform.position.x, targetX, ref smoothVelocityX, smoothTimeX);
            currentY = Mathf.SmoothDamp(transform.position.y, targetY, ref smoothVelocityY, (smoothTimeY != 0) ? smoothTimeY : 0.3f);

            transform.position = new Vector3(currentX, currentY, 0) + Vector3.forward * -10;
            cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, targetSize, ref smoothResize, smoothResizeTime);
        }
    }

    public void ChangeToFixedCamera(Vector2 targetPosition, float size)
    {
        follow = false;

        targetX = targetPosition.x;
        targetY = targetPosition.y;

        targetSize = size;
    }

    public void ChangeToFollowCamera()
    {
        follow = true;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 1, 0.5f);
        Gizmos.DrawCube(focusArea.center, focusAreaSize);
    }

    struct FocusArea
    {
        public Vector2 center;
        public Vector2 velocity;
        float left, right;
        float top, bottom;

        public FocusArea(Bounds targetBounds, Vector2 size)
        {
            left = targetBounds.center.x - size.x / 2;
            right = targetBounds.center.x + size.x / 2;
            bottom = targetBounds.min.y;
            top = targetBounds.min.y + size.y;

            center = new Vector2((left + right) / 2, (top + bottom) / 2);
            velocity = Vector2.zero;
        }

        public void Update(Bounds targetBounds)
        {
            float shiftX = 0;
            if (targetBounds.min.x < left)
            {
                shiftX = targetBounds.min.x - left;
            }
            else if (targetBounds.max.x > right)
            {
                shiftX = targetBounds.max.x - right;
            }
            left += shiftX;
            right += shiftX;

            float shiftY = 0;
            if (targetBounds.min.y < bottom)
            {
                shiftY = targetBounds.min.y - bottom;
            }
            else if (targetBounds.max.y > top)
            {
                shiftY = targetBounds.max.y - top;
            }
            top += shiftY;
            bottom += shiftY;

            center = new Vector2((left + right) / 2, (top + bottom) / 2);
            velocity = new Vector2(shiftX, shiftY);
        }
    }
}
