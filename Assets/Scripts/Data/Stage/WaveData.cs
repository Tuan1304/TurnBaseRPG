using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "WaveData",
    menuName = "Game/Battle/Wave Data")]
public class WaveData : ScriptableObject
{
    public List<StageEnemyData> enemies =
        new List<StageEnemyData>();
}