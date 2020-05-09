using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;   // text

public class ResourceManager : MonoBehaviour
{
    public PlayerResources m_resources;
    public GameObject HR_bar;
    public Text HR_text;
    public GameObject MR_bar;
    public Text MR_text;

    float HR_maxScale;
    float MR_maxScale;

    // Start is called before the first frame update
    void Start()
    {
        HR_maxScale = HR_bar.transform.localScale.x;
        MR_maxScale = MR_bar.transform.localScale.x;
    }

    // Update is called once per frame
    void Update()
    {
        // set GUI
        HR_bar.transform.localScale = new Vector3(HR_maxScale * m_resources.HR_curr / m_resources.HR_max, HR_bar.transform.localScale.y, HR_bar.transform.localScale.z);
        MR_bar.transform.localScale = new Vector3(MR_maxScale * m_resources.MR_curr / m_resources.MR_max, MR_bar.transform.localScale.y, MR_bar.transform.localScale.z);

        HR_text.text = ((int)m_resources.HR_curr).ToString() + "/" + m_resources.HR_max.ToString();
        MR_text.text = ((int)m_resources.MR_curr).ToString() + "/" + m_resources.MR_max.ToString();
    }


}
