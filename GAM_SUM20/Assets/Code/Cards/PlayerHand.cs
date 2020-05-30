﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Deck))]
public class PlayerHand : MonoBehaviour
{
    public float cardSelectedScale = 1.1f;
    public Vector3 cardSelectedPivot;

    public RectTransform deckIcon;
    Deck deck;

    public CardPlayable[] cards;      // played card (troop, building, spell)

    //Battlefield battlefield;

    // Start is called before the first frame update
    void Start()
    {
        deck = GetComponent<Deck>();
        Assert.IsTrue(deck != null);
        Assert.IsTrue(cards != null);

        for (int i = 0; i < cards.Length; ++i)
            DrawCardAnimation(cards[i]);

    }

    public void DrawCardAnimation(CardPlayable card)
    {
        StartCoroutine(DrawCard(card));
    }

    IEnumerator DrawCard(CardPlayable card)
    {
        CardType drawed_card = deck.DrawCard();
        if (drawed_card == CardType.None)
        {
            card.SetType(drawed_card);
            yield break;
        }
        int frames = (int)(1f / Time.deltaTime); // 1 sec animation

        Vector3 finalScale = card.initScale;
        Vector3 initScale = finalScale / 3f;
        Vector3 finalPos = card.initPos;
        Vector3 initPos = deckIcon.TransformPoint(deckIcon.anchoredPosition + deckIcon.sizeDelta / 2);

        for (int i = 0; i < frames; ++i)
        {
            float t = (float)i / frames;
            Vector3 pos = Vector3.Lerp(initPos, finalPos, t);
            Vector3 sc = Vector3.Lerp(initScale, finalScale, t);

            card.transform.position = pos;
            card.transform.localScale = sc;

            yield return null;  // on frame step
        }
        card.transform.position = finalPos;
        card.transform.localScale = finalScale;

        // draw the card
        card.SetType(drawed_card);
    }

    public void UnselectCards()
    {
        for(int i = 0; i < cards.Length; ++i)
        {
            cards[i].Unselect();
        }
    }

#if false
    private void OnDrawGizmos()
    {
        Vector3[] corners = new Vector3[4];
        deckIcon.GetWorldCorners(corners);// + deckIcon.pivot;

        Gizmos.color = Color.magenta;
        //Gizmos.DrawLine(corners[0], corners[1]);
        Gizmos.DrawLine(corners[1], corners[2]);
        Gizmos.DrawLine(corners[2], corners[3]);
        //Gizmos.DrawLine(corners[3], corners[0]);

        //Vector3 pos = deckIcon.TransformPoint(deckIcon.anchoredPosition + deckIcon.sizeDelta / 2);
        //Vector3 pos = deckIcon.InverseTransformPoint(deckIcon.position) + deckIcon.anchoredPosition3D;
        //Vector3 pos = deckIcon.anchoredPosition;
       // Gizmos.DrawCube(pos, Vector3.one * 5);
    }
#endif
}
