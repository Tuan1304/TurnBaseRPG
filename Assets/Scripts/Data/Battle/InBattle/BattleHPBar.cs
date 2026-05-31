using UnityEngine;
using UnityEngine.UI;

public class BattleHPBar : MonoBehaviour
{
    public Image fillImage;

    [SerializeField]
    private Color playerColor = Color.blue;

    [SerializeField]
    private Color enemyColor = Color.red;

    private BattleUnit target;

    private Camera cam;

    public Vector3 offset =
        new Vector3(0, 0.1f, 0);

    private void Start()
    {
        cam = Camera.main;

        if (cam == null)
        {
            Debug.LogError("Main Camera not found!");
        }

        transform.SetAsFirstSibling();

        Graphic[] graphics =
            GetComponentsInChildren<Graphic>(
                true);

        foreach (Graphic graphic
            in graphics)
        {
            graphic.raycastTarget = false;
        }
    }

    public void Setup(BattleUnit unit)
    {
        if (unit.teamType == TeamType.Player)
        {
            fillImage.color = playerColor;
        }
        else
        {
            fillImage.color = enemyColor;
        }

        target = unit;

        Refresh();
    }

    private void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        transform.position =
            cam.WorldToScreenPoint(
                target.transform.position + offset);
    }

    public void Refresh()
    {
        fillImage.fillAmount =
            (float)target.currentHP /
            target.runtimeStats.hp;
    }
}
