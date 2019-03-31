using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon Data", menuName = "Weapons/New Weapon Data")]
public class WeaponData : ScriptableObject
{
    public FireTypes fireType;
    public int magazineCapacity;
    public int burstCount;
    public float fireRate;
    public float fireRange;
    public float reloadTime;
    public string weaponName;
}
