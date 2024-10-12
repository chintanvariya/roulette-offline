using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace RouletteByFinix
{
    public class ClearButtonController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private bool isPointerDown = false;
        private bool hasLongPressTriggered = false;
        private float longPressDurationThreshold = 1.0f;
        [SerializeField] private Button myClearButton;
        void Update()
        {
            if (isPointerDown && !hasLongPressTriggered)
            {
                longPressDurationThreshold -= Time.deltaTime;
                if (longPressDurationThreshold <= 0f)
                {
                    hasLongPressTriggered = true;
                    Debug.Log($"Long Pressed");
                    OnLongPress();
                }
            }
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            isPointerDown = true;
            hasLongPressTriggered = false;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isPointerDown = false;
            longPressDurationThreshold = 1.0f;
        }

        private void OnLongPress()
        {
            myClearButton.interactable = false;
            PaymentHandler.Instance.AllClear();
        }

    }
}
