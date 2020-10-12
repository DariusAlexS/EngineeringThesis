using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR   
using UnityEditor;
#endif
#if UNITY_STANDALONE_WIN
using System.Runtime.InteropServices;
using System.IO;
using System.Windows.Forms;
#endif
public class DictionaryPathLoader : MonoBehaviour
{
    public Text textField;
    #if UNITY_STANDALONE_WIN
        [DllImport("user32.dll")]
        private static extern void OpenFileDialog();
    #endif
    public void selectPathToDictionary()
    {   
        #if UNITY_EDITOR 
        textField.text = UnityEditor.EditorUtility.OpenFilePanel("Wybierz plik ze slownikiem","","");
        #endif
        
        #if UNITY_STANDALONE_WIN
        OpenFileDialog ofd = new OpenFileDialog();
        
        if(ofd.ShowDialog() == DialogResult.OK)
            textField.text = ofd.FileName;
         #endif
       
    }   

    
}
