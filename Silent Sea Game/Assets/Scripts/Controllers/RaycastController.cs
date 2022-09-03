using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class RaycastController : MonoBehaviour
{
    protected const float skinWidth = 0.015f;
    [HideInInspector]
    public int horizontalRayCount = 4;
    [HideInInspector]
    public int verticalRayCount = 4;

    protected float horizontalRaySpacing;
    protected float verticalRaySpacing;

    public LayerMask collisionMask;

    public Collider2D main_collider;
    [HideInInspector] public RaycastOrigins raycastOrigins;

    protected virtual void Awake()
    {
        //List<Collider2D> colliders = new List<Collider2D>(GetComponents<Collider2D>());
        //foreach (Collider2D col in colliders)
        //{
        //    if (!col.isTrigger)
        //    {
        //        main_collider = col;
        //        break;
        //    }
        //}
        CalculateRaySpacing();
    }

    protected virtual void Start()
    {

    }

    protected virtual void UpdateRaycastOrigins()
    {
        Bounds bounds = main_collider.bounds;
        bounds.Expand(skinWidth * -2);

        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }

    protected virtual void CalculateRaySpacing()
    {
        Bounds bounds = main_collider.bounds;
        bounds.Expand(skinWidth * -2);

        horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
        verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }

    public RaycastOrigins GetRaycastOrigins()
    {
        return raycastOrigins;
    }

    public struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }
}
