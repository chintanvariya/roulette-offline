using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RouletteByFinix
{
    public class CoinsManager : MonoBehaviour
    {
        public List<Coin> coinRing;

        [SerializeField] private ChipsDetailsController chipsPrefab;
        [SerializeField] private List<Sprite> chipSpriteList;

        public PlayerData playerData;
        public static int coinAmount;
        [SerializeField] private int betValueOrigin;
        public int betValue
        {
            get { return betValueOrigin; }
            set
            {
               
                betValueOrigin = value;
                playerData.betAmount.text = value.ToString();
            }
        }

        public static CoinsManager instance;

        private void Awake()
        {
            if (instance == null)
                instance = this;
        }

        private void Start()
        {
            FTUEController.FTUE_ResetAllData += FTUEEndReset;
            betValue = 0;
        }

        public void InitialBetSelection()
        {
            // Initially 1 bet make  selected
            coinRing[0].OnClick(false);
        }

        public void BetStore()
        {
            StaticData.totalBalance -= coinAmount;
            Debug.Log($"coinamount {coinAmount}   {StaticData.totalBalance}");
            StaticData.lastBet = coinAmount;
        }

        public bool IsChipsAvailableAtClick()
        {
            bool isChipsEnough = PlayerData.Instance.BalanceGetFromText() - coinAmount >= 0;
            if (!isChipsEnough)
            {
                if (!IsSelectBetAsPerBalance(PlayerData.Instance.BalanceGetFromText()))
                {
                    return isChipsEnough;
                }
            }
            return true;
        }

        public bool IsTotalBalanceMoreThanZero()
        {
            if (PaymentHandler.Instance.rebetDatas.Sum() != 0)
            {
                return StaticData.totalBalance >= PaymentHandler.Instance.rebetDatas.Sum();
            }
            else return StaticData.totalBalance > 0;
        }

        public ChipsDetailsController ChipsInitiate(int chipValue)
        {
            ChipsDetailsController clone = Instantiate(chipsPrefab);
            clone.ChipsDataSet(GetSpriteFromChipValue(chipValue));
            clone.ChipsAmountSet(chipValue);
            return clone;
        }

        private Sprite GetSpriteFromChipValue(int chipValue)
        {
            Debug.Log($"chip value {chipValue}");
            return chipValue switch
            {
                1 => chipSpriteList[0],
                10 => chipSpriteList[1],
                100 => chipSpriteList[2],
                500 => chipSpriteList[3],
                1000 => chipSpriteList[4],
                //1000 => chipSpriteList[5],
                _ => null
            };
        }

        public bool IsSelectBetAsPerBalance(int balance)
        {
            for (int i = coinRing.Count - 1; i >= 0; i--)
            {
                if (balance / coinRing[i].coinValue > 0)
                {
                    coinRing[i].coinBtn.onClick.Invoke();
                    return true;
                }
            }
            return false;
        }

        private void FTUEEndReset() => InitialBetSelection();
    }
}