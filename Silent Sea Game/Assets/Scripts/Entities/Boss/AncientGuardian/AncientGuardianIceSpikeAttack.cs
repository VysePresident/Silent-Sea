using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AncientGuardianIceSpikeAttack : MonoBehaviour
{
    public AncientGuardian boss;

    protected List<IceSpikeLauncher> iceSpikeLaunchers;

    public IceSpikeSettings phase1Settings;
    public IceSpikeSettings phase2Settings;

    public float delay = 0.3f;
    protected float timeLeft = 0;

    public bool finished = false;

    public int current = 0;
    public int max;

    // Start is called before the first frame update
    void Start()
    {
        iceSpikeLaunchers = new List<IceSpikeLauncher>(GetComponentsInChildren<IceSpikeLauncher>());

        foreach (IceSpikeLauncher launcher in iceSpikeLaunchers)
        {
            launcher.settings = (boss.currentPhase == 1) ? phase1Settings : phase2Settings;
            if (boss.facingRight)
            {
                launcher.launchDir = IceSpikeLauncher.Direction.right;
                launcher.spawnPositionOffset.x = 1;
            }
        }

        max = (boss.currentPhase == 1) ? phase1Pattern.Length : phase2Pattern.Length;
    }

    // Update is called once per frame
    void Update()
    {
        if (current < max)
        {
            if (timeLeft <= 0)
            {
                // Grow the next set of ice spikes
                int spikes = (boss.currentPhase == 1) ? phase1Pattern[current] : phase2Pattern[current];

                switch (spikes)
                {
                    case 1:
                        iceSpikeLaunchers[0].GrowSpike();
                        iceSpikeLaunchers[0].LaunchSpike();
                        break;
                    case 2:
                        iceSpikeLaunchers[1].GrowSpike();
                        iceSpikeLaunchers[1].LaunchSpike();
                        break;
                    case 3:
                        iceSpikeLaunchers[2].GrowSpike();
                        iceSpikeLaunchers[2].LaunchSpike();
                        break;
                    case 12:
                        iceSpikeLaunchers[0].GrowSpike();
                        iceSpikeLaunchers[1].GrowSpike();
                        iceSpikeLaunchers[0].LaunchSpike();
                        iceSpikeLaunchers[1].LaunchSpike();
                        break;
                    case 13:
                        iceSpikeLaunchers[0].GrowSpike();
                        iceSpikeLaunchers[2].GrowSpike();
                        iceSpikeLaunchers[0].LaunchSpike();
                        iceSpikeLaunchers[2].LaunchSpike();
                        break;
                    case 23:
                        iceSpikeLaunchers[1].GrowSpike();
                        iceSpikeLaunchers[2].GrowSpike();
                        iceSpikeLaunchers[1].LaunchSpike();
                        iceSpikeLaunchers[2].LaunchSpike();
                        break;
                    case 123:
                        iceSpikeLaunchers[0].GrowSpike();
                        iceSpikeLaunchers[1].GrowSpike();
                        iceSpikeLaunchers[2].GrowSpike();
                        iceSpikeLaunchers[0].LaunchSpike();
                        iceSpikeLaunchers[1].LaunchSpike();
                        iceSpikeLaunchers[2].LaunchSpike();
                        break;
                    case 0:
                    default:
                        // do nothing!
                        break;
                }

                current++;
                timeLeft = delay;
            }
            else
            {
                timeLeft -= Time.deltaTime;
            }
        }
        else
        {
            if (iceSpikeLaunchers[0].currentSpike == null &&
                iceSpikeLaunchers[1].currentSpike == null &&
                iceSpikeLaunchers[2].currentSpike == null)
            {
                finished = true;
            }
        }
    }

    public int[] phase1Pattern =
    {
        1, 0, 1, 0, 1, 0, 2, 0, 1, 0, 0, 0, 12, 0, 13, 0, 23, 0, 0, 0, 123
    };

    public int[] phase2Pattern =
    {
        3, 0, 3, 0, 3, 0, 2, 0, 1, 0, 0, 0, 12, 0, 13, 0, 23, 0, 0, 0, 123
    };
}
