using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Assertions;

public class Sensor : MonoBehaviour
{
    [HideInInspector]
    public UnitStats target;
    List<UnitStats> potential_targets;
    TeamType team;

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
    void Update()
    {
        if (target == null && potential_targets.Count > 0)
        {
            // update target
            potential_targets.Sort(SortByDistance);
            target = potential_targets.ElementAt(0);
            potential_targets.RemoveAt(0);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        UnitStats unit = collision.GetComponent<UnitStats>();
        if (unit != null && unit.team != team)
        {
            potential_targets.Add(unit);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
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
}
