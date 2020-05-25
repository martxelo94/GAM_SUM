using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public int quickGameSceneIndex = 2;
    public int campaignSceneIndex = 3;

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

    public void StartQuickGame()
    {
        LoadLevelScene(quickGameSceneIndex);
    }
    public void StartCampaign()
    {
        LoadLevelScene(campaignSceneIndex);
    }
}
