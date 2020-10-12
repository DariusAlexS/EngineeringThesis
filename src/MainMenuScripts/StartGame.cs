using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    public GameObject wordHolderGameObject;
    
    public void startGame()
    {
        if(wordHolderGameObject.GetComponent<WordContainer>().getListOfWordsForGame().Count>0)
        {
            DontDestroyOnLoad(wordHolderGameObject);
            Debug.Log("Liczba slow w liscie:"+wordHolderGameObject.GetComponent<WordContainer>().getListOfWordsForGame().Count);
            SceneManager.LoadScene("GameMap"); 
        }
        

    }


}
