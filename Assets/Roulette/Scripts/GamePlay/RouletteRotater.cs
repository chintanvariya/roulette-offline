using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RouletteByFinix
{
    public class RouletteRotater : MonoBehaviour
    {
        public static System.Action<GameObject> addChips;

        public static RouletteRotater Instance;
        private List<BoxCollider2D> anchorNumbar;
        public List<BoxCollider2D> deflactorCollider;

        [SerializeField] private List<BoxCollider2D> americanAnchor;
        [SerializeField] private List<BoxCollider2D> europeAnchor;

        [SerializeField] private Image wheelImage;
        [SerializeField] private Sprite americanWheel, euroWheel;

        [SerializeField] private GameObject singleZeroParent, doubleZeroParent;

        

        private void Awake() => Instance = this;

        private void Start()
        {
            SetAnchorNumberAsPerMode(GameController.instance.gameMode);
        }

        public void SetAnchorNumberAsPerMode(GameMode currentGameMode)
        {
            anchorNumbar = currentGameMode.Equals(GameMode.American) ? americanAnchor : europeAnchor;
            wheelImage.sprite = currentGameMode.Equals(GameMode.American) ? americanWheel : euroWheel;
            singleZeroParent.SetActive(currentGameMode.Equals(GameMode.Europe));
            doubleZeroParent.SetActive(currentGameMode.Equals(GameMode.American));
           
        }

        public void ReduceTime(bool status)
        {
            for (int i = 0; i < anchorNumbar.Count; i++)
                anchorNumbar[i].enabled = status;
        }

        public void DeflactorColliderMakeOn(bool isMakeOn)
        {
            Debug.Log($"DeflactorColliderMakeOn");
            if (isMakeOn)
                deflactorCollider[Random.Range(0, deflactorCollider.Count)].enabled = true;
            else
                deflactorCollider.ForEach(x => x.enabled = isMakeOn);
        }
    }
}
