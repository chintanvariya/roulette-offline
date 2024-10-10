using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PrahantGames
{
    public class ChipsStoreController : MonoBehaviour
    {
        [Header("== Daily Bonus == ")]
        [SerializeField]
        private TextMeshProUGUI claimStatusText;
        [SerializeField]
        private int dailyBonusChips;
        [SerializeField]
        private Button claimBtn;
        [SerializeField]
        private ParticleSystem chipsAddParticle;

        private string lastDBClaimTimeKey = "dbClaimTime";

        private void Start()
        {
            GetChipsSpriteSet();
            NextRewardTimerSet(true);
        }

        private DateTime lastDBClaimTime
        {
            get { return DateTime.Parse(PlayerPrefs.GetString(lastDBClaimTimeKey)); }
            set { PlayerPrefs.SetString(lastDBClaimTimeKey, value.ToString()); }
        }

        public void VerifyDailyBonusUpdateTime()
        {
            // Daily bonus Button verify
            if (!PlayerPrefs.HasKey(lastDBClaimTimeKey))
            {
                DailyBonusMakeOnOff(true);
            }
            else
            {
                DailyBonusMakeOnOff(IsDailyBonusAllow());
            }
        }

        private DateTime GetCurrentTime()
        {
            return DateTime.UtcNow.AddHours(5).AddMinutes(30);
            //TimeZoneInfo indianTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            //return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, indianTimeZone);
        }

        public void DailyBonusClaim()
        {
            DailyBonusMakeOnOff(false);
            GetAmountAddInWallet();
        }

        private void GetAmountAddInWallet()
        {
            StartCoroutine(ChipsGetParticlePlay());
            StaticData.isDoTextUsing = true;
            StaticData.totalBalance += dailyBonusChips;
            lastDBClaimTime = GetCurrentTime();
        }
        private IEnumerator ChipsGetParticlePlay()
        {
            chipsAddParticle.Play();

            for (int i = 0; i < 12; i++)
            {
                AudioManager.instance.PlayCoinAdd();
                yield return new WaitForSeconds(0.05f);
            }
        }

        private bool IsDailyBonusAllow()
        {
            Debug.Log($"lastDBClaimTime {lastDBClaimTime}");
            Debug.Log($"CurrentTime {GetCurrentTime()}");
            return lastDBClaimTime.Date != GetCurrentTime().Date;
        }

        private void DailyBonusMakeOnOff(bool isMakeOn)
        {
            claimBtn.interactable = isMakeOn;
            claimStatusText.text = isMakeOn ? "Claim" : "Claimed";
        }

        [Space(20)]
        [Header(" === Get Chips By Ads === ")]
        [SerializeField] private Button watchVideoBtn;
        [SerializeField]
        private List<Image> getChips;
        [SerializeField]
        private List<CanvasGroup> getChipsAlpha;
        [SerializeField]
        private List<int> getChipsReward;
        [SerializeField]
        private Sprite enable, disable;
        [SerializeField]
        private int adCounter
        {
            get { return PlayerPrefs.GetInt("AdCounter", 0); }
            set { PlayerPrefs.SetInt("AdCounter", value); }
        }
        [SerializeField]
        TextMeshProUGUI nextRewardTimeText;
        private static string nextRewardString = "Next Reward Get In";
        [SerializeField]
        private int nextRewardInTime = 30;

        private DateTime saveNextRewardCurrentTime
        {
            get { return DateTime.Parse(PlayerPrefs.GetString(nextRewardString)); }
            set { PlayerPrefs.SetString(nextRewardString, value.ToString()); }
        }


        private void GetChipsSpriteSet()
        {
            for (int i = 0; i < getChips.Count; i++)
            {
                getChips[i].sprite = (adCounter == i) ? enable : disable;
                getChipsAlpha[i].alpha = (adCounter == i) ? 1 : 0.7f;
            }
        }



        private void NextRewardTimerSet(bool isInitiallySet)
        {
            if (!isInitiallySet)
            {
                saveNextRewardCurrentTime = GetCurrentTime();
                Timer(nextRewardInTime);
            }
            else
            {
                if (!PlayerPrefs.HasKey(nextRewardString))
                {
                    Timer(0);
                }
                else
                {
                    DateTime currentRewardTime = GetCurrentTime();
                    Debug.Log($"NextRewardTimerSet || currenttime {currentRewardTime}");
                    Debug.Log($"NextRewardTimerSet || savetime {saveNextRewardCurrentTime}");
                    int remainTime = (int)(currentRewardTime - saveNextRewardCurrentTime).TotalSeconds;
                    Debug.Log($"NextRewardTimerSet || remain time {remainTime}");
                    if (remainTime <= nextRewardInTime) Timer(nextRewardInTime - remainTime);
                }
            }
        }


        private void Timer(int _remainTime)
        {
            Debug.Log($"Timer || remain Time {_remainTime}");
            watchVideoBtn.gameObject.SetActive(false);
            nextRewardTimeText.gameObject.SetActive(true);
            int currentTime = _remainTime;

            _ = DOTween.To(() => currentTime, x => currentTime = x, 0, _remainTime).SetEase(Ease.Linear).OnUpdate(() =>
            {
                nextRewardTimeText.text = $"{nextRewardString} {currentTime}s";
            }).OnComplete(() =>
            {
                nextRewardTimeText.gameObject.SetActive(false);
                watchVideoBtn.gameObject.SetActive(true);
            });
        }

        private void GetReward(bool isGamePlayReward)
        {
            StartCoroutine(ChipsGetParticlePlay());
            StaticData.isDoTextUsing = true;
            if (isGamePlayReward)
            {
                StaticData.totalBalance += 500;
            }
            else
            {
                StaticData.totalBalance += getChipsReward[adCounter];
                adCounter = adCounter < 2 ? adCounter + 1 : 0;
                Debug.Log($"selected {adCounter}");
                NextRewardTimerSet(false);
                GetChipsSpriteSet();
            }
        }

        public void RewardGetClick(bool isGamePlayReward)
        {
            Debug.Log($"Show Reward Set");
            GoogleAdmob.instance.ShowRewardedAd(GetReward, isGamePlayReward);

        }
    }
}