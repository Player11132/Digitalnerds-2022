using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Power Source",menuName ="New Power Source")]
public class Power_Source : ScriptableObject
{
    public string name;
    public int id;
    public int production_power;
    public bool emmits;
    public float emmisions;
    public GameObject Model_Prefab;
    public Vector3[] spawn_points;
    public string[] pros;
    public string[] cons;
}
