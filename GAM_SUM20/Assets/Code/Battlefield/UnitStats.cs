using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class UnitStats : ScriptableObject
{
    public int maxHitPoints;
    // protection types (percentages)
    [Range(0, 100)]
    public float pierceArmor;
    [Range(0, 100)]
    public float slashArmor;

    // damage types (on Attack)
    // public float pierceDamage;
    // public float slashDamage;
    // public float blastDamage;

    // attack and reaction
    public bool rangedAttack;
    public float sensorRange;
    public float attackRange;
    public float attackTime;
    //speed
    public float maxSpeed;
    public int capture_radius;
    public int player_damage; // damage to player
    public ProyectilAttack[] attackPrefabs;

}
