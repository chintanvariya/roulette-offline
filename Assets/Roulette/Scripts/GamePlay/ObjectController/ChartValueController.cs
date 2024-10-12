using UnityEngine;
using UnityEngine.UI;

public class ChartValueController : MonoBehaviour
{
    [SerializeField] private Image barImage;
    [SerializeField] private TMPro.TextMeshProUGUI spinNumberText, percentageText;
    [SerializeField] private RectTransform barParent, percentageTextRect;


    public void BarValueDataSet(float percentage, Color barColor, float value)
    {
        barImage.fillAmount = percentage;
        barImage.color = barColor;
        Debug.Log($"percentage {percentage}");
        
        percentageText.text = string.Format("{0:F1}", value * 100);
        percentageTextRect.anchoredPosition = new Vector2(0, (float)barParent.rect.height * percentage);
        percentageTextRect.gameObject.SetActive(true);
    }
    public void SpinNumberTextSet(string spinNumber) => spinNumberText.text = spinNumber;
}
