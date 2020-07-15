using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class InfoPanel : MonoBehaviour
{
    public RectTransform current_deck_panel;
    public RectTransform reward_panel;

    public DeckCard card_prefab;
    private List<DeckCard> deck_card_objects;
    private Dictionary<CardType, DeckCard> reward_card_objects;

    public TMPro.TextMeshProUGUI card_count_text;

    private MapNode currentNode;

    public GameObject reward_received_panel;

    // Start is called before the first frame update
    void Start()
    {
        //deck_card_objects = new List<DeckCard>();
        //reward_card_objects = new Dictionary<CardType, DeckCard>();
    }

    public void SetUp(MapNode node)
    {
        Assert.IsTrue(node != null);

        reward_received_panel.SetActive(node.reward_received);

        if (node == currentNode)
            return;

        currentNode = node;

        if(deck_card_objects != null && deck_card_objects.Count > 0)
            ClearDeck();
        if(node.army != null)
            SetUpDeck(node.army.GetDeck());
        if(reward_card_objects != null && reward_card_objects.Count > 0)
            ClearReward();
        SetUpReward(node.deck_reward);

    }

    void SetUpDeck(CardTypeCount[] deck)
    {
        Assert.IsTrue(deck.Length == (int)CardType.CardType_Count);

        deck_card_objects = new List<DeckCard>();
        for (int i = 0; i < (int)CardType.CardType_Count; ++i)
        {
            CardTypeCount card = deck[i];
            for (int j = 0; j < card.count; ++j)
            {
                DeckCard dc = Instantiate(card_prefab, current_deck_panel) as DeckCard;
                dc.cardImage.type = card.type;
                dc.count_text_frame.SetActive(false);
                dc.eventTrigger.enabled = false;

                deck_card_objects.Add(dc);
            }
        }
        card_count_text.text = "Army size: " + deck_card_objects.Count.ToString();
    }

    void SetUpReward(CardTypeCount[] reward)
    {
        Assert.IsTrue(reward.Length == (int)CardType.CardType_Count);

        reward_card_objects = new Dictionary<CardType, DeckCard>();
        for (int i = 0; i < (int)CardType.CardType_Count; ++i)
        {
            CardTypeCount card = reward[i];
            if (card.count > 0)
            {
                DeckCard dc = Instantiate(card_prefab, reward_panel) as DeckCard;
                dc.cardImage.type = card.type;
                dc.count_text_frame.SetActive(true);
                dc.UpdateText(card.count);
                dc.eventTrigger.enabled = false;

                reward_card_objects.Add(card.type, dc);
            }
        }
    }

    void ClearDeck()
    {
        foreach(DeckCard dc in deck_card_objects)
        {
            Destroy(dc.gameObject);
        }
        deck_card_objects.Clear();
    }

    void ClearReward()
    {
        foreach (var pair in reward_card_objects)
        {
            Destroy(pair.Value.gameObject);
        }
        reward_card_objects.Clear();
    }
}
