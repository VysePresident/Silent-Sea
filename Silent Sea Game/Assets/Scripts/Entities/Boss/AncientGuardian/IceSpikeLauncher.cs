using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceSpikeLauncher : MonoBehaviour
{
    public IceSpike leftSpikePrefab;
    public IceSpike rightSpikePrefab;
    public IceSpike downSpikePrefab;

    public IceSpikeSettings settings;

    public Direction launchDir;
    public Vector2 spawnPositionOffset;

    public bool onCommand;
    public bool launchImmediately;

    protected bool grow = false;
    protected bool launch = false;

    public float regrowDelay; // seconds in-between an ice spike launching and growing a new one when not on-command
    protected float regrowTimer;

    public float launchDelay;   // seconds in-between an ice spike finishing growing and launching when not on-command
    protected float launchTimer;

    public IceSpike currentSpike;
    protected IceSpike oldSpike;

    protected void Update()
    {
        if (!onCommand)
        {
            if (currentSpike == null)
            {
                if (regrowTimer <= 0) // Time to grow a new spike automatically
                {
                    GrowNewSpike();
                }
                else
                {
                    regrowTimer -= Time.deltaTime;
                }
            }
            else
            {
                if ((launchTimer <= 0 || launchImmediately) && currentSpike.status == IceSpike.Status.Ready)
                {
                    LaunchCurrentSpike();
                }
                else
                {
                    launchTimer -= Time.deltaTime;
                }
            }
        }
        else // On Command
        {
            if (currentSpike == null)
            {
                if (grow)
                {
                    GrowNewSpike();
                }
            }
            else // There is a current spike
            {
                if (launch && currentSpike.status == IceSpike.Status.Ready)
                {
                    LaunchCurrentSpike();
                }
            }
        }
    }

    protected void GrowNewSpike()
    {
        if (currentSpike == null)
        {
            Vector3 launcherPos = transform.position + (Vector3)spawnPositionOffset;

            switch(launchDir)
            {
                case Direction.left:
                    currentSpike = Instantiate(leftSpikePrefab, launcherPos, Quaternion.identity);
                    break;
                case Direction.right:
                    currentSpike = Instantiate(rightSpikePrefab, launcherPos, Quaternion.identity);
                    break;
                case Direction.down:
                default:
                    currentSpike = Instantiate(downSpikePrefab, launcherPos, Quaternion.identity);
                    break;
            }

            currentSpike.AttackDamage = settings.damage;
            currentSpike.speed = settings.speed;
            currentSpike.launchVelocity = settings.launchVelocity;
            currentSpike.gravity = settings.gravity;
            currentSpike.affectedByGravity = settings.affectedByGravity;
            currentSpike.launchStart = settings.launchStart;
            currentSpike.timeToReady = settings.timeToReady;
            currentSpike.knockBack = settings.knockBack;

            grow = false;
            launchTimer = launchDelay;
        }
    }

    protected void LaunchCurrentSpike()
    {
        if (currentSpike != null)
        {
            currentSpike.Launch();

            oldSpike = currentSpike;
            currentSpike = null;
            launch = false;
            regrowTimer = regrowDelay;
        }
    }

    public void GrowSpike()
    {
        grow = true;
    }

    public void LaunchSpike()
    {
        launch = true;
    }

    public enum Direction
    {
        left,
        right,
        down
    }
}
