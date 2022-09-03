using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AncientGuardianCeilingIceHandler : HazardHandler
{
    public AncientGuardian boss;

    public int currentPattern;

    public List<IceSpikeLauncher> iceSpikeLaunchers;

    void Start()
    {
        iceSpikeLaunchers = new List<IceSpikeLauncher>(GetComponentsInChildren <IceSpikeLauncher>());
        foreach (IceSpikeLauncher launcher in iceSpikeLaunchers)
        {
            launcher.launchImmediately = true;
        }
    }

    protected void Update()
    {

    }

    //bool once = true;
    //float delay = 2.5f;
    //bool twice = true;

    //void Update()
    //{
    //    if (once)
    //    {
    //        foreach (int launcherID in phaseTwoPatterns[0])
    //        {
    //            iceSpikeLaunchers[launcherID].GrowSpike();
    //            iceSpikeLaunchers[launcherID].LaunchSpike();
    //        }

    //        once = false;
    //    }

    //    if (twice)
    //    {
    //        if (delay <= 0)
    //        {
    //            foreach (int launcherID in phaseTwoPatterns[1])
    //            {
    //                iceSpikeLaunchers[launcherID].GrowSpike();
    //                iceSpikeLaunchers[launcherID].LaunchSpike();
    //            }

    //            twice = false;
    //        }
    //        else
    //        {
    //            delay -= Time.deltaTime;
    //        }
    //    }
    //}

    public override void TriggerHazard()
    {
        throw new System.NotImplementedException();
    }

    public override void TriggerHazard(string code)
    {
        switch(code)
        {
            case "landing smash":
                currentPattern = 1;
                break;
            case "fist":
            default:
                currentPattern = 0;
                break;
        }

        if (boss.currentPhase == 1)
        {
            foreach (int launcherID in phaseOnePatterns[currentPattern])
            {
                iceSpikeLaunchers[launcherID].GrowSpike();
                iceSpikeLaunchers[launcherID].LaunchSpike();
            }
        }
        else if (boss.currentPhase == 2)
        {
            foreach (int launcherID in phaseTwoPatterns[currentPattern])
            {
                iceSpikeLaunchers[launcherID].GrowSpike();
                iceSpikeLaunchers[launcherID].LaunchSpike();
            }
        }
    }

    public int[][] phaseOnePatterns = new int[][]
    {
        new int[]
        {
            2, 5, 6, 12, 13, 18, 21
        },
        new int[]
        {
            0, 3, 7, 7, 11, 16, 22
        }
    };

    public int[][] phaseTwoPatterns = new int[][]
    {
        new int[]
        {
            1, 4, 5, 6, 11, 12, 15, 17, 18, 21
        },
        new int[]
        {
            0, 1, 4, 5, 9, 11, 12, 13, 17, 21, 22
        }
    };
}
