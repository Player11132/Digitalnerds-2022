using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Building", menuName = "New Building", order = 1)]
public class Building : ScriptableObject
{
    public string Building_Name;
    [Tooltip("How many Apartments must exist till this Building Spawns")]
    public int Apartments_till_Spawn;
    public int Consumption;//kwh
    [Tooltip("In P.M 2,5 ug/m3")]
    public float Emmisions;

}
