using UnityEngine;


namespace RouletteByFinix
{
    public class StaticData
    {
        public static string sound = "Sound";
        public static int lastWin;
        public static int lastBet;
        public static int netLastWin;
        public static int SessionWin
        {
            get => PlayerPrefs.GetInt(sessionWinString);
            set => PlayerPrefs.SetInt(sessionWinString, value);
        }
        public static int hitPercent;
        public static int missPercent;

        public static string totalBalanceString = "TotalBelance";
        public static string sessionWinString = "sessionWin";
        public static bool isDoTextUsing, isBalanceStore = true;

        public static int totalBalance
        {
            get
            {
                if (!isBalanceStore)
                {
                    return PlayerData.Instance.BalanceGetFromText();
                }
                return PlayerPrefs.GetInt(totalBalanceString);
            }

            set
            {
                Debug.Log($"isbalancestore {isBalanceStore}    {value}");
                if (!GameController.instance.IsFTUEgameStateOn() && isBalanceStore)
                {
                    Debug.Log($"is balance set {value}");
                    PlayerPrefs.SetInt(totalBalanceString, value);
                }
                else
                {
                    isBalanceStore = true;
                }

                PlayerData.Instance.ShowBalanceText(value, isDoTextUsing);
                isDoTextUsing = false;
            }
        }

        public static int hitPlaceCount
        {
            get { return PlayerPrefs.GetInt("hitPlace", 0); }
            set { PlayerPrefs.SetInt("hitPlace", value); }
        }
        public static int totalBetPlaceCount
        {
            get { return PlayerPrefs.GetInt("totalBetPlaceCount", 0); }
            set { PlayerPrefs.SetInt("totalBetPlaceCount", value); }
        }

        public static string Sound
        {
            get => PlayerPrefs.GetString(sound);
            set => PlayerPrefs.SetString(sound, value);
        }


        public static void ResetStaticData(bool isFTUEResetData)
        {
            lastWin = 0;
            lastBet = 0;
            netLastWin = 0;
            SessionWin = 0;
            hitPercent = 0;
            missPercent = 0;
            hitPlaceCount = 0;
            totalBetPlaceCount = 0;
        }

    }
}