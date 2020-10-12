using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordContainer : MonoBehaviour
{
    private List<Word> listOfWordsForGame;
    void Start()
    {
        listOfWordsForGame = new List<Word>();
    }

    public void setListOfWordsForGame(List<Word> words)
    {
        this.listOfWordsForGame = words;
    }

    public List<Word> getListOfWordsForGame()
    {
        return this.listOfWordsForGame;
    }
}
