using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Scrollbar))]
public class ScrollbarClamper : MonoBehaviour
{
    [Range(2, 10)]
    public int panelCount = 2;
    Scrollbar scrollbar;
    // Start is called before the first frame update
    void Start()
    {
        
        scrollbar = GetComponent<Scrollbar>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0)) {
            scrollbar.value = Mathf.Round(scrollbar.value * (panelCount - 1)) / (panelCount - 1);
        }
    }

    private void OnMouseUp()
    {
        //Debug.Log("MouseUp on" + typeof(ScrollbarClamper).ToString());
    }

    public void MoveLeft()
    {
        float step = 1f / (panelCount - 1);
        scrollbar.value = Mathf.Clamp01(scrollbar.value - step);
    }
    public void MoveRight()
    {
        float step = 1f / (panelCount - 1);
        scrollbar.value = Mathf.Clamp01(scrollbar.value + step);
    }
}
