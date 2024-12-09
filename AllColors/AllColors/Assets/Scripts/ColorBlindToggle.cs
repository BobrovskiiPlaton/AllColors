using UnityEngine;
using UnityEngine.Rendering;

public class ColorBlindToggle : MonoBehaviour
{
    private Volume volume;
    private bool isColorBlindMode = false;

    void Start()
    {
        volume = FindObjectOfType<Volume>();
    }

    public void ToggleColorBlindMode()
    {
        if (volume != null)
        {
            volume.enabled = !volume.enabled;
            isColorBlindMode = !isColorBlindMode;
        }
    }
}