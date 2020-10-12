using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using System;
using UnityEngine.UI;
public class DictionaryToggleData : MonoBehaviour
{
    public IDbConnection dbConnection;
    public IDbCommand dbCommand;
    public IDataReader dbReader;
    private string connectionString; 
    public GameObject toggleHolder;
    public int minimalWordLength;
    public void preLoadGameData()
    {
        List<string> listOfDictionaries = GetToggleDictionaries();
        List<Word> listOfPreloadedWords = loadWordsFromSelectedDictionaries(listOfDictionaries);
        insertPreloadedListOfWordsIntoWordHolder(listOfPreloadedWords);
        this.gameObject.GetComponent<StartGame>().startGame();
    }
    private List<string> GetToggleDictionaries()
    {
        List<string> listOfSelectedDictionaries = new List<string>();
        Toggle[] toggleValues = toggleHolder.GetComponentsInChildren<Toggle>();
        foreach(Toggle t in toggleValues)
        {
            if(t.isOn)
            {
                listOfSelectedDictionaries.Add(t.GetComponent<ToggleValueHolder>().dictName);
            }
        }
        return listOfSelectedDictionaries;
    }

    private List<Word> loadWordsFromSelectedDictionaries(List<string> listOfSelectedDictionaries)
    {
        connectToDB();
        List<Word> listOfGameWords = new List<Word>();
        
        foreach(string dict in listOfSelectedDictionaries)
            listOfGameWords.AddRange(readAndInsertWordsIntoWordList(dict));

        closeConnectionToDB();
        return listOfGameWords;

    }
    private List<Word> readAndInsertWordsIntoWordList(string dictionary)
    {
        
        List<Word> wordsFromGivenDictionary = new List<Word>();
        string readQuery = "SELECT word, charCountInWord FROM Dictionaries "+
                            "JOIN WordsInDictionaries ON dictionaryID = dictionaryIDFK " +
                            "JOIN WordSequences ON wordIDFK = wordID "+
                            "WHERE dictionaryName = '"+dictionary+"' AND charCountInWord>"+ minimalWordLength+";";
        dbCommand.CommandText = readQuery;
        dbReader = dbCommand.ExecuteReader();
        while(dbReader.Read())
            wordsFromGivenDictionary.Add(new Word(dbReader.GetString(0), dbReader.GetInt32(1)));
        dbReader.Close();
        return wordsFromGivenDictionary;
    }

    private void connectToDB()
    {
        connectionString = "URI=file:" + Application.dataPath + "/Database/GameDatabase.db3";
        dbConnection = new SqliteConnection(connectionString);
        dbConnection.Open();
        dbCommand = dbConnection.CreateCommand();
    }

    private void closeConnectionToDB()
    {
        if(dbReader!=null && !dbReader.IsClosed)
            dbReader.Close();
        dbCommand.Dispose();
        dbConnection.Close();
    }

    private void insertPreloadedListOfWordsIntoWordHolder(List<Word> listOfWords)
    {
        GameObject.Find("WordHolder").GetComponent<WordContainer>().setListOfWordsForGame(listOfWords);

    }
}
