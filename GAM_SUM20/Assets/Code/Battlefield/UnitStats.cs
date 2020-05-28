using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class UnitStats : MonoBehaviour
{
    public UnitCommonStats common;
    //[HideInInspector]
    public TeamType team; // set by squad
    [HideInInspector]
    public int currentHitPoints;
    [HideInInspector]
    public float currentAttackTime;
    [HideInInspector]
    public GameObject healthBarInstance;
    [HideInInspector]
    public UnitStats target = null;

    // Start is called before the first frame update
    void Start()
    {
        currentHitPoints = common.maxHitPoints;

        //healthBar.SetActive(false);
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
        if(!healthBarInstance.activeSelf)
            StartCoroutine(ShowHealthBar());
        return false;
    }

    public void Kill()
    {
        //Debug.Log("Unit Killed");
        //Destroy(healthBarInstance);
        Destroy(gameObject);
    }
    IEnumerator ShowHealthBar()
    {
        
        healthBarInstance.SetActive(true);
        //Debug.Log("Show Health Bar");
        yield return new WaitForSeconds(3.0f);
        healthBarInstance.SetActive(false);
    }


}
