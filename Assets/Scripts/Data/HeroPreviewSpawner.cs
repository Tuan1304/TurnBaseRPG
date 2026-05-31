using UnityEngine;

public class HeroPreviewSpawner : MonoBehaviour
{
    public static HeroPreviewSpawner Instance;

    public GameObject previewPrefab;

    public Transform previewPoint;

    private GameObject currentPreview;

    private void Awake()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void ShowPreview(HeroData hero)
    {
        if (hero == null ||
            previewPrefab == null ||
            previewPoint == null)
        {
            return;
        }

        if (currentPreview != null)
        {
            Destroy(currentPreview);
        }

        currentPreview =
            Instantiate(
                previewPrefab,
                previewPoint.position,
                Quaternion.identity);

        CharacterVisual visual =
            currentPreview.GetComponentInChildren<CharacterVisual>();

        if (visual != null)
        {
            visual.ApplyHero(hero);
        }
    }
}
