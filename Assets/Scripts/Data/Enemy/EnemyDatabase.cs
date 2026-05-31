using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "EnemyDatabase",
    menuName = "Game/Enemy Database")]
public class EnemyDatabase : ScriptableObject
{
    public List<EnemyData> enemies;
}