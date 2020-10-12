using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowManager : MonoBehaviour
{
    public GameObject panelToShow;
    public GameObject panelToHide;
    
    public void openWindow()
    {
        this.panelToShow.SetActive(true);
        this.panelToHide.SetActive(false);
    }
}
