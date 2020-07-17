using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class MoveOnEnable : MonoBehaviour
{
    public RectTransform[] targets;
    public Vector3[] localOffsets;

    private void Start()
    {
        Assert.IsTrue(targets.Length == localOffsets.Length);
    }

    private void OnEnable()
    {
        for(int i = 0; i < targets.Length; ++i)
            targets[i].localPosition = localOffsets[i];
    }
}
