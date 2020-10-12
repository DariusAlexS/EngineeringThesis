using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class MeasurementManager : MonoBehaviour
{

    public Text WPMTextRef;
    public Text precisionTextRef;
    public TextMeshProUGUI playerBonusTextRef;
    public HumanPlayer humanPlayerRef;
    private double totalMeasuredTimeInMiliseconds;
    private int totalCharsTyped;
    private int totalErrors;
    private int errorsTillNextPunishment;
    public int errorsBetweenPunishments;
    public int keysBetweenBonusIncreases;
    private int keysPressedSinceLastMistake;
    private int backspaceTimesPressed;
    private double currentWPM;

    private List<string> mistakenKeysList;
    void Start()
    {
        currentWPM=0;
        totalMeasuredTimeInMiliseconds = 0;
        totalCharsTyped = 0;
        keysPressedSinceLastMistake = 0;
        totalErrors = 0;
        mistakenKeysList = new List<string>(400);
        this.WPMTextRef.text = "0.0";
        this.backspaceTimesPressed = 0;
        updatePlayerBonusText();
        resetErrorsTillNextPunishment();
    }

    void Update()
    {
        this.precisionTextRef.text = "Precyzja: "+ calculatePrecisionStringValue();
        punishPlayerBasedOnPerformance();
    }

    public void displayKeyStats()
    {
        Debug.Log("Wszystkie wpisane: "+ this.totalCharsTyped);
        Debug.Log("Błędy wszystkie: " + this.totalErrors);
        Debug.Log("Błędy do kary: " + this.errorsTillNextPunishment);
    }
    public void addTotalMeasurmentTime(double measuredTime) 
    { 
        this.totalMeasuredTimeInMiliseconds+=measuredTime;
        Debug.Log("Zmierzony czas(ms): " + measuredTime);
        this.WPMTextRef.text = "WPM: " + calculateWPMStringValue();
    }
  

    private string calculatePrecisionStringValue()
    {return calculatePrecision().ToString("P2");}

    public string calculateWPMStringValue()
    {return calculateWPM().ToString("0.#");}
    
    // calculate performance values
    public double calculatePrecision()
    {
        if(this.totalCharsTyped>0)
             return 1 - (double)this.totalErrors / (double)totalCharsTyped;
        else return 1;
    }
    public double calculateWPM()
    {
        if(totalMeasuredTimeInMiliseconds>0)
            return (double)this.totalCharsTyped/totalMeasuredTimeInMiliseconds*12000; 
        else return 0;
        
    }
    public double getPlayerWPM()
    { return calculateWPM(); }

    // update text methods
    private void updatePrecisionText()
    {
        this.precisionTextRef.text = "Prezycja: " + (1- totalErrors/totalCharsTyped) + "%";
    }
    private void updatePlayerBonusText()
    {
        double playerBonus = this.humanPlayerRef.getPlayerBonusModifier()-1.0;
        if(playerBonus>=0)
            this.playerBonusTextRef.color = new Color(0.0f, 0.65f, 0f, 1f);
        else this.playerBonusTextRef.color = new Color(0.7f, 0f, 0f, 1f);
        this.playerBonusTextRef.text = "Bonus: " + playerBonus.ToString("P0");   
    }

    private void resetErrorsTillNextPunishment()
    { this.errorsTillNextPunishment = this.errorsBetweenPunishments;}

    public void resetKeysPressedSinceLastMistake()
    { this.keysPressedSinceLastMistake = 0;}

    private void reduceKeysTillPunishment()
    { this.errorsTillNextPunishment--; }

    private void punishPlayerBasedOnPerformance()
    {
        if(doesPlayerDeservePunishment())
        {
            punishPlayerForMistakes();
            resetErrorsTillNextPunishment();
        }
    }
    public void punishPlayerForMistakes()
    { 
        this.humanPlayerRef.issuePunishment();
        updatePlayerBonusText();
    }

    private bool doesPlayerDeservePunishment()
    {
        return this.errorsTillNextPunishment <= 0 && this.totalCharsTyped>0;
    }
    private bool doesPlayerDeserveBonusIncrease()
    { 
        return (this.keysPressedSinceLastMistake>0 && this.keysPressedSinceLastMistake % this.keysBetweenBonusIncreases == 0);
    }
   
    public void increasePlayerBonus()
    { 
        this.humanPlayerRef.increasePlayerBonus(keysPressedSinceLastMistake/keysBetweenBonusIncreases);
        updatePlayerBonusText();
    }
    // increment methods
    public void incrementBackspaceTimesPressed()
    {
        this.backspaceTimesPressed++;
        this.humanPlayerRef.deduceBonusForBackspaceUse();
        updatePlayerBonusText();
    }
    public void incrementTotalCharsTyped() 
    { this.totalCharsTyped++;}

    public void incrementKeysPressedSinceLastMistake()
    { 
        this.keysPressedSinceLastMistake++;
        if(doesPlayerDeserveBonusIncrease())
        {
            increasePlayerBonus();
            if(this.errorsTillNextPunishment<this.errorsBetweenPunishments)
                this.errorsTillNextPunishment++;
        }
    }
    public void incrementTotalErrorsMade(string mistakenKey)
    {
        
        this.totalErrors++;
        this.mistakenKeysList.Add(mistakenKey);
        reduceKeysTillPunishment();
        resetKeysPressedSinceLastMistake();
    }


    public int getTotalErrors()
    { return this.totalErrors;}

    public Dictionary<string,int> getMostMistakenKeyAndCount()
    {
        List<string> listOfUniqueMistakes = new List<string>();
        Dictionary<string, int> keysAndMistakes = new Dictionary<string, int>();
        
        string mostMistakenKey=null;
        foreach(string s in this.mistakenKeysList)
        {
            if(!listOfUniqueMistakes.Contains(s))
                listOfUniqueMistakes.Add(s);
        }
        
        foreach(string s in listOfUniqueMistakes)
        {
            int numberOfOccurences = 0;
            for(int i=0; i<this.mistakenKeysList.Count;i++)
                if(s.Equals(this.mistakenKeysList[i]))
                    numberOfOccurences++;
            keysAndMistakes.Add(s,numberOfOccurences);   
            deleteKeyFromMistakenList(s);    
        }

        return keysAndMistakes;
    }

    private void deleteKeyFromMistakenList(string key)
    {
        while(this.mistakenKeysList.Remove(key));
    }
}
