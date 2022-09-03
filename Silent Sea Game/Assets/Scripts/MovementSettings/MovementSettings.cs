using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MovementSettings")]
public class MovementSettings : ScriptableObject
{
    // XY Movement
    public float horizontalMoveSpeed;
    public float verticalMoveSpeed;

    // Acceleration
    public float airbornTimeToSpeed;
    public float groundedTimeToSpeed;

    public float maxIncline;
    public float maxDecline;

    // Jumping
    public float maxJumpHeight;
    public float minJumpHeight;
    public float timeToJumpApex;
    public int maxNumJumps;

    // Node-based movement
    public bool direct;
    public bool cyclic;
    public bool useLocalNodes;
}
