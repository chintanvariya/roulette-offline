using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

namespace PrahantGames
{
    public class Coin : MonoBehaviour
    {
        public int coinValue;
        public GameObject ring;
        public Button coinBtn;
        public Transform chipsImg;

        //public bool isSelected;

        public void OnClick(bool isClickAudioClipRun)
        {
            if (GameController.instance.IsFTUEgameStateOn())
            {
                if (FTUEController.instance.ftuePhase == FTUEController.Phase.BetSelection)
                {
                    FTUEController.instance.GetCallbackOnChooseBet();
                }
            }

            foreach (Coin coin in CoinsManager.instance.coinRing)
            {
                coin.ring.SetActive(false);
                coin.IpoiterExit();
            }

            chipsImg.DOComplete();
            CoinsManager.coinAmount = coinValue;
            chipsImg.DOScale(1.2f, .2f);
            ring.SetActive(true);
            if (isClickAudioClipRun) AudioManager.instance.PlayObjectSelect();
        }
        public void IpoiterEnter()
        {
            chipsImg.DOComplete();
            chipsImg.DOScale(1.2f, .2f);
        }

        public void IpoiterExit()
        {
            chipsImg.DOComplete();
            chipsImg.DOScale(1, .2f);
        }

    }
}