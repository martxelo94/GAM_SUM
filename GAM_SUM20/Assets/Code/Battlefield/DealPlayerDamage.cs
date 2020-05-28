using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DealPlayerDamage : MonoBehaviour
{
    public TeamType team;
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
            hit_points -= troop.common.player_damage;
            hit_point_text.text = hit_points.ToString();
            troop.Kill();
            //Destroy(troop.gameObject);

            if (hit_points <= 0) {
                BattlefieldMenu menu = FindObjectOfType<BattlefieldMenu>();
                menu.ShowEndGamePanel(team == TeamType.Opponent);
            }
            Debug.Log("Toop Attacked");
        }
    }
}
