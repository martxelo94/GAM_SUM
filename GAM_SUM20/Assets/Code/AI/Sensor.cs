using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Assertions;

[RequireComponent(typeof(CircleCollider2D))]
public class Sensor : MonoBehaviour
{
    public Unit unit;
    public CircleCollider2D circle;

    private List<Unit> potentialTargets;

    private void Awake()
    {
        Assert.IsTrue(unit != null);
        Assert.IsTrue(circle != null);

        potentialTargets = new List<Unit>();
    }

    private void Start()
    {

        SetLayer(unit.team);
        SetRange(unit.common.sensorRange);
    }

    private void Update()
    {
        // get closest target
        if (potentialTargets.Count > 0)
        {
            // sort by distance
            potentialTargets.Sort(SortByDistance);
            // set first as target
            unit.SetTarget(potentialTargets[0]);
            // reset
            potentialTargets.Clear();
            // switch off sensor
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // get unit colliding and set as target
        Unit unitTarget = collision.GetComponent<Unit>();
        // if layers set correctly, this should success
        Assert.IsTrue(unitTarget != null);
        // target shouldn't be in potentials already
        Assert.IsTrue(potentialTargets.Contains(unitTarget) == false);

        potentialTargets.Add(unitTarget);
    }
    public void SetRange(float range)
    {
        circle.radius = range;
    }
    public void SetLayer(TeamType team)
    {
        circle.gameObject.layer = 10 + (int)team;
    }
    bool UnitCompare(Unit a, Unit b) { return a == b; }
    int SortByDistance(Unit a, Unit b)
    {
        float dist2A = (transform.position - a.transform.position).sqrMagnitude;
        float dist2B = (transform.position - b.transform.position).sqrMagnitude;
        return dist2A.CompareTo(dist2B);
    }

#if false

    [HideInInspector]
    public UnitStats target;
    List<UnitStats> potential_targets;
    TeamType team;

    public void SenseTarget(UnitStats unit)
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(unit.transform.position, unit.common.range, Vector2.up, Mathf.Infinity, 9 + (int)unit.team);
        if (hits.Length > 0) {
            target = hits[0].collider.GetComponent<UnitStats>();
            Assert.IsTrue(target != null);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        team = GetComponentInParent<Squad>().team;
        potential_targets = new List<UnitStats>();
    }

    int SortByDistance(UnitStats a, UnitStats b)
    {
        float dist2A = (transform.position - a.transform.position).sqrMagnitude;
        float dist2B = (transform.position - b.transform.position).sqrMagnitude;
        return dist2A.CompareTo(dist2B);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (target == null && potential_targets.Count > 0)
        {
            //remove nulls
            List<UnitStats> to_remove = new List<UnitStats>();
            foreach(UnitStats t in potential_targets)
                if (t == null)
                    to_remove.Add(t);
            foreach(UnitStats r in to_remove)
                potential_targets.Remove(r);
            if (potential_targets.Count == 0)
                return;
            // sort by closest distance
            potential_targets.Sort(SortByDistance);
            // update target
            target = potential_targets.ElementAt(0);
            potential_targets.RemoveAt(0);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.isTrigger)
            return;
        UnitStats unit = collision.GetComponent<UnitStats>();
        if (unit != null && unit.team != team)
        {
            potential_targets.Add(unit);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.isTrigger)
            return;
        UnitStats unit = collision.GetComponent<UnitStats>();
        if (unit != null) {
            potential_targets.Remove(unit);

            // may happen if unit killed
            //Assert.IsTrue(target.gameObject != collision.gameObject);
            if (target != null)
                if (target.gameObject == collision.gameObject)
                    target = null;
        }
    }
#endif
}
