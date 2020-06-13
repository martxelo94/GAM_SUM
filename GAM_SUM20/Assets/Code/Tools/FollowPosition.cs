using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPosition : MonoBehaviour
{
    public Transform target;
    public bool follow_x = true;
    public bool follow_y = true;
    public bool follow_z = true;

    // Update is called once per frame
    void Update()
    {
        Vector3 v = transform.position;
        Vector3 p = target.position;
        if (follow_x)
            v.x = p.x;
        if (follow_y)
            v.y = p.y;
        if (follow_z)
            v.z = p.z;
        transform.position = v;
    }
}
