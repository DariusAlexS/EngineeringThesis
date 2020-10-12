using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class OpenWindow : MonoBehaviour
{
    public GameObject window;
    public void openWindow()
    {
        window.SetActive(true);
    }
}
