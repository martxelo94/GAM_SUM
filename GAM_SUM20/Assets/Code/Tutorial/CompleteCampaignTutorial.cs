﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Assertions;

[RequireComponent(typeof(MapCampaign))]
public class CompleteCampaignTutorial : TutorialManager
{
    MapCampaign map;

    Deck player_deck = null;
    int init_card_count;

    public GameObject tip_add_cards_inactive_condition;
    public DeckManager deckManager;
    int init_card_pool_count;
    int init_card_deck_count;

    // Start is called before the first frame update
    new void Start()
    {
        save_filepath = GameSettings.INSTANCE.tuto_campaign_savename;
        base.Start();

        Assert.IsTrue(tip_add_cards_inactive_condition != null);
        Assert.IsTrue(deckManager != null);

        map = GetComponent<MapCampaign>();
        // if tip 4, search target for tip 5... (your army can be anywhere)
        List<Deck> player_armies = map.GetArmiesOfTeam(TeamType.Player);
        Assert.IsTrue(player_armies.Count > 0);
        player_deck = player_armies[0];
        // set army as target of tip 8
        TutorialTip tip_select_your_army = FindTipByName("Tip_select_your_army");
        Assert.IsTrue(tip_select_your_army != null);
        Assert.IsTrue(tip_select_your_army.highlighted_objects != null);
        Transform nodeTransform = player_deck.transform.parent;
        Assert.IsTrue(nodeTransform != null);
        tip_select_your_army.highlighted_objects[0] = nodeTransform;

        init_card_count = player_deck.cards_to_play_count;
        init_card_pool_count = deckManager.card_pool_count;
        init_card_deck_count = deckManager.card_deck_count;


        // check battle won after Tip_deck_editor_confirm (6)
        if (tips[current_tip].name == "Tip_deck_editor_confirm" && GameSettings.INSTANCE.IsBattle()) {
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
        if (IsValidTip())
        {
            string tip_name = tips[current_tip].gameObject.name;
            if (tip_name == "Tip_add_cards")
            {
                if (deckManager.card_pool_count > init_card_pool_count && tip_add_cards_inactive_condition.activeSelf == false)
                    NextTip();
            }
            else if (tip_name == "Tip_capture_node") // SELECT FIRST NODE TO ATTACK
            {
                if (map.selected_node != null && map.selected_node.team == TeamType.Opponent)
                {
                    NextTip();
                }
            }
            else if (tip_name == "Tip_confirm_move") // MOVE ARMY TO BATTLE
            {
                // Obsolete with deck editor

                //if (map.is_moving && tips[current_tip].panel.activeSelf == true)
                //    HideTip();

                if (map.is_moving == true)
                    NextTip();
            }
            else if (tip_name == "Tip_deck_editor_drag")
            {
                if (deckManager.card_deck_count > init_card_deck_count)
                    NextTip();
            }
            else if (tip_name == "Tip_after_battle")  // open AID PACKET
            {
                if (init_card_count < player_deck.cards_to_play_count)
                {
                    // cards has been added
                    HideTip();
                }
            }
            else if (tip_name == "Tip_remember_info") // rememeber info tip
            {
                if (deckManager.gameObject.activeSelf == true)
                    NextTip();
                // if info panel opened
                else if (map.menu.infoPanel.activeSelf == true && tips[current_tip].gameObject.activeSelf == true)
                {
                    HideTip();  // TODO HACK WARNING CHANGE ((last tip))
                                //NextTip();
                }
            }

        }
    }

    private void OnDestroy()
    {
        //save tip
        PlayerPrefs.SetInt(save_filepath, current_tip);

    }
}
