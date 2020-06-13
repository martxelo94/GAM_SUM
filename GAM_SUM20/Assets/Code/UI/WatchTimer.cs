using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WatchTimer : MonoBehaviour
{
    public float seconds_per_revolution = 1f;
    private float angleStep;
    public Transform agujaPivot;
    // Start is called before the first frame update
    void Start()
    {
        angleStep = Time.deltaTime * 360f / (seconds_per_revolution);
    }

    // Update is called once per frame
    void Update()
    {
        agujaPivot.Rotate(new Vector3(0, 0, angleStep));
    }
}
