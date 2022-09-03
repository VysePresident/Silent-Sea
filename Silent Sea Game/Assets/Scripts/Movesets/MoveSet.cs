using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSet : MonoBehaviour
{
    protected Entity entity;
    protected RaycastController raycastController;

    public bool isEnabled = true;

    public Vector2 velocity;

    public MovementSettings settings;
    [HideInInspector] public bool ignoreGravity;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        entity = GetComponent<Entity>();
        raycastController = GetComponent<RaycastController>();
    }

    public Vector2 DisableMoveset()
    {
        isEnabled = false;
        return velocity;
    }

    public virtual void EnableMoveset(Vector2 vel)
    {
        isEnabled = true;
        velocity = vel;
    }

    public virtual void EnableMoveset()
    {
        isEnabled = true;
        velocity = new Vector2(0, 0);
    }

    public void TakeKB(Vector2 kb)
    {
        velocity.x = kb.x;
        velocity.y = kb.y;
    }
}