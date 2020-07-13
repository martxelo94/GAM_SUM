using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using TMPro;
using UnityEngine.EventSystems;

[RequireComponent(typeof(EventTrigger))]
[RequireComponent(typeof(CardImage))]
public class DeckCard : MonoBehaviour
{
    public EventTrigger eventTrigger;
    public CardImage cardImage;
    public DeckManager manager;
    public bool in_pool = true; // is this card in pool or deck?
    [HideInInspector]
    public bool changed_panel = false;
    public TextMeshProUGUI count_text; // how many cards of this type in pool
    public GameObject count_text_frame;
    public GameObject frame_highlight;

    private bool is_selected = false;

    // Start is called before the first frame update
    void Start()
    {
        Assert.IsTrue(frame_highlight != null);
        Assert.IsTrue(count_text != null);
        Assert.IsTrue(count_text_frame != null);
    }


    public void PointerDown()
    {
        Assert.IsTrue(manager != null);
        //if (GameSettings.IsPointerOverUIObject())
        //    return;

        is_selected = true;
        // select card and move from pool to deck and viceversa
        manager.SelectCard(this);

    }

    public void PointerUp()
    {
        Assert.IsTrue(manager != null);
        // if card selected, add to deck or pool
        if (is_selected)
        {
            Assert.IsTrue(manager.selected_card == this);
            manager.UnselectCard();
        }
    }

    public void UpdateText(int count)
    {
        Assert.IsTrue(cardImage.type != CardType.None);
        if (in_pool)
        {
            count_text_frame.SetActive(true);
            count = Mathf.Max(count, 0);
            count_text.text = count.ToString();
            if (count == 0)
            {
                // non selectable
                Enable(false);
            }
            else {
                Enable(true);
            }
        }
        else {
            count_text_frame.gameObject.SetActive(false);
        }
    }

    public void Enable(bool enable)
    {
        eventTrigger.enabled = enable;
        cardImage.image.color = enable ? Color.white : Color.grey;
    }

}
