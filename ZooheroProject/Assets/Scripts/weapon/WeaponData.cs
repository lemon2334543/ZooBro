using System;
using UnityEngine;


[Serializable]
public class WeaponData
{
    public int id;                 // Serializable
    public string name;            // Serializable
    public string avatar;          // Serializable
    public int grade;              // Serializable
    public float damage;           // Serializable
    public int isLong;            // Serializable
    public int range;              // Serializable
    public float critical_strikes_multiple;    // Serializable
    public float critical_strikes_probability; // Serializable
    public float cooling;          // Serializable
    public int repel;              // Serializable
    public string weapon_describe; // Serializable
}