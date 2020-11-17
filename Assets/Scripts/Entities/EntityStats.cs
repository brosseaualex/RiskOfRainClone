using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EntityStats
{
    [Header("Entity Base Stats")]
    public float hp = 100;
    public float maxHp = 100;

    [Header("Entity Weapon Stats")]
    [Range(0.1f, 0.5f)]
    public float rateOfFire = 0.1f;
    public float weaponRange = 100f;
    public float minWeaponDamage = 0;
    public float maxWeaponDamage = 15;
    [HideInInspector]
    public float timeBeforeNextShot;
    public LayerMask gunHitLayers;

    public static EntityStats operator +(EntityStats stats1, EntityStats stats2)
    {
        EntityStats newStats = new EntityStats();

        newStats.hp = stats1.hp + stats2.hp;
        newStats.maxHp = stats1.maxHp + stats2.maxHp;
        if(stats1.rateOfFire != 0.1f)
        {
            newStats.rateOfFire = stats1.rateOfFire - stats2.rateOfFire;
        }        
        newStats.weaponRange = stats1.weaponRange + stats2.weaponRange;
        newStats.minWeaponDamage = stats1.minWeaponDamage + stats2.minWeaponDamage;
        newStats.maxWeaponDamage = stats2.minWeaponDamage + stats1.maxWeaponDamage;
        newStats.gunHitLayers = stats1.gunHitLayers;

        return newStats;
    }
}
