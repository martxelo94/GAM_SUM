using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class TutorialManager : MonoBehaviour
{
    public GameObject highlighterPrefab;
    private GameObject[] highlighterInstances;
    public int current_tip = -1;
    public TutorialTip[] tips;
    public Transform tips_holder;

    protected string save_filepath = "";

    protected void Start()
    {
        Assert.IsTrue(save_filepath.CompareTo("") != 0);    // path initialized

        // get tips
        tips = new TutorialTip[tips_holder.childCount];
        for (int i = 0; i < tips.Length; ++i)
        {
            Transform t = tips_holder.GetChild(i);
            TutorialTip tip = t.GetComponent<TutorialTip>();
            Assert.IsTrue(tip != null);
            tips[i] = tip;
            tip.gameObject.SetActive(false);
        }

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
            Transform[] transforms = tips[i].highlighted_objects;
            if (transforms != null) {
                foreach (var t in transforms) {
                    if (t == null)
                    {
                        tips[i].original_highlighted_scales.Add(Vector3.one);
                        continue;
                    }
                    // hack fix when size 0
                    if (t.localScale.sqrMagnitude == 0)
                        t.localScale = Vector3.one;
                    Assert.IsTrue(t.localScale.sqrMagnitude != 0);
                    tips[i].original_highlighted_scales.Add(t.localScale);
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
        if (IsValidTip() && tips[current_tip].gameObject.activeSelf == true)
        {
            Transform[] transforms = tips[current_tip].highlighted_objects;
            if (transforms != null)
            {
                Assert.IsTrue(transforms.Length == highlighterInstances.Length);

                List<Vector3> scales = tips[current_tip].original_highlighted_scales;
                Assert.IsTrue(scales.Count == transforms.Length);
                for(int i = 0; i < scales.Count; ++i)
                {
                    float t = Mathf.PingPong(Time.time, 1f / 1.5f);
                    Vector3 initScale = scales[i];
                    highlighterInstances[i].transform.localScale = initScale + initScale * t;

                    // without remembering original scale
                    //renderers[i].transform.localScale +=
                }
            }
        }
#endif
    }

    public TutorialTip FindTipByName(string name)
    {
        TutorialTip tip = null;

        foreach (TutorialTip t in tips) {
            if (t.name == name)
            {
                tip = t;
                break;
            }
        }

        return tip;
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
            tips[current_tip].Exit();
            tips[current_tip].gameObject.SetActive(false);
            HighlightTip(false);         
        }
        current_tip++;
        if (current_tip < tips.Length)
        {
            tips[current_tip].gameObject.SetActive(true);
            tips[current_tip].Enter();
            HighlightTip(true);
        }
    }

    void HighlightTip(bool show)
    {
        Transform[] highlights = tips[current_tip].highlighted_objects;
        if (highlights != null)
        {
            // add or destroy Outliners
            if (show)
            {
                Assert.IsTrue(highlighterInstances == null);
                highlighterInstances = new GameObject[highlights.Length];
                // add
                for (int i = 0; i < highlights.Length; ++i)
                {
                    highlighterInstances[i] = Instantiate(highlighterPrefab, highlights[i]);
                }
            }
            else if(highlighterInstances != null)
            {
                Assert.IsTrue(highlighterInstances.Length == highlights.Length);
                // destroy
                for (int i = 0; i < highlights.Length; ++i)
                {
                    Destroy(highlighterInstances[i]);
                    highlights[i].transform.localScale = tips[current_tip].original_highlighted_scales[i];
                }
                highlighterInstances = null;
               
            }
        }
    }

    public void HideTip(bool hide = true)
    {
        Assert.IsTrue(IsValidTip());
        HighlightTip(!hide);
        tips[current_tip].gameObject.SetActive(!hide);
    }

    public void ShowTip(int idx)
    {
        Assert.IsTrue(idx >= 0 && idx < tips.Length);

        // highlight previous
        if (IsValidTip()) {
            // crash on init becouse objects from default, not used previously (assert check Outline present to remove)
            if (tips[current_tip].gameObject.activeSelf == true)
            {
                tips[current_tip].Exit();
                HideTip();
            }
        }
        current_tip = idx;
        tips[current_tip].gameObject.SetActive(true);
        tips[current_tip].Enter();
        HighlightTip(true);
    }

    public void DeleteTutorialSave()
    {
        PlayerPrefs.DeleteKey(save_filepath);
        current_tip = -1;
    }
}
