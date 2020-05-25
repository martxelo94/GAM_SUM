using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Assertions;
using System.Linq;

public class Deck : MonoBehaviour
{
    public TeamType team;

    [HideInInspector]
    public Battlefield battlefield;
    public CardManager cards { get; private set; }
    [HideInInspector]
    public GameObject selected_card;
    CardType selected_type;


    public CardType[] deck_types;   // card types in deck
    private BitArray deck_drawed;        // cards that passed by your hand
    private int current_card = 0;
    private int played_cards = 0;
    private System.Random randomizer = new System.Random();   // to shuffle deck

    private void Awake()
    {
        battlefield = FindObjectOfType<Battlefield>();
        cards = FindObjectOfType<CardManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        deck_drawed = new BitArray(deck_types.Length);
        ShuffleDeck();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SelectType(CardType type)
    {
        selected_type = type;
        selected_card = Instantiate(cards.blueprints[(int)type]);
        selected_card.transform.localScale *= battlefield.cell_size;
    }

    public GameObject PlaySelected()
    {
        Assert.IsTrue(selected_card != null);
        //selected_card.transform.localScale *= battlefield.cell_size;
        // activate logic components
        //var components = selected_card.GetComponentsInChildren<MonoBehaviour>();
        //foreach (var c in components)
        //{
        //    c.enabled = true;
        //}
        GameObject squadObj = Instantiate(cards.card_prefabs[(int)selected_type]) as GameObject;
        squadObj.transform.position = selected_card.transform.position;
        squadObj.transform.localScale = selected_card.transform.localScale;
        Destroy(selected_card);
        selected_card = null;
        //Squad squad = squadObj.GetComponent<Squad>();
        //Assert.IsTrue(squad != null);

        played_cards++;

        return squadObj;
    }

    public void RemoveDrawCards()
    {
        CardType[] new_deck = new CardType[Mathf.Max(deck_types.Length - played_cards, 0)];
        for (int i = played_cards; i < deck_types.Length; ++i)
            new_deck[i - played_cards] = deck_types[i];
        deck_types = new_deck;
        deck_drawed = new BitArray(deck_types.Length);
    }

    public void CombineCards(CardType[] other_deck)
    {
        CardType[] new_deck = new CardType[deck_types.Length + other_deck.Length];
        for (int i = 0; i < deck_types.Length; ++i)
            new_deck[i] = deck_types[i];
        for (int i = 0; i < other_deck.Length; ++i)
            new_deck[deck_types.Length + i] = other_deck[i];
        deck_types = new_deck;
        deck_drawed = new BitArray(deck_types.Length);
    }

    public CardType DrawCard()
    {
        // shuffle if ended
        if (current_card >= deck_types.Length)
        {
            ShuffleDeck();
            current_card = 0;
        }
        deck_drawed[current_card] = true;
        return deck_types[current_card++];
    }

    void ShuffleDeck()
    {
        int p = deck_types.Length;
        for (int n = p - 1; n > 0; n--) {
            int r = randomizer.Next(0, n);
            // swap type
            CardType t = deck_types[r];
            deck_types[r] = deck_types[n];
            deck_types[n] = t;
        }
        // reset drawed
        deck_drawed.SetAll(false);
        current_card = 0;
    }

    void AddToDeck(CardType card_type, int count)
    {
        CardType[] new_deck = new CardType[deck_types.Length + count];
        deck_types.CopyTo(new_deck, 0);
        for (int i = deck_types.Length; i < deck_types.Length + count; ++i)
            new_deck[i] = card_type;
        deck_types = new_deck;
    }

    public void Save()
    {
        Dictionary<CardType, int> cards_in_deck = new Dictionary<CardType, int>();
        foreach (CardType t in deck_types) {
            int val;
            if (cards_in_deck.TryGetValue(t, out val)) {
                cards_in_deck.Add(t, val + 1);
            }
            else
                cards_in_deck.Add(t, 1);
        }
        string key_pref = gameObject.GetInstanceID().ToString();
        PlayerPrefs.SetInt(key_pref, 1);
        foreach (var pair in cards_in_deck) {
            string key = key_pref + pair.Key.ToString();
            PlayerPrefs.SetInt(key, pair.Value);
        }
    }

    public bool Load()
    {
        string key_pref = gameObject.GetInstanceID().ToString();
        int id_found = PlayerPrefs.GetInt(key_pref, 0);
        if (id_found == 0)
            return false;

        deck_types = new CardType[0];
        for (int i = 0; i < (int)CardType.CardType_Count; ++i) {
            CardType t = (CardType)i;
            int val = PlayerPrefs.GetInt(key_pref + t.ToString(), 0);
            if (val > 0) {
                AddToDeck(t, val);
            }
        }
        return true;
    }
}
