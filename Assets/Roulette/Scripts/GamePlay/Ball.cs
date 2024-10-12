using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

namespace RouletteByFinix
{
    public class Ball : MonoBehaviour
    {
        [SerializeField] RectTransform ballRectTransform;
        private Vector2 ballStartpos;
        [SerializeField] private GameObject ballPerant;
        [SerializeField] private float forceSpeed, gravityForce;
        [SerializeField] private bool isBallSpin;

        public GameObject center;
        public Rigidbody2D myballRB;

        [Header("Scripts")]
        public Wheel _wheel;
        // public Center _center;
        public LastNumberDisplay lastNumberDisplay;
        public string num;

        private void Start()
        {
            ballStartpos = ballRectTransform.anchoredPosition;
            myballRB.bodyType = RigidbodyType2D.Static;
        }
        void FixedUpdate()
        {
            if (!isBallSpin) return;
            RotateAround(center.transform, transform);
            transform.localEulerAngles = Vector3.zero;
        }
        private void RotateAround(Transform target, Transform rotateObj)
        {
            rotateObj.RotateAround(target.transform.position, new Vector3(0, 0, -1), forceSpeed * Time.deltaTime);
            rotateObj.position = Vector3.MoveTowards(rotateObj.position, target.position, gravityForce * Time.deltaTime);
        }

        private void Gravity()
        {
            Debug.Log($"Gravity");
            RouletteRotater.Instance.DeflactorColliderMakeOn(true);
            Invoke(nameof(DeflactorTouchInvoke), 1f);
            _ = DOTween.To(() => gravityForce, x => gravityForce = x, 1, 0.2f)
            .OnComplete(() =>
            {
                // forceSpeed -= 200;
            });
            // RouletteRotater.Instance.DeflactorColliderMakeOn(true);
            // forceSpeed -= (Random.Range(200, 250));

            //_ = DOTween.To(() => gravityForce, x => gravityForce = x, 0.5f, 2f).OnComplete(() =>
            //{
            //});
        }

        bool isBallTouch;

        System.DateTime touchTime, numberGetTime;

        bool isBallEnter = false;
        GameObject collisionObject = null;
        List<Tween> ballMoveTween = new List<Tween>();

        private void BallMoveTweenKill() => ballMoveTween.ForEach(x => x.Kill());

