using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using DG.Tweening;

namespace RouletteByFinix
{
    public class PlayerData : MonoBehaviour
    {
        public static PlayerData Instance;

        [SerializeField] private LastNumberDisplay lastNumberDisplay;

        public TextMeshProUGUI balanceText;
        public List<TextMeshProUGUI> staticBalanceText;
        public TextMeshProUGUI betAmount;

        [Header("SESSOION STATS Text")]
        public TextMeshProUGUI lastwin;
        public TextMeshProUGUI lastBet;
        public TextMeshProUGUI netLastWin;
        public TextMeshProUGUI sessionWin;
        public TextMeshProUGUI hitPercent;
        public TextMeshProUGUI missPercent;

        public static int winAmountValue;


        private void Awake() => Instance = this;

        private void Start()
        {
            StaticData.ResetStaticData(false);
            InitiallyTotalBalanceSet();
            FTUEController.FTUE_ResetAllData += FTUEEndReset;
            CalculateSesstion();
        }


        public void InitiallyTotalBalanceSet()
        {
            if (!PlayerPrefs.HasKey("NewUser"))
            {
                StaticData.totalBalance = 5000;
                PlayerPrefs.SetInt("NewUser", 0);
            }
            else
                ShowBalanceText(StaticData.totalBalance, false);
        }

        public void ShowBalanceText(int str, bool isDoTextUsing)
        {
            Debug.Log($"ShowBalanceText || isDoTextUsing {isDoTextUsing}  total balance {str}");
            if (isDoTextUsing)
            {
                _ = balanceText.DOText($"{str}", 1f, true, ScrambleMode.Numerals);
                staticBalanceText.ForEach(sBText => _ = sBText.DOText($"{str}", 1f, true, ScrambleMode.Numerals));
            }
            else
            {
                balanceText.text = $"{str}";
                staticBalanceText.ForEach(sBText => sBText.text = $"{str}");
            }
        }

        public int BalanceGetFromText()
        {
            int balance = int.Parse(new string(balanceText.text.Where(char.IsDigit).ToArray()));
            Debug.Log($"BalanceGetFromText {balance}");
            return balance;
        }

        public void SesstionStateAdd(int _lastWin, int _lastBet, int _netLastWin, int _sessionWin, int _hitPercent, int _missPercent)
        {
            lastwin.text = _lastWin.ToString();
            lastBet.text = _lastBet.ToString();
            netLastWin.text = _netLastWin.ToString();
            sessionWin.text = _sessionWin.ToString();
            hitPercent.text = _hitPercent.ToString() + " % ";
            missPercent.text = _missPercent.ToString() + " % ";

        }

        public void CalculateSesstion(bool isPaymentCalci = false, int _lastBet = 0, int _lastWin = 0, int _existWinBet = 0)
        {
            StaticData.lastWin = _lastWin;
            StaticData.lastBet = _lastBet;

            if (isPaymentCalci)
            {
                Debug.Log($"<color=cyan>{_lastWin} ||  {_existWinBet} ||  {_lastBet}</color>");
                StaticData.netLastWin = _lastWin + _existWinBet - _lastBet;
                Debug.Log($"Session Win is raise by {StaticData.netLastWin}");

                StaticData.SessionWin += StaticData.netLastWin;
                // StaticData.SessionWin = Mathf.Clamp(StaticData.SessionWin, 0, StaticData.SessionWin);

                Debug.Log($"total bet {StaticData.totalBetPlaceCount}  hit place {StaticData.hitPlaceCount}");
                if (StaticData.totalBetPlaceCount != 0)
                {
                    StaticData.hitPercent = StaticData.hitPlaceCount * 100 / StaticData.totalBetPlaceCount;
                    StaticData.missPercent = 100 - StaticData.hitPercent;
                }

                // dashboard Statistics
                LossChipsAdd(-StaticData.lastWin);
                WinChipsAdd(StaticData.lastWin);
            }
            Debug.Log($"  StaticData.hitPercent  || {StaticData.hitPercent} ||  Balance  {StaticData.totalBalance}");

            SesstionStateAdd(StaticData.lastWin, StaticData.lastBet, StaticData.netLastWin, StaticData.SessionWin, StaticData.hitPercent, StaticData.missPercent);
        }

        [Header(" ===== Statistics ===== ")]
        [SerializeField] private List<SpinningScriptable> numberTypeScriptable;
        [SerializeField] private SpinNumberStatisticsData spinNumberStaticData = new SpinNumberStatisticsData();
        private Dictionary<string, int> spinNumberCountDic = new Dictionary<string, int>();
        private string spinNumberJsonData
        {
            get { return PlayerPrefs.GetString("spinNumberJson"); }
            set { PlayerPrefs.SetString("spinNumberJson", value); }
        }
        [System.Serializable]
        private class SpinNumberStatisticsData
        {
            public List<string> americanSpinNumber = new List<string>();
            public List<string> euroSpinNumber = new List<string>();
            public FullStatistics americanFullStates = new FullStatistics();
            public FullStatistics euroFullStates = new FullStatistics();

