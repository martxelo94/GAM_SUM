using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CampaignMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToggleActive(GameObject obj)
    {
        obj.SetActive(!obj.activeSelf);
    }

    public void ToggleButton(Button button)
    {
        button.interactable = !button.interactable;
    }

    public void ToggleSound(AudioSource source)
    {
        source.mute = !source.mute;
    }

    public void ReplayCampaign()
    {
        MapCampaign.DeleteFile();
        GameSettings.INSTANCE.ResetBattle();
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void EndCampaign()
    {
        MapCampaign.DeleteFile();
        LoadMainMenu();
    }

    public void LoadMainMenu()
    {
        // erase attack data from GameSettings
        GameSettings.INSTANCE.ResetBattle();
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("Scenes/MainMenu");
    }
}
