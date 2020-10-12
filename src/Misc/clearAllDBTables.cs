using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mono.Data.Sqlite;
using System.Data;
using System;
public class clearAllDBTables : MonoBehaviour
{
    private IDbConnection dbConnection;
    private IDbCommand dbCommand;
    private IDataReader dbReader;
    private string connectionString;
    public Text textMsgRef;
    private List<Int32> idsOfDictionariesToDelete;

    public void wykasujWszystkoZBazy()
    { 
        connectToDB();
        dbCommand.CommandText = "DELETE FROM WordSequences;";
        dbCommand.ExecuteReader().Close();
        dbCommand.ExecuteReader().Close();
        dbCommand.CommandText = "DELETE FROM Dictionaries;";
        dbCommand.ExecuteReader().Close();
        Debug.Log("Usunalem");
        closeConnectionToDB();
        textMsgRef.text = "Usunięto!";
        textMsgRef.color = new Color(0f,0f,0f,1f);
    }

    void Update()
    {
        if(textMsgRef.color.a > 0f)
            textMsgRef.color = new Color(0f,0f,0f,textMsgRef.color.a-0.01f);
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


}
