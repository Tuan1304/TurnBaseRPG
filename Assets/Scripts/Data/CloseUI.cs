using UnityEngine;

public class CloseUI : MonoBehaviour
{
    public GameObject targetUI;

    public void Close()
    {
        targetUI.SetActive(false);
    }
}