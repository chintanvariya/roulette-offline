using GoogleMobileAds.Api;
using System;
using UnityEngine;
namespace RouletteByFinix
{
    public class GoogleAdmob : MonoBehaviour
    {

        //public string appId = "ca-app-pub-3940256099942544~3347511713";

        string bannerId;
        string interId;
        string rewardedId;


        BannerView bannerView;
        InterstitialAd interstitialAd;
        RewardedAd rewardedAd;


        public bool isTestApp;

        private bool isInitialize = false;
        private bool isInitializing = false;

        public AdsIdController testID;
        public AdsIdController realID;


        [Serializable]
        public class AdsIdController
        {
            public AdsId AndroidMode;
            public AdsId IOSMode;
        }
        [Serializable]
        public class AdsId
        {
            public string bannerId;
            public string interId;
            public string rewardedId;
        }


        private void AdIDAssign(AdsIdController adsIDController)
        {
#if UNITY_ANDROID
            bannerId = adsIDController.AndroidMode.bannerId;
            interId = adsIDController.AndroidMode.interId;
            rewardedId = adsIDController.AndroidMode.rewardedId;

#elif UNITY_IPHONE
     bannerId = adsIDController.IOSMode.bannerId;
     interId = adsIDController.IOSMode.interId;
     rewardedId = adsIDController.IOSMode.rewardedId;
#endif
        }

        public static GoogleAdmob instance;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else Destroy(this.gameObject);
        }

        private void Start()
        {
            AdIDAssign(isTestApp ? testID : realID);

            Debug.Log($"bannerId {bannerId}");
            Debug.Log($"interId {interId}");
            Debug.Log($"rewardedId {rewardedId}");



            MobileAds.RaiseAdEventsOnUnityMainThread = true;
            Initialization();
        }

        public void NetworkChecking()
        {
            InvokeRepeating(nameof(NetworkCheck), 10, 10);
        }

        private void Initialization()
        {
            isInitializing = true;
            isInitialize = false;
            MobileAds.Initialize(initStatus =>
            {
                isInitializing = false;
                isInitialize = true;
                print("Ads Initialised !!");
                LoadBannerAd();
                LoadInterstitialAd();
                LoadRewardedAd();
            });
        }

        #region Banner

        public void LoadBannerAd()
        {
            Debug.Log($"LoadBannerAd .... ");
            //create a banner
            CreateBannerView();

            //listen to banner events
            ListenToBannerEvents();

            ////load the banner
            //if (bannerView == null)
            //{
            //    CreateBannerView();
            //}
            ShowBannerAd();
        }

        public void ShowBannerAd()
        {
            var adRequest = new AdRequest();
            adRequest.Keywords.Add("unity-admob-sample");
            print("Showing banner Ad !!");
            bannerView.LoadAd(adRequest);//show the banner on the screen
                                         // BannerShowAndHide(false);
        }

        public void BannerShowAndHide(bool isShow)
        {
            Debug.Log($"banner view {isShow}");
            if (bannerView != null)
            {
                if (isShow)
                {
                    bannerView.Show();
                }
                else bannerView.Hide();
            }
        }


        void CreateBannerView()
        {
            if (bannerView != null) return;
            //if (bannerView != null)
            //{
            //    DestroyBannerAd();
            //}
            bannerView = new BannerView(bannerId, AdSize.SmartBanner, AdPosition.Bottom);
        }
        void ListenToBannerEvents()
        {
            bannerView.OnBannerAdLoaded += () =>
            {
                Debug.Log("Banner view loaded an ad with response : "
                    + bannerView.GetResponseInfo());
            };
            // Raised when an ad fails to load into the banner view.
            bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
            {
                Debug.LogError("Banner view failed to load an ad with error : "
                    + error);
                // LoadBannerAd();
            };
            // Raised when the ad is estimated to have earned money.
            bannerView.OnAdPaid += (AdValue adValue) =>
            {
                Debug.Log("Banner view paid {0} {1}." +
                    adValue.Value +
                    adValue.CurrencyCode);
            };
            // Raised when an impression is recorded for an ad.
            bannerView.OnAdImpressionRecorded += () =>
            {
                Debug.Log("Banner view recorded an impression.");
            };
            // Raised when a click is recorded for an ad.
            bannerView.OnAdClicked += () =>
            {
                Debug.Log("Banner view was clicked.");
            };
            // Raised when an ad opened full screen content.
            bannerView.OnAdFullScreenContentOpened += () =>
            {
                Debug.Log("Banner view full screen content opened.");
            };
            // Raised when the ad closed full screen content.
            bannerView.OnAdFullScreenContentClosed += () =>
            {
                LoadBannerAd();
                Debug.Log("Banner view full screen content closed.");
            };
        }
        public void DestroyBannerAd()
        {
            if (bannerView != null)
            {
                print("Destroying banner Ad");
                bannerView.Destroy();
                bannerView = null;

                LoadBannerAd();
            }
        }
        #endregion

        #region Interstitial

