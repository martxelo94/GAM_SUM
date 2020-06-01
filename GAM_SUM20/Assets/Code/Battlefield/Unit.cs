using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Unit : MonoBehaviour
{
    public UnitStats common;
    //[HideInInspector]
    public TeamType team; // set by squad
    public int currentHitPoints { get; private set; }
    [HideInInspector]
    public float currentAttackTime;

    //[HideInInspector]
    public Sensor sensor;
    public HealthBar healthBarInstance;

    private Unit target = null;

    // Start is called before the first frame update
    void Start()
    {
        currentHitPoints = common.maxHitPoints;

        // set sensor
        Assert.IsTrue(sensor != null);

        if (healthBarInstance != null)
        {
            // set healthbar
            healthBarInstance.SetUnit(this);
            // copy healthbar prefab
            healthBarInstance.gameObject.SetActive(false);   // invisible inactive
        }
    }

    private void Update()
    {
        if (IsTargetAlive() == false)
        {
            // activate sensor
            sensor.gameObject.SetActive(true);
        }
    }

    public Vector2 DifToTarget()
    {
        // make sure we dont call this without target
        Assert.IsTrue(IsTargetAlive() == true);
        Vector2 dir = target.transform.position - transform.position;
        return dir;
    }
    public void SpawnAttack()
    {
        // make sure we dont call this without target
        Assert.IsTrue(IsTargetAlive() == true);

        ProyectilAttack attack = GameObject.Instantiate(common.attackPrefabs[0]);
        attack.SetAttack(target, this);

        Debug.Log(name + " attaked " + target.name);
    }

    public bool ReceiveDamage(int _damage)
    {
        //Debug.Log(name + " receives " + _damage.ToString() + " damage!");
        currentHitPoints -= _damage;
        if (currentHitPoints <= 0) {
            Kill();
            return true;
        }
        Assert.IsTrue(healthBarInstance != null);
        if (!healthBarInstance.gameObject.activeSelf)
            StartCoroutine(ShowHealthBar());
        return false;
    }

    public bool IsAlive()
    {
        return gameObject.activeSelf;
    }

    public bool IsTargetAlive()
    {
        return target != null && target.gameObject.activeSelf;
    }

    public bool Kill()
    {
        if (gameObject.activeSelf == false)
            return false;
        DealPlayerDamage.totalTroopCount--;
        gameObject.SetActive(false);
        return true;
    }
    IEnumerator ShowHealthBar()
    {

        healthBarInstance.gameObject.SetActive(true);
        //Debug.Log("Show Health Bar");
        yield return new WaitForSeconds(3.0f);
        healthBarInstance.gameObject.SetActive(false);
    }

    public Vector3 GetTargetPosition() { Assert.IsTrue(target != null); return target.transform.position; }
    public void SetTarget(Unit t) { target = t; }
    //public Unit GetTarget() { return target; }


}
