using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Assertions;

[RequireComponent(typeof(MapCampaign))]
public class CompleteCampaignTutorial : TutorialTips
{
    MapCampaign map;

    Deck player_deck = null;
    int init_card_count;

    // Start is called before the first frame update
    new void Start()
    {
        save_filepath = GameSettings.INSTANCE.tuto_campaign_savename;
        map = GetComponent<MapCampaign>();
        // if tip 4, search target for tip 5... (your army can be anywhere)
        List<Deck> player_armies = map.GetArmiesOfTeam(TeamType.Player);
        Assert.IsTrue(player_armies.Count > 0);
        player_deck = player_armies[0];
        // set army as target of tip 8
        Assert.IsTrue(tips.Length > 8);
        Assert.IsTrue(tips[8].highlighted_objects != null);
        Transform nodeTransform = player_deck.transform.parent;
        Assert.IsTrue(nodeTransform != null);
        tips[8].highlighted_objects[0] = nodeTransform;

        init_card_count = player_deck.cards_to_play_count;



        base.Start();

        // check battle won after Tip_deck_editor_confirm (6)
        if (current_tip == 6 && GameSettings.INSTANCE.IsBattle()) {
            NextTip();
        }
        // check if campaign end
        MapNode lastNode = map.GetDeadEndNodeOfTeam(TeamType.Player);
        if (lastNode != null) {
            ShowTip(tips.Length - 1);
        }
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
        if (current_tip == 2) // SELECT ARMY TO ATTACK
        {
            if (map.selected_node != null && map.selected_node.team == TeamType.Opponent) {
                NextTip();
            }
        }
        if (current_tip == 3) // MOVE ARMY TO BATTLE
        {
            // Obsolete with deck editor

            //if (map.is_moving && tips[current_tip].panel.activeSelf == true)
            //    HideTip();

            if (map.menu.deckManager.gameObject.activeSelf == true)
                NextTip();
        }
        if (current_tip == 9)  // open AID PACKET
        {
            if (init_card_count < player_deck.cards_to_play_count) {
                // cards has been added
                NextTip();
            }
        }
        if (current_tip == 11) // rememeber info tip
        {
            // if info panel opened
            if (map.menu.infoPanel.activeSelf == true && tips[current_tip].panel.activeSelf == true) {
                HideTip();  // TODO HACK WARNING CHANGE ((last tip))
                //NextTip();
            }
        }
    }

    private void OnDestroy()
    {
        //save tip
        PlayerPrefs.SetInt(save_filepath, current_tip);

    }
}
