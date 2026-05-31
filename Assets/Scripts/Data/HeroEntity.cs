using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroEntity : MonoBehaviour
{
    public HeroData heroData;

    private List<Transform> pathPoints;

    private CharacterVisual visual;

    public bool IsMoving { get; private set; }
    private Transform visualRoot;

    public float moveSpeed = 3f;

    private HeroVillageState currentState;
    private Vector3 wanderTarget;

    private float idleTimer;

    private List<Vector3> currentPath;

    private int pathIndex;

    public void Setup(HeroData data, List<Transform> paths)
    {
        heroData = data;

        pathPoints = paths;

        CharacterVisual visual =
            GetComponentInChildren<CharacterVisual>();

        visual.ApplyHero(heroData);

        visualRoot = visual.transform;

        transform.position = pathPoints[0].position;

        currentState = HeroVillageState.EnterVillage;

        StartCoroutine(MoveAlongPath());
    }

    public void RefreshVisual()
    {
        CharacterVisual visual =
            GetComponentInChildren<CharacterVisual>();

        if (visual == null ||
            heroData == null)
        {
            return;
        }

        visual.ApplyHero(heroData);
    }

    IEnumerator MoveAlongPath()
    {
        IsMoving = true;

        for (int i = 0; i < pathPoints.Count; i++)
        {
            Vector3 target = pathPoints[i].position;

            while (Vector3.Distance(transform.position, target) > 0.05f)
            {
                Vector3 dir =
                    target - transform.position;

                // flip trái phải
                if (dir.x > 0.05f)
                {
                    // đi phải
                    visualRoot.localScale =
                        new Vector3(-1, 1, 1);
                }
                else if (dir.x < -0.05f)
                {
                    // đi trái
                    visualRoot.localScale =
                        new Vector3(1, 1, 1);
                }

                transform.position =
                    Vector3.MoveTowards(
                        transform.position,
                        target,
                        moveSpeed * Time.deltaTime
                    );

                yield return null;
            }
        }

        IsMoving = false;

        currentState = HeroVillageState.Idle;

        idleTimer = Random.Range(2f, 5f);
    }

    private void Update()
    {
        HandleVillageAI();
    }

    private void HandleVillageAI()
    {
        if (currentState == HeroVillageState.Idle)
        {
            idleTimer -= Time.deltaTime;

            if (idleTimer <= 0)
            {
                FindRandomDestination();
            }
        }
        else if (currentState == HeroVillageState.Wander)
        {
            MoveToTarget();
        }
    }

    private void FindRandomDestination()
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPos =
                transform.position +
                new Vector3(
                    Random.Range(-5, 6),
                    Random.Range(-5, 6),
                    0
                );

            if (GridManager.Instance.IsWalkable(randomPos))
            {
                // wanderTarget =
                //     GridManager.Instance.GetCellCenter(randomPos);

                // currentState =
                //     HeroVillageState.Wander;
                currentPath =
                    AStarPathfinder.Instance.FindPath(
                        transform.position,
                        randomPos);

                if (currentPath != null &&
                    currentPath.Count > 0)
                {
                    pathIndex = 0;

                    currentState =
                        HeroVillageState.Wander;

                    IsMoving = true;

                    return;
                }

                IsMoving = true;

                return;
            }
        }

        currentState = HeroVillageState.Idle;

        idleTimer = 2f;
    }

    private void MoveToTarget()
    {
        if (currentPath == null ||
            pathIndex >= currentPath.Count)
        {
            IsMoving = false;

            currentState =
                HeroVillageState.Idle;

            idleTimer =
                Random.Range(2f, 5f);

            return;
        }

        Vector3 target =
            currentPath[pathIndex];

        Vector3 dir =
            target - transform.position;

        // flip
        if (dir.x > 0.05f)
        {
            visualRoot.localScale =
                new Vector3(-1, 1, 1);
        }
        else if (dir.x < -0.05f)
        {
            visualRoot.localScale =
                new Vector3(1, 1, 1);
        }

        transform.position =
            Vector3.MoveTowards(
                transform.position,
                target,
                moveSpeed * Time.deltaTime
            );

        if (Vector3.Distance(
            transform.position,
            target) < 0.05f)
        {
            pathIndex++;
        }
    }
}
public enum HeroVillageState
{
    EnterVillage,
    Idle,
    Wander
}
