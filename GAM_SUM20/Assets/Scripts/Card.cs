using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;   // Text
using UnityEngine.Assertions;

public class Card : MonoBehaviour
{

    Vector2 initPos;
    [HideInInspector]
    public Vector3 initScale;
    Image image;
    Battlefield battlefield;
    public DeckManager deck;
    PlayerResources player_resources;

    [SerializeField]
    private CardType m_type;
    public CardType type
    {
        get { return m_type; }
        set { m_type = value; SetCardName(value); }
    }
    private bool has_resources = false;
    private bool can_spawn = false;

    // Start is called before the first frame update
    void Start()
    {
        initPos = transform.localPosition;
        initScale = transform.localScale;
        image = GetComponent<Image>();
        battlefield = FindObjectOfType<Battlefield>();
        //deck = battlefield.gameObject.GetComponent<DeckManager>();
        Assert.IsTrue(deck != null);
        player_resources = deck.gameObject.GetComponent<PlayerResources>();
        type = m_type;
    }

    // Update is called once per frame
    void Update()
    {
        //if (isDragging) {
        //    Vector3 pos = Input.mousePosition;
        //    transform.localPosition = pos;
        //}
        HasResources();
    }

    private void OnMouseDrag()
    {
        if (!has_resources)
            return;
        Vector2Int coord = battlefield.GetCellCoordAtTouch();
        // show unit on battlefield
        if (battlefield.IsInsideGrid(coord)) {
            // spawn unit
            if (deck.selected_card == null)
            {
                deck.SelectType(type);
                image.enabled = false;
            }
            can_spawn = battlefield.SnapToCaptured(ref coord, TeamType.Player) ? true : false; ;
            // move unit
            deck.selected_card.transform.position = battlefield.GetCellPos(coord);
        }
        // show card
        else {
            can_spawn = false;
            // remove spawned unit
            if (deck.selected_card != null) {
                Destroy(deck.selected_card);
                image.enabled = true;
            }
            // move card
            Vector3 pos = Input.mousePosition - new Vector3(Screen.width, Screen.height, 0) / 2;
            transform.localPosition = pos;
        }
    }
    private void OnMouseDown()
    {
        deck.UnselectCards();
        transform.localScale *= 1.1f;
    }
    private void OnMouseEnter()
    {
        //Debug.Log("Mouse Enter " + name);
        
    }

    private void OnMouseUp()
    {
        //Debug.Log("Mouse Up " + name);
        transform.localPosition = initPos;
        //transform.localScale = initScale;

        if (deck.selected_card != null && !has_resources)
            return;
        if (!can_spawn)
            return;
        // Vector2Int coord = battlefield.GetCellCoordAtTouch();
        // show unit on battlefield
        //if (battlefield.IsInsideGrid(coord))
        {
            // consume resources
            player_resources.ConsumeResources(deck.cardCosts[(int)type]);

            // randomize next type
            type = deck.DrawCard();

            // make visible again
            image.enabled = true;

            // confirm spawn
            deck.PlaySelected();
        }

    }

    public void SetCardName(CardType _type)
    {
        Text text = gameObject.GetComponentInChildren<Text>();
        text.text = _type.ToString();
    }

    void HasResources()
    {
        Vector2Int cost = deck.cardCosts[(int)type];
        if (cost.x < player_resources.HR_curr && cost.y < player_resources.MR_curr)
        {
            has_resources = true;
            image.color = Color.white;
            //Debug.Log("Card playable!");
        }
        else {
            has_resources = false;
            image.color = Color.grey;
        }
    }
    
}
