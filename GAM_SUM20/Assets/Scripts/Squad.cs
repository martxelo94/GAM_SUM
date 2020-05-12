using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Squad : MonoBehaviour
{
    public TeamType team;

    Battlefield battlefield;
    UnitStats[] troops;

    bool troops_updated = true;

    // Start is called before the first frame update
    void Start()
    {
        battlefield = FindObjectOfType<Battlefield>();

        troops = GetComponentsInChildren<UnitStats>();

        Material mat = Resources.Load<Material>(team == TeamType.Opponent ? "Materials/M_Opponent" : "Materials/M_Player");
        var renderables = GetComponentsInChildren<Renderer>();
        foreach (var r in renderables)
            r.material = mat;
        // set team to attackers
        foreach (UnitStats t in troops) {
            t.team = team;
        }
    }

    IEnumerator CaptureTerrain()
    {
        troops_updated = false;
        int troops_destroyed = 0;
        // capture land
        foreach (UnitStats t in troops)
        {
            if (t == null)
            {
                troops_destroyed++;
                continue;
            }
            battlefield.CaptureCells(t.transform.position, team, t.capture_radius);
            yield return null;  // update one unit each frame
        }
        // destroy self if no units left
        if (troops_destroyed == troops.Length)
        {
            Destroy(gameObject);
        }

        troops_updated = true;

        Debug.Log("Squad updated");
    }

    // Update is called once per frame
    void Update()
    {
        if (troops_updated)
            StartCoroutine(CaptureTerrain());
    }
}
