﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(BattlefieldMenu))]
public class CompleteBattlefieldTutorial : TutorialManager
{
    BattlefieldMenu menu;
    int player_init_hitpoints;
    int opponent_init_hitpoints;

    private string tuto_campaign_filepath;

    new private void Start()
    {
        tuto_campaign_filepath = GameSettings.INSTANCE.tuto_campaign_savename;
        save_filepath = GameSettings.INSTANCE.tuto_battle_savename;
        base.Start();

        menu = GetComponent<BattlefieldMenu>();

        player_init_hitpoints = menu.playerHitPoints[0].hit_points;
        opponent_init_hitpoints = menu.playerHitPoints[1].hit_points;

        string tip_name = tips[current_tip].gameObject.name;
        // disable opponent ai
        if (tip_name == "Tip_hand") {
            OpponentAI ai = menu.decks[1].GetComponent<OpponentAI>();
            ai.enabled = false;
            menu.useEndGameCheckTime = false;
        }

    }
    new void Update()
    {
        base.Update();

        // HARDCODED INPUT CHECK
        string tip_name = tips[current_tip].gameObject.name;
        if (tip_name == "Tip_hand")
        {
            if (DealPlayerDamage.totalTroopCount > 0)
            {
                NextTip();
                // enable opponent ai
                OpponentAI ai = FindObjectOfType<OpponentAI>();
                ai.enabled = true;
                menu.useEndGameCheckTime = true;
            }
        }
        else if (tip_name == "Tip_any_cards")
        {
            if (player_init_hitpoints > menu.playerHitPoints[0].hit_points)
            {
                NextTip();  // player damage
            }
            else if (opponent_init_hitpoints > menu.playerHitPoints[1].hit_points)
            {
                NextTip(); // opponent damage
            }
        }
        else if (tip_name == "Tip_damage_taken") {
            if (menu.IsBattleEnd()) {
                ShowTip(5);
            }
        }
    }
    private void OnDestroy()
    {
        //save tip
        PlayerPrefs.SetInt(save_filepath, current_tip);

        // update campaign tip
        //int tip = PlayerPrefs.GetInt(tuto_campaign_filepath, -2);
        //Assert.IsTrue(tip != -2);
        //PlayerPrefs.SetInt(tuto_campaign_filepath, tip + 1);
    }
}