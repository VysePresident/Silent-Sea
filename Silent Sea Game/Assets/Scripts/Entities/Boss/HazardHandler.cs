using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HazardHandler : MonoBehaviour
{
    public abstract void TriggerHazard();

    public abstract void TriggerHazard(string code);
}
