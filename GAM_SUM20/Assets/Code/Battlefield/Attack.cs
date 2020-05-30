using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Attack : ScriptableObject
{
    // damage types
    public float pierceDamage;
    public float slashDamage;

    public float attackRange = 1.0f;

    public bool damage_falloff = true;
    public bool areaDamage; // does damage by circle check or instant to target?

    public GameObject effectPrefab;

    // Start is called before the first frame update
    public void SpawnAttack(Vector3 attackPosition, Vector3 sourcePosition, TeamType sourceTeam, Unit target)
    {
        if (areaDamage)
        {
            LayerMask targetLayerMask = LayerMask.GetMask(sourceTeam == TeamType.Player ? "Opponent" : "Player");
            RaycastHit2D[] hits = Physics2D.CircleCastAll(attackPosition, attackRange, Vector2.zero, 0f, targetLayerMask);
            for (int i = 0; i < hits.Length; ++i)
            {
                Unit unit = hits[i].transform.GetComponent<Unit>();
                if (unit != null)
                {
                    if (unit.ReceiveDamage((int)CalculateDamage(unit, attackPosition)))
                    {
                        // source unit might be already killed
                        //Debug.Log(unit.name + " killed by " + invocator.name);
                    }
                    //Debug.Log("Unit " + unit.name + " receive " + damage + " damage.");
                }

            }
        }
        // target still alive, do instant damage
        else if(target != null){
            if (target.ReceiveDamage((int)CalculateDamage(target, attackPosition)))
            {
                // source unit might be already killed
                //Debug.Log(target.name + " killed by " + invocator.name);
            }
        }
        if (effectPrefab != null) {
            GameObject inst = Instantiate(effectPrefab);
            inst.transform.position = attackPosition;
            inst.transform.up = (attackPosition - sourcePosition).normalized;
            inst.transform.localScale *= attackRange * 2;
        }
    }


    float CalculateDamage(Unit targetUnit, Vector3 attackPosition)
    {
        UnitStats coms = targetUnit.common;
        float total_damage = pierceDamage - pierceDamage * coms.pierceArmor * 0.01f
            + slashDamage - slashDamage * coms.slashArmor * 0.01f;

        if (damage_falloff)
        {
            // compute damage by distance
            Vector3 dif = targetUnit.transform.position - attackPosition;
            float dist2 = dif.sqrMagnitude;
            float maxDist2 = attackRange * attackRange;
            float finalDamage = total_damage - total_damage * (dist2 / maxDist2);
            finalDamage = Mathf.Max(finalDamage, 0);

            return finalDamage;
        }
        return total_damage;
    }

}
