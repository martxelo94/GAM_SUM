using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Assertions;

public class DeckManager : MonoBehaviour
{
    [HideInInspector]
    public Battlefield battlefield;
    [HideInInspector]
    public GameObject selected_card;
    CardType selected_type;

    public Vector2Int[] cardCosts;
    public GameObject[] blueprints; // blueprinted card before being played
    public GameObject[] card_prefabs;   // cards to play all logic included
    private Card[] cards;      // played card (troop, building, spell)

    public CardType[] deck_types;   // card types in deck
    private BitArray deck_drawed;        // cards that passed by your hand
    private int current_card = 0;
    private System.Random randomizer = new System.Random();   // to shuffle deck

    private void Awake()
    {
        Assert.IsTrue(card_prefabs.Length == blueprints.Length);
        Assert.IsTrue(blueprints.Length == cardCosts.Length);
        battlefield = FindObjectOfType<Battlefield>();
    }

    // Start is called before the first frame update
    void Start()
    {
        cards = FindObjectsOfType<Card>();
        deck_drawed = new BitArray(deck_types.Length);
        ShuffleDeck();
        foreach (Card c in cards)
            c.type = DrawCard();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SelectType(CardType type)
    {
        selected_type = type;
        selected_card = Instantiate(blueprints[(int)type]);
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
        GameObject squadObj = Instantiate(card_prefabs[(int)selected_type]) as GameObject;
        squadObj.transform.position = selected_card.transform.position;
        squadObj.transform.localScale = selected_card.transform.localScale;
        Destroy(selected_card);
        selected_card = null;
        //Squad squad = squadObj.GetComponent<Squad>();
        //Assert.IsTrue(squad != null);
        return squadObj;
    }

    public void UnselectCards()
    {
        foreach (Card c in cards) {
            c.transform.localScale = c.initScale;
        }
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
}
