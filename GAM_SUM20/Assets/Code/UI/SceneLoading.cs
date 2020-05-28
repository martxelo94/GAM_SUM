using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class SceneLoading : MonoBehaviour
{
    public Image progressBar;

    private int sceneIndex = -1; // get from PlayerPrefs

    // Start is called before the first frame update
    void Start()
    {
        sceneIndex = GameSettings.INSTANCE.nextSceneIdx;
        // start async operation
        StartCoroutine(LoadAsyncOperation());
    }

    IEnumerator LoadAsyncOperation()
    {
        // create async operation
        AsyncOperation gameLevel = SceneManager.LoadSceneAsync(sceneIndex);

        while (gameLevel.progress < 1)
        {
            // take progress bar fill
            progressBar.fillAmount = gameLevel.progress;
            yield return new WaitForEndOfFrame();
        }
    }
}
