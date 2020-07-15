using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public int aiGameSceneIndex = 2;
    public int playerGameSceneIndex = 3;
    public int campaignSceneIndex = 5;

    public DeckManager deckManager;
    public GameObject invalidDeckWarningPanel;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.S)) {
            // save current open deck
            deckManager.SaveDeck();
        }
#endif
    }

    public void DeleteCampaignSavedData()
    {
        GameSettings.INSTANCE.DeleteCampaignSavedData();
    }
    public void DeleteDeckSavedData()
    {
        GameSettings.INSTANCE.DeleteDeckSavedData();
    }
    public void DeletePoolSavedData()
    {
        GameSettings.INSTANCE.DeletePoolSavedData();
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
        GameSettings.INSTANCE.SetAttackDeck(deckManager.CollapseDeck(deckManager.LoadDeckRaw(-1)));
        // pray to have a save...
        GameSettings.INSTANCE.SetTargetDeck(deckManager.CollapseDeck(deckManager.LoadDeckRaw(-2)));
    }

    public void CancelQuickGame()
    {
        deckManager.gameObject.SetActive(false);
        GameSettings.INSTANCE.ResetBattleDecks();
    }

    public void ConfirmQuickGame()
    {
        SetQuickGameDecks();
        // check if both decks have cards
        int playerDeckCount = Deck.CardCount(GameSettings.INSTANCE.attack_deck);
        int enemyDeckCount = Deck.CardCount(GameSettings.INSTANCE.target_deck);
        if (playerDeckCount == 0 || enemyDeckCount == 0) {
            // deck warning
            StartCoroutine(MakeInactiveAtTime(invalidDeckWarningPanel, 2f));
            return;
        }

        SceneManager.LoadScene("Scenes/LoadingScreen");
    }
    public void PrepareAIGame()
    {
        deckManager.gameObject.SetActive(true);
        GameSettings.INSTANCE.SetNextSceneIdx(aiGameSceneIndex);
    }
    public void PreparePlayerGame()
    {
        deckManager.gameObject.SetActive(true);
        GameSettings.INSTANCE.SetNextSceneIdx(playerGameSceneIndex);
    }

    IEnumerator MakeInactiveAtTime(GameObject obj, float seconds)
    {
        obj.SetActive(true);
        yield return new WaitForSecondsRealtime(seconds);
        obj.SetActive(false);
    }
}
