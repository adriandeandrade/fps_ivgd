using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Gun Data", menuName = "Guns/New Gun Data")]
public class GunData : ScriptableObject
{
    public FireTypes fireType;
    public int magazineCapacity;
    public int burstCount;
    public float fireRate;
    public float fireRange;
    public float reloadTime;
    public string gunName;
}
