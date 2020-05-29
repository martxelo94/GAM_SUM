using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomTransform : MonoBehaviour
{
    public Vector3 offset;
    public Vector3 eulerAngles;

    // Update is called once per frame
    void Update()
    {
        transform.position = transform.parent.position + offset;
        transform.eulerAngles = eulerAngles;
    }
}
