using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MinimapIconColor : MonoBehaviour
{
    public GameObject gObj;
    public GameObject colorSource;
    private SpriteRenderer minmapIcon;
    private Image imageToTakeColorFrom;

    void Start()
    {
        minmapIcon = gObj.GetComponent<SpriteRenderer>();
        imageToTakeColorFrom = colorSource.GetComponent<Image>();
    }
    // Update is called once per frame
    void Update()
    {
        this.minmapIcon.color = imageToTakeColorFrom.color;
    }
}
