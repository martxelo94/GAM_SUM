using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DealPlayerDamage : MonoBehaviour
{
    public int hit_points = 20;
    public Text hit_point_text;
    // Start is called before the first frame update
    void Start()
    {
        hit_point_text.text = hit_points.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Something trigger");
        Movement troop = other.gameObject.GetComponent<Movement>();
        if (troop != null) {
            hit_points -= troop.troop_count;
            hit_point_text.text = hit_points.ToString();
            Destroy(troop.gameObject);
            Debug.Log("Toop Attacked");
        }
    }
}
