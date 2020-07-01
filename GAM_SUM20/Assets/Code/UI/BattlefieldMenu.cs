using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Text;
using TMPro;

public class BattlefieldMenu : MonoBehaviour
{
    [Header("Debug")]
    public Text fps_text;
    // update every second
    float fps_update_counter = 0;

    [Header("GUI Panels")]
    public GameObject endGamePanel;
    public GameObject victoryPanel;
    public GameObject defeatPanel;
    public GameObject tiePanel;
    public RawImage endGameTimerPanel;

    [Header("Players")]
    public Deck[] decks;
    public DealPlayerDamage[] playerHitPoints;

    [Header("End Game")]
    public int nextLevel = 2;
    private bool game_ended = false;

    public TextMeshProUGUI timerText;
    public float endGameCheckTime = 5f;
    private float endGameCheckCurrentTime = 0f;
    private bool showing_timer = false; // used for coroutine start
    public bool fade_timer = true;

    private void Awake()
    {
        game_ended = false;
        Assert.IsTrue(decks.Length == 2);
        Assert.IsTrue(playerHitPoints != null);
        
    }

    private void Start()
    {
        //if (GameSettings.INSTANCE.IsBattle())
        {
            // set decks from campaign or quickgame
            for (int i = 0; i < decks.Length; ++i)
            {
                Deck d = decks[i];
                if (d.team == TeamType.Player)
                {
                    d.SetDeck(GameSettings.INSTANCE.CopyAttackDeck());
                }
                else
                {
                    d.SetDeck(GameSettings.INSTANCE.CopyTargetDeck());
                }
            }
            Debug.Log("Decks set from GameSettings");
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        fps_update_counter += Time.deltaTime;
        if (fps_update_counter > 1.0f) {
            DrawFPS(fps_text);
            fps_update_counter = 0.0f;
        }
        // game time end condition
        if (game_ended == false)
        {
            // no troops alive
            if (IsBattleEnd())
            {
                ShowEndGamePanel();
                return;

            }

#if false // CHECK LAST MAN STAND!!!
            if (decks[0].cards_to_play_count == 0 && decks[1].cards_to_play_count == 0)
            {
                if (DealPlayerDamage.totalTroopCount == 0)
                {
                }
                // need to store player troop count somewhere...
                if (DealPlayerDamage.totalTroopCount == 1) {
                    // find that bitch
                    Unit[] last_man = FindObjectsOfType<Unit>();  // may not be deleted yet
                    foreach (Unit u in last_man)
                    {
                        if (u.IsAlive()) {
                            int hit_dif = playerHitPoints[0].hit_points - playerHitPoints[1].hit_points;
                            if (hit_dif == 0) {

                            }


                            return;
                        }
                    }

                }
            }
#endif
            endGameCheckCurrentTime += Time.deltaTime;
            // end game
            if (endGameCheckCurrentTime > endGameCheckTime)
            {
                endGameTimerPanel.gameObject.SetActive(false);
                ShowEndGamePanel();
            }
            else if (fade_timer)
            {
                // show time
                if (((int)endGameCheckCurrentTime % 60) == 0)
                {
                    if (showing_timer == false)
                        StartCoroutine(FadeTimerPanel());
                }
                else if (endGameCheckCurrentTime < 3.1f)
                {
                    if (showing_timer == false)
                        StartCoroutine(FadeTimerPanel());
                }
            }
            else
                UpdateTimerText();
        }
    }
#if false
    private void OnGUI()
    {
        GUI.Label(new Rect(300, 10, 200, 80), "Troops alive = " + DealPlayerDamage.totalTroopCount.ToString());
    }

#endif

    void UpdateTimerText()
    {
        timerText.text = (endGameCheckTime - endGameCheckCurrentTime).ToString("00.0");
    }

    IEnumerator FadeTimerPanel()
    {
        Debug.Log("FadeTimerPanel Coroutine started.");
        showing_timer = true;
        endGameTimerPanel.gameObject.SetActive(true);
        CanvasRenderer[] renderers = endGameTimerPanel.GetComponentsInChildren<CanvasRenderer>();

        int fade_frames = (int)(2f / Time.deltaTime);  // two sec
        for (int i = 0; i < fade_frames; ++i) {
            float a = Mathf.Lerp(0f, 1f, (float)i / fade_frames);
            for (int r = 0; r < renderers.Length; ++r) {
                renderers[r].SetAlpha(a);
            }
            UpdateTimerText();
            yield return null;
        }
        for (int i = 0; i < fade_frames; ++i)
        {
            float a = Mathf.Lerp(1f, 0f, (float)i / fade_frames);
            for (int r = 0; r < renderers.Length; ++r)
            {
                renderers[r].SetAlpha(a);
            }
            UpdateTimerText();
            yield return null;
        }
        endGameTimerPanel.gameObject.SetActive(false);
        showing_timer = false;
    }

    public bool IsBattleEnd()
    {
        if (decks[0].cards_to_play_count == 0 && decks[1].cards_to_play_count == 0)
            if (DealPlayerDamage.totalTroopCount == 0)
                return true;
        return false;

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

    public void ShowEndGamePanel()
    {
        if (game_ended)
            return;
        game_ended = true;
        // stop time
        Time.timeScale = 0.3f;

        // get hitPoints dif
        int pointsDif = playerHitPoints[0].hit_points - playerHitPoints[1].hit_points;

        DisablePlayers();

        endGamePanel.SetActive(true);
        if (pointsDif > 0)
        {
            victoryPanel.SetActive(true);
            GameSettings.INSTANCE.last_battle_won = true;
            Debug.Log("VICTORY!");
        }
        else if (pointsDif < 0)
        {
            defeatPanel.SetActive(true);
            GameSettings.INSTANCE.last_battle_won = false;
            Debug.Log("DEFEAT!");
        }
        else {
            tiePanel.SetActive(true);
            GameSettings.INSTANCE.last_battle_won = false;
            Debug.Log("DRAW...");
        }
    }

    public void ShowWinGamePanel()
    {
        game_ended = true;
        DisablePlayers();
        endGamePanel.SetActive(true);
        victoryPanel.SetActive(true);
        defeatPanel.SetActive(false);
        GameSettings.INSTANCE.last_battle_won = true;
    }
    public void ShowDefeatGamePanel()
    {
        game_ended = true;
        DisablePlayers();
        endGamePanel.SetActive(true);
        victoryPanel.SetActive(false);
        defeatPanel.SetActive(true);
        GameSettings.INSTANCE.last_battle_won = false;
    }



    void LoadNextLevel()
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
        GameSettings.INSTANCE.SetNextSceneIdx(nextLevel);

        // load loading scene
        SceneManager.LoadScene("Scenes/LoadingScreen");
    }

    public void ReplayLevel()
    {
        Time.timeScale = 1.0f;
        GameSettings.INSTANCE.SetNextSceneIdx(nextLevel);
        // load loading scene
        SceneManager.LoadScene("Scenes/LoadingScreen");
    }

    void LoadMainMenu()
    {
        if (GameSettings.INSTANCE.IsBattle()) {
            UpdateCampaignFile();
        }
        else
            GameSettings.INSTANCE.ResetBattle();
        Time.timeScale = 1.0f;


        GameSettings.INSTANCE.SetNextSceneIdx(nextLevel);

        SceneManager.LoadScene("Scenes/MainMenu");
    }

    void UpdateCampaignFile()
    {
        Assert.IsTrue(GameSettings.INSTANCE.prevSceneIdx != -1);
        nextLevel = GameSettings.INSTANCE.prevSceneIdx;

        // update decks
        Deck[] decks = FindObjectsOfType<Deck>();
        CardTypeCount[] attacker_deck = null;
        CardTypeCount[] defender_deck = null;

        Assert.IsTrue(decks.Length == 2);
        for(int i = 0; i < decks.Length; ++i)
        {
            Deck d = decks[i];
            if (d.team == TeamType.Player)
            {
                attacker_deck = d.GetDeck();
            }
            else
            {
                defender_deck = d.GetDeck();
            }
        }
        // update GameSettings
        GameSettings.INSTANCE.SetAttackDeck(attacker_deck);
        GameSettings.INSTANCE.SetTargetDeck(defender_deck);
        // update file
        MapCampaign.UpdateDecksInFile(attacker_deck, defender_deck);
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

    public void LevelWinExit()
    {
        GameSettings.INSTANCE.last_battle_won = true;
        LoadMainMenu();
    }
    public void LevelLoseExit()
    {
        GameSettings.INSTANCE.last_battle_won = false;
        LoadMainMenu();
    }

    public void CheatWin()
    {
        ShowWinGamePanel();
    }

    public void CheatLose()
    {
        ShowDefeatGamePanel();
    }

    public void CheatAddResources(PlayerResources resources)
    {
        resources.MR_curr += 100;
        resources.MR_max += 100;
        resources.HR_curr += 100;
        resources.HR_max += 100;
    }

    void DisablePlayers()
    {
        OpponentAI ai = FindObjectOfType<OpponentAI>();
        if (ai != null)
        {
            // add cards not played
            for (int i = 0; i < ai.hand_types.Length; ++i)
            {
                if(ai.hand_types[i] != CardType.None)
                    ai.deck.AddToDeck(ai.hand_types[i], 1);
            }
            ai.enabled = false;
        }
        PlayerHand[] hands = FindObjectsOfType<PlayerHand>();
        foreach (PlayerHand hand in hands)
        {
            CardPlayable[] cards = hand.cards;
            // add card to deck for saving
            AddUnplayedCardsToDeck(hand.GetDeck(), cards);
             // disable cards
            for (int i = 0; i < cards.Length; ++i) {
                cards[i].enabled = false;
            }
        }
    }

    void AddUnplayedCardsToDeck(Deck deck, CardPlayable[] cards)
    {
        for (int i = 0; i < cards.Length; ++i) {
            if (cards[i].card.type != CardType.None)
                deck.AddToDeck(cards[i].card.type, 1);
        }
    }

}

