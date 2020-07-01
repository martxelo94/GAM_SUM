using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using TMPro;

[RequireComponent(typeof(CardImage))]
[RequireComponent(typeof(BoxCollider2D))]
public class DeckCard : MonoBehaviour
{
    private BoxCollider2D boxCollider;
    public CardImage cardImage;
    public DeckManager manager;
    public bool in_pool = true; // is this card in pool or deck?
    [HideInInspector]
    public bool changed_panel = false;
    public TextMeshProUGUI count_text; // how many cards of this type in pool
    public GameObject count_text_frame;
    public GameObject frame_highlight;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Assert.IsTrue(manager != null);
        Assert.IsTrue(frame_highlight != null);
        Assert.IsTrue(count_text != null);
        Assert.IsTrue(count_text_frame != null);
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
                boxCollider.enabled = false;
                cardImage.image.color = Color.grey;
            }
            else {
                boxCollider.enabled = true;
                cardImage.image.color = Color.white;
            }
        }
        else {
            count_text_frame.gameObject.SetActive(false);
        }
    }


}
