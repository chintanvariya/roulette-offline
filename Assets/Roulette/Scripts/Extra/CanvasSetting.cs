using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace RouletteByFinix
{
    public class CanvasSetting : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private CanvasScaler canvasScaler;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.pointerCurrentRaycast.gameObject.CompareTag("SettingBlack")) return;
            UiManager.Instance.ProfileClick(true);
            Debug.Log($"canvas click {eventData.pointerCurrentRaycast.gameObject.name} {!UiManager.Instance.isClearModeOn} {PaymentHandler.Instance.isChipsMoving}");
            if (!UiManager.Instance.isClearModeOn) return;
            if (PaymentHandler.Instance.isChipsMoving) return;
            UiManager.Instance.ClearModeOnOff();
        }

        private void OnEnable()
        {
            SetMatchRatio();
        }

        private void SetMatchRatio()
        {
            float screenWidth = Screen.width;
            float screenHeight = Screen.height;
            Debug.Log("screenWidth || " + screenWidth + "   screenHeight || " + screenHeight);
            float scaleFactor = screenWidth / screenHeight;
            float standardFactor = 1920f / 1080f;
            canvasScaler.matchWidthOrHeight = (scaleFactor <= standardFactor) ? 0 : 1;
        }
    }
}