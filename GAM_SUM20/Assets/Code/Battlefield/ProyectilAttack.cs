using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(LineRenderer))]
public class ProyectilAttack : MonoBehaviour
{
    LineRenderer line;

    private Unit unit;
    private Unit target;

    private Vector3 startShoot;
    private Vector3 endShoot;
    private TeamType unitTeam;

    public float shootSpeed = 1.0f;
    public Attack attack;

    // vars to control trajectory
    private float maxTime;
    private float curTime = 0.0f;
    Vector3 dif;

    private void Awake()
    {
        line = GetComponent<LineRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // is set
        Assert.IsTrue(target != null);
    }

    // Update is called once per frame
    void Update()
    {
        curTime += Time.deltaTime;
        if (curTime >= maxTime) {
            attack.SpawnAttack(endShoot, startShoot, unitTeam, target);
            Destroy(gameObject);
            return;
        }
        Vector3 endPos = Vector3.Lerp(startShoot, endShoot, curTime / maxTime);
        line.SetPosition(1, endPos);
    }

    public void SetAttack(Unit _target, Unit _unit)
    {
        target = _target;
        unit = _unit;

        startShoot = unit.transform.position;
        endShoot = target.transform.position;
        unitTeam = unit.team;
        // calculate if moving
        Rigidbody2D rig = target.GetComponent<Rigidbody2D>();
        if (rig != null)
        {
            Vector2 v = rig.velocity;
            endShoot += new Vector3(v.x, v.y, 0f);
        }

        line.SetPosition(0, startShoot);
        line.SetPosition(1, startShoot);
        Vector3 dif = endShoot - startShoot;
        maxTime = dif.magnitude / shootSpeed;
    }
}
