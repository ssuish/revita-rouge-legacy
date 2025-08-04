using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class ProgressBar : MonoBehaviour
{
    public int min;
    public int max = 100;
    public int currentProgress;
    public Image mask;
    public Image fill;
    public Color color;
    public TextMeshProUGUI progressText;

    // Update is called once per frame
    void Update()
    {
        GetCurrentFill();
    }
    
    void GetCurrentFill()
    {
        if (!mask || !fill)
        {
            //Debug.LogError("Mask or Fill is not assigned.");
            return;
        }

        if (max <= min)
        {
            //Debug.LogError("Max should be greater than Min.");
            return;
        }

        
        float currentOffset = currentProgress - min;
        float maxOffset = max - min;
        float fillAmount = currentOffset / maxOffset;
        mask.fillAmount = fillAmount;
        
        Color fillColor = color;
        fillColor.a = color.a; // Explicitly set the alpha value
        fill.color = fillColor;
        
        if (progressText)
        {
            progressText.text = $"{currentProgress}/{max}";
        }
    }
}
