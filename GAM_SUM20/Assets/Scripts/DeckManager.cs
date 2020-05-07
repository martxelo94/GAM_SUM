using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DeckManager : MonoBehaviour
{

    public Image[] cards;
    int selected_card = -1;
    public float selection_scale = 1.1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UnselectCard(int idx)
    {
        selected_card = -1;
        cards[idx].transform.scale *= selection_scale;
    }

    public void SelectCard(int idx)
    {
        // unselect previous card
        if (selected_card > -1) {
            UnselectCard(selected_card);
        }
        selected_card = idx;
        cards[idx].transform.scale *= selection_scale;
    }
}
