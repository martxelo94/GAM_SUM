using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public int troop_count = 0;
    public float speed = 1.0f;
    public float acceleration = 1.0f;
    public bool troop_forward = true;

    Rigidbody2D rig;


    // Start is called before the first frame update
    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // physics move
        float dt = Time.deltaTime * Time.timeScale;
        float speed2 = rig.velocity.sqrMagnitude;
        Vector2 dir = troop_forward ? Vector2.up : -Vector2.up;
        if (speed2 < speed * speed) {
            rig.AddForce(new Vector2(0, dir.y * acceleration));
        }
        rig.AddTorque(Vector2.Dot(new Vector2(-transform.up.y, transform.up.x), dir) * speed * acceleration);

    }
}
