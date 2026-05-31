using System.Collections;
using TMPro;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    public TextMeshProUGUI damageText;

    private CanvasGroup canvasGroup;

    public float moveSpeed = 50f;

    private void Awake()
    {
        canvasGroup =
            GetComponent<CanvasGroup>();
    }

    public void Setup(int damage)
    {
        damageText.text =
            "-" + damage;

        StartCoroutine(
            PopupRoutine());
    }

    private IEnumerator PopupRoutine()
    {
        float time = 0f;

        Vector3 start =
            transform.position;

        Vector3 end =
            start + Vector3.up * 80f;

        while (time < 1f)
        {
            time += Time.deltaTime;

            transform.position =
                Vector3.Lerp(
                    start,
                    end,
                    time);

            canvasGroup.alpha =
                1f - time;

            yield return null;
        }

        Destroy(gameObject);
    }
}