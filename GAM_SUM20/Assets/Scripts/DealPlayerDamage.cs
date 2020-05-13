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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        UnitStats troop = collision.gameObject.GetComponentInParent<UnitStats>();
        if (troop != null) {
            hit_points -= troop.player_damage;
            hit_point_text.text = hit_points.ToString();
            Destroy(troop.gameObject);
            Debug.Log("Toop Attacked");
        }
    }
}
