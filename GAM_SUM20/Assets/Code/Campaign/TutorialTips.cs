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
        public Renderer[] highlighted_objects;
        public List<Vector3> original_highlighted_scales;
    }
    public int current_tip = -1;
    public TipElements[] tips;

    protected string save_filepath = "";

    protected void Start()
    {
        Assert.IsTrue(save_filepath.CompareTo("") != 0);    // path initialized
        // set tip
        current_tip = PlayerPrefs.GetInt(save_filepath, -1);
        //current_tip = current_tip >= tips.Length ? -1 : current_tip;
        if (IsValidTip())
            ShowTip(current_tip);
        else if (current_tip < tips.Length)
            NextTip();

        // set original scales
        for (int i = 0; i < tips.Length; ++i)
        {
            tips[i].original_highlighted_scales = new List<Vector3>();
            Renderer[] renderers = tips[i].highlighted_objects;
            if (renderers != null) {
                foreach (var r in renderers) {
                    // hack fix when size 0
                    if (r.transform.localScale.sqrMagnitude == 0)
                        r.transform.localScale = Vector3.one;
                    Assert.IsTrue(r.transform.localScale.sqrMagnitude != 0);
                    tips[i].original_highlighted_scales.Add(r.transform.localScale);
                }
            }
        }
    }

    private void OnDisable()
    {
        //DeleteTutorialSave();
    }

    protected void Update()
    {
#if true
        // make pulse
        if (IsValidTip() && tips[current_tip].panel.activeSelf == true)
        {
            Renderer[] renderers = tips[current_tip].highlighted_objects;
            if (renderers != null)
            {
                List<Vector3> scales = tips[current_tip].original_highlighted_scales;
                Assert.IsTrue(scales.Count == renderers.Length);
                for(int i = 0; i < scales.Count; ++i)
                {
                    float t = Mathf.PingPong(Time.time, 1f / cakeslice.OutlineAnimation.animationSpeed);
                    Vector3 initScale = scales[i];
                    renderers[i].transform.localScale = initScale + initScale * t;

                    // without remembering original scale
                    //renderers[i].transform.localScale +=
                }
            }
        }
#endif
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
            HighlightTip(false);         
        }
        current_tip++;
        if (current_tip < tips.Length)
        {
            tips[current_tip].panel.SetActive(true);
            HighlightTip(true);
        }
    }

    void HighlightTip(bool show)
    {
        Renderer[] highlights = tips[current_tip].highlighted_objects;
        if (highlights != null)
        {
            // add or destroy Outliners
            if (show)
            {
                // add
                for (int i = 0; i < highlights.Length; ++i)
                {
                    cakeslice.Outline null_outline = highlights[i].GetComponent<cakeslice.Outline>();
                    Assert.IsTrue(null_outline == null);
                    highlights[i].gameObject.AddComponent<cakeslice.Outline>();
                }
            }
            else
            {
                // destroy
                for (int i = 0; i < highlights.Length; ++i)
                {
                    highlights[i].transform.localScale = tips[current_tip].original_highlighted_scales[i];
                    cakeslice.Outline outline = highlights[i].GetComponent<cakeslice.Outline>();
                    Assert.IsTrue(outline != null);
                    outline.enabled = false;
                    Destroy(outline);
                }
               
            }
        }
    }

    public void HideTip(bool hide = true)
    {
        Assert.IsTrue(IsValidTip());
        HighlightTip(!hide);
        tips[current_tip].panel.SetActive(!hide);
    }

    public void ShowTip(int idx)
    {
        Assert.IsTrue(idx >= 0 && idx < tips.Length);

        // highlight previous
        if (IsValidTip()) {
            // crash on init becouse objects from default, not used previously (assert check Outline present to remove)
            if(tips[current_tip].panel.activeSelf == true)
                HideTip();
        }
        current_tip = idx;
        tips[current_tip].panel.SetActive(true);
        HighlightTip(true);
    }

    public void DeleteTutorialSave()
    {
        PlayerPrefs.DeleteKey(save_filepath);
        current_tip = -1;
    }
}
