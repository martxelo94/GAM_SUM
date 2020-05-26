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

    //[HideInInspector]
    //public Battlefield battlefield;
    public CardManager cards { get; private set; }
    [HideInInspector]
    private GameObject selected_card;
    private CardType selected_type = CardType.None;

    public Transform selected_transform {
        get {
            Assert.IsTrue(selected_card != null);
            return selected_card.transform;
        }
    }

    [SerializeField]
    private CardType[] _deck_types;
    public CardType[] deck_types {
        get { return _deck_types; }
        set
        {
            _deck_types = value;
            deck_drawed = new BitArray(_deck_types.Length);
        }
    }// card types in deck
    private BitArray deck_drawed;        // cards that passed by your hand
    private int current_card = 0;
    private int played_cards = 0;
    private System.Random randomizer = new System.Random();   // to shuffle deck

    private void Awake()
    {
        //battlefield = FindObjectOfType<Battlefield>();
        cards = FindObjectOfType<CardManager>();
        deck_drawed = new BitArray(deck_types.Length);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (deck_drawed != null)
            Assert.IsTrue(deck_drawed.Length == deck_types.Length);
        else
            deck_drawed = new BitArray(deck_types.Length);
        ShuffleDeck();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool HasSelected()
    {
        //Assert.IsTrue(selected_card != null);
        return selected_type != CardType.None;
    }

    public void Unselect()
    {
        selected_type = CardType.None;
        Assert.IsTrue(selected_card != null);
        Destroy(selected_card);
        selected_card = null;
    }

    public void SelectType(CardType type)
    {
        Assert.IsTrue(type != CardType.None);
        selected_type = type;
        selected_card = cards.BlueprintType(type);
        //selected_card.transform.localScale *= battlefield.cell_size;
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
        GameObject squadObj = cards.PlayType(selected_type);
        squadObj.transform.position = selected_card.transform.position;
        squadObj.transform.localScale = selected_card.transform.localScale;

        Unselect();
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

    public void Randomize()
    {
        for (int i = 0; i < deck_types.Length; ++i) {
            deck_types[i] = (CardType)randomizer.Next(0, (int)(CardType.CardType_Count));
        }
    }
}
