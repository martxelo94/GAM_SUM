using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Deck))]
public class PlayerHand : MonoBehaviour
{
    public float cardSelectedScale = 1.1f;
    public Vector3 cardSelectedPivot;

    public CardPlayable[] cards { get; private set; }      // played card (troop, building, spell)
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


    public void UnselectCards()
    {
        for(int i = 0; i < cards.Length; ++i)
        {
            cards[i].Unselect();
        }
    }
}
