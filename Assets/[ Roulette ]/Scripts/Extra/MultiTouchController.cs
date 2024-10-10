using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PrahantGames
{
    public class MultiTouchController : MonoBehaviour
    {
        int count = 0;
        private void Awake()
        {
            InvokeRepeating(nameof(MultiTouchMakeFalse), 0, 0.2f);
            DontDestroyOnLoad(this.gameObject);
        }
        private void MultiTouchMakeFalse()
        {
            Debug.Log($"version {Application.version}");
            Debug.Log($"MultiTouchMakeFalse");
            Input.multiTouchEnabled = false;
            count++;
            if (count > 10) CancelInvoke(nameof(MultiTouchMakeFalse));
        }
    }
}