            public DashBoardStatistics dbStatistics = new DashBoardStatistics();
        }

        [System.Serializable]
        public class FullStatistics
        {
            public int red;
            public int black;
            public int even;
            public int odd;
            public int eighteenth1;
            public int eighteenth2;
        }

        [System.Serializable]
        public class DashBoardStatistics
        {
            public int totalGame;
            public int winChips;
            public int lossChips;
        }
        private void OnEnable()
        {
            Debug.Log($"SpinNumberJsonData Enable {spinNumberJsonData}");
            if (!string.IsNullOrEmpty(spinNumberJsonData))
                spinNumberStaticData = JsonUtility.FromJson<SpinNumberStatisticsData>(spinNumberJsonData);

        }


        private List<string> GetNumberListAsPerMode()
        {
            return GameController.instance.gameMode.Equals(GameMode.American) ? spinNumberStaticData.americanSpinNumber : spinNumberStaticData.euroSpinNumber;
        }

        private FullStatistics GetFullStatesAsPerMode()
        {
            return GameController.instance.gameMode.Equals(GameMode.American) ? spinNumberStaticData.americanFullStates : spinNumberStaticData.euroFullStates;
        }


        public void SpinNumberAdd()
        {
            GetNumberListAsPerMode().ForEach(num => lastNumberDisplay.AddPosition(num));

            SpinNumberListGet();
        }

        private void LastDataSet()
        {
            spinNumberJsonData = JsonUtility.ToJson(spinNumberStaticData);
            Debug.Log($"SpinNumberJsonData Disable {spinNumberJsonData}");
        }

        private void OnDisable()
        {
            LastDataSet();
        }

        public void SpinNumberTypeDataCount(string spinValue)
        {
            GetNumberListAsPerMode().Add(spinValue);

            FullStatistics fullState = GetFullStatesAsPerMode();
            //int spinNumValue = int.Parse(spinValue);
            if (spinValue != "0" && spinValue != "00")
            {
                for (int i = 0; i < numberTypeScriptable.Count; i++)
                {
                    if (numberTypeScriptable[i].pieceNumber.Contains(spinValue))
                    {
                        switch (numberTypeScriptable[i].stateSet)
                        {
                            case SpinningScriptable.state.Red:
                                {
                                    fullState.red++;
                                    break;
                                }
                            case SpinningScriptable.state.Black:
                                {
                                    fullState.black++;
                                    break;
                                }
                            case SpinningScriptable.state.Even:
                                {
                                    fullState.even++;
                                    break;
                                }
                            case SpinningScriptable.state.Odd:
                                {
                                    fullState.odd++;
                                    break;
                                }
                            case SpinningScriptable.state.Eighteenth1:
                                {
                                    fullState.eighteenth1++;
                                    break;
                                }
                            case SpinningScriptable.state.Eighteenth2:
                                {
                                    fullState.eighteenth2++;
                                    break;
                                }
                        }
                    }
                }
            }
            LastDataSet();
        }

        private void SpinNumberCount()
        {
            spinNumberCountDic = new Dictionary<string, int>();
            var duplicates = GetNumberListAsPerMode().GroupBy(x => x)
                      .Where(g => g.Count() >= 1)
                      .Select(y => new { Element = y.Key, Counter = y.Count() })
                      .ToList();

            foreach (var item in duplicates)
            {
                Debug.Log($"spin number {item.Element}  count  {item.Counter}");
                spinNumberCountDic.Add(item.Element, item.Counter);
            }
        }

        private List<string> spinNumberOnBoard;
        [SerializeField] private List<ChartValueController> chartValues;
        [System.Serializable]
        private struct BarColor
        {
            public Color greenBar, redBar, greyBar;
        }

        [SerializeField] private BarColor barColor;

        private void SpinNumberListGet()
        {
            spinNumberOnBoard = new List<string>() { "00" };
            for (int i = 0; i <= 36; i++)
            {
                spinNumberOnBoard.Add($"{i}");
            }

            for (int i = 0; i < spinNumberOnBoard.Count; i++)
            {
                chartValues[i].SpinNumberTextSet(spinNumberOnBoard[i]);
            }

            chartValues[0].gameObject.SetActive(GameController.instance.gameMode == GameMode.American);
        }

