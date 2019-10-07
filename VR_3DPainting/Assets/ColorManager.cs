using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorManager : MonoBehaviour
{
    public static ColorManager Instance;
    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }
    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
    private Color color;

    void OnColorChange(HSBColor color2)
    {
        this.color = color2.ToColor();
    }

    public Color GetCurrentColor()
    {
        return this.color;
    }
}
