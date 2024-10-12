using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;
using UnityEngine.Purchasing;
namespace RouletteByFinix
{
    public class PAInAppPurchasing : MonoBehaviour, IStoreListener
    {
        public static PAInAppPurchasing instance;
        private Action OnPurchaseCompleted;
        private IStoreController StoreController;
        private IExtensionProvider ExtensionProvider;

        [SerializeField] private List<PurchaseData> purchaseData;

        public bool isFirstPurchaseBonusGet
        {
            get { return (PlayerPrefs.GetString("isFirstPurchaseBonusGet") == "True"); }
            set { PlayerPrefs.SetString("isFirstPurchaseBonusGet", value.ToString()); }
        }
        private void Awake()
        {
            if (instance == null)
                instance = this;
        }

        public void IAP_Initialization()
        {
            if (StoreController == null || ExtensionProvider == null)
            {
                Inst();
            }
        }


        public async void Inst()
        {
            InitializationOptions options = new InitializationOptions()
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                .SetEnvironmentName("test");
#else
            .SetEnvironmentName("production");
#endif
            await UnityServices.InitializeAsync(options);
            ResourceRequest operation = Resources.LoadAsync<TextAsset>("IAPProductCatalog");
            operation.completed += HandleIAPCatalogLoaded;
        }

        private void HandleIAPCatalogLoaded(AsyncOperation Operation)
        {
            ResourceRequest request = Operation as ResourceRequest;

            Debug.Log($"Loaded Asset: {request.asset}");
            ProductCatalog catalog = JsonUtility.FromJson<ProductCatalog>((request.asset as TextAsset).text);
            Debug.Log($"Loaded catalog with {catalog.allProducts.Count} items");

            // ------
            /*  StandardPurchasingModule.Instance().useFakeStoreUIMode = FakeStoreUIMode.StandardUser;
              StandardPurchasingModule.Instance().useFakeStoreAlways = true;
    */
#if UNITY_ANDROID
            ConfigurationBuilder builder = ConfigurationBuilder.Instance(
                StandardPurchasingModule.Instance(AppStore.GooglePlay)
            );
#elif UNITY_IOS
        ConfigurationBuilder builder = ConfigurationBuilder.Instance(
            StandardPurchasingModule.Instance(AppStore.AppleAppStore)
        );
#else
            ConfigurationBuilder builder = ConfigurationBuilder.Instance(
                StandardPurchasingModule.Instance(AppStore.NotSpecified)
            );
#endif
            foreach (ProductCatalogItem item in catalog.allProducts)
            {
                Debug.Log("ID IAP=> " + item.id + "" + item.type);
                builder.AddProduct(item.id, item.type);
            }

            Debug.Log($"Initializing Unity IAP with {builder.products.Count} products");
            UnityPurchasing.Initialize(this, builder);

        }

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            StoreController = controller;
            ExtensionProvider = extensions;
            Debug.Log($"Successfully Initialized Unity IAP. Store Controller has {StoreController.products.all.Length} products");
            /* PAStoreIconProvider.Initialize(StoreController.products);
             PAStoreIconProvider.OnLoadComplete += HandleAllIconsLoaded;*/
            HandleAllIconsLoaded();
        }

        private void HandleAllIconsLoaded()
        {
            Debug.Log("ON call");
            StartCoroutine(CreateUI());
        }

        [SerializeField] private List<PAProduct> getProducts = new List<PAProduct>();

        private IEnumerator CreateUI()
        {
            List<Product> sortedProducts = StoreController.products.all.ToList();
            Debug.Log("sortedProducts => " + sortedProducts.Count);

            for (int i = 0; i < sortedProducts.Count; i++)
            {
                Product product = sortedProducts[i];
                PAProduct uiProduct = getProducts[i];
                uiProduct.OnPurchase += HandlePurchase;
                uiProduct.Setup(product, purchaseData[i].chipsQty);
                yield return null;
            }
        }

        private void HandlePurchase(Product Product, Action OnPurchaseCompleted)
        {
            this.OnPurchaseCompleted = OnPurchaseCompleted;
            StoreController.InitiatePurchase(Product);
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            Debug.Log($"Failed to purchase {product.definition.id} because {failureReason}");
            OnPurchaseCompleted?.Invoke();
            OnPurchaseCompleted = null;
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
        {
            Debug.Log($"Successfully purchased {purchaseEvent.purchasedProduct.definition.id}");
            Debug.Log($"Successfully purchased {purchaseEvent.purchasedProduct.metadata.localizedPrice}");
            OnPurchaseCompleted?.Invoke();
            OnPurchaseCompleted = null;

            int buyChips = purchaseData.Find(purchase => purchase.purchaseId == purchaseEvent.purchasedProduct.definition.id).chipsQty;
            Debug.Log($"Plus buy chips {buyChips}");
            StaticData.totalBalance += buyChips;


            if (!isFirstPurchaseBonusGet)
            {
                Debug.Log($"Give First Purchase Bonus");
                StaticData.totalBalance += 15000;
                isFirstPurchaseBonusGet = true;
            }

            UiManager.Instance.ChipsStoreClose();
            return PurchaseProcessingResult.Complete;
        }



        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            Debug.LogError($"Error initializing IAP because of {error}." +
                 $"\r\nShow a message to the player depending on the error.");
        }
        public void OnInitializeFailed(InitializationFailureReason error)
        {
            Debug.LogError($"Error initializing IAP because of {error}." +
                 $"\r\nShow a message to the player depending on the error.");
        }
    }
    [System.Serializable]
    public class PurchaseData
    {
        public string purchaseId;
        public int chipsQty;
    }

}