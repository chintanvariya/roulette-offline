using UnityEngine;

namespace PrahantGames
{
    public class Wheel : MonoBehaviour
    {
        public static Wheel Instance;
        [SerializeField] private CircleCollider2D outerWheelCol;

        private void Awake()
        {
            Instance = this;
        }


        public float rotationSpeed;
        public bool isSpinning;

        void FixedUpdate()
        {
            // Rotate the game object around its up axis (Y-axis) over time
            transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
        }

        public void OuterWheelColMakeOnOff(bool isMakeOn)
        {
            outerWheelCol.enabled = isMakeOn;
        }
    }
}