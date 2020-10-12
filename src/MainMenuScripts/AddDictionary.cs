using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mono.Data.Sqlite;
using System.Data;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

public class AddDictionary : MonoBehaviour
{
    public Text filePath;
    public Text dictionaryNameText;
    public Text itMakeTakeAWhile;
    
    private IDbConnection dbConnection;
    private IDbCommand dbCommand;
    private IDataReader dbReader;
    private string connectionString;
    public void addDictionaryToDatabase()
    {
        connectionString = "URI=file:" + Application.dataPath + "/Database/GameDatabase.db3";
        dbConnection = new SqliteConnection(connectionString);
        dbConnection.Open();
        dbCommand = dbConnection.CreateCommand();
        
        if(!checkIfDictionaryExists(dbCommand, this.dictionaryNameText.text))
        {
            int insertedDictionaryID, maxWordID;
            insertedDictionaryID = createDictionaryEntryInDatabase(dbCommand, this.dictionaryNameText.text);
            wordTableNotEmptyCheck();
            this.dbReader.Close();
            maxWordID = getMaxWordID(dbCommand);

            string[] listOfWords = readAndSplittFileByDelimiter(this.filePath.text);
            string insertWordsQuery = buildInsertWordsQuery(listOfWords);
            string insertWordDictRefQuery = buildInsertWordDictRefQuery(insertedDictionaryID, maxWordID, listOfWords.Length);
            
            itMakeTakeAWhile.text = "Dodaje slownik. Moze to zajac do kilku minut.";
            dbCommand.CommandText = insertWordsQuery;
            dbCommand.ExecuteReader().Close();
            dbCommand.CommandText = insertWordDictRefQuery;
            dbCommand.ExecuteReader().Close();

            itMakeTakeAWhile.text = "Dodano slownik.";
            //new WaitForSeconds(3);
            //itMakeTakeAWhile.text = "";
        }
        else
            itMakeTakeAWhile.text = "Podana nazwa slownika juz istnieje!";
        
        dbCommand.Dispose();  
        dbConnection.Close();

        
        

    }

    private string[] readAndSplittFileByDelimiter(string pathToDictionary)
    {
        string[] words = Regex.Split(File.ReadAllText(pathToDictionary),";/");
        return words;
    }
    
    private int createDictionaryEntryInDatabase(IDbCommand command, string dictName)
    {
        
        command.CommandText = "INSERT INTO Dictionaries (dictionaryName) VALUES ( '"+dictName+"');";
        
        try{
              command.ExecuteReader().Close();
              
        }
        catch(SqliteException ex)
        {
            this.dictionaryNameText.text = "Slownik o danej nazwie juz istnieje";
        }
        command.CommandText = "SELECT dictionaryID FROM Dictionaries WHERE dictionaryName = '"+dictName+"';";
        dbReader = command.ExecuteReader();
        int dictID=0;
        if(dbReader.Read())
            dictID = dbReader.GetInt32(0);
        dbReader.Close();
        

        return dictID;
   }

    private int getMaxWordID(IDbCommand command)
    {
        int maxWordID=0;
        command.CommandText =  "SELECT wordID FROM WordSequences WHERE wordID ="
                            +   "(SELECT MAX(wordID) FROM WordSequences)";
        dbReader = command.ExecuteReader();
        if(dbReader.Read())
            maxWordID = dbReader.GetInt32(0);
        this.dbReader.Close();
        
        return maxWordID;
    }

    private string buildInsertWordsQuery(string[] wordArray)
    {
        StringBuilder queryBuilder = new StringBuilder();
        queryBuilder.Append("INSERT INTO WordSequences (word) VALUES ");
        foreach(string word in wordArray)
            {queryBuilder.Append("('"+word+"'), ");}
        queryBuilder.Remove(queryBuilder.Length-2,2);
        queryBuilder.Append(";");
        
        return queryBuilder.ToString();
    }

    private string buildInsertWordDictRefQuery(int dictionaryID, int startingWordID, int insertedWords)
    {
        StringBuilder queryBuilder = new StringBuilder();
        queryBuilder.Append("INSERT INTO WordsInDictionaries (dictionaryIDFK, wordIDFK) VALUES ");
        for(int i=startingWordID + 1; i< (startingWordID + insertedWords); i++)
           { queryBuilder.Append("("+dictionaryID+","+ i +"), ");}

        queryBuilder.Remove(queryBuilder.Length-2,2);
        queryBuilder.Append(";");
        
        return queryBuilder.ToString();
    }

    private bool checkIfDictionaryExists(IDbCommand command, string dictName)
    {
        bool exists = false;
        command.CommandText = "SELECT COUNT(dictionaryName) FROM Dictionaries WHERE dictionaryName ='"+dictName+"';";
        this.dbReader = command.ExecuteReader();
        if(dbReader.Read())
            if(dbReader.GetInt32(0)>0)
               exists = true; 
            else exists=false;
        this.dbReader.Close();
        //Debug.Log("slownik istnieje?" + exists);
        return exists;
    }

    private void wordTableNotEmptyCheck()
    {
        dbCommand.CommandText = "SELECT COUNT(*) FROM WordSequences;";
        dbReader = dbCommand.ExecuteReader();
        if(dbReader.Read())
            if(dbReader.GetInt32(0)==0)
                {
                    dbReader.Close();
                    dbCommand.CommandText = "INSERT INTO WordSequences (word) VALUES ('initializer');";
                    dbCommand.ExecuteReader().Close();
                }
    }
}
