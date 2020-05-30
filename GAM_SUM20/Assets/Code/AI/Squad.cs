using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

public class Squad : MonoBehaviour
{
    public TeamType team;
    public float update_time = 1f;
    public Battlefield battlefield;
    public Unit[] troops;
    private bool units_updated = false;

    private void Awake()
    {
        Assert.IsTrue(battlefield != null);
        if(troops == null)
            troops = GetComponentsInChildren<Unit>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // scale to battlefield grid size
        //transform.localScale *= battlefield.cell_size;
        //targetLayerMask = LayerMask.GetMask(team == TeamType.Player ? "Opponent" : "Player");

        Spawn();
    }



    // Update is called once per frame
    void Update()
    {
        if (units_updated)
            StartCoroutine(UnitUpdate());
    }
    public void Spawn()
    {
        Assert.IsTrue(team != TeamType.None);
        gameObject.layer = (int)team + 8;
        
        //Material mat = Resources.Load<Material>(team == TeamType.Opponent ? "Materials/M_Opponent" : "Materials/M_Player");
        var renderables = GetComponentsInChildren<MeshRenderer>();
        foreach (var r in renderables)
            //r.material = mat;
            r.material.color = battlefield.team_color[(int)team + 1];

        // set team
        Assert.IsTrue(troops != null);

        // create troops in positions
        for (int i = 0; i < troops.Length; ++i)
        {
            Unit t = troops[i];
            Assert.IsTrue(t.transform.parent == transform);
            // set layers
            t.gameObject.layer = gameObject.layer;
            // set team
            t.team = team;
            // set healthbar team color
            Assert.IsTrue(t.healthBarInstance != null);
            t.healthBarInstance.SetTeamColor(battlefield.team_color[(int)team + 1]);
        }

        // troops can capture terrain and sense
        units_updated = true; // allows starting coroutine

        DealPlayerDamage.totalTroopCount += troops.Length;
    }

#if false
    public int maxUnitSense = 5;
    private int targetLayerMask = -1;   // layer of the oposite team to sense

    Unit SenseTarget(Unit unit)
    {
        RaycastHit2D[] hits = new RaycastHit2D[maxUnitSense];
        int hitCount = Physics2D.CircleCastNonAlloc(unit.transform.position, unit.common.sensorRange, Vector2.zero, hits, 0f, targetLayerMask);
        if (hitCount > 0)
        {
            Assert.IsTrue(hitCount <= hits.Length);
            Unit unitTarget = GetClosest(unit, hits, hitCount);
            Assert.IsTrue(unitTarget != null);
            return unitTarget;
        }
        return null;
    }

    Unit GetClosest(Unit sensor, RaycastHit2D[] hits, int hitCount)
    {
        RaycastHit2D closestHit = hits[0];
        Vector3 sensorPos = sensor.transform.position;
        float closestDist2 = (sensorPos - closestHit.transform.position).sqrMagnitude;
        for (int i = 1; i < hitCount; ++i) {
            Vector3 dif = sensorPos - hits[i].transform.position;
            float dist2 = dif.sqrMagnitude;
            if (dist2 < closestDist2) {
                closestDist2 = dist2;
                closestHit = hits[i];
            }
        }
        Unit unit = closestHit.transform.GetComponent<Unit>();
        Assert.IsTrue(unit != null);
        return unit;
    }
#endif
    IEnumerator UnitUpdate()
    {
        units_updated = false;
        int troops_destroyed = 0;
        // capture land
        for (int i= 0; i < troops.Length; ++i)
        {

            Unit t = troops[i];
            Assert.IsTrue(t != null);
            if (t.IsAlive() == false)
            {
                troops_destroyed++;
                continue;
            }
            // capture terrain
            battlefield.CaptureCells(t.transform.position, team, t.common.capture_radius);

#if false   // no target assigning
            // set target
            if (t.IsTargetAlive() == false)
                t.SetTarget(SenseTarget(t));
#endif   
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
            Unit t = troops[i];
            if (t == null)
                continue;
            Gizmos.color = team == TeamType.Player ? Color.blue : Color.red;
            for (int j = 0; j < circleGizmo.Length - 1; ++j)
            {
                Vector3 s = t.transform.position + circleGizmo[j] * t.common.sensorRange;
                Vector3 e = t.transform.position + circleGizmo[j + 1] * t.common.sensorRange;
                Gizmos.DrawLine(s, e);
            }
            Gizmos.DrawLine(
                t.transform.position + circleGizmo[circleGizmo.Length - 1] * t.common.sensorRange,
                t.transform.position + circleGizmo[0] * t.common.sensorRange);
#if true
            Gizmos.color = Color.yellow;
            for (int j = 0; j < circleGizmo.Length - 1; ++j)
            {
                Vector3 s = t.transform.position + circleGizmo[j] * t.common.attackRange;
                Vector3 e = t.transform.position + circleGizmo[j + 1] * t.common.attackRange;
                Gizmos.DrawLine(s, e);
            }
            Gizmos.DrawLine(
                t.transform.position + circleGizmo[circleGizmo.Length - 1] * t.common.attackRange,
                t.transform.position + circleGizmo[0] * t.common.attackRange);
#endif
        }
    }
}
