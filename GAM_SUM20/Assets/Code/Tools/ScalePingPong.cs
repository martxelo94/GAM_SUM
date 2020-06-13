using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScalePingPong : MonoBehaviour
{
    [Range(0, 10)]
    public float speed;
    [Range(0, 10)]
    public float factor;

    Vector3 initScale;

    // Start is called before the first frame update
    void Start()
    {
        initScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        float t = Mathf.PingPong(Time.time * speed, factor);
        transform.localScale = initScale + initScale * t;
    }
}
