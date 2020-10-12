using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using System;
using UnityEngine.UI;
public class DictionaryToggleLoader : MonoBehaviour
{
    public GameObject ToggleHolder;
    public GameObject TogglePrefab;
    public IDbConnection dbConnection;
    public IDbCommand dbCommand;
    public IDataReader dbReader;
    private string connectionString; 
    
    public List<GameObject> listOfToggles;
    void OnEnable()
    {   
        
        clearToggles(listOfToggles);
        connectToDB();
        createToggles();
        closeConnectionToDB();
    }
    private void createToggles()
    {
        dbCommand.CommandText = "SELECT dictionaryName, COUNT(wordIDFK) " +
                            "FROM Dictionaries dict " +
                "JOIN WordsInDictionaries used ON dict.dictionaryID = used.dictionaryIDFK " +
                "GROUP BY dictionaryName;";
        dbReader = dbCommand.ExecuteReader();
        while(dbReader.Read())
        {
            addToggle(dbReader.GetString(0), dbReader.GetInt32(1));
        }
    }
    private void connectToDB()
    {
        
        connectionString = "URI=file:" + Application.dataPath + "/Database/GameDatabase.db3";
        dbConnection = new SqliteConnection(connectionString);
        dbConnection.Open();
        dbCommand = dbConnection.CreateCommand();
    }
    private void addToggle(string dictName, int wordCount)
    {
        GameObject newToggle = Instantiate(this.TogglePrefab);
        this.listOfToggles.Add(newToggle);
        newToggle.transform.SetParent(this.ToggleHolder.transform);
        newToggle.transform.localScale = new Vector3(1f,1f,1f);
        newToggle.GetComponentInChildren<Text>().text = dictName + " ("+wordCount+")";
        newToggle.GetComponent<ToggleValueHolder>().dictName = dictName;
    }
    private void closeConnectionToDB()
    {
        if(dbReader!=null && !dbReader.IsClosed)
            dbReader.Close();
        dbCommand.Dispose();
        dbConnection.Close();
    }

    private void clearToggles(List<GameObject> toggleList)
    {
        foreach(GameObject gObj in toggleList)
            Destroy(gObj);
    }


    
}
