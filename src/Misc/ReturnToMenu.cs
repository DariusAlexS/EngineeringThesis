using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ReturnToMenu : MonoBehaviour
{

    public void returnToMenu()
    {
        GameObject wordHolder = GameObject.Find("WordHolder");
        if(wordHolder!=null)
        {
            Destroy(wordHolder);
            SceneManager.LoadScene("MenuScreen"); 
        }
            
    }
}
