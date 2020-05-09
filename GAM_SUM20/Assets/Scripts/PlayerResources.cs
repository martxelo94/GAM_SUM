using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerResources : MonoBehaviour
{
    public uint HR_max = 20;
    //[HideInInspector]
    public float HR_curr = 0;

    public uint MR_max = 20;
    //[HideInInspector]
    public float MR_curr = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // update resource bar
        float dt = Time.deltaTime * Time.timeScale;
        HR_curr += dt;
        MR_curr += dt;
        HR_curr = Mathf.Clamp(HR_curr, 0, HR_max);
        MR_curr = Mathf.Clamp(MR_curr, 0, MR_max);

    }

    public void ConsumeResources(Vector2Int res)
    {
        HR_curr -= res.x;
        MR_curr -= res.y;
    }
}
