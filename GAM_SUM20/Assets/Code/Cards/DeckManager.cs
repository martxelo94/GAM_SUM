using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.UI;

public class DeckManager : MonoBehaviour
{
    public int current_deck_index = 0;
    private bool current_deck_saved = false;
    public RectTransform current_deck_panel;
    public RectTransform card_pool_panel;
    public CardTypeCount[] card_pool;   // this is saved in the player account
    public DeckCard card_prefab;
    private DeckCard[] card_pool_objects;   // auto generated
    private List<DeckCard> card_deck_objects;   // auto generated from input deck
    const string FILE_PATH_POOL = "PlayerCardPool";
    public static string FILE_PATH_DECK = "PlayerCardDeck";
    private Camera mainCamera;

    public RectTransform deck_highlight;
    public RectTransform pool_highlight;

    // switch off scrolls when dragging cards
    public ScrollRect deck_scroll;
    public ScrollRect pool_scroll;
    public ScrollRect menu_scroll;  // horizontal scroll


    [HideInInspector]
    public DeckCard selected_card = null;

    private void Awake()
    {
        mainCamera = Camera.main;

        Assert.IsTrue(pool_scroll != null);
        Assert.IsTrue(deck_scroll != null);
        Assert.IsTrue(deck_highlight != null);
        Assert.IsTrue(pool_highlight != null);
    }

    // Start is called before the first frame update
    void Start()
    {
        card_deck_objects = new List<DeckCard>();
        if (LoadPool() && LoadDeck())
        {
            Debug.Log(FILE_PATH_POOL + " loaded.");
        }
        if(card_pool.Length != (int)CardType.CardType_Count)
        {
            // initialize default pool
            card_pool = CardTypeCount.Initialize();

            Debug.Log(FILE_PATH_POOL + " failed loading!");
        }
        Assert.IsTrue(card_pool.Length == (int)CardType.CardType_Count);

        // set up pool
        card_pool_objects = new DeckCard[(int)CardType.CardType_Count];
        for (int i = 0; i < (int)CardType.CardType_Count; ++i) {
            DeckCard c = Instantiate(card_prefab, card_pool_panel) as DeckCard;
            c.manager = this;
            c.in_pool = true;
            c.cardImage.type = (CardType)i;
            c.UpdateText(card_pool[(int)i].count);

            card_pool_objects[i] = c;
        }
    }

    private void OnDestroy()
    {
        SavePool();
        if(current_deck_saved == false)
            SaveDeck();
    }

    // Update is called once per frame
    void Update()
    {
        if (selected_card != null) {
            // move card
            // MoveSelectedCard(); // not working
            // check if changes area
            bool prev_frame_changed = selected_card.changed_panel;
            bool curr_frame_changed = HasChangedPanel();
            if (prev_frame_changed != curr_frame_changed)
            {
                // highlight
                if (selected_card.in_pool)
                {
                    deck_highlight.gameObject.SetActive(curr_frame_changed);
                    pool_highlight.gameObject.SetActive(!curr_frame_changed);
                }
                else {
                    deck_highlight.gameObject.SetActive(!curr_frame_changed);
                    pool_highlight.gameObject.SetActive(curr_frame_changed);

                }
            }
        }    
    }

    public void AddRawDeckToPool(List<CardType> rawDeck)
    {
        foreach (CardType t in rawDeck)
        {
            card_pool[(int)t].count++;
            card_pool_objects[(int)t].UpdateText(card_pool[(int)t].count);
        }
    }

    public void SelectCard(DeckCard card)
    {
        selected_card = card;
        selected_card.frame_highlight.SetActive(true);
        deck_scroll.enabled = false;
        pool_scroll.enabled = false;
        if (menu_scroll != null)
            menu_scroll.enabled = false;
    }

    public void UnselectCard()
    {
        selected_card.frame_highlight.SetActive(false);
        // pass card to other panel if changed
        if (selected_card.changed_panel)
        {
            if (selected_card.in_pool)
            {
                PoolToDeck(selected_card);
            }
            else {
                DeckToPool(selected_card);
            }
            current_deck_saved = false;
            selected_card.changed_panel = false;
        }
        deck_highlight.gameObject.SetActive(false);
        pool_highlight.gameObject.SetActive(false);
        selected_card = null;

        deck_scroll.enabled = true;
        pool_scroll.enabled = true;
        if (menu_scroll != null)
            menu_scroll.enabled = true;

    }

    public void DeckToPool(DeckCard card)
    {
        CardType type = card.cardImage.type;
        card_pool[(int)type].count++;
        card_pool_objects[(int)type].UpdateText(card_pool[(int)type].count);

        card_deck_objects.Remove(card);
        Destroy(card.gameObject);
    }

