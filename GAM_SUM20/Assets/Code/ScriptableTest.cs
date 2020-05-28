using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ScriptableTest : ScriptableObject
{
    public int data;

    private void OnEnable()
    {
        Debug.Log(this.name + " enabled.");
        data = 1;
    }
    private void OnDisable()
    {
        data = 0;
    }
    private void OnDestroy()
    {
        Debug.Log(name + " destroyed.");
    }
}
