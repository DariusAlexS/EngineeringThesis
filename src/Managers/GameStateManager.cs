using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class GameStateManager : MonoBehaviour
{

    
    public MeasurementManager measurementManagerRef;


    public AIPlayer player2Ref;
    public AIPlayer player3Ref;
    public AIPlayer player4Ref;

    // finish screen ref

    public GameObject finishScreenRef;

    public TextMeshProUGUI resultRef;
    public TextMeshProUGUI gameDurationRef;
    public TextMeshProUGUI endWPMRef;
    public TextMeshProUGUI precisionRef;
    public TextMeshProUGUI mistakesCountRef;
    public TextMeshProUGUI mostMistakenKeyRef;
    public TextMeshProUGUI secondMistakenKeyRef; 
    public TextMeshProUGUI thirdMistakenKeyRef;


    
    public void humanPlayerLost()
    {
        hideGameObjects();
        Dictionary<string,int> keysAndMistakes = measurementManagerRef.getMostMistakenKeyAndCount();

        this.resultRef.text = "Przegrana...";
        this.gameDurationRef.text = "Czas gry - " + getGameDuration();
        this.endWPMRef.text = "WPM: " + measurementManagerRef.calculateWPMStringValue();
        this.precisionRef.text = measurementManagerRef.precisionTextRef.text;
        this.mistakesCountRef.text = "Bledy: " + measurementManagerRef.getTotalErrors();
        this.mostMistakenKeyRef.text = "Najczesciej mylony klawisz: " + getMostlyMistakenKeyAndString(keysAndMistakes);
        this.secondMistakenKeyRef.text = "Drugi najczesciej mylony: " +  getMostlyMistakenKeyAndString(keysAndMistakes);
        this.thirdMistakenKeyRef.text = "Trzeci najczesciej mylony: " + getMostlyMistakenKeyAndString(keysAndMistakes);
        this.finishScreenRef.SetActive(true);
    }

    public void AIPLayerHasLost(AIPlayer lostPlayer)
    {
        if(player2Ref == lostPlayer)
            player2Ref = null;
        else if(player3Ref == lostPlayer)
            player3Ref = null;
        else if(player4Ref == lostPlayer)
            player4Ref = null;
        
        if(player2Ref==null && player3Ref == null && player4Ref == null)
            humanPlayerWon();

    }
    public string getMostlyMistakenKeyAndString(Dictionary<string,int> dict)
    {
        string mostMistakenString="";
        string endString="";
        int highestNumberOfMistakes = 0;
        foreach(string s in dict.Keys)
        {
            if(dict[s]>highestNumberOfMistakes)
            {
                highestNumberOfMistakes = dict[s];
                mostMistakenString = s;
            }
        }
        dict.Remove(mostMistakenString);

        if(highestNumberOfMistakes>0)
        endString= mostMistakenString +" - "+highestNumberOfMistakes;
        

        return endString;
    }

    public void humanPlayerWon()
    {
        hideGameObjects();
        Dictionary<string,int> keysAndMistakes = measurementManagerRef.getMostMistakenKeyAndCount();

        this.resultRef.text = "Zwyciestwo!";
        this.gameDurationRef.text = "Czas gry - " + getGameDuration();
        this.endWPMRef.text = "WPM: " + measurementManagerRef.calculateWPMStringValue();
        this.precisionRef.text = measurementManagerRef.precisionTextRef.text;
        this.mistakesCountRef.text = "Bledy: " + measurementManagerRef.getTotalErrors();
        this.mostMistakenKeyRef.text = "Najczesciej mylony klawisz: " + getMostlyMistakenKeyAndString(keysAndMistakes);
        this.secondMistakenKeyRef.text = "Drugi najczesciej mylony: " +  getMostlyMistakenKeyAndString(keysAndMistakes);
        this.thirdMistakenKeyRef.text = "Trzeci najczesciej mylony: " + getMostlyMistakenKeyAndString(keysAndMistakes);
        this.finishScreenRef.SetActive(true);
    }

    public string getGameDuration()
    {
        return GameObject.Find("TimeTracker").GetComponent<TimeManager>().getTimeString();
    }

    public void hideGameObjects()
    {
        
        Destroy(GameObject.Find("TextToWriteHolder"));
        Destroy(GameObject.Find("MinimapBorder"));
        Destroy(GameObject.Find("Players"));
        GameObject.Find("WorldMapObjects").SetActive(false);
        this.finishScreenRef.SetActive(true);
    }

}