        private void MakeBarChart()
        {
            SpinNumberCount();
            if (spinNumberCountDic.Count == 0) return;
            List<float> percentageList = new List<float>();

            foreach (var item in spinNumberCountDic)
            {
                percentageList.Add(((float)item.Value / (float)GetNumberListAsPerMode().Count));
            }

            float max = percentageList.Max() * 100;
            float maxPercentage = Mathf.FloorToInt(max) + ((max - Mathf.FloorToInt(max) == 0) ? 0 : 1);
            Debug.Log($"max {maxPercentage}");
            foreach (var item in spinNumberCountDic)
            {
                if (GetNumberListAsPerMode().Count != 0)
                {
                    float percentage = ((float)item.Value / (float)GetNumberListAsPerMode().Count);
                    Debug.Log($"percentage count {percentage}");
                    Debug.Log($" key {item.Key}  index {spinNumberOnBoard.IndexOf(item.Key)}");
                    string numType = SpinNumberTypeGet(item.Key);
                    Color refColor = numType == "red" ? barColor.redBar : numType == "grey" ? barColor.greyBar : barColor.greenBar;
                    chartValues[spinNumberOnBoard.IndexOf(item.Key)].BarValueDataSet(percentage * 100 / maxPercentage, refColor, percentage);
                }
            }
        }

        public string SpinNumberTypeGet(string spinNumberString)
        {
            if (spinNumberString == "00" || spinNumberString == "0")
            {
                return "green";
            }
            else
            {
                for (int i = 0; i < numberTypeScriptable.Count; i++)
                {
                    if (numberTypeScriptable[i].pieceNumber.Contains(spinNumberString))
                    {
                        switch (numberTypeScriptable[i].stateSet)
                        {
                            case SpinningScriptable.state.Red:
                                {
                                    return "red";
                                }
                            case SpinningScriptable.state.Black:
                                {
                                    return "grey";
                                }
                        }
                    }
                }

                return string.Empty;
            }
        }
        [SerializeField] List<HeaderStatistics> headerStatisticsList;
        private List<int> refSpinType;
        [System.Serializable]
        private struct HeaderStatistics
        {
            public Image fillerImg;
            public TextMeshProUGUI percentage;
        }
        public void HeaderStatisticsSet()
        {
            FullStatistics fullStates = GetFullStatesAsPerMode();
            refSpinType = new List<int> { fullStates.red, fullStates.black, fullStates.even, fullStates.odd,
             fullStates.eighteenth1, fullStates.eighteenth2};

            float effectiveCount = GetNumberListAsPerMode().Count(x => !(x == "00" || x == "0"));
            Debug.Log($"efective count {effectiveCount}");
            float percentage = 0;
            for (int i = 0; i < headerStatisticsList.Count; i++)
            {
                if (effectiveCount != 0) percentage = (float)refSpinType[i] / effectiveCount;
                headerStatisticsList[i].fillerImg.fillAmount = percentage;
                percentage *= 100;
                headerStatisticsList[i].percentage.text = string.Format("{0:F1}%", percentage);
            }

            MakeBarChart();
        }

        private void FTUEEndReset()
        {
            spinNumberStaticData = new SpinNumberStatisticsData();
            spinNumberJsonData = string.Empty;
            StaticData.ResetStaticData(true);
            CalculateSesstion();
        }

        [Header("Dashboard Statistics")]
        [SerializeField] private TextMeshProUGUI totalGamePlayText;
        [SerializeField] private TextMeshProUGUI winChipsText, lossChipsText;
        public void TotalGameAdd()
        {
            spinNumberStaticData.dbStatistics.totalGame = spinNumberStaticData.dbStatistics.totalGame + 1;
            Debug.Log($"game count adding => {spinNumberStaticData.dbStatistics.totalGame}");
        }

        public void WinChipsAdd(int winChips)
        {
            spinNumberStaticData.dbStatistics.winChips += winChips;
            Debug.Log($"win chips add by  {winChips}  and make {spinNumberStaticData.dbStatistics.winChips}");
        }

        public void LossChipsAdd(int lossChips)
        {
            spinNumberStaticData.dbStatistics.lossChips -= lossChips;
            Debug.Log($"loss chips cut by  {lossChips}  and make {spinNumberStaticData.dbStatistics.lossChips}");

        }

        public void DashboardStatisticsDataShow()
        {
            totalGamePlayText.text = $"{spinNumberStaticData.dbStatistics.totalGame}";
            winChipsText.text = $"  {spinNumberStaticData.dbStatistics.winChips}";
            lossChipsText.text = $"  {spinNumberStaticData.dbStatistics.lossChips}";
        }

        public void InterstitialShow()
        {
            int totalGamePlay = spinNumberStaticData.dbStatistics.totalGame;
            if (totalGamePlay.Equals(0)) return;
            if (totalGamePlay % 4 != 0) return;
            Debug.Log($"Show interstitial");
            GoogleAdmob.instance.ShowInterstitialAd();
        }

    }
}