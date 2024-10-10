using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PrahantGames
{
    public class WheelPrototypeHandler : MonoBehaviour
    {
        [SerializeField] private float rotationSpeed;
        void FixedUpdate()
        {
            // Rotate the game object around its up axis (Z-axis) over time
            transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
        }
    }
}
