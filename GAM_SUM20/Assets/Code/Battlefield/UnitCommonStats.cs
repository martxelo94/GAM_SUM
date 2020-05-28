using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class UnitCommonStats : ScriptableObject
{
    public int maxHitPoints;
    public int damage;  // damage to other units
    public float sensorRange;
    public float attackRange;
    public float attackTime;
    public float maxSpeed;
    public float acceleration;
    public int capture_radius;
    public int player_damage; // damage to player
    public ProyectilAttack attackPrefab;

}
