using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : Entity
{
    public PlatformMoveset platformMoveset;

    // Start is called before the first frame update
    void Start()
    {
        moveSet = platformMoveset = GetComponent<PlatformMoveset>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }
}