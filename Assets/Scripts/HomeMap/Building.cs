using UnityEngine;
using UnityEngine.EventSystems;

public enum BuildingType
{
    Recruit,
    Upgrade,
    Forge,
    Shop,
    Adventure,
    Storage
}

public class Building : MonoBehaviour
{
    public BuildingType type;

    private void Reset()
    {
        EnsureCollider();
    }

    private void Awake()
    {
        EnsureCollider();
    }

    private void OnMouseDown()
    {
        if (EventSystem.current != null &&
            EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        HomeBuildingUIManager.Open(type);
    }

    private void EnsureCollider()
    {
        if (GetComponent<Collider2D>() != null)
            return;

        if (GetComponent<SpriteRenderer>() == null)
            return;

        gameObject.AddComponent<BoxCollider2D>();
    }
}
