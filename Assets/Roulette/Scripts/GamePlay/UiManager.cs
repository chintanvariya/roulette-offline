using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace RouletteByFinix
{
    public class UiManager : MonoBehaviour
    {
        public RectTransform uiCanvasBG;

        [SerializeField] private Transform chipAddParent;

        [Header(" === Setting Menu === ")]
        public static Action SettingCloseAction;
        public SettingMenuController gamePlaySetting;
        public SettingMenuController dashBoardSetting;

        [Header("Scripts")]
        public PaymentHandler paymentHandler;
        public Ball ball;
        public Wheel wheel;

        [Header("Controls Buttons")]
        public Button spinBtn;
        public Button undoBtn;
        public Button rebetBtn;
        public Button clearBtn;

        private Camera _camera;

        [Header(" === Statistics Button ==== ")]
        [SerializeField] private RectTransform statisticsScreen;
        [SerializeField] private Button statisticsBtn;
        [SerializeField] private Button fullStatisticsBtn;
        [SerializeField] private Button fullStatisticsCloseBtn;

        [Header(" ==== Mini Stats ===")]
        [SerializeField] private GameObject miniStats;
        [SerializeField] private GameObject miniStatsContent;


        [Header("==== Instruction === ")]
        [SerializeField] private RectTransform instructionScreen;
        [SerializeField] private Button instructionCloseBtn;

        [Header("==== Leave Game === ")]
        [SerializeField] private RectTransform leaveGameScreen;
        // [SerializeField] private Button leaveGameBtn;
        [SerializeField] private Button leaveYesBtn, leaveNoBtn;

        [Header("=== Ads Not Load === ")]
        [SerializeField] private RectTransform adsNotLoadScreen;
        [SerializeField] private Button adsNotLoadCloseBtn;



        public static UiManager Instance;

        private void Awake()
        {
            Instance = this;
            Input.multiTouchEnabled = false;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Debug.Log($"device back btn pressed");
                if (GameController.instance.gameState != GameState.Playing) return;
                if (wheel.isSpinning) return;
                if (statisticsScreen.gameObject.activeInHierarchy)
                {
                    Debug.Log($"statisticsScreen off");
                    fullStatisticsCloseBtn.onClick.Invoke();
                }
                else if (instructionScreen.gameObject.activeInHierarchy)
                {
                    Debug.Log($"instructionScreen off");
                    instructionCloseBtn.onClick.Invoke();
                }
                else if (adsNotLoadScreen.gameObject.activeInHierarchy)
                {
                    Debug.Log($"ads no load off");
                    adsNotLoadCloseBtn.onClick.Invoke();
                }
                else if (gamePlaySetting.settingBlackBG.activeInHierarchy || dashBoardSetting.settingBlackBG.activeInHierarchy)
                {
                    Debug.Log($"settingBlackBG off");
                    SettingCloseAction?.Invoke();
                }
                else if (leaveGameScreen.gameObject.activeInHierarchy)
                {
                    Debug.Log($"leaveGameScreen off");
                    leaveNoBtn.onClick.Invoke();
                }
                else
                {
                    Debug.Log($"leave game on");
                    // leaveGameBtn.onClick.Invoke();
                    ScreenMakeOnOff(true, leaveGameScreen);
                }
            }
        }

        private void Start()
        {
            InitiallyDataSet();

            ButtonEventInitialize();
            FTUEController.FTUE_ResetAllData += () =>
            {
                ProfileClick(true);
                rebetBtn.interactable = false;
                undoBtn.interactable = false;
                clearBtn.interactable = false;
            };

            Application.targetFrameRate = 90;
            _camera = Camera.main;
            Debug.Log($"NetworkChecking");
            GoogleAdmob.instance.NetworkChecking();
        }

        private void ButtonEventInitialize()
        {
            fullStatisticsBtn.onClick.AddListener(delegate
            {
                Debug.Log($"fullStatisticsBtn clcik");
                ScreenMakeOnOff(true, statisticsScreen);
            });
            leaveYesBtn.onClick.AddListener(GameLeave);
        }

        public void SpinClick()
        {
            if (isScreenMoving) return;
            if (paymentHandler.isChipsMoving) return;
            if (isClearModeOn)
                ClearModeOnOff();

            if (GameController.instance.IsFTUEgameStateOn())
            {
                FTUEController.instance.GetCallbackOnSpinToWheel();
            }
            SpinPlay();
        }

        public void RebetClick()
        {
            if (paymentHandler.isChipsMoving) return;
            rebetBtn.interactable = false;

            paymentHandler.RebetClick();
        }


        public bool isUICanvasAtTop = false;

        private bool isActive;
        public bool isScreenMoving = false;

        [Header(" === Update Wheel Show === ")]
        [SerializeField] private GameObject wheelBg;
        [SerializeField] private RectTransform wheelMain;
        public Ease currentEase;


        Sequence wheelPosition;
        Sequence wheelScale;

        private void UICanvasMakeUpAndDown(float timeDuration, bool isSpinPlay, Action endCallback = null)
        {
            if (isSpinPlay)
            {
                if (!isUICanvasAtTop)
                {
                    wheelMain.transform.localScale = Vector2.zero;
                    wheelBg.SetActive(true);
                }

                wheelPosition = DOTween.Sequence().SetAutoKill(true); ;
                wheelScale = DOTween.Sequence().SetAutoKill(true); ;


                wheelPosition.Append(wheelMain.DOAnchorPos(new Vector2(0, 0), !isUICanvasAtTop ? 0.3f : 0.6f).SetEase(Ease.Linear)); ;
                wheelPosition.Append(wheelMain.DOAnchorPos(!isUICanvasAtTop ? Vector2.zero : new Vector2(650, 478), !isUICanvasAtTop ? 0.6f : 0.3f).SetEase(Ease.Linear));

                wheelScale.Append(wheelMain.DOScale(Vector2.one * 0.5f, !isUICanvasAtTop ? 0.3f : 0.6f).SetEase(Ease.Linear));
                wheelScale.Append(wheelMain.DOScale(!isUICanvasAtTop ? Vector2.one * 1.75f : Vector2.zero, !isUICanvasAtTop ? 0.6f : 0.3f).SetEase(Ease.OutFlash));


                wheelScale.OnStart(() =>
                {
                    isScreenMoving = true;
                    Debug.Log($"isUICanvasAtTop start {isUICanvasAtTop}");
                }).OnComplete(() =>
                {
                    Debug.Log($"isUICanvasAtTop end {isUICanvasAtTop}");

                    isScreenMoving = false;
                    wheelBg.SetActive(!isUICanvasAtTop);
                    isUICanvasAtTop = !isUICanvasAtTop;
                    endCallback?.Invoke();
                });

                return;
            }



        }

        private Vector2 GetSpinFocusValue()
        {
            float standardRatio = (float)16 / 9;
            float currentRatio = (float)Screen.width / Screen.height;
            Debug.Log($"standardRatio {standardRatio} currentRatio {currentRatio} ");
            return new Vector2(755 * (currentRatio / standardRatio), -1042);
        }

        public void ProfileClick(bool isCanvasClick)
        {
            Debug.Log($"ProfileClick 0");
            if (isCanvasClick && !isActive) return;

            if (isScreenMoving) return;
            UICanvasMakeUpAndDown(0.7f, false);
            isActive = !isActive;
            SettingCloseAction?.Invoke();
        }

        public void MiniStatsShow(bool isShow)
        {
            if (GameController.instance.IsFTUEgameStateOn())
            {
                if (isShow) FTUEController.instance.GetCallbackOnClickOfStatistics();
                else FTUEController.instance.GetCallbackOnSwipe();
            }

            Tween miniStatsTween;
            miniStats.SetActive(isShow);

            if (!isShow) return;

            miniStatsContent.transform.localScale = Vector2.zero;
            miniStatsTween = miniStatsContent.transform.DOScale(Vector2.one, 0.3f).SetEase(Ease.OutFlash);

            miniStatsTween.OnStart(() =>
            {
                isScreenMoving = true;
            });

            miniStatsTween.OnComplete(() =>
            {
                isScreenMoving = false;
            });
        }


        private void StatisticsBtnMakeOnOff(bool isMakeON/*, bool isStatisticsBtnClick = false*/)
        {
            fullStatisticsBtn.interactable = isMakeON;
            statisticsBtn.interactable = isMakeON;
            // statisticsBtn.interactable = isMakeON ? isMakeON : isStatisticsBtnClick;
        }

        public void UndoClick()
        {
            if (paymentHandler.isChipsMoving) return;
            paymentHandler.Undo(true);
        }

        public void ClearClick()
        {
            if (paymentHandler.isChipsMoving) return;
            ClearModeOnOff();
            // clearBtn.interactable = false;
            paymentHandler.Clear();
        }


        public bool isClearModeOn = false;
        public bool isAllClearModeOn = false;
        public void ClearModeOnOff()
        {
            isClearModeOn = !isClearModeOn;
            ClearHighLightMakeOnOff(isClearModeOn);
            for (int i = 0; i < paymentHandler.storeBetPiece.Count; i++)
            {
                Debug.Log($"ClearModeOnOff || storeBetPiece {paymentHandler.storeBetPiece[i].gameObject.name}");
                for (int j = 0; j < paymentHandler.storeBetPiece[i].chipsList.Count; j++)
                {
                    Debug.Log($"ClearModeOnOff || chip {paymentHandler.storeBetPiece[i].chipsList[j].name}");
                    paymentHandler.storeBetPiece[i].chipsList[j].DeleteBtnMakeOnOff(isClearModeOn);
                }
            }
            AudioManager.instance.PlayButtonClick();
        }




        private void SpinPlay()
        {
            if (wheel.isSpinning) return;

            if (!IsSpinAllow()) return;

            // Show Interstitial Ads Every 4 Game Play;
            PlayerData.Instance.InterstitialShow();
            // Setting Btn Make Off
            gamePlaySetting.settingBtn.interactable = false;

            StatisticsBtnMakeOnOff(false/*, false*/);

            spinBtn.interactable = false;
            rebetBtn.interactable = false;
            undoBtn.interactable = false;
            clearBtn.interactable = false;
            // Play Spining Clip
            AudioManager.instance.PlayAndStopSpinning(true);
            AudioManager.instance.PlayNoMoreBetClip();
            UICanvasMakeUpAndDown(0.3f, true, () =>
            {
                ball.SpinBall();
            });
            //ball.SpinBall();

            // Chips cutting Balance store
            Debug.Log($"Chips cutting Balance store");
            StaticData.totalBalance = PlayerData.Instance.BalanceGetFromText();

            // Dashboard statistics add
            if (!GameController.instance.IsFTUEgameStateOn())
            {
                PlayerData.Instance.TotalGameAdd();
                PlayerData.Instance.LossChipsAdd(+CoinsManager.instance.betValue);
            }

            // saveLastbetlist => list should be not clear when user spin empty
            if (paymentHandler.checkValue.Count != 0)
                paymentHandler.saveLastbetlist = paymentHandler.checkValue.Select(x => x).ToList();
        }

        IEnumerator resetGameCR;
        public void ResetGameCall()
        {
            if (resetGameCR != null)
            {
                StopCoroutine(resetGameCR);
            }
            resetGameCR = ResetGame();
            StartCoroutine(resetGameCR);
        }

        private IEnumerator ResetGame() // RESET GAME
        {
            yield return new WaitForSeconds(1.5f);
            // FTUE 
            if (GameController.instance.IsFTUEgameStateOn())
            {
                FTUEController.instance.GetCallbackOnWheelSpinning();
            }
            // Stop Spining Clip
            UICanvasMakeUpAndDown(1, true, () =>
              {
                  // Statistics Button make off for FTUE mode
                  if (GameController.instance.IsFTUEgameStateOn())
                  {
                      StatisticsBtnMakeOnOff(false);
                  }
                  WinningBoxHighlightAnim(ball.num);
                  NetWinLossPopupMakeOnOff(true);

              });
            gamePlaySetting.settingBtn.onClick.Invoke();
            instructionCloseBtn.onClick.Invoke();
            leaveNoBtn.onClick.Invoke();

            // Ball Data Reset
            ball.BallResetData();

            yield return new WaitForSeconds(0.1f);
            //ball.transform.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
            anchorNumbarShow.gameObject.SetActive(false);

            // payment script reset
            PaymentHandler.Instance.CalculatePayAmount(ball.num);
            yield return new WaitForSeconds(2.4f);
            // If Chips less than zero then chips add popup make on
            SpinEndReset();
            ChipsAddPanelMakeOn();


        }

        public void SpinEndReset()
        {
            if (GameController.instance.IsFTUEgameStateOn())
            {
                StatisticsBtnMakeOnOff(true);
                FTUEController.instance.GetCallbackOnEndOfResult();
            }
            Debug.Log($"SpinEndReset");
            paymentHandler.ResetPaySystem(true);
            Debug.Log($"wheen spinning reset {wheel.isSpinning}");
            wheel.isSpinning = false;
            // Setting Btn Make ON
            gamePlaySetting.settingBtn.interactable = true;
            spinBtn.interactable = true;
            if (paymentHandler.rebetDatas.Count != 0 && CoinsManager.instance.IsTotalBalanceMoreThanZero())
            {
                rebetBtn.interactable = true;
            }
            WinningBoxHighlightAnimKill();
            NetWinLossPopupMakeOnOff(false);
        }

        public void ScreenMakeOnOff(bool isShow, RectTransform givenScreenObject)
        {
            if (isScreenMoving) return;
            givenScreenObject.gameObject.SetActive(isShow);
            if (isShow)
            {
                PanelOnAnimtion(givenScreenObject.gameObject);
                if (givenScreenObject == statisticsScreen) PlayerData.Instance.HeaderStatisticsSet();
                _ = givenScreenObject.DOAnchorPosY(isUICanvasAtTop ? 632 : 0, 0f);
            }
        }

        private void PanelOnAnimtion(GameObject panel)
        {
            panel.SetActive(true);
            GameObject childContent = panel.transform.GetChild(0).gameObject;
            childContent.transform.localScale = Vector2.zero;
            _ = childContent.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutFlash);
        }

        private void GameLeave()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            // DashBoardPanleOpen();
            //#if UNITY_EDITOR
            //            UnityEditor.EditorApplication.isPlaying = false;
            //#endif
            //#if PLATFORM_ANDROID

            //            Application.Quit();
            //#endif
        }

        public void ClickOnLeaveGameBtn()
        {
            ScreenMakeOnOff(true, leaveGameScreen);
        }

        public void ClickOnInstructionBtn()
        {
            ScreenMakeOnOff(true, instructionScreen);
        }

        public void AdsNotLoadPopup()
        {
            ScreenMakeOnOff(true, adsNotLoadScreen);
        }


        [Header(" === Alert Message ==== ")]
        [SerializeField] private List<Coin> betCoinList;
        [SerializeField] private Transform msgAlertPanel, msgAlertRoot;
        [SerializeField] private TextMeshProUGUI msgText;
        private static string noMoreChips = "You have no more chips for bet.";
        private static string placeBets = "Please place your bets.";
        private static string noInternet = "Internet connection is not found.";
        private static string netCheckAPI = "https://www.baps.org/";

        public void AlertMsgPopupOpen(bool isShow, string msg)
        {
            msgAlertPanel.gameObject.SetActive(isShow);
            if (isShow)
            {
                msgAlertRoot.localScale = Vector2.zero;
                msgText.text = msg;
                msgAlertRoot.DOScale(Vector3.one, 0.7f).SetEase(Ease.OutBounce).OnComplete(() =>
                {
                    msgAlertRoot.DOScale(Vector3.zero, 0f).SetDelay(0.8f).OnComplete(() => AlertMsgPopupOpen(false, msg));
                });
            }
        }

        public IEnumerator CheckNetwork(bool isContinueCall, Action<bool> NetworkCallback = null)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                NetworkCallback?.Invoke(false);
                if (!isContinueCall) AlertMsgPopupOpen(true, noInternet);
                yield break;
            }


            using (UnityWebRequest www = UnityWebRequest.Head(netCheckAPI))
            {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    NetworkCallback?.Invoke(true);
                }
                else
                {
                    NetworkCallback?.Invoke(false);
                    if (!isContinueCall) AlertMsgPopupOpen(true, noInternet);
                }
            }
        }

        private bool IsNetworkAvailable(bool isNetAvail) => isNetAvail;

        [Header(" ===== Box Highlight === ")]
        public List<BoxController> numberBoxList = new List<BoxController>();
        [SerializeField] private ParticleSystem winnerStarParticlePrefab;
        private ParticleSystem refParticle;
        private void NumbervalueAssignToHighLight(GameMode currentGameMode)
        {
            if (currentGameMode.Equals(GameMode.American))
            {
                numberBoxList[0].highLightBox.numberValue = "00";
            }

            for (int i = 0; i <= 36; i++)
            {
                numberBoxList[currentGameMode.Equals(GameMode.American) ? i + 1 : i].highLightBox.numberValue = i.ToString();
            }
        }

        // Winner Box Highlight
        GameObject spinNumberHighlightBox;
        private void WinningBoxHighlightAnim(string spinNum)
        {
            spinNumberHighlightBox = numberBoxList.Single(x => x.highLightBox.numberValue == spinNum).highLightBox.gameObject;
            spinNumberHighlightBox.transform.DOScale(Vector3.one, 0.2f).SetLoops(14).OnStepComplete(HighLight);
            WinningNumberParticleInitiate(paymentHandler.checkValue, spinNum);

            void HighLight()
            {
                spinNumberHighlightBox.SetActive(!spinNumberHighlightBox.activeInHierarchy);
            }
        }

        private void WinningBoxHighlightAnimKill()
        {
            spinNumberHighlightBox?.transform.DOKill();
            spinNumberHighlightBox?.SetActive(false);
        }

        // Winning Paricle Initiate
        private void WinningNumberParticleInitiate(List<BetCenter> betCenterList, string spinNumber)
        {
            Debug.Log($"mybetCenter 1");

            BetCenter mybetCenter = null;// = betCenterList.Single(x => x.betData.isSelected && x.winningObject.pieceNumber.Contains(spinNumber));

            List<BetCenter> refCenterBetList = betCenterList.FindAll(bet => bet.betData.isSelected && bet.winningObject.pieceNumber.Contains(spinNumber));

            List<BetCenter> lossCenterBetList = betCenterList.FindAll(bet => bet.betData.isSelected && !bet.winningObject.pieceNumber.Contains(spinNumber));

            // Loss Bet Throwing Out Side of screen
            // Dotween Use only for delay purpose.
            transform.DOScale(Vector3.one, 0f).SetDelay(0.8f).OnComplete(() =>
             {
                 lossCenterBetList.ForEach(bet => bet.chipsList.ForEach(chip =>
                 {
                     bet.ChipsRemoveAnimation(chip.gameObject, () => DestroyImmediate(chip.gameObject));
                 }));
             });

            // Winnig bet added into wallet
            for (int i = 0; i < refCenterBetList.Count; i++)
            {
                refCenterBetList[i].chipsList.Last().LableTextAnim(refCenterBetList[i].winningObject.winningMuliply);
                refCenterBetList[i].chipsList.Last().WinChipAddingCall(chipAddParent);
                mybetCenter = refCenterBetList[i];
            }

            Debug.Log($"mybetCenter {mybetCenter}");
            if (mybetCenter == null)
            {
                // StaticBtn Start
                StatisticsBtnMakeOnOff(true);
                return;
            }
            else
                Invoke(nameof(WinAmountAdd), 0.5f);

            Debug.Log($"particle on");
            refParticle = Instantiate(winnerStarParticlePrefab, spinNumberHighlightBox.transform.parent.transform);
            refParticle.Play();
            Invoke(nameof(DestroyParticle), 2f);
        }

        void WinAmountAdd() => StaticData.totalBalance += PlayerData.winAmountValue;
        private void DestroyParticle()
        {
            //paymentHandler.ResetPaySystam(true);
            DestroyImmediate(refParticle.gameObject);

            // StaticBtn Start
            StatisticsBtnMakeOnOff(true);
        }

        [Header("=== Anchor Number Show ==== ")]
        public Image anchorNumbarShow;
        public TextMeshProUGUI anchorNumbarShowText;
        [SerializeField] private AnchorBackColor backColor;
        [Serializable]
        private struct AnchorBackColor
        {
            public Color green;
            public Color red;
            public Color black;
        }

        public void AnchorNumberShow(string spinNumber)
        {
            anchorNumbarShowText.text = spinNumber;
            string numberType = PlayerData.Instance.SpinNumberTypeGet(spinNumber);
            anchorNumbarShow.color = (numberType == "red") ? backColor.red : (numberType == "grey") ? backColor.black : backColor.green;
            anchorNumbarShow.gameObject.SetActive(true);
        }

        [Header(" =====  Clear HighLight")]
        [SerializeField] private List<BoxController> allBoxList;
        public void ClearHighLightMakeOnOff(bool isMakeON)
        {
            allBoxList.ForEach(x => x.clearHighLighterBox.SetActive(isMakeON));
        }

        [Header(" === Win Loss Popup === ")]
        [SerializeField] private Image winLossPopup;
        [SerializeField] private TextMeshProUGUI winLossText;
        [SerializeField] private Sprite winSprite, lossSprite;
        private void NetWinLossPopupMakeOnOff(bool isMakeOn)
        {
            if (isMakeOn)
            {
                winLossPopup.transform.localScale = new Vector2(1, 0);
                winLossPopup.sprite = StaticData.netLastWin > 0 ? winSprite : lossSprite;
                winLossPopup.gameObject.SetActive(true);
                winLossPopup.transform.DOScaleY(1, 0.3f);
                winLossText.text = $"{(StaticData.netLastWin >= 0 ? "Net Win" : "Net Loss")}\n{(StaticData.netLastWin > 0 ? "+" : "")}{StaticData.netLastWin}";
            }
            else
            {
                winLossPopup.transform.DOScaleY(0, 0.3f).OnComplete(() => winLossPopup.gameObject.SetActive(false));
            }
        }

        [Header(" === Chips Add Section ==== ")]
        [SerializeField] private GameObject chipsAddPanel;

        public void ChipsAddPanelMakeOn()
        {
            if (PlayerData.Instance.BalanceGetFromText() <= 0)
            {
                SettingCloseAction?.Invoke();
                Debug.Log($"ChipsAddPanelMakeOn || betvalue {CoinsManager.instance.betValue}");
                if (CoinsManager.instance.betValue.Equals(0))
                {
                    Debug.Log($"ChipsAddPanelMakeOn betvalue zero");
                    PanelOnAnimtion(chipsAddPanel);
                }
                else
                {
                    Debug.Log($"ChipsAddPanelMakeOn betvalue not zero");
                    AlertMsgPopupOpen(true, noMoreChips);
                }
            }
        }



        /// <summary>
        /// First check bet is place or not.
        /// If place is not bet then Spin is not allow and open alert panel.
        /// If user have no enough balance then open chipsAdd panel.
        /// 
        /// </summary>
        public bool IsSpinAllow()
        {
            if (CoinsManager.instance.betValue.Equals(0))
            {
                if (StaticData.totalBalance > 0) AlertMsgPopupOpen(true, placeBets);
                else ChipsAddPanelMakeOn();
                return false;
            }
            return true;
        }


        [Header(" === DashBoard Controller ==== ")]
        [SerializeField] private GameObject dashBoardPanel;
        [SerializeField] private GameObject gamePlayPanel;
        [SerializeField] private FTUEController fTUEController;

        public void DashBoardPanleOpen()
        {
            GameController.instance.GameStateSet(GameState.DashBoard);
            dashBoardPanel.SetActive(true);
            PanelOnAnimtion(dashBoardPanel);
            GoogleAdmob.instance.BannerShowAndHide(true);
        }

        public void ClickOnPlayNowBtn(int gameMode)
        {
            GameMode currentGameMode = gameMode == 0 ? GameMode.American : GameMode.Europe;
            GameController.instance.GameModeSet(currentGameMode);
            FTUEVerify();
            dashBoardPanel.SetActive(false);
            GoogleAdmob.instance.BannerShowAndHide(false);
            gamePlayPanel.SetActive(true);

            try
            {
                TableSetAsPerMode(GameController.instance.gameMode);
            }
            catch (Exception ex)
            {
                Debug.Log($"exceprion {ex}");
                throw;
            }
        }


        private void FTUEVerify()
        {
            if (!PlayerPrefs.HasKey("FTUE"))
            {
                fTUEController.enabled = true;
                PlayerPrefs.SetString("FTUE", "FTUE Done");
            }
            else
            {
                // Game Play Initial Setup
                GameController.instance.GameStateSet(GameState.Playing);
                AudioManager.instance.PlayWelcomeClip();
                CoinsManager.instance.InitialBetSelection();
                // Check user have balance or not
                ChipsAddPanelMakeOn();
            }
        }

        [Space(15)]
        [Header("== Mode Transformation == ")]
        [SerializeField] GameObject zeroHolderAmerican;
        [SerializeField] GameObject zeroHolderEurope;
        [SerializeField] private List<BetCenter> americanBetCenter;
        [SerializeField] private List<BetCenter> europeBetCenter;
        [SerializeField] private List<BoxController> americanBox;
        [SerializeField] private List<BoxController> europeBox;

        [Header(" === Wheel SS === ")]

        [SerializeField] private Image wheelSSImage;
        [SerializeField] private Sprite americanWheelSS, euroWheelSS;

        private void TableSetAsPerMode(GameMode currentMode)
        {
            americanBetCenter.ForEach(bc => bc.gameObject.SetActive(currentMode.Equals(GameMode.American)));
            europeBetCenter.ForEach(bc => bc.gameObject.SetActive(currentMode.Equals(GameMode.Europe)));

            zeroHolderAmerican.SetActive(currentMode.Equals(GameMode.American));
            zeroHolderEurope.SetActive(currentMode.Equals(GameMode.Europe));

            allBoxList = BoxListUpdateAsPerMode(allBoxList, currentMode);
            numberBoxList = BoxListUpdateAsPerMode(numberBoxList, currentMode);

            Debug.Log($"allboxlist {allBoxList.Count} numberBox {numberBoxList.Count} ");

            NumbervalueAssignToHighLight(currentMode);

            allBoxList.ForEach(bl => AnimImageSet(bl));

            wheelSSImage.sprite = currentMode.Equals(GameMode.American) ? americanWheelSS : euroWheelSS;

            PlayerData.Instance.SpinNumberAdd();
        }

        private void AnimImageSet(BoxController boxController)
        {
            List<BetCenter> allBetCenter = Enumerable.Range(0, boxController.betCenterParent.transform.childCount)
            .Select(i => boxController.betCenterParent.transform.GetChild(i).GetComponent<BetCenter>())
            .ToList();

            foreach (BetCenter bc in allBetCenter)
            {
                bc.AnimImageSet();
            }
        }

        private List<BoxController> BoxListUpdateAsPerMode(List<BoxController> givenBoxList, GameMode currentMode)
        {
            givenBoxList = givenBoxList.Except(europeBox).ToList();
            givenBoxList = givenBoxList.Except(americanBox).ToList();

            givenBoxList.InsertRange(0, currentMode.Equals(GameMode.American) ? americanBox : europeBox);

            return givenBoxList;
        }


        [Header("===== Profile Edit ======")]
        [SerializeField] private GameObject profileEditPanel;
        [SerializeField] private GameObject profileEditBg, AvatarSelectionBg;
        private int profilePicInt
        {
            get { return PlayerPrefs.GetInt("profilePicInt", 0); }
            set
            {
                PlayerPrefs.SetInt("profilePicInt", value);
                allGamePlayerProfilePic.ForEach(playerProfile => playerProfile.sprite = avatarSpriteList[value]);
            }
        }

        private string userName
        {
            get
            {
                return PlayerPrefs.GetString("userName", "Guest");
            }
            set
            {
                PlayerPrefs.SetString("userName", value);
                allGameUserNameText.ForEach(uName => uName.text = value);
            }
        }

        [SerializeField] private List<Sprite> avatarSpriteList;
        [SerializeField] private List<AvatarController> avatarList;
        [SerializeField] private GameObject avatarContent;
        [SerializeField] private AvatarController avatarPrefab;
        [SerializeField] private Sprite avatarSelectBoarder, avatarDeSelectBoarder;
        [SerializeField] private List<Image> allGamePlayerProfilePic;
        [SerializeField] private TMP_InputField userNameIF;
        [SerializeField] private List<TMP_Text> allGameUserNameText;
        private const string matchNamePattern = "^[a-zA-Z]+[a-zA-Z0-9]+$";
        [SerializeField] private TMP_Text warningText;
        public void AllProfileEditPanelOpen()
        {
            PanelOnAnimtion(profileEditPanel);
            warningText.gameObject.SetActive(false);
            // Dashboard Statistics Data set
            PlayerData.Instance.DashboardStatisticsDataShow();
        }

        public void AllProfileEditPanelClose()
        {
            profileEditPanel.SetActive(false);
        }

        public void ProfileAvatarEdit(bool isOpen)
        {
            if (isOpen) AvatarSelectionScreenSet();
            SinglePanelOpenAndClose(isOpen, AvatarSelectionBg);
            SinglePanelOpenAndClose(!isOpen, profileEditBg);
            warningText.gameObject.SetActive(false);
        }
        private void SinglePanelOpenAndClose(bool isOpen, GameObject givenPanel)
        {
            givenPanel.SetActive(isOpen);
            if (isOpen)
            {
                givenPanel.transform.localScale = Vector2.zero;
                _ = givenPanel.transform.DOScale(Vector2.one, 0.3f);
            }
        }
        private void InitiallyDataSet()
        {
            allGameUserNameText.ForEach(uName => uName.text = userName);
            allGamePlayerProfilePic.ForEach(playerProfile => playerProfile.sprite = avatarSpriteList[profilePicInt]);
        }

        // Name Edit
        public void OnActiveUserNameInputField()
        {
            warningText.gameObject.SetActive(false);
            userNameIF.text = this.userName;
        }

        public void OnEditUserNameInputField()
        {
            if (userNameIF.text == userName) return;
            if (!IsUserNameValid(userNameIF.text, warningText))
            {
                warningText.gameObject.SetActive(true);
                return;
            }

            userName = userNameIF.text;
        }

        private bool IsUserNameValid(string inputString, TMP_Text warningText)
        {
            bool isIFValid = true;
            if (string.IsNullOrEmpty(inputString))
            {
                warningText.text = "Field should not be blank";
                isIFValid = false;
            }
            else
            {
                if (inputString.Length < 2)
                {
                    warningText.text = "Minimum 2 characters required.";
                    isIFValid = false;
                }
                else if (inputString.Length > 10)
                {
                    warningText.text = "Maximum 10 characters allow.";
                    isIFValid = false;
                }
                else if (!Regex.IsMatch(inputString, matchNamePattern))
                {
                    warningText.text = "Enter valid characters";
                    isIFValid = false;
                }
            }
            return isIFValid;
        }


        // Avatar Selection
        private void AvatarSelectionScreenSet()
        {
            Debug.Log("AvatarSelectionScreenSet || profilePicInt || " + profilePicInt);

            for (int i = 0; i < avatarSpriteList.Count; i++)
            {
                AvatarController avatarClone = null;
                if (avatarContent.transform.childCount.Equals(avatarSpriteList.Count)) avatarClone = avatarList[i];
                else
                {
                    avatarClone = Instantiate(avatarPrefab, avatarContent.transform).GetComponent<AvatarController>();
                    avatarClone.avatarIndex = i;
                    avatarClone.gameObject.name = $"avatar {i}";
                    avatarClone.profileImg.sprite = avatarSpriteList[i];
                    avatarClone.avatarSelectBtn.onClick.AddListener(delegate { OnClickAvatarSelection(avatarClone); });
                    avatarList.Add(avatarClone);
                }
                SelectDeselectAvatarSet(avatarClone, i.Equals(profilePicInt) ? avatarSelectBoarder : avatarDeSelectBoarder, i.Equals(profilePicInt));

                if (i.Equals(profilePicInt))
                {
                    Debug.Log("selected avatar => " + profilePicInt);
                    // selctedAvatarClone = avatarClone.GetComponent<RectTransform>();
                }
            }

            //if (selctedAvatarClone != null)
            //    _ = StartCoroutine(ScrollHighLighter.BringChildIntoViewSlide(avatarScrollRect, selctedAvatarClone));
        }

        private void SelectDeselectAvatarSet(AvatarController avatar, Sprite _selectDeselectBoarder, bool isSelect)
        {
            avatar.profileSlectBoarderImg.sprite = _selectDeselectBoarder;
            avatar.profileSelectImg.SetActive(isSelect);
        }

        private void OnClickAvatarSelection(AvatarController selectAvatar)
        {
            if (selectAvatar.profileSelectImg.activeInHierarchy) return;

            AudioManager.instance.PlayObjectSelect();

            foreach (AvatarController _avatarClone in avatarList)
            {
                SelectDeselectAvatarSet(_avatarClone, avatarDeSelectBoarder, false);
            }
            SelectDeselectAvatarSet(selectAvatar, avatarSelectBoarder, true);

            profilePicInt = selectAvatar.avatarIndex;
            ProfileAvatarEdit(false);
        }

        [Header(" === Purchase Store === ")]
        [SerializeField] private GameObject chipsStorePanel;
        [SerializeField] private ChipsStoreController cSC;

        public void ChipsStoreOpen()
        {
            PanelOnAnimtion(chipsStorePanel);
            // Daily Bonus Renewed Time Check
            cSC.VerifyDailyBonusUpdateTime();
            dashBoardPanel.SetActive(false);
            GoogleAdmob.instance.BannerShowAndHide(false);
        }

        public void ChipsStoreClose()
        {
            chipsStorePanel.SetActive(false);
            switch (GameController.instance.gameState)
            {
                case GameState.DashBoard:
                    dashBoardPanel.SetActive(true);
                    GoogleAdmob.instance.BannerShowAndHide(true);
                    break;

            }
        }

        [Header(" ==== Privacy Policy === ")]
        [SerializeField] private string policyURL;
        public void GetPrivacyPolicy()
        {
            Debug.Log($"GetPrivacyPolicy");
            Application.OpenURL(policyURL);
        }
    }
}