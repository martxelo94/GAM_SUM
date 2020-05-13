using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

public class Squad : MonoBehaviour
{
    public TeamType team;

    Battlefield battlefield;
    UnitStats[] troops;

    bool troops_updated = false;

    // Start is called before the first frame update
    void Start()
    {
        battlefield = FindObjectOfType<Battlefield>();

        troops = GetComponentsInChildren<UnitStats>();

        Spawn();

        // deactivate every non render component
        //var components = gameObject.
        //foreach (Transform t in transform)
        //{
        //    Debug.Log(t.name);
        //} // only prints INMEDIATE children CONFIRMED
    }

    public void Spawn()
    {
        Assert.IsTrue(team != TeamType.None);
        Material mat = Resources.Load<Material>(team == TeamType.Opponent ? "Materials/M_Opponent" : "Materials/M_Player");
        var renderables = GetComponentsInChildren<Renderer>();
        foreach (var r in renderables)
            r.material = mat;
        // set team and health bar
        GameObject healthBarObj = Resources.Load("Prefabs/UI/HealthBar") as GameObject;
        HealthBar healthBar = healthBarObj.GetComponent<HealthBar>();
        Assert.IsTrue(healthBar != null);
        Assert.IsTrue(troops != null);
        foreach (UnitStats t in troops)
        {
            t.gameObject.layer = (int)team + 7;
            t.team = team;
            healthBar.SetUnit(t);
            healthBar.SetTeamColor(battlefield.team_color[(int)team]);
            // instance
            t.healthBarInstance = Instantiate(healthBar.gameObject);
            t.healthBarInstance.transform.position = t.transform.position;
            t.healthBarInstance.SetActive(false);
        }
        // troops can capture terrain
        troops_updated = true; // allows starting coroutine
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

        //Debug.Log("Squad updated");
    }

    // Update is called once per frame
    void Update()
    {
        if (troops_updated)
            StartCoroutine(CaptureTerrain());
    }
}
