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
        // set army as target of tip 5
        Assert.IsTrue(tips.Length > 5);
        Assert.IsTrue(tips[5].highlighted_object == null);
        tips[5].highlighted_object = player_deck.gameObject;

        init_card_count = player_deck.cards_to_play_count;

        base.Start();
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();

        if (current_tip == 3) // MOVE ARMY TO BATTLE
        {
            if (map.is_moving)
                HideTip();
        }
        if (current_tip == 6)  // open AID PACKET
        {
            if (init_card_count < player_deck.cards_to_play_count) {
                // cards has been added
                NextTip();
            }
        }
    }

    private void OnDestroy()
    {
        //save tip
        PlayerPrefs.SetInt(save_filepath, current_tip);

    }
}
