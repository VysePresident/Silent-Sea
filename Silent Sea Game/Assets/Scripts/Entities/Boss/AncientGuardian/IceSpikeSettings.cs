using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "IceSpikeSettings")]
public class IceSpikeSettings : ScriptableObject
{
    public int damage;

    public float speed;
    public float launchVelocity;
    public float gravity;

    public bool affectedByGravity;
    public bool launchStart;

    public float timeToReady;

    public Vector2 knockBack;
}
