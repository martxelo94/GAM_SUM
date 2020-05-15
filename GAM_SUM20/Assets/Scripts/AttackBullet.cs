using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class AttackBullet : MonoBehaviour
{
    public float lifeTime = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetTarget(Vector3 start, Vector3 end)
    {
        LineRenderer line = GetComponent<LineRenderer>();

        line.SetPosition(0, start);
        line.SetPosition(1, end);

        StartCoroutine(LifeTime());
    }

    IEnumerator LifeTime()
    {
        yield return new WaitForSeconds(lifeTime);

        Destroy(gameObject);
    }
}
