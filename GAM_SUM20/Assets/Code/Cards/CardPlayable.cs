using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

[RequireComponent(typeof(CardImage))]
public class CardPlayable : MonoBehaviour
{
    public CardImage card;
    public PlayerHand hand; // set from hand

    public Text HR_text;
    public Text MR_text;


    public Vector3 initPos { get; private set; }
    public Vector3 initScale { get; private set; }

    private bool has_resources = false;
    private bool can_spawn = false;
    private bool has_selected = false;

//#if UNITY_EDITOR
    private Camera mainCamera;
    private int touchIndex = 0;

    private void Awake()
    {
        mainCamera = Camera.main;
        Assert.IsTrue(card != null);

    }

    // Start is called before the first frame update
    void Start()
    {
        initPos = transform.position;
        //initPos = transform.InverseTransformPoint(initPos);
        //initPos = new Vector3(initPos.z, initPos.y, 0f);
        //initPos = transform.TransformPoint(initPos);

        initScale = transform.localScale;

    }

    // Update is called once per frame
    void Update()
    {
        if (hand.GetDeck().cards_to_play_count > 0)
        {
            if (card.type != CardType.None)
            {
                HasResources();
            }
            else
            {
                return;
            }
            if (has_selected && touchIndex > -1)
            {
                if (IsTouchDown() || IsTouchDrag())
                {                    
                    Vector2Int coord = hand.battlefield.GetCellCoordAtTouch();
                    // show unit on battlefield
                    if (hand.battlefield.IsInsideGrid(coord))
                    {
                        // spawn unit
                        if (hand.GetDeck().HasSelected() == false)
                        {
                            hand.GetDeck().SelectType(card.type);
                            // set scale
                            hand.GetDeck().selected_transform.localScale = 
                                new Vector3(hand.battlefield.cell_size, hand.battlefield.cell_size, hand.battlefield.cell_size);
                            ShowCard(false);
                        }
                        can_spawn = hand.battlefield.SnapToCaptured(ref coord, hand.GetDeck().team) ? true : false;
                        // move unit
                        hand.GetDeck().selected_transform.position = hand.battlefield.GetCellPos(coord);

                        //Debug.Log(name + " inside grid.");

                    }
                    // show card
                    else
                    {
                        can_spawn = false;
                        // remove spawned unit
                        if (hand.GetDeck().HasSelected())
                        {
                            hand.GetDeck().Unselect();
                            ShowCard(true);
                        }
                        // move card
                        Vector3 pos = Input.mousePosition - new Vector3(Screen.width, Screen.height, 0) / 2;
                        SetCardPos(pos);
                        // set selected transform card
                        //SetCardPos(initPos);
                        transform.localScale = initScale * hand.cardSelectedScale;

                        //Debug.Log(name + " outside grid.");
                    }
                }
                else if (IsTouchUp())
                {
                    //touchIndex = -1;
                    SetCardPos(initPos);
                    if (can_spawn)
                    {
                        transform.localScale = initScale;

                        if (hand.GetDeck().HasSelected() == false)
                            return;
                        if (!has_resources)
                        {
                            // delete blueprint
                            if(hand.GetDeck().HasSelected())
                                hand.GetDeck().Unselect();
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
                            hand.GetResources().ConsumeResources(hand.GetDeck().cm.cards[(int)card.type].cost);

                            // make visible again
                            ShowCard(true);

                            // randomize next type
                            hand.DrawCardAnimation(this);

                            // confirm spawn
                            GameObject squadObj = hand.GetDeck().PlaySelected();
                            Squad squad = squadObj.GetComponent<Squad>();
                            // squad stuff
                            squad.team = hand.GetDeck().team;

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
        touchIndex = GetTouchIndex();
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
            hand.UnselectCards();
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
            Vector2Int cost = hand.GetDeck().cm.cards[(int)_type].cost;
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
        transform.position += hand.cardSelectedPivot;
    }

    public void Select()
    {
        has_selected = true;
        transform.localScale = initScale * hand.cardSelectedScale;
    }
    public void Unselect()
    {
        has_selected = false;
        SetCardInitTransform();
        //ShowCard(true);
    }

    bool HasResources()
    {
        Vector2Int cost = hand.GetDeck().cm.cards[(int)card.type].cost;
        if (cost.x < hand.GetResources().HR_curr && cost.y < hand.GetResources().MR_curr)
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

    int GetTouchIndex()
    {
#if UNITY_EDITOR

        return 0;
#else
        Touch[] touches = Input.touches;
        int closest = -1;
        float bestDist = Mathf.Infinity;
        for (int i = 0; i < touches.Length; ++i)
        {
            Vector2 screenPos = mainCamera.WorldToScreenPoint(transform.position);
            float dist2 = (screenPos - touches[i].position).sqrMagnitude;
            if (dist2 < bestDist)
            {
                bestDist = dist2;
                closest = i;
            }
        }
        return closest;
#endif
    }

    bool IsTouchDown()
    {
#if UNITY_EDITOR
        return Input.GetMouseButtonDown(0);
#else
        return Input.touchCount > 0 && Input.GetTouch(touchIndex).phase == TouchPhase.Began;
#endif
    }

    bool IsTouchDrag()
    {
#if UNITY_EDITOR
        return Input.GetMouseButton(0);
#else
        return Input.touchCount > 0 && Input.GetTouch(touchIndex).phase == TouchPhase.Moved;
#endif
    }

    bool IsTouchUp()
    {
#if UNITY_EDITOR
        return Input.GetMouseButtonUp(0);
#else
        return Input.touchCount > 0 && Input.GetTouch(touchIndex).phase == TouchPhase.Ended;
#endif
    }
}
