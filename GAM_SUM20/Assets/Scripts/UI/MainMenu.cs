using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public int quickGameSceneIndex = 2;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void StartQuickGame()
    {
        // save Battlefield scene to PlayerPrefs
        //int battlefieldSceneIdx = SceneManager.GetSceneByName("Battlefield").buildIndex;
        PlayerPrefs.SetInt("NextScene", quickGameSceneIndex); // MAGIC NUMBER: build index
        // load loading scene
        SceneManager.LoadScene("Scenes/LoadingScreen");
    }
}
