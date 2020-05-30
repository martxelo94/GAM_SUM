using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

[RequireComponent(typeof(CardImage))]
public class CardPlayable : MonoBehaviour
{
    public CardImage card;
    private PlayerHand playerHand;

    public Text HR_text;
    public Text MR_text;

    public Deck deck;
    public Battlefield battlefield;
    PlayerResources player_resources;

    public Vector3 initPos { get; private set; }
    public Vector3 initScale { get; private set; }

    private bool has_resources = false;
    private bool can_spawn = false;
    private bool has_selected = false;

    private void Awake()
    {
        Assert.IsTrue(battlefield != null);
        Assert.IsTrue(card != null);
        playerHand = FindObjectOfType<PlayerHand>();
        Assert.IsTrue(deck != null);
        player_resources = deck.gameObject.GetComponent<PlayerResources>();

        initPos = transform.position;
        initPos = transform.InverseTransformPoint(initPos);
        initPos = new Vector3(initPos.z, initPos.y, 0f);
        initPos = transform.TransformPoint(initPos);

        initScale = transform.localScale;

    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (deck.cards_to_play_count > 0)
        {
            if (has_selected)
            {
                if (IsTouchDrag())
                {                    
                    Vector2Int coord = battlefield.GetCellCoordAtTouch();
                    // show unit on battlefield
                    if (battlefield.IsInsideGrid(coord))
                    {
                        // spawn unit
                        if (deck.HasSelected() == false)
                        {
                            deck.SelectType(card.type);
                            // set scale
                            deck.selected_transform.localScale = new Vector3(battlefield.cell_size, battlefield.cell_size, battlefield.cell_size);
                            ShowCard(false);
                        }
                        can_spawn = battlefield.SnapToCaptured(ref coord, TeamType.Player) ? true : false;
                        // move unit
                        deck.selected_transform.position = battlefield.GetCellPos(coord);

                        //Debug.Log(name + " inside grid.");

                    }
                    // show card
                    else
                    {
                        can_spawn = false;
                        // remove spawned unit
                        if (deck.HasSelected())
                        {
                            deck.Unselect();
                            ShowCard(true);
                        }
                        // move card
                        Vector3 pos = Input.mousePosition - new Vector3(Screen.width, Screen.height, 0) / 2;
                        SetCardPos(pos);
                        // set selected transform card
                        //SetCardPos(initPos);
                        transform.localScale = initScale * playerHand.cardSelectedScale;

                        //Debug.Log(name + " outside grid.");
                    }
                }
                else if (IsTouchUp())
                {
                    SetCardPos(initPos);
                    if (can_spawn)
                    {
                        transform.localScale = initScale;

                        if (deck.HasSelected() == false)
                            return;
                        if (!has_resources)
                        {
                            // delete blueprint
                            if(deck.HasSelected())
                                deck.Unselect();
                            ShowCard(true);
                            return;
                        }
                        //if (!can_spawn)
                        //    return;
                        // Vector2Int coord = battlefield.GetCellCoordAtTouch();
                        // show unit on battlefield
                        //if (battlefield.IsInsideGrid(coord))
                        {
                            // consume resources
                            player_resources.ConsumeResources(deck.cm.cards[(int)card.type].cost);

                            // make visible again
                            ShowCard(true);

                            // randomize next type
                            playerHand.DrawCardAnimation(this);

                            // confirm spawn
                            deck.PlaySelected();

                            // reset the selection
                            ToggleSelect(); // unselect
                        }
                    }

                }
            }
        }

    }
    private void OnMouseDown()
    {
        if(card.type != CardType.None)
            ToggleSelect();
    }

    public void SetCardInitTransform()
    {
        transform.localScale = initScale;
        transform.position = initPos;
    }
    void ToggleSelect()
    {
        if (has_selected == false)
        {
            playerHand.UnselectCards();
            Select();
            //Debug.Log(name + " selected.");
        }
        else {
            Unselect();
            //Debug.Log(name + " unselected.");
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
        if (_type != CardType.None)
        {
            HasResources();
            Vector2Int cost = deck.cm.cards[(int)_type].cost;
            ShowCard(true);
            HR_text.text = cost.x.ToString();
            MR_text.text = cost.y.ToString();
        }
        else {
            ShowCard(false);
        }
    }

    void SetCardPos(Vector3 pos)
    {
        transform.position = pos;
        transform.position += playerHand.cardSelectedPivot;
    }

    public void Select()
    {
        has_selected = true;
        transform.localScale = initScale * playerHand.cardSelectedScale;
    }
    public void Unselect()
    {
        has_selected = false;
        SetCardInitTransform();
        //ShowCard(true);
    }

    bool HasResources()
    {
        Vector2Int cost = deck.cm.cards[(int)card.type].cost;
        if (cost.x < player_resources.HR_curr && cost.y < player_resources.MR_curr)
        {
            has_resources = true;
            card.image.color = Color.white;
            //Debug.Log("Card playable!");
            return true;
        }
        else
        {
            has_resources = false;
            card.image.color = Color.grey;
            return false;
        }
    }


    bool IsTouchDown()
    {
#if UNITY_EDITOR
        return Input.GetMouseButtonDown(0);
#else
        return Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began;
#endif
    }

    bool IsTouchDrag()
    {
#if UNITY_EDITOR
        return Input.GetMouseButton(0);
#else
        return Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved;
#endif
    }

    bool IsTouchUp()
    {
#if UNITY_EDITOR
        return Input.GetMouseButtonUp(0);
#else
        return Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended;
#endif
    }
}
