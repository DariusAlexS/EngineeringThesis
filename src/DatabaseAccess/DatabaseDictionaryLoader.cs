using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Data;
using Mono.Data.Sqlite;


public class DatabaseDictionaryLoader : MonoBehaviour
{
    private string connectionString;
    
    void Start()
    {
        connectionString = "URI=file:" + Application.dataPath + "/Database/GameDatabase.db3";
        using(IDbConnection dbConnection = new SqliteConnection(connectionString))
        {
            dbConnection.Open();
            using(IDbCommand dbCommand = dbConnection.CreateCommand())
            {
                string sqlquery = "SELECT word FROM WordSequences";
                dbCommand.CommandText = sqlquery;
                using(IDataReader reader = dbCommand.ExecuteReader())
                {
                    while(reader.Read())
                    {Debug.Log(reader.GetString(0));}
                     reader.Close();
                     dbConnection.Close();   
                }
               
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //public LinkedList<WordHolder>[] 
}
