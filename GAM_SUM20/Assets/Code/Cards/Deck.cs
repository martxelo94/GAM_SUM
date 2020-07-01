using System;
using UnityEditor;
using System.Reflection;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;
using UnityEngine.Assertions;
using System.Linq;

public class Deck : MonoBehaviour
{


    public TeamType team;

    //[HideInInspector]
    //public Battlefield battlefield;
    public CardManager cm;
    public TextMeshPro deckCountText;
    private GameObject selected_card;
    private CardType selected_type = CardType.None;

    public Transform selected_transform {
        get {
            Assert.IsTrue(selected_card != null);
            return selected_card.transform;
        }
    }
    [SerializeField]
    private CardTypeCount[] deck_types;
    private int total_card_count = -1;
    public int cards_to_play_count { get; private set; } = -1;
    // card types in deck
    //private BitArray deck_drawed;        // cards that passed by your hand
    //private int current_card = 0;
    //private int played_cards = 0;

    private void Awake()
    {
        //battlefield = FindObjectOfType<Battlefield>();
        
        //if (cm == null)
        //    cm = Resources.Load("Scripts/CardManager") as CardManager;
        Assert.IsTrue(cm != null);


    }

    // Start is called before the first frame update
    void Start()
    {

        Assert.IsTrue(deck_types != null);
        UpdateCardCount();
        Assert.IsTrue(total_card_count > -1);
        if (deckCountText != null)
        {
            UpdateText();
        }
    }

    public CardTypeCount[] GetDeck() { return deck_types; }
    public void SetDeck(CardTypeCount[] deck)
    {
        deck_types = deck;
        total_card_count = cards_to_play_count = CardCount();
    }

    public void UpdateText()
    {
        Assert.IsTrue(deckCountText != null);

        int count = cards_to_play_count;
        bool overflow = count > 999;
        count = Math.Min(999, count);
        deckCountText.text = count.ToString();
        if (overflow)
            deckCountText.text += "+";
    }

    public bool HasSelected()
    {
        //Assert.IsTrue(selected_card != null);
        return selected_type != CardType.None;
    }

    public void Unselect(bool destroy_card = true)
    {
        selected_type = CardType.None;
        Assert.IsTrue(selected_card != null);
        if(destroy_card)
            Destroy(selected_card);
        selected_card = null;
    }

    public void SelectType(CardType type)
    {
        Assert.IsTrue(type != CardType.None);
        selected_type = type;
        selected_card = cm.BlueprintType(type);
        //selected_card.transform.localScale *= battlefield.cell_size;
    }

    public void PlaySelected()
    {
        Assert.IsTrue(selected_card != null && selected_type > CardType.None && selected_type < CardType.CardType_Count);
        StartCoroutine(SpawnWithDelay());
    }

    IEnumerator SpawnWithDelay()
    {
        CardType tmp_type = selected_type; // remember becouse can be set to None during Coroutine
        GameObject tmp_card = selected_card;
        // unselect first to allow multiple coroutines at the same time
        Unselect(false);    // dont destroy instance, yet

        float seconds = cm.cards[(int)tmp_type].spawnTime;
        GameObject timerObj = Instantiate(Resources.Load("Prefabs/UI/WatchTimer") as GameObject);
        WatchTimer timer = timerObj.GetComponent<WatchTimer>();
        Assert.IsTrue(timer != null);
        timer.seconds_per_revolution = seconds;
        timer.transform.position = tmp_card.transform.position + Vector3.back * 3;

        yield return new WaitForSeconds(seconds);

        Destroy(timerObj);

        Assert.IsTrue(tmp_card != null);

        GameObject squadObj = cm.PlayType(tmp_type);
        squadObj.transform.position = tmp_card.transform.position;
        squadObj.transform.localScale = tmp_card.transform.localScale;

        PlayerTeam player = squadObj.GetComponent<PlayerTeam>();
        Assert.IsTrue(player != null);
        // squad stuff
        player.team = team;
        // remove from deck
        cards_to_play_count--;
        UpdateText();

        // destroy card instance
        Destroy(tmp_card);
    }

    public void InitCardTypes()
    {
        // set types to show in the inspector and reset the count
        deck_types = CardTypeCount.Initialize();

        total_card_count = 0;
    }

    public void ClearCardTypes()
    {
        for (int i = 0; i < (int)CardType.CardType_Count; ++i)
        {
            Assert.IsTrue(deck_types[i].type == (CardType)i);
            deck_types[i].count = 0;
        }
        total_card_count = cards_to_play_count = 0;
    }
    public void CombineCards(CardTypeCount[] other_deck)
    {
        Assert.IsTrue(deck_types.Length == other_deck.Length);
        for (int i = 0; i < (int)CardType.CardType_Count; ++i)
        {
            int count = other_deck[i].count;
            deck_types[i].count += count;
            total_card_count += count;
            cards_to_play_count += count;
        }
        UpdateText();
    }

    public CardType DrawCard()
    {
        if (total_card_count == 0)
        {
            return CardType.None;
        }
        CardType type = GetRandomType();
        Assert.IsTrue(type != CardType.None);
        deck_types[(int)type].count -= 1;
        total_card_count--;
        return type;
    }

    public bool RemoveFromDeck(CardType type, int count)
    {
        if (type == CardType.None)
            return false;
        int c = deck_types[(int)type].count;
        bool underflow = c < count;
        int dif = cards_to_play_count - total_card_count;
        if (underflow)
        {
            total_card_count = total_card_count - (count - c);
        }
        else {
            total_card_count = Mathf.Max(total_card_count - count, 0);
        }
        cards_to_play_count = total_card_count + dif;
        c = Mathf.Max(c - count, 0);
        deck_types[(int)type].count = c;
        return true;
    }

    public bool AddToDeck(CardType card_type, int count)
    {
        if (card_type == CardType.None)
            return false;
        Assert.IsTrue(card_type == deck_types[(int)card_type].type);
        deck_types[(int)card_type].count += count;
        total_card_count += count;
        cards_to_play_count += count;
        return true;
    }

    public void Randomize()
    {
        System.Random randomizer = GameSettings.INSTANCE.randomizer;
        // shuffle indices
        int[] indices = new int[deck_types.Length];
        int p = indices.Length;
        for (int n = p - 1; n > 0; n--)
        {
            int r = randomizer.Next(0, n);
            // swap type
            int t = indices[r];
            indices[r] = indices[n];
            indices[n] = t;
        }
        // randomize values
        int weight = total_card_count;
        for (int i = 0; i < deck_types.Length - 1; ++i) {
            deck_types[i].count = randomizer.Next(0, weight);
            weight -= deck_types[i].count;
        }
        deck_types[deck_types.Length - 1].count = weight;
    }

    CardType GetRandomType()
    {
        Assert.IsTrue(total_card_count > -1);
        int randomNum = GameSettings.INSTANCE.randomizer.Next(0, total_card_count);
        CardType type = CardType.None;
        for (int i = 0; i < deck_types.Length; ++i)
        {
            if (randomNum < deck_types[i].count)
            {
                type = deck_types[i].type;
                break;
            }
            randomNum = randomNum - deck_types[i].count;
        }
        return type;
    }

    public int CardCount()
    {
        if (deck_types == null)
            return -1;
        int total_count = 0;
        for (int i = 0; i < (int)CardType.CardType_Count; ++i)
        {
            total_count += deck_types[i].count;
        }
        return total_count;
    }
    public void UpdateCardCount()
    {
        int dif = cards_to_play_count - total_card_count;
        total_card_count = CardCount();
        cards_to_play_count = total_card_count + dif;
    }
}
