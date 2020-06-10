using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public int aiGameSceneIndex = 2;
    public int playerGameSceneIndex = 3;
    public int campaignSceneIndex = 5;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void LoadLevelScene(int nextLevel)
    {
        GameSettings.INSTANCE.nextSceneIdx = nextLevel;
        // load loading scene
        SceneManager.LoadScene("Scenes/LoadingScreen");

    }

    public void StartAIGame()
    {
        LoadLevelScene(aiGameSceneIndex);
    }
    public void StartPlayerGame()
    {
        LoadLevelScene(playerGameSceneIndex);
    }
    public void StartCampaign(int idx)
    {
        LoadLevelScene(campaignSceneIndex + idx);
    }
}
