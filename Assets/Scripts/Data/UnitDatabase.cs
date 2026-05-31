using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "UnitDatabase",
    menuName = "Game/Unit Database")]
public class UnitDatabase
    : ScriptableObject
{
    public List<UnitData> units =
        new List<UnitData>();

    public UnitData GetUnitByID(
        string id)
    {
        foreach (UnitData unit
            in units)
        {
            if (unit.id == id)
            {
                return unit;
            }
        }

        Debug.LogError(
            "Missing Unit ID: " + id);

        return null;
    }
}