using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(BattlefieldMenu))]
public class CompleteBattlefieldTutorial : TutorialTips
{
    BattlefieldMenu menu;
    int player_init_hitpoints;
    int opponent_init_hitpoints;

    private string tuto_campaign_filepath;

    new private void Start()
    {
        tuto_campaign_filepath = GameSettings.INSTANCE.tuto_campaign_savename;
        save_filepath = GameSettings.INSTANCE.tuto_battle_savename;
        menu = GetComponent<BattlefieldMenu>();

        player_init_hitpoints = menu.playerHitPoints[0].hit_points;
        opponent_init_hitpoints = menu.playerHitPoints[1].hit_points;

        base.Start();

        // disable opponent ai
        if (current_tip == 0) {
            OpponentAI ai = menu.decks[1].GetComponent<OpponentAI>();
            ai.enabled = false;
        }

    }
    new void Update()
    {
        base.Update();

        // HARDCODED INPUT CHECK
        if (current_tip == 0)
        {
            if (DealPlayerDamage.totalTroopCount > 0)
            {
                NextTip();
                // enable opponent ai
                OpponentAI ai = FindObjectOfType<OpponentAI>();
                ai.enabled = true;
            }
        }
        else if (current_tip == 3)
        {
            if (player_init_hitpoints > menu.playerHitPoints[0].hit_points)
            {
                ShowTip(4);
            }
            else if (opponent_init_hitpoints > menu.playerHitPoints[1].hit_points)
            {
                ShowTip(5);
            }
        }
        else if (current_tip == 4 || current_tip == 5) {
            if (menu.IsBattleEnd()) {
                ShowTip(6);
            }
        }
    }
    private void OnDestroy()
    {
        //save tip
        PlayerPrefs.SetInt(save_filepath, current_tip);

        // update campaign tip
        int tip = PlayerPrefs.GetInt(tuto_campaign_filepath, -2);
        Assert.IsTrue(tip != -2);
        PlayerPrefs.SetInt(tuto_campaign_filepath, tip + 1);
    }
}
