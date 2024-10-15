using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace RouletteByFinix
{
    public class SettingMenuController : MonoBehaviour
    {

        [Header(" === Setting Menu === ")]
        public GameObject settingBlackBG;
        public RectTransform settingBG;
        public Image settingBGImage;
        public Sprite upperBG, bottomBg;
        public Button settingCloseBtn, settingBtn;
        public Button leaveGameBtn, instructionBtn;
        public UiManager uiManager;

        public RectTransform menuSetting, gamePlaySetting;

        public GameObject leaveBtn, privacyPolicyBtn;
        private void Start()
        {
            leaveGameBtn.onClick.AddListener(delegate
            {
                uiManager.ClickOnLeaveGameBtn();
            });

            instructionBtn.onClick.AddListener(delegate
            {
                uiManager.ClickOnInstructionBtn();
            });
        }

        private void OnEnable()
        {
            UiManager.SettingCloseAction += SettingClose;
        }

        private void OnDisable()
        {
            UiManager.SettingCloseAction -= SettingClose;

        }
        public void SettingClose()
        {
            settingBlackBG.SetActive(false);
            if (GameController.instance.gameState == GameState.DashBoard) return;
            GoogleAdmob.instance.BannerShowAndHide(false);
        }


        public void SettingClick()
        {
            if (uiManager.isScreenMoving) return;
            if (uiManager.wheel.isSpinning) return;
            settingBlackBG.SetActive(true);
            // settingBGImage.sprite = uiManager.isUICanvasAtTop ? upperBG : bottomBg;
            // settingBG.anchoredPosition = new Vector2(settingBG.anchoredPosition.x, uiManager.isUICanvasAtTop ? -14 : -122);
            // settingBG.pivot = new Vector2(0.8f, uiManager.isUICanvasAtTop ? 0 : 1);
            settingBG.localScale = Vector3.zero;

            privacyPolicyBtn.SetActive(false);
            leaveBtn.SetActive(false);
            if (GameController.instance.gameState == GameState.DashBoard)
            {
                settingBG.anchorMin = menuSetting.anchorMin;
                settingBG.anchorMax = menuSetting.anchorMax;
                settingBG.pivot = menuSetting.pivot;
                settingBG.position = menuSetting.position;
                privacyPolicyBtn.SetActive(true);
            }
            else
            {
                settingBG.anchorMin = gamePlaySetting.anchorMin;
                settingBG.anchorMax = gamePlaySetting.anchorMax;
                settingBG.pivot = gamePlaySetting.pivot;
                settingBG.position = gamePlaySetting.position;
                leaveBtn.SetActive(true);
            }

            settingBG.DOScale(Vector3.one, 0.2f).SetEase(Ease.Linear);

            if (GameController.instance.gameState == GameState.DashBoard) return;
            GoogleAdmob.instance.BannerShowAndHide(true);
        }


    }
}
