using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public class wordManager : MonoBehaviour
{
    private GameObject wordHolder;
    private LinkedList<Word> structSelectionSequences;
    private LinkedList<Word> actionSelectionSequences;
    private LinkedList<Word> actionExecutionSequences;
    public int shortUpperLimit;
    public int mediumUpperLimit;
    public int longUpperLimit;

    void Start()
    {
        initializeWordManager();
    }
    void Update()
    {
       
    }

    private void initializeWordManager()
    {
        initializeWordLists();
        wordHolder = GameObject.Find("WordHolder");
        List<Word> listOfAllWords = wordHolder.GetComponent<WordContainer>().getListOfWordsForGame();
   
        loadWordSequencesIntoLists(listOfAllWords);
        shuffleWordsInList<Word>(this.structSelectionSequences);
        shuffleWordsInList<Word>(this.actionSelectionSequences);
        shuffleWordsInList<Word>(this.actionExecutionSequences);


    }
    private void initializeWordLists()
    {
        this.structSelectionSequences = new LinkedList<Word>();
        this.actionSelectionSequences = new LinkedList<Word>();
        this.actionExecutionSequences = new LinkedList<Word>();
    }
    private void loadWordSequencesIntoLists(List<Word> listOfAllWords)
    {
        foreach(Word word in listOfAllWords)
        {
            if(word.stringLength<=shortUpperLimit)
            {
                this.structSelectionSequences.AddFirst(word);
            }
            else if(word.stringLength>shortUpperLimit && word.stringLength<=mediumUpperLimit
    )
            {
                this.actionSelectionSequences.AddFirst(word);
            }
            else if(word.stringLength>mediumUpperLimit
                && word.stringLength<=longUpperLimit
)
            {
                this.actionExecutionSequences.AddFirst(word);
            }
        }
    }

    public void shuffleWordsInList<T>(LinkedList<T> listOfWords)
    {   
        int elementsOnList = listOfWords.Count;
        System.Random rand = new System.Random(System.DateTime.Now.Millisecond);
        for(int i=0;i<elementsOnList;i++)
            repositionElements(rand.Next(0,elementsOnList/2), rand.Next(elementsOnList/2+1,elementsOnList), listOfWords);

    }
    public  void repositionElements<T>(int firstElemIndex, int secondElemIndex, LinkedList<T> list)
    {
        T val1 = list.ElementAt(firstElemIndex);
        T val2 = list.ElementAt(secondElemIndex);

        list.Remove(val1);
        list.Remove(val2);
        list.AddFirst(val1);     
        list.AddFirst(val2);   
    }

    public string getStructSelectionString()
    {return extractStringAndMoveWordToBackOfList(this.structSelectionSequences);}

    public string getActionSelectionString()
    { return extractStringAndMoveWordToBackOfList(this.actionSelectionSequences);}

    public string getActionExecutionString()
    { return extractStringAndMoveWordToBackOfList(this.actionExecutionSequences);}


    private string extractStringAndMoveWordToBackOfList(LinkedList<Word> list)
    {
        Word temp = list.FirstOrDefault();
        list.RemoveFirst();
        list.AddLast(temp);
        return temp.typingString;
    }

}
