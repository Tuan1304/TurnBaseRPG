using UnityEngine;

public class CharacterVisualUI : MonoBehaviour
{
    public CharacterVisual visual;

    public void Setup(CharacterAppearanceData data)
    {
        visual.ApplyAppearance(data);
    }
}