        private void DeflactorTouchInvoke()
        {
            Debug.Log($"DeflactorTouch Invoke");
            forceSpeed -= 200;
            ballMoveTween.Add(DOTween.To(() => gravityForce, x => gravityForce = x, 4, 5f).SetEase(Ease.Linear).OnKill(() => gravityForce = 0));
            AudioManager.instance.PlayDeflectSound();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            string triggerTag = collision.gameObject.tag;
            Debug.Log($"triggerTag {triggerTag}");

            switch (triggerTag)
            {
                case "Deflactor":

                    Debug.Log($"trigger touch deflactor {collision.gameObject.name}");
                    CancelInvoke(nameof(DeflactorTouchInvoke));
                    forceSpeed -= 200;
                    ballMoveTween.Add(DOTween.To(() => gravityForce, x => gravityForce = x, 4, 5f).SetEase(Ease.Linear).OnKill(() => gravityForce = 0));
                    AudioManager.instance.PlayDeflectSound();
                    break;

                case "InnerWheelCol":
                    Debug.Log($"trigger touch InnerWheelCol {collision.gameObject.name}");
                    BallMoveTweenKill();
                    Debug.Log($"gravity before {gravityForce}");
                    //gravityForce = -1f;
                    Wheel.Instance.OuterWheelColMakeOnOff(true);

                    ballMoveTween.Add(DOTween.To(() => gravityForce, x => gravityForce = x, -5, 1f).SetDelay(0.2f).SetEase(Ease.Linear).OnKill(() => gravityForce = 0));
                    break;

                case "AnchorRoullete":
                    Debug.Log($"isBallEnter {isBallEnter}");
                    if (!isBallEnter)
                    {
                        BallMoveTweenKill();
                        Debug.Log($"AnchorRoullete AnchorRoullete");
                        isBallEnter = true;
                        collisionObject = collision.gameObject;
                        string[] numName = collisionObject.name.Split('_');
                        num = numName[1];
                        Debug.Log($"  NUMBER =  {collisionObject.name} || {numName[1]}");
                        //Static Number Generate for FTUE
                        if (GameController.instance.IsFTUEgameStateOn())
                        {
                            if (num != "17") return;
                            else
                            {
                                numberGetTime = System.DateTime.Now;
                                System.TimeSpan timeDifference = numberGetTime - touchTime;
                                Debug.Log($"time diff {timeDifference.TotalSeconds}");

                                if (timeDifference.TotalSeconds < 1f) return;
                            }
                            BallSpeedTimeCount();
                        }
                        transform.SetParent(collisionObject.transform);
                        transform.DOMove(collisionObject.transform.position, 0f);
                        RouletteRotater.Instance.ReduceTime(false);

                        CancelInvoke(nameof(Vibrate));
                        AudioManager.instance.StopSpinLastStageSound();
                    }
                    break;
            }


        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (!isBallSpin) return;
            string triggerTag = collision.gameObject.tag;
            Debug.Log($"OnTriggerExit2D  tag {triggerTag}");
            switch (triggerTag)
            {
                case "AnchorRoullete":
                    isBallEnter = false;
                    break;
                case "WheelCol":
                    if (!isBallTouch)
                    {
                        RouletteRotater.Instance.DeflactorColliderMakeOn(false);
                        Debug.Log($"OnTriggerExit2D object name {collision.gameObject.name}");
                        gravityForce = 0;
                        BallMoveTweenKill();
                        if (GameController.instance.IsFTUEgameStateOn())
                        {
                            //_ = DOTween.To(() => _wheel.rotationSpeed, x => _wheel.rotationSpeed = x, 50, 5);
                            touchTime = System.DateTime.Now;
                            RouletteRotater.Instance.ReduceTime(true);
                            _ = DOTween.To(() => forceSpeed, x => forceSpeed = x, 100, 0.2f);
                        }
                        else
                        {
                            _ = DOTween.To(() => forceSpeed, x => forceSpeed = x, 0, 1.7f).SetEase(Ease.Linear).OnComplete(() => BallSpeedTimeCount());
                            _ = DOTween.To(() => _wheel.rotationSpeed, x => _wheel.rotationSpeed = x, 50, 2);
                        }

                        Debug.Log($"ball touch");
                        isBallTouch = true;
                        BallEndVibration();
                        AudioManager.instance.PlaySpinLastStageSound();
                    }
                    break;
            }
        }

        private void BallEndVibration()
        {
            CancelInvoke(nameof(Vibrate));
            InvokeRepeating(nameof(Vibrate), 0, 0.2f);
        }

        void Vibrate() => SettingPanelController.VibrateAction?.Invoke();

        public void SpinBall()
        {
            _wheel.isSpinning = true;
            _wheel.rotationSpeed = 100;
            myballRB.bodyType = RigidbodyType2D.Dynamic;
            isBallSpin = true;
            Wheel.Instance.OuterWheelColMakeOnOff(false);
            Invoke(nameof(Gravity), Random.Range(1.8f, 2.2f));
        }

        private void BallSpeedTimeCount() // BALL SPEED SLOW 
        {
            Debug.Log($"BallSpeedTimeCount invoke");
            //_center.enabled = false;
            forceSpeed = 0;
            gravityForce = 0;
            isBallSpin = false;
            AudioManager.instance.PlayAndStopSpinning(false);
            if (!GameController.instance.IsFTUEgameStateOn()) RouletteRotater.Instance.ReduceTime(true);

            Invoke(nameof(AnchorNumbarShow), 0.5f);
        }

        private void AnchorNumbarShow() // Sprite Show Winning Amount
        {
            UiManager.Instance.AnchorNumberShow(num);
            lastNumberDisplay.AddPosition(num); // MOVE TO GET POSITION
            PlayerData.Instance.SpinNumberTypeDataCount(num);
            UiManager.Instance.ResetGameCall();
            isBallEnter = false;
        }

        public void BallResetData()
        {
            transform.SetParent(ballPerant.transform);
            ballRectTransform.anchoredPosition = ballStartpos;
            myballRB.bodyType = RigidbodyType2D.Static;
            transform.eulerAngles = Vector2.zero;
            isBallTouch = false;
            forceSpeed = 400;
            gravityForce = 0.01f;
            _wheel.rotationSpeed = 50;
        }
    }
}