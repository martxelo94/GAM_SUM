using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class SceneLoading : MonoBehaviour
{
    public Image progressBar;
    public RawImage background;
    public Texture[] textures;

    private int sceneIndex = -1; // get from PlayerPrefs

    // Start is called before the first frame update
    void Start()
    {
        sceneIndex = GameSettings.INSTANCE.nextSceneIdx;
        // start async operation
        StartCoroutine(LoadAsyncOperation());

        // set random texture
        background.texture = textures[GameSettings.INSTANCE.randomizer.Next(0, textures.Length)];
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
