using System.Collections.Generic;
using UnityEngine;

public class HeroVillageSpawner : MonoBehaviour
{
    public static HeroVillageSpawner Instance;

    public HeroEntity heroPrefab;

    public Transform pathParent;

    private List<Transform> pathPoints =
        new List<Transform>();

    private void Awake()
    {
        Instance = this;

        pathPoints.Clear();

        foreach (Transform point in pathParent)
        {
            pathPoints.Add(point);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void SpawnHero(HeroData hero)
    {
        if (pathPoints.Count == 0)
        {
            Debug.LogError("Không có path point!");
            return;
        }

        // spawn NGAY tại point đầu tiên
        HeroEntity entity =
            Instantiate(
                heroPrefab,
                pathPoints[0].position,
                Quaternion.identity
            );

        entity.Setup(hero, pathPoints);
    }
}
