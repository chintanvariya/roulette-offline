using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace PrahantGames
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
        UiManager uiManager;



        private void Start()
        {

            uiManager = UiManager.Instance;

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
            if(uiManager.wheel.isSpinning) return;
            settingBlackBG.SetActive(true);
           // settingBGImage.sprite = uiManager.isUICanvasAtTop ? upperBG : bottomBg;
           // settingBG.anchoredPosition = new Vector2(settingBG.anchoredPosition.x, uiManager.isUICanvasAtTop ? -14 : -122);
           // settingBG.pivot = new Vector2(0.8f, uiManager.isUICanvasAtTop ? 0 : 1);
            settingBG.localScale = Vector3.zero;
            settingBG.DOScale(Vector3.one, 0.2f).SetEase(Ease.Linear);

            if (GameController.instance.gameState == GameState.DashBoard) return;
            GoogleAdmob.instance.BannerShowAndHide(true);
        }


    }
}
