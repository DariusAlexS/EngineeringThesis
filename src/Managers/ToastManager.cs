using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ToastManager : MonoBehaviour
{
    public float decayFactor;
    public TextMeshProUGUI toastTextRef;
    void Update()
    {
        
        if(this.toastTextRef.color.a > 0f)
            this.toastTextRef.color = new Color(this.toastTextRef.color.r,this.toastTextRef.color.g,this.toastTextRef.color.b,this.toastTextRef.color.a-this.decayFactor);
        if(this.toastTextRef.color.a < 0.2f)
            this.toastTextRef.text = "";
    }

    public void displayMessage(string message, bool isError)
    {
        
        if(isError)
            this.toastTextRef.color = new Color(0.82f,0.07f,0.07f,1f);
        else    
            this.toastTextRef.color = new Color(0f,0f,0f,1f);
        if(isMessageStillDisplaying() && this.toastTextRef.text.Length<30)
            this.toastTextRef.text = this.toastTextRef.text + "\n" + message;
        else this.toastTextRef.text = message;
    }

    public void changeToFontColorToPositive()
    { this.toastTextRef.color = new Color(0.0f, 0.75f, 0.0f,1f);}

    private bool isMessageStillDisplaying()
    { return this.toastTextRef.color.a > 0.2f;}
}
