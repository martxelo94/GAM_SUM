using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ProyectilAttack : MonoBehaviour
{
    LineRenderer line;

    [HideInInspector]
    public Vector3 startShoot;
    [HideInInspector]
    public Vector3 endShoot;

    [HideInInspector]
    public float shootDamage = 1.0f;
    public float shootSpeed = 1.0f;
    public float attackRange = 1.0f;
    public Explosion explosionPrefab;

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
        line.SetPosition(0, startShoot);
        line.SetPosition(1, startShoot);
        Vector3 dif = endShoot - startShoot;
        maxTime = dif.magnitude / shootSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        curTime += Time.deltaTime;
        if (curTime >= maxTime) {
            if (explosionPrefab)
            {
                Explosion inst = Instantiate(explosionPrefab, endShoot, new Quaternion()) as Explosion;
                inst.transform.up = dif.normalized;
                inst.transform.localScale = new Vector3(attackRange, attackRange, attackRange);
                inst.damage = shootDamage;
            }

            Destroy(gameObject);
            return;
        }
        Vector3 endPos = Vector3.Lerp(startShoot, endShoot, curTime / maxTime);
        line.SetPosition(1, endPos);
    }
}
