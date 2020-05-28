using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Deck))]
public class PlayerHand : MonoBehaviour
{
    public float cardSelectedScale = 1.1f;
    public Vector3 cardSelectedPivot;

    private CardPlayable[] cards;      // played card (troop, building, spell)
    Deck deck;
    //Battlefield battlefield;

    // Start is called before the first frame update
    void Start()
    {
        deck = GetComponent<Deck>();
        cards = FindObjectsOfType<CardPlayable>();
        foreach (CardPlayable c in cards)
            c.SetType(deck.DrawCard());

    }

    // Update is called once per frame
    void Update()
    {
           
    }

    public void UnselectCards()
    {
        foreach (CardPlayable c in cards)
        {
            c.Unselect();
        }
    }
}
