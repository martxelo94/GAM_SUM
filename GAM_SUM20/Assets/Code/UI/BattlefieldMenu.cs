using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Text;

public class BattlefieldMenu : MonoBehaviour
{
    public Text fps_text;
    // update every second
    float fps_update_counter = 0;

    public GameObject endGamePanel;
    public GameObject victoryPanel;
    public GameObject defeatPanel;

    public Deck[] decks;
    public DealPlayerDamage[] playerHitPoints;

    public int nextLevel = 2;
    private bool game_ended = false;

    private void Awake()
    {
        game_ended = false;
        Assert.IsTrue(decks.Length == 2);
        Assert.IsTrue(playerHitPoints != null);
            // set decks from campaign
        if (GameSettings.INSTANCE.IsBattle())
        {
            Assert.IsTrue(decks.Length == 2);
            for (int i = 0; i < decks.Length; ++i)
            {
                Deck d = decks[i];
                if (d.team == TeamType.Player)
                {
                    d.SetDeck(GameSettings.INSTANCE.attack_deck);
                }
                else
                {
                    d.SetDeck(GameSettings.INSTANCE.target_deck);
                }
            }
            Debug.Log("Decks set from Campaign");
        }
    }

    // Update is called once per frame
    void Update()
    {
        fps_update_counter += Time.deltaTime;
        if (fps_update_counter > 1.0f) {
            DrawFPS(fps_text);
            fps_update_counter = 0.0f;
        }

        if ((decks[0].cards_to_play_count == 0)
                    && DealPlayerDamage.totalTroopCount == 0)
        {
            // check result
            playerHitPoints[0].EndGamePanelByHitPoints();
        }
    }

    public void ResetLevel()
    {

        Time.timeScale = 1.0f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ChangeTimeScale(float time_scale)
    {
        Time.timeScale = time_scale;
    }

    public void ToggleActive(GameObject obj)
    {
        obj.SetActive(!obj.activeSelf);
    }

    public void ToggleSound(AudioSource source)
    {
        source.mute = !source.mute;
    }


    public void DrawFPS(Text text)
    {
        int frameRate = (int)(1.0f / Time.deltaTime);
        StringBuilder t = new StringBuilder();
        t.Append("FPS:").Append(frameRate.ToString());
        text.text = t.ToString();
    }

    public void ShowEndGamePanel(bool is_victory)
    {
        if (game_ended)
            return;
        game_ended = true;
        // stop time
        Time.timeScale = 0.3f;

        DisablePlayers();

        endGamePanel.SetActive(true);
        if (is_victory)
        {
            defeatPanel.SetActive(false);
            victoryPanel.SetActive(true);
            GameSettings.INSTANCE.last_battle_won = true;
            Debug.Log("VICTORY!");
        }
        else {
            victoryPanel.SetActive(false);
            defeatPanel.SetActive(true);
            GameSettings.INSTANCE.last_battle_won = false;
            Debug.Log("DEFEAT!");
        }
    }

    public void LoadNextLevel()
    {
        if (GameSettings.INSTANCE.IsBattle() == false)
        {
            // QuickGame MODE
            ReplayLevel();
            return;
        }
        else
            UpdateCampaignFile();

        // previously set
        Time.timeScale = 1.0f;
        GameSettings.INSTANCE.nextSceneIdx = nextLevel;

        // load loading scene
        SceneManager.LoadScene("Scenes/LoadingScreen");
    }

    public void ReplayLevel()
    {
        Time.timeScale = 1.0f;
        GameSettings.INSTANCE.nextSceneIdx = SceneManager.GetActiveScene().buildIndex;

        // load loading scene
        SceneManager.LoadScene("Scenes/LoadingScreen");
    }

    public void LoadMainMenu()
    {
        if (GameSettings.INSTANCE.IsBattle()) {
            UpdateCampaignFile();
        }
        Time.timeScale = 1.0f;
        GameSettings.INSTANCE.nextSceneIdx = 0;

        SceneManager.LoadScene("Scenes/MainMenu");
    }

    void UpdateCampaignFile()
    {
        // update decks
        Deck[] decks = FindObjectsOfType<Deck>();
        Assert.IsTrue(decks.Length == 2);
        for(int i = 0; i < decks.Length; ++i)
        {
            Deck d = decks[i];
            if (d.team == TeamType.Player)
            {
                GameSettings.INSTANCE.SetAttackDeck(d.deck_types);
            }
            else
            {
                GameSettings.INSTANCE.SetTargetDeck(d.deck_types);
            }
        }
        MapCampaign.UpdateFile();
    }

    public void LevelWin()
    {
        GameSettings.INSTANCE.last_battle_won = true;
        LoadNextLevel();
    }

    public void LevelLose()
    {
        GameSettings.INSTANCE.last_battle_won = false;

        LoadNextLevel();
    }

    public void CheatWin()
    {
        BattlefieldMenu menu = FindObjectOfType<BattlefieldMenu>();
        menu.ShowEndGamePanel(true);
    }

    public void CheatLose()
    {
        BattlefieldMenu menu = FindObjectOfType<BattlefieldMenu>();
        menu.ShowEndGamePanel(false);
    }

    void DisablePlayers()
    {
        OpponentAI ai = FindObjectOfType<OpponentAI>();
        // add cards not played
        for (int i = 0; i < ai.hand_types.Length; ++i)
        {
            if(ai.hand_types[i] != CardType.None)
                ai.deck.AddToDeck(ai.hand_types[i], 1);
        }
        ai.enabled = false;
        PlayerHand hand = FindObjectOfType<PlayerHand>();
        CardPlayable[] cards = hand.cards;
        for (int i = 0; i < cards.Length; ++i) {
            // add card to deck for saving
            if (cards[i].card.type != CardType.None)
            {
                cards[i].deck.AddToDeck(cards[i].card.type, 1);
            }
            cards[i].enabled = false;
        }
    }
}

