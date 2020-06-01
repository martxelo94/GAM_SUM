using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    [System.Serializable]
[CreateAssetMenu]
public class UnitStats : ScriptableObject
{
    public CardType unitType;   // TODO: separate CardType from UnitType, create UnitType.

    [Header("Movement")]

    //speed
    public float maxSpeed;
    public int capture_radius;


    [Header("Resistance")]

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
    [Header("Attak")]

    // attack and reaction
    public bool rangedAttack;
    public float sensorRange;
    public float attackRange;
    public float attackTime;
    public int player_damage; // damage to player
    public ProyectilAttack[] attackPrefabs;

    public ulong preferedTargetUnit;

}
