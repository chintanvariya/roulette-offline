using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;
using System;

namespace PrahantGames
{
    public class BetCenter : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
    {
        public SpinningScriptable winningObject;
        public List<Image> animImage;
        public BetData betData;
        ChipsDetailsController chipClone;
        private int chipsCount;
        public List<ChipsDetailsController> chipsList = new List<ChipsDetailsController>();

       
        void Start()
        {
            FTUEController.FTUE_ResetAllData += FTUEEndReset;
        }

        public void AnimImageSet()
        {
           

            Debug.Log($"ANim {gameObject.name}");

            if (winningObject == null) return;
            animImage = new List<Image>(UiManager.Instance.numberBoxList.Where(box => winningObject.pieceNumber.Contains(box.highLightBox.numberValue)).
                Select(box => box.highLightBox.highLIghtImg).ToList());
            Debug.Log($"Anim Image count {animImage.Count}");
        }


        private void FTUEEndReset()
        {
            chipsList.ForEach(refChip => DestroyImmediate(refChip.gameObject));
            chipsList.Clear();
            betData.isSelected = false;
            betData.betValue = 0;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log($"click object name {transform.name}", gameObject);
            if (UiManager.Instance.wheel.isSpinning || PaymentHandler.Instance.isChipsMoving || UiManager.Instance.isScreenMoving)
                return;
            if (UiManager.Instance.isClearModeOn)
            {
                if (chipsList.Count > 0)
                {
                    PaymentHandler.Instance.Undo(false, PaymentHandler.Instance.GetBetFootPrintFromList(chipsList.Last()));
                    return;
                }
                else
                {
                    UiManager.Instance.ClearModeOnOff();
                    return;
                }
            }

            if (!CoinsManager.instance.IsChipsAvailableAtClick())
            {
                Debug.Log($"ChipsAddPanelMakeOn ");
                UiManager.Instance.ChipsAddPanelMakeOn();
                // Open Chips Add Panel
                return;
            }

            if (!betData.isSelected)
            {
                PaymentHandler.addBetNum?.Invoke(this);
                betData.isSelected = true;
            }

            animImage.ForEach(refImg => refImg.gameObject.SetActive(true));

            Invoke(nameof(HighLightMakeFalse), 0.1f);

            PaymentHandler.Instance.AddBetPoints(this);

            chipClone = CoinsManager.instance.ChipsInitiate(CoinsManager.coinAmount);

            chipsCount = chipsList.Count;
            chipsList.Add(chipClone);
            chipClone.transform.SetParent(transform, false);

            if (GameController.instance.IsFTUEgameStateOn())
            {
                FTUEController.instance.GetCallbackOnPlaceBet();
            }

            ChipsAddedAnimation(chipClone.gameObject, chipsCount, true, () =>
             {
                 chipClone.ChipsAmountSet(betData.betValue);
                 UiManager.Instance.undoBtn.interactable = true;
                 UiManager.Instance.clearBtn.interactable = true;
                 UiManager.Instance.rebetBtn.interactable = false;
             });

            ApplyBet(CoinsManager.coinAmount);
            CoinsManager.instance.BetStore();
            RouletteRotater.addChips?.Invoke(chipClone.gameObject);
            betData.betValue += CoinsManager.coinAmount;
            chipClone.name = betData.betValue.ToString();
        }

        public void ChipsAddedAnimation(GameObject chipsClone, int existChipsCount, bool isAddClipPlay, Action EndCallBack = null)
        {
            PaymentHandler.Instance.IsChipsMovingMakeTrueFalse(true);
            //Chips Added Sound Call
            if (isAddClipPlay) AudioManager.instance.PlayCoinAdd();
            chipsClone.transform.localPosition = new Vector2(0, 85);
            chipsClone.transform.localScale = Vector2.one * 1.5f;
            chipsClone.transform.DOScale(Vector2.one, 0.15f);
            Canvas cloneCanvas = chipsClone.gameObject.AddComponent<Canvas>();
            cloneCanvas.overrideSorting = true;
            cloneCanvas.sortingOrder = GameController.instance.IsFTUEgameStateOn() ? 14 : 12; //12

            chipsClone.transform.DOLocalMove(existChipsCount == 0 ? Vector2.zero : new Vector2(-4, 4), 0.15f).OnComplete(() =>
            {
                EndCallBack?.Invoke();
                DestroyImmediate(cloneCanvas);
                PaymentHandler.Instance.IsChipsMovingMakeTrueFalse(false);
            });
        }


        private void HighLightMakeFalse()
        {
            Debug.Log($"HighLightMakeFalse");
            animImage.ForEach(refImg => refImg.gameObject.SetActive(false));
        }


        public void ApplyBet(float selectedValue)
        {
            PaymentHandler.Instance.Add(this, selectedValue);
        }

        public void RemoveBet(BetCenter betCenter, float value)
        {
            print("Remove ++++ >> | " + value + " || " + betCenter.gameObject.name);

            betCenter.betData.betValue -= (int)value;
            var lastChips = chipsList.Last();
            lastChips.ChipsAmountSet((int)value);
            chipsList.Remove(lastChips);
            Debug.Log($"lastchips {lastChips.name}");

            if (chipsList.Count > 0)
            {
                chipsList.Last().ChipsAmountSet(betCenter.betData.betValue);
            }

            if (betCenter.betData.betValue == 0)
                betCenter.betData.isSelected = false;

            ChipsRemoveAnimation(lastChips.gameObject, () =>
            {
                Debug.Log($"chips destroy");
                DestroyImmediate(lastChips.gameObject);
            });

            Debug.Log($"RemoveBet ||  Plus {(int)value}");
            StaticData.isBalanceStore = false;
            CoinsManager.instance.betValue -= (int)value;
            StaticData.totalBalance += (int)value;
        }

        public void ChipsRemoveAnimation(GameObject chipsClone, Action EndCallBack = null)
        {
            PaymentHandler.Instance.IsChipsMovingMakeTrueFalse(true);
            Canvas cloneCanvas = chipsClone.gameObject.AddComponent<Canvas>();
            cloneCanvas.overrideSorting = true;
            cloneCanvas.sortingOrder = 12;
            chipsClone.transform.DOScale(Vector2.one * 1.5f, 0.15f);
            chipsClone.GetComponent<Image>().DOFade(0.8f, 0.15f);
            chipsClone.transform.DOLocalMove(new Vector2(0, 85), 0.15f).OnComplete(() =>
            {
                EndCallBack?.Invoke();
                PaymentHandler.Instance.IsChipsMovingMakeTrueFalse(false);
            });
        }

        public void Rebet()
        {
            if (StaticData.totalBalance <= 0)
                return;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (UiManager.Instance.wheel.isSpinning) return;
            if (UiManager.Instance.isClearModeOn) return;
            animImage.ForEach(refImg => refImg.gameObject.SetActive(true));
        }
        public void OnPointerUp(PointerEventData eventData)
        {
            Invoke(nameof(HighLightMakeFalse), 0.1f);
        }


    }
    [Serializable]
    public class BetData
    {
        public bool isSelected;
        public int betValue;
    }

    public class ChipsDetails
    {
        public int chipsAmount;

        public ChipsDetails(int chipsAmount)
        {
            this.chipsAmount = chipsAmount;
        }
    }
    [System.Serializable]
    public class ChipsSprite
    {
        public Sprite chipsSprite;
        public int amount;
    }
}
