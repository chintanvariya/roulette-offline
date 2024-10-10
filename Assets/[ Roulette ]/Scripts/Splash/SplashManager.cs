using System.Collections;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace PrahantGames
{
    public class SplashManager : MonoBehaviour
    {
        [SerializeField] private GameObject splashCanvas;
        [SerializeField] private Image loadingFillImg;
        [SerializeField] private float loadingTime;
        [SerializeField] private TMPro.TextMeshProUGUI percentageText;


        private void Start()
        {
            splashCanvas.SetActive(true);
            loadingFillImg.fillAmount = 0;
            GamePlaySceneLoading();
        }
        private void SplashCanvasMakeOff()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }


        private void GamePlaySceneLoading()
        {
            _ = StartCoroutine(LoaderFill());
        }

        private IEnumerator LoaderFill()
        {
            int totalRandom = 0;

            for (int i = 0; i < (int)loadingTime; i++)
            {
                Debug.Log("int i index || " + i + "  loading time || " + loadingTime);

                if (i == (int)loadingTime - 1) totalRandom = 100;
                else
                {
                    int refere = (int)(100 / loadingTime);
                    totalRandom += Random.Range(refere / 2, refere + (i > 0 ? 0 : refere / 2));
                }

                yield return new WaitForSeconds(1);
                var loadAnim = loadingFillImg.DOFillAmount((float)totalRandom / 100, 0.5f).SetEase(Ease.InOutCirc);

                loadAnim.OnUpdate(() =>
                {
                    float progress = loadingFillImg.fillAmount * 100;
                    percentageText.text = $"{progress:F0}%";
                });

                if (totalRandom == 100)
                {
                    yield return new WaitForSeconds(1);

                    Debug.Log("isSplashLoadSuccess");
                    SplashCanvasMakeOff();
                }
            }
        }


    }
}