using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

public class Squad : MonoBehaviour
{
    public TeamType team;
    public float update_time = 1f;
    public Battlefield battlefield;
    UnitStats[] troops;
    bool units_updated = false;
    int targetLayerMask = -1;   // layer of the oposite team to sense

    private void Awake()
    {
        Assert.IsTrue(battlefield != null);
        troops = GetComponentsInChildren<UnitStats>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // scale to battlefield grid size
        //transform.localScale *= battlefield.cell_size;
        targetLayerMask = LayerMask.GetMask(team == TeamType.Player ? "Opponent" : "Player");

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
        gameObject.layer = (int)team + 8;
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
            // set layers
            t.gameObject.layer = gameObject.layer;
            // set team
            t.team = team;
            // set healthbar
            healthBar.SetUnit(t);
            healthBar.SetTeamColor(battlefield.team_color[(int)team + 1]);
            // copy healthbar prefab
            t.healthBarInstance = Instantiate(healthBar.gameObject);
            t.healthBarInstance.transform.parent = t.transform; // make destructible with unit instance
            t.healthBarInstance.transform.position = t.transform.position;
            t.healthBarInstance.SetActive(false);   // invisible inactive
            
        }
        // troops can capture terrain and sense
        units_updated = true; // allows starting coroutine
    }


    // Update is called once per frame
    void Update()
    {
        if (units_updated)
            StartCoroutine(UnitUpdate());
    }

    public void SenseTarget(UnitStats unit)
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(unit.transform.position, unit.common.sensorRange, Vector2.zero, 0, targetLayerMask);
        if (hits.Length > 0)
        {
            UnitStats target = hits[0].collider.GetComponent<UnitStats>();
            unit.target = target;
            if(unit.target == null)
                Assert.IsTrue(unit.target != null);
        }
    }

    IEnumerator UnitUpdate()
    {
        units_updated = false;
        int troops_destroyed = 0;
        // capture land
        for (int i= 0; i < troops.Length; ++i)
        {
            UnitStats t = troops[i];
            if (t == null)
            {
                troops_destroyed++;
                continue;
            }
            battlefield.CaptureCells(t.transform.position, team, t.common.capture_radius);
            if(t.target == null)
                SenseTarget(t);
            yield return new WaitForSeconds(update_time / troops.Length);  // update one unit each frame
        }
        // destroy self if no units left
        if (troops_destroyed == troops.Length)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
        else
            units_updated = true;

        //Debug.Log("Squad updated");
    }

    Vector3[] circleGizmo = null;

    private void OnDrawGizmos()
    {
        const int segments = 20;
        if (circleGizmo == null) {
            circleGizmo = new Vector3[segments];
            const float angleStep = Mathf.PI * 2f / segments;
            float currentAngle = 0f;
            for (int i = 0; i < segments; ++i)
            {
                circleGizmo[i] = new Vector3(Mathf.Cos(currentAngle), Mathf.Sin(currentAngle), 0f);
                currentAngle += angleStep;
            }
        }

        for (int i = 0; i < troops.Length; ++i)
        {
            UnitStats t = troops[i];
            if (t == null)
                continue;
            Gizmos.color = Color.yellow;
            for (int j = 0; j < circleGizmo.Length - 1; ++j)
            {
                Vector3 s = t.transform.position + circleGizmo[j] * t.common.sensorRange;
                Vector3 e = t.transform.position + circleGizmo[j + 1] * t.common.sensorRange;
                Gizmos.DrawLine(s, e);
            }
            Gizmos.DrawLine(
                t.transform.position + circleGizmo[circleGizmo.Length - 1] * t.common.sensorRange,
                t.transform.position + circleGizmo[0] * t.common.sensorRange);
            Gizmos.color = Color.red;
            for (int j = 0; j < circleGizmo.Length - 1; ++j)
            {
                Vector3 s = t.transform.position + circleGizmo[j] * t.common.attackRange;
                Vector3 e = t.transform.position + circleGizmo[j + 1] * t.common.attackRange;
                Gizmos.DrawLine(s, e);
            }
            Gizmos.DrawLine(
                t.transform.position + circleGizmo[circleGizmo.Length - 1] * t.common.attackRange,
                t.transform.position + circleGizmo[0] * t.common.attackRange);
        }
    }
}
