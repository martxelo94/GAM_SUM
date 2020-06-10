using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class TutorialTips : MonoBehaviour
{

    [System.Serializable]
    public struct TipElements
    {
        public GameObject panel;
        public GameObject highlighted_object;
    }
    public int current_tip = -1;
    public GameObject highlighter;
    public TipElements[] tips;

    protected string save_filepath = "";

    protected void Start()
    {
        Assert.IsTrue(highlighter != null);
        Assert.IsTrue(save_filepath.CompareTo("") != 0);    // path initialized
        // set tip
        current_tip = PlayerPrefs.GetInt(save_filepath, -1);
        //current_tip = current_tip >= tips.Length ? -1 : current_tip;
        if (IsValidTip())
            ShowTip(current_tip);
        else if (current_tip < tips.Length)
            NextTip();
    }

    private void OnDisable()
    {
        //DeleteTutorialSave();
    }

    protected void Update()
    {
        if (IsValidTip()) {
            GameObject obj = tips[current_tip].highlighted_object;
            if (obj != null) {
                highlighter.transform.position = obj.transform.position;
            }
        }
    }

    public bool IsValidTip()
    {
        return current_tip >= 0 && current_tip < tips.Length;
    }

    public void NextTip()
    {
        Assert.IsTrue(current_tip < tips.Length);
        if (current_tip >= 0)
        {
            tips[current_tip].panel.SetActive(false);
            highlighter.SetActive(false);
        }
        current_tip++;
        if (current_tip < tips.Length)
        {
            tips[current_tip].panel.SetActive(true);
            HighlightTip();
        }
    }

    void HighlightTip()
    {
        GameObject obj = tips[current_tip].highlighted_object;
        if (obj != null)
        {
            highlighter.SetActive(true);
            highlighter.transform.localScale = obj.transform.localScale;
            highlighter.transform.position = obj.transform.position;
        }
    }

    public void HideTip(bool hide = true)
    {
        Assert.IsTrue(IsValidTip());
        tips[current_tip].panel.SetActive(!hide);
        highlighter.SetActive(!hide);
    }
    public void ShowTip(int idx)
    {
        Assert.IsTrue(idx >= 0 && idx < tips.Length);

        // highlight previous
        if (current_tip >= 0 && current_tip < tips.Length)
            tips[current_tip].panel.SetActive(false);
        current_tip = idx;
        tips[current_tip].panel.SetActive(true);
        HighlightTip();
    }

    public void DeleteTutorialSave()
    {
        PlayerPrefs.DeleteKey(save_filepath);
    }
}
