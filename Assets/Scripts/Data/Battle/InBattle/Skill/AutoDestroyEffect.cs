using UnityEngine;

public class AutoDestroyEffect : MonoBehaviour
{
    public float lifeTime = 1f;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }
}