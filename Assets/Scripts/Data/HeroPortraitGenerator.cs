using System.Collections;
using UnityEngine;

public class HeroPortraitGenerator : MonoBehaviour
{
    public static HeroPortraitGenerator Instance;

    public Camera snapshotCamera;

    public RenderTexture renderTexture;

    public GameObject previewPrefab;

    public Transform previewPoint;

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

    public IEnumerator GeneratePortrait(HeroData hero)
    {
        if (hero == null ||
            previewPrefab == null ||
            previewPoint == null ||
            renderTexture == null)
        {
            yield break;
        }

        // spawn preview
        GameObject preview =
            Instantiate(previewPrefab,
            previewPoint.position,
            Quaternion.identity);

        CharacterVisual visual =
            preview.GetComponentInChildren<CharacterVisual>();

        if (visual == null)
        {
            Destroy(preview);
            yield break;
        }

        visual.ApplyHero(hero);

        // chờ 1 frame để render
        yield return new WaitForEndOfFrame();

        // đọc texture
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = renderTexture;

        Texture2D tex = new Texture2D(
            renderTexture.width,
            renderTexture.height,
            TextureFormat.ARGB32,
            false);

        tex.ReadPixels(
            new Rect(0, 0, renderTexture.width, renderTexture.height),
            0,
            0);

        tex.Apply();

        RenderTexture.active = currentRT;

        // tạo sprite
        if (hero.portrait != null)
        {
            Destroy(hero.portrait.texture);
            Destroy(hero.portrait);
        }

        hero.portrait = Sprite.Create(
            tex,
            new Rect(0, 0, tex.width, tex.height),
            new Vector2(0.5f, 0.5f));

        Destroy(preview);
    }
}
