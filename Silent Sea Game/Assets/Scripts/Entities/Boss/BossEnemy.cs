using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BossEnemy : Enemy
{
    public string name;
    public int currentPhase;
    public int numPhases;
    public float phaseChangeDuration;
    protected float phaseChangeTimeLeft;

    protected BossHealthController healthController;

    public abstract void StartNextPhase();
}
