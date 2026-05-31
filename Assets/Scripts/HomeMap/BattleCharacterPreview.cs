using UnityEngine;

public class BattleCharacterPreview : MonoBehaviour
{
    public static BattleCharacterPreview Instance;

    [Header("Preview")]
    public Transform previewSpawnPoint;

    public GameObject previewPrefab;

    [Header("Preview Transform")]
    public float previewScale = 4f;

    private GameObject currentPreview;

    private void Awake()
    {
        Instance = this;
    }

    public void ShowHero(HeroData hero)
    {
        ClearPreview();

        currentPreview =
            Instantiate(
                previewPrefab,
                previewSpawnPoint.position,
                Quaternion.identity);

        currentPreview.transform.localScale =
            Vector3.one * previewScale;

        CharacterVisual visual =
            currentPreview
            .GetComponentInChildren<CharacterVisual>();

        if (visual != null)
        {
            visual.ApplyHero(hero);
        }
    }

    public void ShowEnemy(EnemyData enemy)
    {
        ClearPreview();

        currentPreview =
            Instantiate(
                previewPrefab,
                previewSpawnPoint.position,
                Quaternion.identity);

        currentPreview.transform.localScale =
            Vector3.one * previewScale;

        CharacterVisual visual =
            currentPreview
            .GetComponentInChildren<CharacterVisual>();

        if (visual != null)
        {
            visual.ApplyAppearance(
                enemy.appearance);
        }
    }

    public void ClearPreview()
    {
        if (currentPreview != null)
        {
            Destroy(currentPreview);
        }
    }
}
