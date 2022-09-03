using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : RaycastController
{
    public LayerMask passengerMask;
    public Vector2 move;

    List<PassengerMovement> passengerMovements;
    Dictionary<Transform, MovementController> passengerDictionary = new Dictionary<Transform, MovementController>();

    protected override void Start()
    {
        base.Start();
        verticalRayCount = 8;
        CalculateRaySpacing();
    }

    public void MovePlatform(Vector2 velocity)
    {
        UpdateRaycastOrigins();

        CalculatePassengerMovement(velocity);

        MovePassengers(true);
        transform.Translate(velocity);
        Physics2D.SyncTransforms(); // Updates Collider2D's bounds to new correct position
        MovePassengers(false);
    }

    void CalculatePassengerMovement(Vector2 velocity)
    {
        HashSet<Transform> movedPassengers = new HashSet<Transform>();
        passengerMovements = new List<PassengerMovement>();

        float dirX = Mathf.Sign(velocity.x);
        float dirY = Mathf.Sign(velocity.y);

        // Vertical motion
        if (velocity.y != 0)
        {
            float rayLength = Mathf.Abs(velocity.y) + skinWidth;
            for (int i = 0; i < verticalRayCount; i++)
            {
                Vector2 rayOrigin = (dirY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
                rayOrigin += Vector2.right * (verticalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * dirY, rayLength, passengerMask);

                Debug.DrawRay(rayOrigin, Vector2.up * dirY * rayLength, Color.red);

                if (hit)
                {
                    if (!movedPassengers.Contains(hit.transform))
                    {
                        BasicMoveSet hitMoveSet = null;
                        // If the hit was with an entity with a BasicMoveSet, set hitMoveSet to its BasicMoveSet
                        if (hit.collider.CompareTag("Player") || hit.collider.CompareTag("Enemy"))
                        {
                            hitMoveSet = hit.collider.GetComponentInParent<BasicMoveSet>();
                        }

                        // ALEX A: Lets Entities actually jump off of moving platforms
                        // If the hit was with an Entity without a BasicMoveSet, add it to the passenger list as normal
                        // Otherwise, only add it to the passenger list if jumpAlreadyPressed is false
                        if (hitMoveSet == null || !hitMoveSet.jumpAlreadyPressed)
                        {
                            movedPassengers.Add(hit.transform);

                            float pushX = (dirY == 1) ? velocity.x : 0;
                            float pushY = velocity.y - (hit.distance - skinWidth) * dirY;

                            passengerMovements.Add(new PassengerMovement(hit.transform, new Vector2(pushX, pushY), dirY == 1, true));
                        }
                    }
                }
            }
        }

        // Horizontal motion
        if (velocity.x != 0)
        {
            float rayLength = Mathf.Abs(velocity.y) + skinWidth;
            for (int i = 0; i < verticalRayCount; i++)
            {
                Vector2 rayOrigin = (dirY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
                rayOrigin += Vector2.right * (verticalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * dirY, rayLength, passengerMask);

                Debug.DrawRay(rayOrigin, Vector2.up * dirY * rayLength, Color.red);

                if (hit)
                {
                    if (!movedPassengers.Contains(hit.transform))
                    {
                        movedPassengers.Add(hit.transform);

                        float pushX = velocity.x - (hit.distance - skinWidth) * dirX;
                        float pushY = 0;

                        passengerMovements.Add(new PassengerMovement(hit.transform, new Vector2(pushX, pushY), false, true));
                    }
                }
            }
        }

        // Passenger on top of platform not moving up
        if (dirY == -1 || (velocity.y == 0 && velocity.x != 0))
        {
            float rayLength = skinWidth * 2;
            for (int i = 0; i < verticalRayCount; i++)
            {
                Vector2 rayOrigin = raycastOrigins.topLeft + Vector2.right * (verticalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, rayLength, passengerMask);

                Debug.DrawRay(rayOrigin, Vector2.up * dirY * rayLength, Color.red);

                if (hit)
                {
                    if (!movedPassengers.Contains(hit.transform))
                    {
                        movedPassengers.Add(hit.transform);

                        float pushX = velocity.x;
                        float pushY = velocity.y;

                        passengerMovements.Add(new PassengerMovement(hit.transform, new Vector2(pushX, pushY), true, false));
                    }
                }
            }
        }
    }

    void MovePassengers(bool beforeMovePlatform)
    {
        foreach (PassengerMovement passenger in passengerMovements)
        {
            if (!passengerDictionary.ContainsKey(passenger.transform))
            {
                passengerDictionary.Add(passenger.transform, passenger.transform.GetComponent<MovementController>());
            }
            if (passenger.moveBeforePlatform == beforeMovePlatform)
            {
                passengerDictionary[passenger.transform].Move(passenger.velocity, passenger.onPlatform);
            }
        }
    }

    struct PassengerMovement
    {
        public Transform transform;
        public Vector2 velocity;
        public bool onPlatform;
        public bool moveBeforePlatform;

        public PassengerMovement(Transform _transform, Vector2 _velocity, bool _onPlatform, bool _moveBeforePlatform)
        {
            transform = _transform;
            velocity = _velocity;
            onPlatform = _onPlatform;
            moveBeforePlatform = _moveBeforePlatform;
        }
    }
}
