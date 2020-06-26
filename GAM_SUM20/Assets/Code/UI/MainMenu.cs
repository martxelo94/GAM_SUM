using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Assertions;

public class MainMenu : MonoBehaviour
{
    public int aiGameSceneIndex = 2;
    public int playerGameSceneIndex = 3;
    public int campaignSceneIndex = 5;

    public DeckManager deckManager;

    // Start is called before the first frame update
    void Start()
    {
        deckManager = GetComponent<DeckManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S)) {
            // save current open deck
            deckManager.SaveDeck();
        }
    }

    public void DeleteAllSavedData()
    {
        GameSettings.INSTANCE.DeleteAllSavedData();
    }

    public void LoadLevelScene(int nextLevel)
    {
        GameSettings.INSTANCE.SetNextSceneIdx(nextLevel);
        // load loading scene
        SceneManager.LoadScene("Scenes/LoadingScreen");

    }

    public void StartAIGame()
    {
        SetQuickGameDecks();
        GameSettings.INSTANCE.is_campaign_battle = false;
        LoadLevelScene(aiGameSceneIndex);
    }
    public void StartPlayerGame()
    {
        SetQuickGameDecks();
        GameSettings.INSTANCE.is_campaign_battle = false;
        LoadLevelScene(playerGameSceneIndex);
    }
    public void StartCampaign(int idx)
    {
        GameSettings.INSTANCE.is_campaign_battle = true;
        LoadLevelScene(campaignSceneIndex + idx);
    }

    void SetQuickGameDecks()
    {
        Assert.IsTrue(deckManager != null);
        // save current editing deck
        deckManager.SaveDeck();

        // set decks on GameSettings static data
        GameSettings.INSTANCE.SetAttackDeck(deckManager.CollapseDeck(deckManager.LoadDeckRaw(0)));
        // pray to have a save...
        GameSettings.INSTANCE.SetTargetDeck(deckManager.CollapseDeck(deckManager.LoadDeckRaw(1)));
    }
}