    public void PoolToDeck(DeckCard card)
    {
        CardType type = card.cardImage.type;
        card_pool[(int)type].count--;
        card_pool_objects[(int)type].UpdateText(card_pool[(int)type].count);

        DeckCard copy = Instantiate(card, current_deck_panel) as DeckCard;
        copy.in_pool = false;
        copy.count_text_frame.SetActive(false);
        copy.frame_highlight.SetActive(false);

        card_deck_objects.Add(copy);

    }

    public void SetUpDeck(Deck deck)
    {
        Assert.IsTrue(card_deck_objects != null && card_deck_objects.Count == 0);
        // copy deck data to deckCards
        CardTypeCount[] deck_to_copy = deck.GetDeck();
        for (int i = 0; i < (int)CardType.CardType_Count; ++i) {
            CardType type = deck_to_copy[i].type;
            for (int j = 0; j < deck_to_copy[i].count; ++j) {
                DeckCard dc = Instantiate(card_prefab, current_deck_panel) as DeckCard;
                dc.manager = this;
                dc.cardImage.type = type;
                dc.count_text_frame.SetActive(false);
                dc.in_pool = false;
                card_deck_objects.Add(dc);

            }
        }
    }

    void SetUpDeck(List<CardType> deck)
    {
        Assert.IsTrue(card_deck_objects != null && card_deck_objects.Count == 0);
        foreach (CardType t in deck) {
            DeckCard dc = Instantiate(card_prefab, current_deck_panel) as DeckCard;
            dc.manager = this;
            dc.cardImage.type = t;
            dc.count_text_frame.SetActive(false);
            dc.in_pool = false;

            card_deck_objects.Add(dc);

        }
    }

    public void RemoveCurrentDeck()
    {
        foreach(DeckCard dc in card_deck_objects)
        {
            Destroy(dc.gameObject);
        }
        card_deck_objects = new List<DeckCard>();
    }

    public List<CardType> GetDeckUncollapsed()
    {
        List<CardType> deck = new List<CardType>();
        foreach (DeckCard dc in card_deck_objects)
            deck.Add(dc.cardImage.type);
        return deck;
    }

    public CardTypeCount[] CollapseDeck(List<CardType> raw_deck)
    {
        CardTypeCount[] deck = CardTypeCount.Initialize();
        if (raw_deck == null)
            return deck;    // return initialized empty deck

        foreach (CardType t in raw_deck) {
            deck[(int)t].count++;
        }

        return deck;
    }

    void SavePool()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(FILE_PATH_POOL, FileMode.Create);

        formatter.Serialize(stream, card_pool);
        stream.Close();
    }

    public void SaveDeck()
    {
        string path = FILE_PATH_DECK + current_deck_index.ToString();
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);

        List<CardType> deck = GetDeckUncollapsed();
        formatter.Serialize(stream, deck);
        
        stream.Close();

        current_deck_saved = true;

        Debug.Log(path + " saved.");
    }

    bool LoadPool()
    {
        if (File.Exists(FILE_PATH_POOL)) {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(FILE_PATH_POOL, FileMode.Open);

            card_pool = formatter.Deserialize(stream) as CardTypeCount[];

            stream.Close();

            return true;
        }
        return false;
    }

    public List<CardType> LoadDeckRaw(int index)
    {
        List<CardType> deck = new List<CardType>();
        string path = FILE_PATH_DECK + index.ToString();
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

           deck = formatter.Deserialize(stream) as List<CardType>;

            stream.Close();
        }
        return deck;
    }

    bool LoadDeck()
    {
        string path = FILE_PATH_DECK + current_deck_index.ToString();
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            List<CardType> deck = formatter.Deserialize(stream) as List<CardType>;
            RemoveCurrentDeck();
            SetUpDeck(deck);
            stream.Close();

            current_deck_saved = false;

            return true;
        }
        return false;
    }

    // used with GUI buttons
    public void LoadDeckImmediate()
    {
        LoadDeck();
    }

    void MoveSelectedCard()
    {
#if UNITY_EDITOR
        Vector3 screenPos = Input.mousePosition;
#else
        Vector3 screenPos = Input.touches[0].position;
#endif
        Vector3 pos = mainCamera.ScreenToWorldPoint(screenPos);

        selected_card.transform.position = pos;

    }

    public bool HasChangedPanel()
    {
        RectTransform tr = selected_card.in_pool ? deck_highlight : pool_highlight;
#if UNITY_EDITOR
        Vector3 screenPos = Input.mousePosition;
#else
        Vector3 screenPos = Input.touches[0].position;
#endif
        bool result = RectTransformUtility.RectangleContainsScreenPoint(tr, screenPos, mainCamera);
        
        return selected_card.changed_panel = result;
    }

    public void SetCurrentDeckIndex(int index) => current_deck_index = index;
}
