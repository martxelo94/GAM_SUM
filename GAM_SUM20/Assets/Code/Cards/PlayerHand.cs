using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(PlayerResources))]
[RequireComponent(typeof(Deck))]
public class PlayerHand : MonoBehaviour
{
    public float cardSelectedScale = 1.1f;
    public Vector3 cardSelectedPivot;

    public RectTransform deckIcon;

    Deck deck;
    PlayerResources resources;
    public Battlefield battlefield;

    public CardPlayable[] cards;      // played card (troop, building, spell)


    private void Awake()
    {
        deck = GetComponent<Deck>();
        resources = GetComponent<PlayerResources>();
        Assert.IsTrue(cards != null);
        Assert.IsTrue(battlefield != null);
    }

    // Start is called before the first frame update
    void Start()
    {
        deck.UpdateCardCount();
        for (int i = 0; i < cards.Length; ++i) {
            cards[i].hand = this;
            DrawCardAnimation(cards[i]);
        }

    }

    public void DrawCardAnimation(CardPlayable card)
    {
        StartCoroutine(DrawCard(card));
    }

    IEnumerator DrawCard(CardPlayable card)
    {
        yield return null;  // wait one frame for the initialization
        CardType drawed_card = deck.DrawCard();
        if (drawed_card == CardType.None)
        {
            card.SetType(drawed_card);
            yield break;
        }
        // make visible
        card.ShowCard(true);

        int frames = (int)(1f / Time.deltaTime); // 1 sec animation

        Vector3 finalScale = card.initScale;
        Vector3 initScale = finalScale / 3f;
        Vector3 finalPos = card.initPos;
        Vector3 initPos = deckIcon.position;
        //Vector3 initRot = deckIcon.eulerAngles;
        //card.transform.eulerAngles = new Vector3(initRot.x, initRot.y - Mathf.PI, initRot.z);
        Vector3 angles = new Vector3(30f, 180f, 30f);
        card.transform.localEulerAngles = -angles;

        for (int i = 0; i < frames; ++i)
        {
            float t = (float)i / frames;
            Vector3 pos = Vector3.Lerp(initPos, finalPos, t);
            Vector3 sc = Vector3.Lerp(initScale, finalScale, t);

            card.transform.position = pos;
            card.transform.localScale = sc;
            //card.transform.Rotate(angles / frames);
            card.transform.localEulerAngles += angles / frames;

            // reveal card at the middle of rotation
            if (i == frames / 2) {
                card.SetType(drawed_card);
            }

            yield return null;  // on frame step
        }
        card.transform.position = finalPos;
        card.transform.localScale = finalScale;

        // draw the card
        //card.SetType(drawed_card);
    }

    public void UnselectCards()
    {
        for(int i = 0; i < cards.Length; ++i)
        {
            cards[i].Unselect();
        }
    }

    public Deck GetDeck() { return deck; }
    public PlayerResources GetResources() { return resources; }

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
