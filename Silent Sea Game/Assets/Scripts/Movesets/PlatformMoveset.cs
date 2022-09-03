using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Platform)), RequireComponent(typeof(PlatformController))]
public class PlatformMoveset : NodeMoveSet
{
    protected PlatformController controller;

    protected override void Start()
    {
        base.Start();

        controller = (PlatformController)raycastController;
    }

    void FixedUpdate()
    {
        controller.MovePlatform(CalculatePlatformMovement());
    }

    public Vector2 CalculatePlatformMovement()
    {
        if (Time.time < nextMoveTime)
        {
            return Vector2.zero;
        }

        fromNodeIndex %= globalNodes.Length;
        int toNodeIndex = (fromNodeIndex + 1) % globalNodes.Length;
        float distanceBetweenNodes = Vector2.Distance(globalNodes[fromNodeIndex], globalNodes[toNodeIndex]);
        percentBetweenNodes += Time.deltaTime * settings.horizontalMoveSpeed / distanceBetweenNodes;
        percentBetweenNodes = Mathf.Clamp01(percentBetweenNodes);
        float easedPercentBetweenNodes = Ease(percentBetweenNodes);

        Vector2 newPos = Vector2.Lerp(globalNodes[fromNodeIndex], globalNodes[toNodeIndex], easedPercentBetweenNodes);

        if ( percentBetweenNodes >= 1)
        {
            NextNode();
        }

        return newPos - (Vector2)transform.position;
    }
}