using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnTime : MonoBehaviour
{
    public float timeToDestroy = 0f;

    private void Awake()
    {
        Destroy(gameObject, timeToDestroy);
    }
}
