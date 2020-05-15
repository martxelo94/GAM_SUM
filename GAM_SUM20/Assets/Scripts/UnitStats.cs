using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class UnitStats : MonoBehaviour
{
    [HideInInspector]
    public TeamType team; // set by squad
    public int maxHitPoints;
    [HideInInspector]
    public int hitPoints;
    public int damage;  // damage to other units
    public float range;
    public float attackTime;
    public float maxSpeed;
    public float acceleration;
    public int capture_radius;
    public int player_damage; // damage to player

    [HideInInspector]
    public GameObject healthBarInstance;

    // Start is called before the first frame update
    void Start()
    {
        hitPoints = maxHitPoints;

        //healthBar.SetActive(false);
    }

    public bool ReceiveDamage(int _damage)
    {
        //Debug.Log(name + " receives " + _damage.ToString() + " damage!");
        hitPoints -= _damage;
        if (hitPoints <= 0) {
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
        Destroy(healthBarInstance);
        Destroy(gameObject);
    }
    IEnumerator ShowHealthBar()
    {
        
        healthBarInstance.SetActive(true);
        //Debug.Log("Show Health Bar");
        yield return new WaitForSeconds(3.0f);
        healthBarInstance.SetActive(false);
    }

    private void OnDestroy()
    {
        //StopCoroutine(ShowHealthBar());
        //Destroy(healthBarInstance);
    }
}
