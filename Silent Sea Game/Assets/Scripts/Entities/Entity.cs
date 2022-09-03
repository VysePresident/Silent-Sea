using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public Vector2 move;
    public bool jump;

    public Vector2 attackKnockBack;

    public MoveSet moveSet;

    //[HideInInspector]
    public bool isStandingOnLedge;
    public float maxDropOffDistance = 1.0f;
    //[HideInInspector]
    public Vector2 ledgeEdge;
    public bool DrawLedgeDetectionDebugs = false;
    [HideInInspector]
    public bool ignoreNewLedgeDetection = false;

    public bool facingRight;
    public bool facingUp;

    public virtual void OnLedgeDetected()
    {
        if (!ignoreNewLedgeDetection)
        {
            isStandingOnLedge = true;
        }
    }

    public virtual void OnLedgeDectected(Vector2 newLedgeEdge)
    {
        if (!ignoreNewLedgeDetection)
        {
            isStandingOnLedge = true;
            ledgeEdge = newLedgeEdge;
        }
    }

    public virtual void OnMoveAwayFromLedge()
    {
        if (!ignoreNewLedgeDetection)
        {
            isStandingOnLedge = false;
            ledgeEdge = new Vector2();
        }
    }

    protected virtual void OnDrawGizmos()
    {
        if (DrawLedgeDetectionDebugs)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(ledgeEdge, 0.1f);
        }
    }
}
