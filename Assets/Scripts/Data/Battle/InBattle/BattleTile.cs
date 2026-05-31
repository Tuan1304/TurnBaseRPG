using UnityEngine;

public class BattleTile : MonoBehaviour
{
    public Vector2Int gridPos;

    public bool walkable = true;

    [Header("Highlight")]
    [SerializeField]
    private GameObject moveHighlight;

    [SerializeField]
    private GameObject attackHighlight;

    public BattleUnit occupiedUnit;

    public Vector3 GetWorldPosition()
    {
        return transform.position;
    }

    [Header("Obstacle")]
    public bool isBlocked;

    public bool IsOccupied()
    {
        return occupiedUnit != null
            || isBlocked;
    }


    public void HighlightMove(bool value)
    {
        if (moveHighlight != null)
        {
            moveHighlight.SetActive(value);
        }
    }

    public void HighlightAttack(bool value)
    {
        if (attackHighlight != null)
        {
            attackHighlight.SetActive(value);
        }
    }

    private void OnMouseDown()
    {
        if (BattleManager.Instance.IsBusy)
            return;

        BattleManager.Instance.OnTileClicked(this);
    }
}