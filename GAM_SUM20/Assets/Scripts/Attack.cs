using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [HideInInspector]
    public TeamType team = TeamType.None;   // set from squad
    public int damage = 1;
    public float range = 20;

    public float attack_time = 1.0f;
    private float curr_attack_time = 0.0f;

    Health target;
    
    List<Health> targets_in_range;

    // Start is called before the first frame update
    void Start()
    {
        targets_in_range = new List<Health>();
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null) {
            if (targets_in_range.Count > 0)
            {
                target = targets_in_range.Last();
                targets_in_range.RemoveAt(targets_in_range.Count - 1);
            }
        }
        else {
            float dt = Time.deltaTime * Time.timeScale;

            // face towards
            transform.LookAt(target.transform, -Vector3.forward);

            curr_attack_time += dt;
            if (curr_attack_time > attack_time) {
                if (target.Damage(damage)) {
                    targets_in_range.Remove(target);
                    Destroy(target.gameObject);

                    Debug.Log("Killed enemy!");
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Health health = collision.GetComponent<Health>();
        if (health != null)
            targets_in_range.Add(health);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Health health = collision.GetComponent<Health>();
        if (health != null)
            targets_in_range.Remove(health);
    }
}
