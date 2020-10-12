using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TimeManager : MonoBehaviour
{
    private int seconds;
    private int minutes;
    public Text timerTextRef;
    void Start()
    {
        this.seconds = 0;
        this.minutes = 0;
        InvokeRepeating("incrementSecond", 0, 1);
        InvokeRepeating("updateTimer", 0, 1);
    }

    public void incrementSecond()
    { 
        this.seconds++;
        if(this.seconds==60)
        {
            this.seconds=0;
            this.minutes++;
        }
    }

    private void updateTimer()
    {
        if(this.seconds<10)
            this.timerTextRef.text = "Czas gry: "+ this.minutes +":0"+ this.seconds;
        else this.timerTextRef.text = "Czas gry: "+ this.minutes +":"+ this.seconds;
    }

    public string getTimeString()
    {
        if(this.seconds>9)
         return this.minutes+":"+this.seconds;
        else return this.minutes+":0"+this.seconds;
    }
}
