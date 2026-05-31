using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "StageData",
    menuName = "Game/Battle/Stage Data")]
public class StageData : ScriptableObject
{
    [Header("Name")]
    public string stageName;

    [Header("Display")]
    public string objective =
        "Tieu diet toan bo";

    public string difficulty =
        "Binh thuong";

    [Header("Reward")]
    public StageReward reward;

    [Header("Wave")]
    public List<WaveData> waves =
        new List<WaveData>();
}
