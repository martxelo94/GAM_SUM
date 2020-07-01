using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using System.IO;

public class GameSettings : MonoBehaviour
{
    // SINGLETON
    private static GameSettings _instance;
    public static GameSettings INSTANCE {
        get {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<GameSettings>();
            }
            return _instance;
        }
        private set { _instance = value; }
    }

    public System.Random randomizer = new System.Random();   // to shuffle deck

    public int prevSceneIdx { get; private set; } = -1;
    public int nextSceneIdx { get; private set; } = -1;

    public bool is_campaign_battle;

    private CardTypeCount[] attack_deck = null;    // player deck
    public int attack_idx { get; private set; } = -1;
    private CardTypeCount[] target_deck = null;    // opponent deck
    public int target_idx { get; private set; } = -1;
    public bool last_battle_won = false;
    public string campaign_battle_name;
    public string tuto_campaign_savename = "CampaignTip";
    public string tuto_battle_savename = "BattleTip";


    public CardTypeCount[] CopyAttackDeck()
    {
        CardTypeCount[] copy = new CardTypeCount[attack_deck.Length];
        attack_deck.CopyTo(copy, 0);
        return copy;
    }
    public CardTypeCount[] CopyTargetDeck()
    {
        CardTypeCount[] copy = new CardTypeCount[target_deck.Length];
        target_deck.CopyTo(copy, 0);
        return copy;
    }
    public void SetNextSceneIdx(int idx)
    {
        prevSceneIdx = nextSceneIdx;
        nextSceneIdx = idx;
    }

    public void SetAttackDeck(CardTypeCount[] deck)
    {
        attack_deck = deck;
    }
    public void SetTargetDeck(CardTypeCount[] deck)
    {
        target_deck = deck;
    }
    public void SetAttackDeck(CardTypeCount[] deck, int node_idx)
    {
        attack_deck = deck;
        attack_idx = node_idx;
    }
    public void SetTargetDeck(CardTypeCount[] deck, int node_idx)
    {
        target_deck = deck;
        target_idx = node_idx;
    }
    public void SetBattle(MapNode attaker, MapNode target)
    {
        Assert.IsTrue(attaker.army != null && target.army != null);
        SetAttackDeck(attaker.army.GetDeck(), attaker.node_idx);
        SetTargetDeck(target.army.GetDeck(), target.node_idx);

        is_campaign_battle = true;
        campaign_battle_name = SceneManager.GetActiveScene().name;
    }

    public void ResetBattle()
    {
        attack_deck = target_deck = null;
        attack_idx = target_idx = -1;
        campaign_battle_name = "";
    }

    public bool IsBattle()
    {
        return is_campaign_battle && attack_idx > -1;
    }

    public void DeleteAllSavedData()
    {
        PlayerPrefs.DeleteAll();
        //PlayerPrefs.DeleteKey(tuto_battle_savename);
        //PlayerPrefs.DeleteKey(tuto_campaign_savename);

        Debug.Log("Tutorial savedata deleted.");

        const int CAMPAIGN_COUNT = 2;
        string path = Application.persistentDataPath + "/Campaign_";
        for (int i = 0; i < CAMPAIGN_COUNT; ++i)
        {
            string tmp = path + i.ToString();
            File.Delete(tmp);
            Debug.Log(tmp + " file deleted.");
        }
        DeckManager deckManager = FindObjectOfType<DeckManager>();
        if (deckManager != null)
        {
            // delete decks
            for (int i = -2; i < CAMPAIGN_COUNT; ++i)
            {
                List<CardType> rawDeck = deckManager.LoadDeckRaw(i);
                deckManager.AddRawDeckToPool(rawDeck);
                string tmp = DeckManager.FILE_PATH_DECK + i.ToString();
                File.Delete(tmp);
                Debug.Log(tmp + " file deleted.");
            }
            deckManager.RemoveCurrentDeck();
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        Application.targetFrameRate = 30;

    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        // HACK - DELETE SAVED GAMES
        if (Input.GetKeyDown(KeyCode.Q))
        {
            DeleteAllSavedData();
        }
    }

    public void ToggleButton(UnityEngine.UI.Button button)
    {
        button.interactable = !button.interactable;
    }
}


