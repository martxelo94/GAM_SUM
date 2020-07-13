using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(MapCampaign))]
public class CampaignMenu : MonoBehaviour
{
    public GameObject campaignEndPanel;
    public GameObject confirmPanel;
    public GameObject selectedNodePanel;
    public GameObject selectedArmyPanel;

    public Button moveButton;
    public Button addCardsButton;

    public InfoPanel infoPanel;

    public DeckManager deckManager;


    public void ToggleActive(GameObject obj)
    {
        obj.SetActive(!obj.activeSelf);
    }

    public void EnableButton(Button button) => button.interactable = true;
    public void DisableButton(Button button) => button.interactable = false;

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
        LoadMainMenu();
        MapCampaign.DeleteFile();
    }

    public void LoadMainMenu()
    {
        MapCampaign map = GetComponent<MapCampaign>();
        map.SaveFile();
        // erase attack data from GameSettings
        GameSettings.INSTANCE.ResetBattle();
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("Scenes/MainMenu");
    }
    public void ShowInfoPanel(bool show)
    {
        infoPanel.gameObject.SetActive(show);
    }
    public void ShowEndPanel(bool show)
    {
        campaignEndPanel.SetActive(show);
    }
    public void ShowArmyPanel(bool show)
    {
        selectedArmyPanel.SetActive(show);
    }
    public void ShowNodePanel(bool show)
    {
        selectedNodePanel.SetActive(show);
    }

    public void ShowConfirmPanel(bool show)
    {
        confirmPanel.SetActive(show);
    }

    public void ShowMoveButton(bool show)
    {
        moveButton.gameObject.SetActive(show);
        addCardsButton.gameObject.SetActive(!show);
    }
    public void LockBattlePanel(bool show)
    {
        // toggle buttons
        Button[] buttons = selectedArmyPanel.GetComponentsInChildren<Button>();
        foreach (Button b in buttons)
            b.interactable = !show;

    }


}
