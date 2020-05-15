using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BattlefieldMenu : MonoBehaviour
{
    public Text fps_text;
    // update every second
    float fps_update_counter = 0;

    public GameObject endGamePanel;
    public GameObject victoryPanel;
    public GameObject defeatPanel;

    public int nextLevel = 2;

    // Start is called before the first frame update
    void Start()
    {
        
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

    public void Toggle(GameObject obj)
    {
        obj.SetActive(!obj.activeSelf);
    }

    public void DrawFPS(Text text)
    {
        int frameRate = (int)(1.0f / Time.deltaTime);
        text.text = "FPS: " + frameRate.ToString();
    }

    public void ShowEndGamePanel(bool is_victory)
    {
        // stop time
        Time.timeScale = 0.3f;

        endGamePanel.SetActive(true);
        if (is_victory)
        {
            victoryPanel.SetActive(true);
            Debug.Log("VICTORY!");
        }
        else {
            defeatPanel.SetActive(true);
            Debug.Log("DEFEAT!");
        }
    }

    public void LoadNextLevel()
    {
        Time.timeScale = 1.0f;
        PlayerPrefs.SetInt("NextScene", nextLevel);
        // load loading scene
        SceneManager.LoadScene("Scenes/LoadingScreen");
    }

    public void ReplayLevel()
    {
        Time.timeScale = 1.0f;
        PlayerPrefs.SetInt("NextScene", SceneManager.GetActiveScene().buildIndex);
        // load loading scene
        SceneManager.LoadScene("Scenes/LoadingScreen");
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("Scenes/MainMenu");
    }
}

