using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PrahantGames
{
    public class HandController : MonoBehaviour
    {
        public void OnTouchToObject()
        {
            Debug.Log($"hand is touch to object");
            FTUEController.instance.HandTouchToObject();
        }


        public void OnEndOfClip()
        {
            Debug.Log($"Clip completed;");
            FTUEController.instance.OnHandClipCompleted();
        }
    }
}
