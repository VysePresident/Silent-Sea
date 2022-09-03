using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeBox : MonoBehaviour
{
    DeadbonesDamageController damageController;
    Deadbones db;
    // Start is called before the first frame update
    void Start()
    {
        damageController = GetComponent<DeadbonesDamageController>();
    }

    public bool setEnabled(bool enabled)
    {
        return damageController.enabled = enabled;
    }

    public void dealDamage()
    {
        damageController.doMeleeDamage();
    }

    // Update is called once per frame
    void Update()
    {
    }
}