        public void LoadInterstitialAd()
        {
            if (Time.timeScale == 0) Time.timeScale = 1;
            if (interstitialAd != null)
            {
                interstitialAd.Destroy();
                interstitialAd = null;
            }
            var adRequest = new AdRequest();
            adRequest.Keywords.Add("unity-admob-sample");

            InterstitialAd.Load(interId, adRequest, (InterstitialAd ad, LoadAdError error) =>
            {
                if (error != null || ad == null)
                {
                    print("Interstitial ad failed to load" + error);
                    return;
                }

                print("Interstitial ad loaded !!" + ad.GetResponseInfo());

                interstitialAd = ad;
                InterstitialEvent(interstitialAd);
            });

        }
        public void ShowInterstitialAd()
        {

            if (interstitialAd != null && interstitialAd.CanShowAd())
            {
                interstitialAd.Show();
            }
            else
            {
                print("Intersititial ad not ready!!");
            }
        }
        public void InterstitialEvent(InterstitialAd ad)
        {
            // Raised when the ad is estimated to have earned money.
            ad.OnAdPaid += (AdValue adValue) =>
            {
                Debug.Log("Interstitial ad paid {0} {1}." +
                    adValue.Value +
                    adValue.CurrencyCode);
            };
            // Raised when an impression is recorded for an ad.
            ad.OnAdImpressionRecorded += () =>
            {
                Debug.Log("Interstitial ad recorded an impression.");
            };
            // Raised when a click is recorded for an ad.
            ad.OnAdClicked += () =>
            {
                Debug.Log("Interstitial ad was clicked.");
            };
            // Raised when an ad opened full screen content.
            ad.OnAdFullScreenContentOpened += () =>
            {
                Debug.Log("Interstitial ad full screen content opened.");
            };
            // Raised when the ad closed full screen content.
            ad.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log("Interstitial ad full screen content closed.");
                LoadInterstitialAd();
            };
            // Raised when the ad failed to open full screen content.
            ad.OnAdFullScreenContentFailed += (AdError error) =>
            {
                Debug.LogError("Interstitial ad failed to open full screen content " +
                               "with error : " + error);
                // LoadInterstitialAd();
            };
        }

        #endregion

        #region Rewarded

        public void LoadRewardedAd()
        {
            if (rewardedAd != null)
            {
                rewardedAd.Destroy();
                rewardedAd = null;
            }
            var adRequest = new AdRequest();
            adRequest.Keywords.Add("unity-admob-sample");

            RewardedAd.Load(rewardedId, adRequest, (RewardedAd ad, LoadAdError error) =>
            {
                if (error != null || ad == null)
                {
                    print("Rewarded failed to load" + error);

                    return;
                }

                print("Rewarded ad loaded !!");
                rewardedAd = ad;
                RewardedAdEvents(rewardedAd);
            });
        }
        public void ShowRewardedAd(Action<bool> RewardGet, bool isGamePlayReward)
        {
            if (rewardedAd != null && rewardedAd.CanShowAd())
            {
                rewardedAd.Show((Reward reward) =>
                {
                    print("Give reward to player !!");
                    RewardGet?.Invoke(isGamePlayReward);
                });
            }
            else
            {
                StartCoroutine(UiManager.Instance.CheckNetwork(false));
                UiManager.Instance.AdsNotLoadPopup();
                print("Rewarded ad not ready");
            }
        }
        public void RewardedAdEvents(RewardedAd ad)
        {
            // Raised when the ad is estimated to have earned money.
            ad.OnAdPaid += (AdValue adValue) =>
            {
                Debug.Log("Rewarded ad paid {0} {1}." +
                    adValue.Value +
                    adValue.CurrencyCode);
            };
            // Raised when an impression is recorded for an ad.
            ad.OnAdImpressionRecorded += () =>
            {
                Debug.Log("Rewarded ad recorded an impression.");
            };
            // Raised when a click is recorded for an ad.
            ad.OnAdClicked += () =>
            {
                Debug.Log("Rewarded ad was clicked.");
            };
            // Raised when an ad opened full screen content.
            ad.OnAdFullScreenContentOpened += () =>
            {
                Debug.Log("Rewarded ad full screen content opened.");
            };
            // Raised when the ad closed full screen content.
            ad.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log("Rewarded ad full screen content closed.");
                LoadRewardedAd();
            };
            // Raised when the ad failed to open full screen content.
            ad.OnAdFullScreenContentFailed += (AdError error) =>
            {
                Debug.LogError("Rewarded ad failed to open full screen content " +
                               "with error : " + error);
                // LoadRewardedAd();
            };
        }

        private void NetworkCheckForAds(bool isNetworkOn)
        {
            if (!isNetworkOn) return;

            if (!isInitialize) Initialization();
            PAInAppPurchasing.instance.IAP_Initialization();

            if (isInitializing || !isInitialize) return;
            // Debug.Log($"NetworkCheckForAds");

            Debug.Log($"Banner View instance {bannerView}");

            if (bannerView == null)
            {
                Debug.Log("banner load repeated");
                LoadBannerAd();
            }

            if (rewardedAd == null)
            {
                LoadRewardedAd();
            }

            if (interstitialAd == null)
            {
                LoadInterstitialAd();
            }

        }

        private void NetworkCheck()
        {
            //  Debug.Log($"NetworkCheck");
            StartCoroutine(UiManager.Instance.CheckNetwork(true, NetworkCheckForAds));

        }


        #endregion
    }
}
