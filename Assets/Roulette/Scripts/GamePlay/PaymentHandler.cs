using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace RouletteByFinix
{
    public class PaymentHandler : MonoBehaviour
    {
        public static Action<BetCenter> addBetNum;

        [SerializeField] RouletteRotater rouletteRotater;

        public bool isChipsMoving = false;

        [Header("========= Payment need Lists =============")]
        public List<BetCenter> checkValue = new List<BetCenter>();
        public List<GameObject> checkValue2 = new List<GameObject>();


        [Space(3)]
        [Header("=========== Rebet & Undo need Lists =============")]
        public List<BetCenter> storeBetPiece = new List<BetCenter>();
        public List<BetCenter> saveLastbetlist = new List<BetCenter>();
        public List<int> rebetDatas = new List<int>();
        [SerializeField] private List<BetFootprint> _BetFootprints = new List<BetFootprint>();
        [SerializeField] private List<BetFootprint> _RebetBetFootprints = new List<BetFootprint>();

        [Header("Anchor number")]
        private int single;  // 1 num
        private int split; // 2 num
        private int split2; // 2 num
        private int street; // 3 num
        private int Corner; // 4 num
        private int sixLine; // 6 num
        private int column; // 12 num
        private int _dozen; // 2 * num
        private int eighteenth1; // 1 * num
        private int eighteenth2; // 1 * num
        private int Black; // 1 * num
        private int red; // 1 * num
        private int even; // 1 * num
        private int odd; // 1 * num
        private int winNum;

        public static PaymentHandler Instance;

        private void Awake() => Instance = this;
        private void OnEnable()
        {
            FTUEController.FTUE_ResetAllData += FTUEEndReset;
            addBetNum += AddBetNumbers;
        }

        private void OnDisable()
        {
            addBetNum -= AddBetNumbers;
        }

        private void AddBetNumbers(BetCenter numObject)
        {
            checkValue.Add(numObject);
            StaticData.totalBetPlaceCount += 1;
        }

        public void AddBetPoints(BetCenter betCenter)
        {
            storeBetPiece.Add(betCenter);
        }

        public void IsChipsMovingMakeTrueFalse(bool isTrue)
        {
            isChipsMoving = isTrue;
        }


        private List<BetCenter> winBetList;

        public void CalculatePayAmount(string betAmount)
        {
            Debug.Log($" checkValue.Count|| {checkValue.Count} || selectedBets || ");
            Debug.Log($"<color=yellow>_BetFootprints 1 || {_BetFootprints.Count} || ");

            //  PlayerData.Instance.CalculateSesstion(CoinsManager.instance.betValue); // 1
            checkValue2.Clear();
            winBetList = checkValue.FindAll(bet => bet.winningObject.pieceNumber.Contains(betAmount));
            for (int i = 0; i < winBetList.Count; i++)
            {
                // CHECK WIN LOGIC
                Debug.Log($" Win Amount add || {winBetList[i].winningObject.stateSet}");
                checkValue2.Add(winBetList[i].gameObject);
                StaticData.hitPlaceCount += 1;
                CheckPieceState(winBetList[i]);
            }
            ResultShowWait();
        }

        private void ResultShowWait()
        {
            int netWinAmount = CalculateValue();
            Debug.Log($"sum {winBetList.Select(x => x.betData.betValue).ToList().Sum()}");
            int existWinBet = winBetList.Select(x => x.betData.betValue).ToList().Sum();
            int winAmount = netWinAmount + existWinBet;//+ checkValue[i].betData.betValue;
            Debug.Log($"win amounts {winAmount}");
            PlayerData.winAmountValue = winAmount;
            Debug.Log($"Total Balance Increase to {PlayerData.winAmountValue}");
            StaticData.isDoTextUsing = netWinAmount != 0;
            Debug.Log($"ResetGame ||  Plus   {PlayerData.winAmountValue}");
            PlayerData.Instance.CalculateSesstion(true, CoinsManager.instance.betValue, netWinAmount, existWinBet); // 2
        }

        void CheckPieceState(BetCenter winBet)
        {
            Debug.Log($"checkpiecestate");
            var winAmount = winBet.betData.betValue * winBet.winningObject.winningMuliply;
            switch (winBet.winningObject.stateSet)
            {
                case SpinningScriptable.state.NumberPleno:

                    single += winAmount;
                    break;

                case SpinningScriptable.state.NumberMiddle:

                    split += winAmount;
                    break;

                case SpinningScriptable.state.Three:

                    street += winAmount;
                    break;

                case SpinningScriptable.state.NumberCorner:

                    Corner += winAmount;
                    break;

                case SpinningScriptable.state.NumberRow:

                    split2 += winAmount;
                    break;

                case SpinningScriptable.state.Column:

                    column += winAmount;
                    break;

                case SpinningScriptable.state.Dozen:

                    _dozen += winAmount;
                    break;

                case SpinningScriptable.state.Eighteenth1:

                    eighteenth1 += winAmount;
                    break;

                case SpinningScriptable.state.Eighteenth2:

                    eighteenth2 += winAmount;
                    break;

                case SpinningScriptable.state.Even:

                    even += winAmount;
                    break;

                case SpinningScriptable.state.Odd:

                    odd += winAmount;
                    break;

                case SpinningScriptable.state.Black:

                    Black += winAmount;
                    break;

                case SpinningScriptable.state.Red:

                    red += winAmount;
                    break;


                case SpinningScriptable.state.ButtomCorner:

                    sixLine = winAmount;
                    break;
            }
        }
        private int CalculateValue()
        {
            winNum = single + split + split2 + street + column + Corner + _dozen + eighteenth1 + eighteenth2 + Black + red + even + odd + sixLine;
            Debug.Log($" <color=cyan>|| {single} || {street} || {split} || {split2} || {column} ||  {Corner} || {_dozen} || {eighteenth1} || {eighteenth2} || {Black} || {red} || {even} || {odd} ||{sixLine} </color> = {winNum}");
            return winNum;
        }

        public void ResetPaySystem(bool isSpinEndReset)
        {
            Debug.Log($"<color=yellow>_BetFootprints 0 || {_BetFootprints.Count} || </color>");

            single = split = split2 = column = street = Corner = _dozen = eighteenth1 = eighteenth2 = red = Black = even = odd = sixLine = 0;
            Debug.Log($"ResetPaySystam || check value count {checkValue.Count}");

            if (isSpinEndReset)
            {
                if (_BetFootprints.Count != 0)
                {
                    _RebetBetFootprints = _BetFootprints.Select(item => item).ToList();
                    rebetDatas.Clear();
                }
            }



            for (int i = 0; i < checkValue.Count; i++)
            {
                Debug.Log($"check value at reset {checkValue[i].name}");
                if (isSpinEndReset) rebetDatas.Add(checkValue[i].betData.betValue);
                checkValue[i].betData.betValue = 0;
                checkValue[i].betData.isSelected = false;

                foreach (var item in checkValue[i].chipsList)
                {
                    Debug.Log($"ResetPaySystem chip {item}");
                    if (item != null)
                    {
                        Destroy(item.gameObject);
                    }
                }
                checkValue[i].chipsList.Clear();
            }
            CoinsManager.instance.betValue = 0;
            checkValue.Clear();
            checkValue2.Clear();
            _BetFootprints.Clear();
            storeBetPiece.Clear();
        }

        public void Undo(bool isUndoCalling, BetFootprint removeBetFootPrint = null)
        {
            if (CoinsManager.instance.betValue < 0)
                return;
            if (!UiManager.Instance.isAllClearModeOn) AudioManager.instance.PlayUndoClick();
            RemoveGivenBetFromList(isUndoCalling ? _BetFootprints.Last() : removeBetFootPrint);
            if (storeBetPiece.Count == 0)
            {
                UiManager.Instance.undoBtn.interactable = false;
                UiManager.Instance.clearBtn.interactable = false;

                if (rebetDatas.Count != 0 && CoinsManager.instance.IsTotalBalanceMoreThanZero()) UiManager.Instance.rebetBtn.interactable = true;

                if (UiManager.Instance.isClearModeOn)
                    UiManager.Instance.ClearModeOnOff();
            }

        }

        public void AllClear()
        {
            UiManager.Instance.isAllClearModeOn = true;
            List<BetFootprint> saveBetFootPrint = new List<BetFootprint>(_BetFootprints);
            AudioManager.instance.PlayUndoClick();
            for (int i = 0; i < saveBetFootPrint.Count; i++)
            {
                Undo(false, saveBetFootPrint[i]);
            }
            SettingPanelController.VibrateAction?.Invoke();
            UiManager.Instance.isAllClearModeOn = false;
        }

        private void RemoveGivenBetFromList(BetFootprint removeBetFootPrint)
        {
            _BetFootprints.Remove(removeBetFootPrint);
            Debug.Log($"betFootPrint count {_BetFootprints.Count}  value {removeBetFootPrint.value}");
            removeBetFootPrint.betCenter.RemoveBet(removeBetFootPrint.betCenter, removeBetFootPrint.value);

            var objCount = storeBetPiece.FindAll(x => x.Equals(removeBetFootPrint.betCenter));

            if (checkValue.Contains(removeBetFootPrint.betCenter) && objCount.Count == 1)
            {
                checkValue.Remove(removeBetFootPrint.betCenter);
                StaticData.totalBetPlaceCount -= 1;
            }

            if (storeBetPiece.Contains(removeBetFootPrint.betCenter))
                storeBetPiece.Remove(removeBetFootPrint.betCenter);
        }
        public void Clear()
        {
            if (CoinsManager.instance.betValue < 0)
                return;
            return;
        }

        public void RebetClick()
        {
            try
            {
                AudioManager.instance.PlayCoinAdd();
                checkValue.AddRange(saveLastbetlist);
                StaticData.totalBetPlaceCount += saveLastbetlist.Count;
                Debug.Log($"<color=yellow> saveLastbetlist || {storeBetPiece.Count} || rebetDatas.Count || {rebetDatas.Count} </color>");

                for (int i = 0; i < rebetDatas.Count; i++)
                {
                    saveLastbetlist[i].betData.betValue = rebetDatas[i];
                }

                for (int i = 0; i < _RebetBetFootprints.Count; i++)
                {
                    var chipClone = CoinsManager.instance.ChipsInitiate((int)_RebetBetFootprints[i].value);
                    _RebetBetFootprints[i].betCenter.chipsList.Add(chipClone);
                    chipClone.transform.SetParent(_RebetBetFootprints[i].betCenter.transform, false);
                    _RebetBetFootprints[i].betCenter.ChipsAddedAnimation(chipClone.gameObject, _RebetBetFootprints[i].betCenter.chipsList.Count - 1, false);

                    Add(_RebetBetFootprints[i].betCenter, (int)_RebetBetFootprints[i].value, true);
                    chipClone.ChipsAmountSet(_RebetBetFootprints[i].betCenter.betData.betValue);
                    storeBetPiece.Add(_RebetBetFootprints[i].betCenter);
                    StaticData.totalBalance -= (int)_RebetBetFootprints[i].value;
                    Debug.Log($"RebetClick ||  Minus  {(int)_RebetBetFootprints[i].value}");
                }

                Debug.Log($"||| <color=cyan> _BetFootprints || { _BetFootprints.Count} </color> ");
                UiManager.Instance.undoBtn.interactable = true;
                UiManager.Instance.clearBtn.interactable = true;
            }
            catch (Exception ex)
            {
                Debug.Log($"esception {ex}");
            }
        }

        private int GetChipsPrefabIndex(int value)
        {
            return value switch
            {
                1 => 0,
                5 => 1,
                10 => 2,
                100 => 3,
                1000 => 4,
                _ => -1
            };
        }

        public void Add(BetCenter space, float value, bool isRebetPiece = false)
        {
            space.betData.isSelected = true;
            _BetFootprints.Add(new BetFootprint(space, value, isRebetPiece));
            StaticData.isBalanceStore = false;
            CoinsManager.instance.betValue += (int)value;
        }


        public BetFootprint GetBetFootPrintFromList(ChipsDetailsController givenChips)
        {
            var abc = _BetFootprints.Select(x => x).Reverse().ToList();
            return abc.Find(x => x.betCenter.chipsList.Contains(givenChips));
        }

        private void FTUEEndReset()
        {
            CoinsManager.instance.betValue = 0;
            rebetDatas.Clear();
            saveLastbetlist.Clear();
            _RebetBetFootprints.Clear();
            _BetFootprints.Clear();
            storeBetPiece.Clear();
            checkValue.Clear();
            checkValue2.Clear();
            UiManager.Instance.rebetBtn.interactable = false;
        }

    }

    [System.Serializable]
    public class BetFootprint
    {
        public BetCenter betCenter;
        public float value;
        // Rebet Section
        public bool isRebetPiece;

        public BetFootprint(BetCenter betSpace, float value, bool isRebetPiece)
        {
            this.betCenter = betSpace;
            this.value = value;
            this.isRebetPiece = isRebetPiece;
        }
    }
}