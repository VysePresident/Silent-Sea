using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeMoveSet : MoveSet
{
    public bool direct;
    public bool cyclic;
    public bool useLocalNodes;

    [Range(0,2)]
    public float easeAmount;
    public float waitTime;
    protected float nextMoveTime;

    public Vector2[] localNodes;
    public Vector2[] globalNodes;

    protected int fromNodeIndex;
    protected float percentBetweenNodes;

    protected void Awake()
    {
        if (useLocalNodes)
        {
            GenerateGlobalNodes();
        }
    }

    protected override void Start()
    {
        base.Start();
    }

    public Vector2 GetNextNode()
    {
        if (fromNodeIndex < globalNodes.Length - 1) // Moving from before the 2nd to last node
        {
            return globalNodes[fromNodeIndex + 1];
        }
        else if (cyclic) // Moving from the last node. If cyclic, return the first node
        {
            return globalNodes[0];
        }
        else // If NOT cyclic, the array of globalNodes will be reversed, so the current from node will be next
        {
            return globalNodes[fromNodeIndex];
        }
    }

    public void NextNode()
    {
        percentBetweenNodes = 0;
        fromNodeIndex++;

        if (!cyclic)
        {
            if (fromNodeIndex >= globalNodes.Length - 1)
            {
                fromNodeIndex = 0;
                System.Array.Reverse(globalNodes);
            }
        }

        nextMoveTime = Time.time + waitTime;
    }

    protected void GenerateGlobalNodes()
    {
        globalNodes = new Vector2[localNodes.Length];

        for (int i = 0; i < localNodes.Length; i++)
        {
            globalNodes[i] = localNodes[i] + (Vector2)transform.position;
        }
    }

    protected float Ease(float x)
    {
        float a = easeAmount + 1;
        return Mathf.Pow(x, a) / (Mathf.Pow(x, a) + Mathf.Pow(1 - x, a));
    }

    public void ResetNumbers()
    {
        fromNodeIndex = 0;
        percentBetweenNodes = 0;
    }

    protected void OnDrawGizmos()
    {
        if (localNodes != null)
        {
            Gizmos.color = Color.cyan;
            float size = 0.1f;
            int numNodes = (useLocalNodes) ? localNodes.Length : globalNodes.Length;
            
            for (int i = 0; i < numNodes; i++)
            {
                Vector2 globalNodePos = (!useLocalNodes || Application.isPlaying) ? globalNodes[i] : localNodes[i] + (Vector2)transform.position;
                Gizmos.DrawSphere(globalNodePos, size);
            }
        }
    }
}
