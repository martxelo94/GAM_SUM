using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float lifeTime;
    [HideInInspector]
    public float damage = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Die());
    }

    // Update is called once per frame
    void Update()
    {
    }
    IEnumerator Die()
    {
        yield return new WaitForSeconds(lifeTime);

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        UnitStats unit = collision.GetComponent<UnitStats>();
        if (unit != null) {
            // compute damage by distance
            Vector3 dif = unit.transform.position - transform.position;
            float dist2 = dif.sqrMagnitude;
            float maxDist2 = transform.localScale.x * transform.localScale.y;
            float finalDamage = damage - damage * (dist2 / maxDist2);
            finalDamage = Mathf.Max(finalDamage, 0);
            unit.ReceiveDamage((int)finalDamage);
        }

    }

}
