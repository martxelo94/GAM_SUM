using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpponentAI : MonoBehaviour
{

    public Deck deck;
    public PlayerResources m_resources;

    public float reaction_time = 0.5f;
    private float reaction_time_counter = 0.0f;

    CardType[] hand_types;
    int hand_size = 5;
    int card_to_play = -1;


    // Start is called before the first frame update
    void Start()
    {
        hand_types = new CardType[hand_size];
        for (int i = 0; i < hand_size; i++)
            hand_types[i] = deck.DrawCard();
    }

    // Update is called once per frame
    void Update()
    {
        float dt = Time.deltaTime * Time.timeScale;
        reaction_time_counter += dt;
        if (reaction_time_counter > reaction_time) {

            // do stuff

            // select random on hand
            if (card_to_play < 0) {
                card_to_play = Random.Range(0, hand_size);
            }
            CardType card_type = hand_types[card_to_play];
            // confirm spawn (a frame later to ensure initialization)
            if (deck.selected_card != null)
            {
                // confirm type
                GameObject squadObj = deck.PlaySelected();
                //squadObj.transform.Rotate(new Vector3(0, 0, 180));
                Squad squad = squadObj.GetComponent<Squad>();
                // squad stuff
                squad.team = TeamType.Opponent;
                // consume resources
                m_resources.ConsumeResources(deck.cardCosts[(int)card_type]);

                // draw new card
                hand_types[card_to_play] = deck.DrawCard();
                card_to_play = -1;
                reaction_time_counter = 0.0f;
                return;
            }
            Vector2Int cost = deck.cardCosts[(int)card_type];
            if (cost.x < m_resources.HR_curr && cost.y < m_resources.MR_curr)
            {
                deck.SelectType(card_type);
                // randomize position
                int randX = Random.Range(0, deck.battlefield.grid_size.x);
                Vector2Int coord = new Vector2Int(randX, 2);
                deck.battlefield.SnapToCaptured(ref coord, TeamType.Opponent);
                deck.selected_card.transform.position = deck.battlefield.GetCellPos(coord);
                //rotate towards down
                deck.selected_card.transform.Rotate(new Vector3(0, 0, 180));

            }
           
        }
    }
}
