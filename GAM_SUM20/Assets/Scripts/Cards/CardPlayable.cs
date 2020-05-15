using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

[RequireComponent(typeof(Card))]
public class CardPlayable : MonoBehaviour
{
    Card card;
    private PlayerHand playerHand;

    public Text HR_text;
    public Text MR_text;

    public Deck deck;

    Vector2 initPos;
    Vector3 initScale;
    Battlefield battlefield;
    PlayerResources player_resources;

    private bool has_resources = false;
    private bool can_spawn = false;
    private bool has_selected = false;

    private void Awake()
    {
        card = GetComponent<Card>();
        playerHand = FindObjectOfType<PlayerHand>();
        battlefield = FindObjectOfType<Battlefield>();
        Assert.IsTrue(deck != null);
        player_resources = deck.gameObject.GetComponent<PlayerResources>();
    }

    // Start is called before the first frame update
    void Start()
    {

        initPos = transform.localPosition;
        initScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        //if(has_selected)
            HasResources();
    }

    private void OnMouseDrag()
    {
        if (!has_selected || !has_resources)
            return;
        Vector2Int coord = battlefield.GetCellCoordAtTouch();
        // show unit on battlefield
        if (battlefield.IsInsideGrid(coord))
        {
            // spawn unit
            if (deck.selected_card == null)
            {
                deck.SelectType(card.type);
                ShowCard(false);
            }
            can_spawn = battlefield.SnapToCaptured(ref coord, TeamType.Player) ? true : false; ;
            // move unit
            deck.selected_card.transform.position = battlefield.GetCellPos(coord);
        }
        // show card
        else
        {
            can_spawn = false;
            // remove spawned unit
            if (deck.selected_card != null)
            {
                Destroy(deck.selected_card);
                ShowCard(true);
            }
            // move card
            Vector3 pos = Input.mousePosition - new Vector3(Screen.width, Screen.height, 0) / 2;
            SetCardPos(pos);
        }
    }
    private void OnMouseDown()
    {
        playerHand.UnselectCards();
        Select();
    }
    private void OnMouseEnter()
    {
        //Debug.Log("Mouse Enter " + name);
        
    }

    private void OnMouseUp()
    {
        //Debug.Log("Mouse Up " + name);
        Unselect();

        if (deck.selected_card == null)
            return;
        if (!has_resources)
        {
            // destroy blueprint
            Destroy(deck.selected_card);
            ShowCard(false);
        }
        if (!can_spawn)
            return;
        // Vector2Int coord = battlefield.GetCellCoordAtTouch();
        // show unit on battlefield
        //if (battlefield.IsInsideGrid(coord))
        {
            // consume resources
            player_resources.ConsumeResources(deck.cardCosts[(int)card.type]);

            // make visible again
            ShowCard(true);

            // randomize next type
            SetType(deck.DrawCard());

            // confirm spawn
            deck.PlaySelected();
        }

    }

    void ShowCard(bool show)
    {
        card.image.enabled = show;
        HR_text.gameObject.SetActive(show);
        MR_text.gameObject.SetActive(show);
    }

    public void SetType(CardType _type)
    {
        card.type = _type;
        // set resource UI
        Vector2Int costs = deck.cardCosts[(int)_type];
        HR_text.text = costs.x.ToString();
        MR_text.text = costs.y.ToString();
    }

    void SetCardPos(Vector3 pos)
    {
        transform.localPosition = pos;
        transform.localPosition += playerHand.cardSelectedPivot;
    }

    public void Select()
    {
        has_selected = true;
        transform.localScale *= playerHand.cardSelectedScale;
        
    }

    public void Unselect()
    {
        transform.localScale = initScale;
        transform.localPosition = initPos;
    }

    void HasResources()
    {
        Vector2Int cost = deck.cardCosts[(int)card.type];
        if (cost.x < player_resources.HR_curr && cost.y < player_resources.MR_curr)
        {
            has_resources = true;
            card.image.color = Color.white;
            //Debug.Log("Card playable!");
        }
        else
        {
            has_resources = false;
            card.image.color = Color.grey;
        }
    }


}
