using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RouletteByFinix
{
    public class FTUEController : MonoBehaviour
    {
        public static FTUEController instance;

        public static Action FTUE_ResetAllData;


        [Header(" === Starting Phase ===== ")]
        [SerializeField] private GameObject FTUE, blackWall;
        [SerializeField] private RectTransform startText;
        public Phase ftuePhase;
        [SerializeField] private Button settingBtn;
        [SerializeField] private List<GameObject> boarderListForFTUE;

        [Header("=== Hand Indicator === ")]
        [SerializeField] private GameObject handHolder;
        [SerializeField] private Animator handAnimator, swipeAnimator;
        [SerializeField] private List<Transform> handHoder;

        [Header("== Bet Selection == ")]
        [SerializeField] private Transform betSelectMsgBox;
        [SerializeField] private Coin betChip100;
        [SerializeField] private GameObject betChip100Parent;

        [Header("== Bet Placing == ")]
        [SerializeField] private Transform betPlaceMsgBox;
        [SerializeField] private BoxController fixNumberBox;
        [SerializeField] private BetCenter fixBetCenter;
        [SerializeField] private List<BetCenter> betCenterList;


        [Header("== Spin TO Wheel == ")]
        [SerializeField] private Transform spinWheelMsgBox;
        [SerializeField] private Image spinButton;
        [SerializeField] private Sprite spinBtnPressed, spinBtnUnPressed;
        [SerializeField] private GameObject wheelRoot;


        [Header(" ==== Statistics  === ")]
        [SerializeField] private Transform statisticsMsgBox;
        [SerializeField] private GameObject statisticsBtn;

        [Header(" ==== Screen Swipe  === ")]
        [SerializeField] private Transform screenSwipeMsgBox;
        [SerializeField] private GameObject statisticsBg;

        [Header(" ==== Lets Start  === ")]
        [SerializeField] private Button letsStartBtn;
        [SerializeField] private Transform letsStartMsgBox;

        [Header(" === Skipped To FTUE")]
        private List<Tween> ftueDotween = new List<Tween>();
        [SerializeField] private Button skipBtn;

        private void Awake()
        {
            if (instance == null) instance = this;
            else Destroy(instance);
        }
        private void Start()
        {
            FTUEStartReset();
            GameController.instance.GameStateSet(GameState.FTUE);
            FTUE_ResetAllData += FTUEEndReset;
            FTUE.SetActive(true);
            skipBtn.gameObject.SetActive(true);
            settingBtn.enabled = false;
            StartTextShowing();
            // LetsStartBtn Btn Event Initialize
            letsStartBtn.onClick.AddListener(() =>
            {
                GameController.instance.GameStateSet(GameState.Playing);
                AudioManager.instance.PlayWelcomeClip();
                FTUE_ResetAllData?.Invoke();
                // Check user have balance or not
                UiManager.Instance.ChipsAddPanelMakeOn();
                PlayerData.Instance.InitiallyTotalBalanceSet();
            });

            // Skip Btn Event Initialize
            skipBtn.onClick.AddListener(() => SkipToFTUE());
        }
        public void BoarderHighlightForFTUE(bool isMakeTrue)
        {
            boarderListForFTUE.ForEach(x => x.SetActive(isMakeTrue));
        }

        void FTUEStartReset()
        {
            FTUE.SetActive(false);
            skipBtn.gameObject.SetActive(false);
            startText.gameObject.SetActive(false);
            settingBtn.enabled = true;
        }

        private void StartTextShowing()
        {
            startText.anchoredPosition = new Vector2(-Screen.width - startText.sizeDelta.x, -370);
            startText.gameObject.SetActive(true);
            ftueDotween.Add(startText.DOAnchorPosX(0, 1f).SetEase(Ease.OutBounce).OnComplete(() =>
          {
              ftueDotween.Add(startText.DOAnchorPosX(Screen.width + startText.sizeDelta.x, 1f).SetEase(Ease.InBounce).SetDelay(0.5f).OnComplete(() =>
              {
                  BetSelection();
              }));
          }));
        }

        private void BetSelection()
        {
            ftuePhase = Phase.BetSelection;
            MessageBoxShow(betSelectMsgBox, true);
            ObjectHighlightAndOff(betChip100Parent, true, 13, true);
            HandAnimationMakeOnOff(true);
        }

        private void ResetBetSelection()
        {
            HandAnimationMakeOnOff(false);
            MessageBoxShow(betSelectMsgBox, false);
        }


        Canvas refCanvas = null;
        GraphicRaycaster refGraphicRaycaser = null;
        private void ObjectHighlightAndOff(GameObject highLightObject, bool isMakeHighLight, int sortingOrder = 0, bool isGraphicRayNeed = false)
        {
            Debug.Log($"ObjectHighlightAndOff {isMakeHighLight}  object name {highLightObject.name}");
            refCanvas = GetOrAddComp.GetOrAddComponent<Canvas>(highLightObject);
            if (!isMakeHighLight)
            {
                refGraphicRaycaser = highLightObject.GetComponent<GraphicRaycaster>();
                Debug.Log($"destroy object {highLightObject.name}");
                if (refGraphicRaycaser != null) DestroyImmediate(refGraphicRaycaser);
                if (refCanvas != null) DestroyImmediate(refCanvas);
                return;
            }
            refCanvas.overrideSorting = true;
            Debug.Log($"ObjectHighlightAndOff overriding  {refCanvas.name } {refCanvas.overrideSorting}  sorting order {refCanvas.sortingOrder}");
            refCanvas.sortingOrder = sortingOrder;
            if (isGraphicRayNeed)
                refGraphicRaycaser = GetOrAddComp.GetOrAddComponent<GraphicRaycaster>(highLightObject);
        }

        private void HandAnimationMakeOnOff(bool isMakeON)
        {
            if (isMakeON) handHolder.transform.position = handHoder[(int)ftuePhase].position;
            if (ftuePhase == Phase.ScreenSwipe)
            {
                handHolder.transform.Rotate(0f, 180f, 0f);
            }else handHolder.transform.Rotate(0f, 0, 0f);
            handHolder.SetActive(isMakeON);
            handAnimator.enabled = isMakeON;
            handAnimator.gameObject.SetActive(isMakeON);
            //(ftuePhase == Phase.ScreenSwipe ? swipeAnimator : handAnimator).enabled = isMakeON;
            //(ftuePhase == Phase.ScreenSwipe ? swipeAnimator : handAnimator).gameObject.SetActive(isMakeON);
        }

        private void MessageBoxShow(Transform msgBox, bool isMakeShow)
        {
            if (!isMakeShow)
            {
                msgBox.gameObject.SetActive(false);
                ftueDotween.ForEach(refTween => refTween.Kill());
                ftueDotween.Clear();
                return;
            }

            msgBox.localScale = new Vector2(0, 1);
            msgBox.gameObject.SetActive(true);
            ftueDotween.Add(msgBox.DOScaleX(1, 0.5f).SetEase(Ease.OutBounce));
        }

        public void HandTouchToObject()
        {
            switch (ftuePhase)
            {
                case Phase.BetSelection:
                    betChip100.IpoiterEnter();
                    break;
                case Phase.BetPlace:
                    fixNumberBox.highLightBox.gameObject.SetActive(true);
                    break;
                case Phase.SpinToWheel:
                    spinButton.sprite = spinBtnPressed;
                    break;
            }
        }
        public void OnHandClipCompleted()
        {
            switch (ftuePhase)
            {
                case Phase.BetSelection:
                    betChip100.IpoiterExit();
                    break;
                case Phase.BetPlace:
                    fixNumberBox.highLightBox.gameObject.SetActive(false);
                    break;
                case Phase.SpinToWheel:
                    spinButton.sprite = spinBtnUnPressed;
                    break;
            }
        }

        // 3. Place the bet
        public void GetCallbackOnChooseBet()
        {
            ResetBetSelection();
            BetPlacing();
        }
        public void BetCenterForFTUE(bool isMakeTrue)
        {
            betCenterList.ForEach(x => x.enabled = isMakeTrue);
        }

        private void BetPlacing()
        {
            ftuePhase = Phase.BetPlace;
            ObjectHighlightAndOff(fixNumberBox.gameObject, true, 13, true);
            ObjectHighlightAndOff(fixBetCenter.gameObject, true, 14, true);
            BoarderHighlightForFTUE(true);
            BetCenterForFTUE(false);
            HandAnimationMakeOnOff(true);
            MessageBoxShow(betPlaceMsgBox, true);
        }

        private void ResetBetPlacing()
        {
            HandAnimationMakeOnOff(false);
            MessageBoxShow(betPlaceMsgBox, false);
        }

        // 4. Spin the ball

        public void GetCallbackOnPlaceBet()
        {
            ResetBetPlacing();
            SpinToWheel();
            fixBetCenter.enabled = false;
        }

        private void SpinToWheel()
        {
            ftuePhase = Phase.SpinToWheel;
            HandAnimationMakeOnOff(true);
            ObjectHighlightAndOff(spinButton.gameObject, true, 13, true);
            MessageBoxShow(spinWheelMsgBox, true);

        }
        private void ResetSpinToWheel()
        {
            ObjectHighlightAndOff(spinButton.gameObject, false);
            HandAnimationMakeOnOff(false);
            MessageBoxShow(spinWheelMsgBox, false);
            spinButton.sprite = spinBtnUnPressed;
        }

        public void GetCallbackOnSpinToWheel()
        {
            skipBtn.gameObject.SetActive(false);
            ResetSpinToWheel();
            WheelSpinning();
        }

        private void WheelSpinning()
        {
            ftuePhase = Phase.WheelSpinning;
           // ObjectHighlightAndOff(wheelRoot, true, 13, true);
        }

        public void GetCallbackOnWheelSpinning()
        {
            ResetWheelSpinning();
            SpinResult();
        }

        private void ResetWheelSpinning()
        {
            Debug.Log($"ResetWheelSpinning");
           // ObjectHighlightAndOff(wheelRoot, false);
            ObjectHighlightAndOff(betChip100Parent, false);
            ObjectHighlightAndOff(fixNumberBox.gameObject, false);
            ObjectHighlightAndOff(fixBetCenter.gameObject, false);
            BoarderHighlightForFTUE(false);
        }

        private void SpinResult()
        {
            blackWall.SetActive(false);
            ftuePhase = Phase.SpinResult;
        }

        public void GetCallbackOnEndOfResult()
        {
            // Statistics();
           // skipBtn.gameObject.SetActive(true);
            GetCallbackOnSwipe();
        }

        private void Statistics()
        {
            blackWall.SetActive(true);
            ftuePhase = Phase.Statistics;
            ObjectHighlightAndOff(statisticsBtn, true, 13, true);
            HandAnimationMakeOnOff(true);
            MessageBoxShow(statisticsMsgBox, true);
        }

        private void ResetStatistics()
        {
            ObjectHighlightAndOff(statisticsBtn, false);
            HandAnimationMakeOnOff(false);
            MessageBoxShow(statisticsMsgBox, false);
            fixBetCenter.enabled = true;
        }

        public void GetCallbackOnClickOfStatistics()
        {
            ResetStatistics();
            ScreenSwipe();
            BetCenterForFTUE(true);
           
        }

        private void ScreenSwipe()
        {
            ftuePhase = Phase.ScreenSwipe;
            ObjectHighlightAndOff(statisticsBg, true, 13, true);
            HandAnimationMakeOnOff(true);
            MessageBoxShow(screenSwipeMsgBox, true);
        }

        public void GetCallbackOnSwipe()
        {
            ResetScreenSwipe();
            LetsStartGame();
            // FTUE Mode is Over
            // Reste All Data .
        }
        private void ResetScreenSwipe()
        {
            ObjectHighlightAndOff(statisticsBg, false);
            HandAnimationMakeOnOff(false);
            MessageBoxShow(screenSwipeMsgBox, false);
            blackWall.SetActive(true);
        }

        private void LetsStartGame()
        {
            ftuePhase = Phase.LetsStart;
            HandAnimationMakeOnOff(true);
            MessageBoxShow(letsStartMsgBox, true);
        }

        private void ResetLetsStartGame()
        {
            HandAnimationMakeOnOff(false);
            MessageBoxShow(letsStartMsgBox, false);
        }
        private void FTUEEndReset()
        {
            ResetLetsStartGame();
            FTUEStartReset();
            blackWall.SetActive(false);
            fixBetCenter.enabled = true;
            ftueDotween.ForEach(refTween => refTween.Kill());
        }

        public void SkipToFTUE()
        {
            if (UiManager.Instance.wheel.isSpinning) return;
            FTUEStartReset();
            ResetBetSelection();
            ResetBetPlacing();
            ResetSpinToWheel();
            ResetWheelSpinning();
            ResetStatistics();
            ResetScreenSwipe();
            ResetLetsStartGame();
            letsStartBtn.onClick.Invoke();
        }
        public static class GetOrAddComp
        {
            public static T GetOrAddComponent<T>(GameObject givenObject) where T : Component
            {
                if (givenObject.TryGetComponent<T>(out T t))
                {
                    return t;
                }
                else
                {
                    return givenObject.AddComponent<T>();
                }
            }
        }

        public enum Phase
        {
            BetSelection,
            BetPlace,
            SpinToWheel,
            WheelSpinning,
            SpinResult,
            Statistics,
            ScreenSwipe,
            LetsStart
        }
    }
}
