using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using TMPro;

[RequireComponent(typeof(CardImage))]
[RequireComponent(typeof(BoxCollider2D))]
public class DeckCard : MonoBehaviour
{
    public CardImage image;
    public DeckManager manager;
    public bool in_pool = true; // is this card in pool or deck?
    [HideInInspector]
    public bool changed_panel = false;
    public TextMeshProUGUI count_text; // how many cards of this type in pool
    public GameObject frame_highlight;

    // Start is called before the first frame update
    void Start()
    {
        Assert.IsTrue(manager != null);
        Assert.IsTrue(frame_highlight != null);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        // select card and move from pool to deck and viceversa
        manager.SelectCard(this);

    }

    private void OnMouseUp()
    {
        // if card selected, add to deck or pool
        Assert.IsTrue(manager.selected_card == this);
        manager.UnselectCard();

    }

    public void UpdateText(CardTypeCount[] pool)
    {
        Assert.IsTrue(image.type != CardType.None);
        if (in_pool)
        {
            count_text.gameObject.SetActive(true);
            count_text.text = pool[(int)image.type].count.ToString();
        }
        else {
            count_text.gameObject.SetActive(false);
        }
    }


}
