using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using TMPro;

public class DealPlayerDamage : MonoBehaviour
{
    // veery hacky...
    public static int totalTroopCount = 0;


    public BattlefieldMenu menu;
    public DealPlayerDamage opponent;
    public TeamType team;
    public int hit_points = 20;
    public TextMeshPro hit_point_text;
    // Start is called before the first frame update
    void Start()
    {
        Assert.IsTrue(menu != null);
        Assert.IsTrue(opponent != null);
        hit_point_text.text = hit_points.ToString();
        totalTroopCount = 0;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Unit troop = collision.gameObject.GetComponentInParent<Unit>();
        if (troop != null) {
            hit_points -= troop.common.player_damage;
            hit_points = Mathf.Clamp(hit_points, 0, hit_points);
            hit_point_text.text = hit_points.ToString();

            troop.Kill();
  
            //Destroy(troop.gameObject);

            // TOTAL VICTORY
            if (hit_points <= 0)
            {
                menu.ShowEndGamePanel();
            }
#if false
            else {
                // CHECK PARTIAL VICTORY
                if ((menu.decks[0].cards_to_play_count == 0 && menu.decks[1].cards_to_play_count == 0)
                    && totalTroopCount == 0)
                {
                    // check result
                    EndGamePanelByHitPoints();
                }
            }
            //Debug.Log("Toop Attacked");
#endif
        }
    }

}
