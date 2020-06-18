using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Assertions;

public class SceneLoading : MonoBehaviour
{
    public Image progressBar;
    public RawImage background;
    public Texture[] textures;

    private int sceneIndex = -1; // get from PlayerPrefs
    private int textureIdx = 0;
    private const string textureIdxSavename = "LoadBackgroundIdx";

    // Start is called before the first frame update
    void Start()
    {
        sceneIndex = GameSettings.INSTANCE.nextSceneIdx;

        progressBar.fillAmount = 0f;

        // set random texture
        textureIdx = PlayerPrefs.GetInt(textureIdxSavename, 0);
        Assert.IsTrue(textureIdx < textures.Length);

        background.texture = textures[textureIdx];
        // update idx
        textureIdx = textureIdx + 1 < textures.Length ? textureIdx + 1 : 0;
        PlayerPrefs.SetInt(textureIdxSavename, textureIdx);

        // start async operation
        StartCoroutine(LoadAsyncOperation());

    }

    IEnumerator LoadAsyncOperation()
    {
        yield return new WaitForSeconds(1f);
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
