using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
namespace RouletteByFinix
{
    public class ChipsDetailsController : MonoBehaviour
    {
        [SerializeField] private TMPro.TextMeshProUGUI chipsAmountText;
        [SerializeField] private Image chipsImage;
        [SerializeField] private GameObject winLable;
        [SerializeField] TMPro.TextMeshProUGUI lableText;
        [SerializeField] GameObject dltBtn;
        [SerializeField] private ParticleSystem[] chipsAddParticle;

        public void ChipsAmountSet(int amount)
        {
            chipsAmountText.text = $"{amount}";
        }

        public void ChipsDataSet(Sprite chipsSprite)
        {
            chipsImage.sprite = chipsSprite;
        }

        public void LableTextAnim(int multyplyingFactor)
        {
            lableText.text = $"x{multyplyingFactor}";
            winLable.SetActive(true);
            gameObject.transform.DOScale(Vector3.one * 1.2f, 0.5f);
            winLable.transform.DOScale(Vector3.one * 1.5f, 0.5f);
        }

        IEnumerator winChipAddCR;
        public void WinChipAddingCall(Transform target)
        {
            if (winChipAddCR != null) StopCoroutine(winChipAddCR);
            winChipAddCR = WinChipAdding(target);
            StartCoroutine(winChipAddCR);
        }

        private IEnumerator WinChipAdding(Transform target)
        {
            yield return new WaitForSeconds(0.8f);

            for (int i = 0; i < 8; i++)
            {
                Vector3 startPosition = new Vector3(transform.position.x + UnityEngine.Random.Range(-0.7f, 0.7f), transform.position.y + UnityEngine.Random.Range(-0.7f, 0.7f), 0);
                Transform clone = Instantiate(gameObject, gameObject.transform.parent).transform;
                clone.transform.position = startPosition;
                Canvas cloneCanvas = clone.gameObject.AddComponent<Canvas>();
                cloneCanvas.overrideSorting = true;
                cloneCanvas.sortingOrder = 11;
                clone.DOScale(0.5f, 0.3f);
                _ = clone.DOJump(target.position + new Vector3(-0.5f, 0.2f, 0), -0.35f, 1, 0.45f).OnComplete(() =>
                {
                    Destroy(clone.gameObject);
                });
                clone.GetComponent<Image>().DOFade(0.3f, 0.3f);
                AudioManager.instance.PlayCoinAdd();
                yield return new WaitForSeconds(0.05f);
            }

            //for (int i = 0; i < chipsAddParticle.Length; i++)
            //{
            //    ParticleSystem instantParticle = Instantiate(chipsAddParticle[i], gameObject.transform.parent);
            //    instantParticle.Play();

            //    //instantParticle.transform.DOJump(target.position + new Vector3(-0.5f, 0.2f, 0), 0.3f, 1, 0.8f).OnComplete(() =>
            //    instantParticle.transform.DOMove(target.position + new Vector3(-0.5f, 0.2f, 0), 0.7f).OnComplete(() =>
            //   {
            //       Destroy(instantParticle.gameObject);
            //   });
            //}

            //for (int i = 0; i < 9; i++)
            //{
            //    AudioManager.instance.PlayCoinAdd();
            //    yield return new WaitForSeconds(0.07f);
            //}

        }

        public void DeleteBtnMakeOnOff(bool isMakeON)
        {
            dltBtn.SetActive(isMakeON);
        }
        public void OnClickCrossMark()
        {
            Debug.Log($"OnClickCrossMark Alert");
            if (UiManager.Instance.isClearModeOn)
                PaymentHandler.Instance.Undo(false, PaymentHandler.Instance.GetBetFootPrintFromList(this));
        }
    }
}
