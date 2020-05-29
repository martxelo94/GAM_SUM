using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

[RequireComponent(typeof(CardImage))]
public class CardPlayable : MonoBehaviour
{
    CardImage card;
    private PlayerHand playerHand;

    public Text HR_text;
    public Text MR_text;

    public Deck deck;
    public Battlefield battlefield;
    PlayerResources player_resources;

    Vector2 initPos;
    Vector3 initScale;

    private bool has_resources = false;
    private bool can_spawn = false;
    private bool has_selected = false;

    private void Awake()
    {
        Assert.IsTrue(battlefield != null);
        card = GetComponent<CardImage>();
        playerHand = FindObjectOfType<PlayerHand>();
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
        //if (has_selected)
        {
            HasResources();
            if (has_selected)
            {
                if (IsTouchDrag())
                {
                    //Debug.Log(name + " dragging but Selected=" + has_selected.ToString());
                    if (!has_selected)
                        return;
                    //Debug.Log(name + " dragging.");
                    
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
                            SetType(deck.DrawCard());

                            // confirm spawn
                            deck.PlaySelected();

                            // reset the selection
                            ToggleSelect();
                        }
                    }

                }
            }
        }

    }
    private void OnMouseDown()
    {
        //if (has_exited == false)
        //    return;
        //if(IsTouchDown())
            ToggleSelect();
    }
#if false
    private void OnMouseDrag()
    {
        if (!has_selected || !has_resources)
            return;
        Vector2Int coord = battlefield.GetCellCoordAtTouch();
        // show unit on battlefield
        if (battlefield.IsInsideGrid(coord))
        {
            ConfirmSpawn(coord);
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
            //Vector3 pos = Input.mousePosition - new Vector3(Screen.width, Screen.height, 0) / 2;
            //SetCardPos(pos);
            // set selected transform card
            SetCardPos(initPos);
            transform.localScale = initScale * 1.2f;
        }
    }

    private void OnMouseEnter()
    {
        //Debug.Log("Mouse Enter " + name);
    }

    private void OnMouseExit()
    {
        has_exited = true;
    }

    private void OnMouseUp()
    {
        //Debug.Log("Mouse Up " + name);
        //if (has_exited == false)
        //    return;
        if (has_exited == false)
            return;
        has_exited = true;
        SetCardInitTransform();

        if (deck.HasSelected() == false)
            return;
        if (!has_resources)
        {
            ShowCard(false);
        }
        if (!can_spawn)
            return;
        // Vector2Int coord = battlefield.GetCellCoordAtTouch();
        // show unit on battlefield
        //if (battlefield.IsInsideGrid(coord))
        {
            PlaySpawned();

        }

    }


        void PlaySpawned()
    {
        // consume resources
        player_resources.ConsumeResources(deck.cm.cards[(int)card.type].cost);

        // make visible again
        ShowCard(true);

        // randomize next type
        SetType(deck.DrawCard());

        // confirm spawn
        deck.PlaySelected();

        // reset the selection
        has_selected = false;
    }

    bool ConfirmSpawn(Vector2Int coord)
    {
        has_exited = true;
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

        return can_spawn;
    }
#endif

    public void SetCardInitTransform()
    {
        transform.localScale = initScale;
        transform.localPosition = initPos;
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
        Vector2Int cost = deck.cm.cards[(int)_type].cost;
        HR_text.text = cost.x.ToString();
        MR_text.text = cost.y.ToString();
    }

    void SetCardPos(Vector3 pos)
    {
        transform.localPosition = pos;
        transform.localPosition += playerHand.cardSelectedPivot;
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
        ShowCard(true);
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
