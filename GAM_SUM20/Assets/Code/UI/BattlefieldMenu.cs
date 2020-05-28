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

    public int nextLevel = 2;

    private void Awake()
    {
        // set decks from campaign
        if (GameSettings.INSTANCE.IsBattle())
        {
            Deck[] decks = FindObjectsOfType<Deck>();
            Assert.IsTrue(decks.Length == 2);
            for (int i = 0; i < decks.Length; ++i)
            {
                Deck d = decks[i];
                if (d.team == TeamType.Player)
                {
                    d.deck_types = GameSettings.INSTANCE.attack_deck;
                }
                else
                {
                    d.deck_types = GameSettings.INSTANCE.target_deck;
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
        // stop time
        Time.timeScale = 0.3f;

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
        foreach (Deck d in decks)
        {
            d.RemoveDrawCards();
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
}

