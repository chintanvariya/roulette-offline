using UnityEngine;
using UnityEngine.UI;
namespace PrahantGames
{
    public class LastNumberDisplay : MonoBehaviour
    {
        public Image pastSpinNumber;
        public Transform anchorPerant;
        [SerializeField] private NumberSprite numSprite;
        private Image pastSpinNumberBg;
      

        [System.Serializable]
        private struct NumberSprite
        {
            public Sprite black;
            public Sprite red;
            public Sprite green;
        }

        public void AddPosition(string Num)
        {
            pastSpinNumberBg = Instantiate(pastSpinNumber, anchorPerant);
            string numberType = PlayerData.Instance.SpinNumberTypeGet(Num);
            pastSpinNumberBg.sprite = (numberType == "red") ? numSprite.red : (numberType == "grey") ? numSprite.black : numSprite.green;
            pastSpinNumberBg.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = Num;
            pastSpinNumberBg.transform.SetSiblingIndex(0);
        }

        private void Start()
        {
            FTUEController.FTUE_ResetAllData += () =>
            {
                if (pastSpinNumberBg != null) DestroyImmediate(pastSpinNumberBg.gameObject);
            };
        }
    }
}