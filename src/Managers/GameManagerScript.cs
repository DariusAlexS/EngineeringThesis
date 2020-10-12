using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System;
using TMPro;
public class GameManagerScript : MonoBehaviour
{
    private class StringComparer : IComparer<string>
    {
        public int Compare(string first, string second)
        { return first.CompareTo(second);}
    }
    

    public wordManager wordManagerReference;
    public InputFieldManager inputField;
    public GameObject actionPanel;
    public GameObject selectedActionInfoPanel;
    public GameObject upgradeStructPanel;
    public Text selectedActionInfoTextRef;
    public TextMeshProUGUI produceUnitsTextRef;
    public TextMeshProUGUI produceGoldTextRef;
    public TextMeshProUGUI upgradeStructureTextRef;
    public TextMeshProUGUI howManyProducedUnitsInfoTextRef;
    public TextMeshProUGUI howMuchProducedGoldInfoTextRef;
    public TextMeshProUGUI upgradeCostRef;
    public ToastManager toastRef;
    private Structure currentlySelectedStructure;
    private Structure destinationSendToStructure;
    private int inputPhase; //1 - struct select, 2 - action select, 3 - action exec, (4 - soldiers send)
    private SortedDictionary<string,Structure> selectionStringDictionary;
    private SortedDictionary<string,Structure> moveToStringDictionary;
    public ActionType selectedAction;
    private Structure[] structuresInGame;
    void Start()
    {
        GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
        foreach(GameObject gObj in obstacles)
            gObj.GetComponent<Collider2D>().isTrigger = true;
        this.selectedAction = ActionType.None;
        this.currentlySelectedStructure = null;
        this.destinationSendToStructure = null;
        this.inputField = GameObject.Find("InputField").GetComponent<InputFieldManager>();
        this.toastRef = GameObject.Find("ToastManager").GetComponent<ToastManager>();
        setInputPhaseToAnfang();
        this.structuresInGame = initializeArrayOfStructures();
        initializeStringStructureDictionaries(this.structuresInGame);
        updateDisplayTextToWrite(this.inputPhase);
        Debug.Log("Input phase: "+ this.inputPhase);
        inputField.getCurrentPhaseWords(collectCurrentPhaseWords(getCurrentTypingPhase()),getCurrentTypingPhase());
    }
   
    void Update()
    {
        if(getCurrentTypingPhase() == 4)
            this.inputField.setTextToType("Dostępni żołnierze: "+this.currentlySelectedStructure.stationingUnits);
    }

    public void updateDisplayTextToWrite(int phase)
    {
        this.selectedActionInfoPanel.SetActive(false);
        this.actionPanel.SetActive(false);
        foreach(Structure structure in this.structuresInGame)
            {structure.setDisplayTextToWrite(phase);}
        if(phase == 2 && this.currentlySelectedStructure!=null)
        {
            this.currentlySelectedStructure.setTextToWriteEmpty();
            this.actionPanel.SetActive(true); 
            if(this.currentlySelectedStructure.isMaxLevelUpgrade())
                upgradeStructPanel.SetActive(false);
            else   
                upgradeStructPanel.SetActive(true);
            this.howMuchProducedGoldInfoTextRef.text =  this.currentlySelectedStructure.calculateActiveGoldProduction().ToString() + " sztuk złota";
            this.howManyProducedUnitsInfoTextRef.text = this.currentlySelectedStructure.calculateActiveUnitProduction().ToString() + " żołnierzy";
        }
        else if(phase == 3 && this.selectedAction!=ActionType.None)
        {
            this.selectedActionInfoPanel.SetActive(true);
            switch(this.selectedAction)
            {
                case ActionType.MoveToCommand: this.selectedActionInfoTextRef.text = "Wysyłanie żołnierzy"; break;
                case ActionType.ProduceGold: this.selectedActionInfoTextRef.text = "Produkcja złota"; break;
                case ActionType.ProduceSoldiers: this.selectedActionInfoTextRef.text = "Produkcja żołnierzy"; break;
                case ActionType.UpgradeStructure: this.selectedActionInfoTextRef.text = "Ulepszanie struktury"; break;
            }
        }
        
    }
    
    // initializers
    private Structure[] initializeArrayOfStructures()
    {
        Structure[] structureArray;
        GameObject[] gameObjectStructures = GameObject.FindGameObjectsWithTag("Structure");

        structureArray = new Structure[gameObjectStructures.Length];

        for(int i=0; i<gameObjectStructures.Length;i++)
            structureArray[i] = gameObjectStructures[i].GetComponent<Structure>();

        return structureArray;
    }

    private void initializeStringStructureDictionaries(Structure[] structures)
    {
        StringComparer stringComparer = new StringComparer();
        this.selectionStringDictionary = new SortedDictionary<string, Structure>(stringComparer);
        this.moveToStringDictionary = new SortedDictionary<string, Structure>(stringComparer);

        foreach(Structure structure in structures)
        {
            structure.selectionString = this.wordManagerReference.getStructSelectionString();
            structure.moveToStructString = this.wordManagerReference.getActionSelectionString();
            selectionStringDictionary.Add(structure.selectionString, structure);
            moveToStringDictionary.Add(structure.moveToStructString, structure);
        }
    }

    private void initializeActionSelectionText()
    {
        this.produceGoldTextRef.text = this.wordManagerReference.getActionSelectionString();
        this.produceUnitsTextRef.text = this.wordManagerReference.getActionSelectionString();
        this.upgradeStructureTextRef.text = this.wordManagerReference.getActionSelectionString();
    }
    
    public int getCurrentTypingPhase()
    { return this.inputPhase; }

    // handle escape key presses
    public void moveToPreviousPhase()
    {
        
        this.inputField.clearInputField();
            
        if(this.inputPhase==2)
            this.currentlySelectedStructure = null;
        else if(this.inputPhase==3)
        {
            destinationSendToStructure = null;
            this.selectedAction = ActionType.None;
        }
        else if(this.inputPhase==4)
        {
            this.inputField.enterUnitAmountMode(false);
        }
        previousInputPhase();
        updateDisplayTextToWrite(getCurrentTypingPhase());
        inputField.getCurrentPhaseWords(collectCurrentPhaseWords(getCurrentTypingPhase()),getCurrentTypingPhase());
        
    }

    // look up dictionary, delete entry, set sourcestructure, set actiontype, order sendTo command
    // update missing entries in dictionary and strings in TextFields/structures
    
    public void processInputFieldText(string inputText)
    {
        switch(this.inputPhase)
        {
            case 1: processStructSelection(inputText); break;
            case 2: processActionSelection(inputText); break;
            case 3: processActionExecution(inputText); break;
            case 4: issueSendToOrderForSourceStructure(inputText); break;
        }
    }
   
    public void updateInputAfterConquering(Structure attackedStructure)
    {
        Debug.Log("Input pusty?: " + isInputEmpty());
        if(isInputEmpty() && isSelectionPhase() )
            inputField.getCurrentPhaseWords(collectCurrentPhaseWords(getCurrentTypingPhase()), getCurrentTypingPhase());

        else if(this.currentlySelectedStructure == attackedStructure)
        {
            resetSelectedStructures();
            setInputPhaseToAnfang();
            this.inputField.clearInputField();
            this.inputField.getCurrentPhaseWords(collectCurrentPhaseWords(getCurrentTypingPhase()), getCurrentTypingPhase());
        }
    }
 
    private bool processIfWordInDictionary(string key, SortedDictionary<string, Structure> dictionary)
    {
        Structure structure=null;
        try
        {  structure = dictionary[key]; }
        catch(KeyNotFoundException ex)
        {
            Debug.Log("Struktury nie ma w słowniku.");
            return false;
        }

        if(structure == null)
            Debug.Log("Blad! Nie istnieje struktura w słowniku, nie miało to prawa zajść!");
        else
        {
            string newlyAddedString; 
            dictionary.Remove(key);
            if(dictionary == this.selectionStringDictionary)
            {
                do{
                    newlyAddedString=getSelectionString();
                }while(dictionary.Keys.Contains(newlyAddedString));

                structure.selectionString = newlyAddedString;
                dictionary.Add(newlyAddedString, structure);
                this.currentlySelectedStructure = structure;
            }
            else if(dictionary == this.moveToStringDictionary)
            {
                
                newlyAddedString=getUnusedActionSelectionString();
                
                this.selectedAction = ActionType.MoveToCommand;
                structure.moveToStructString = newlyAddedString;
                dictionary.Add(structure.moveToStructString, structure);
                this.destinationSendToStructure = structure;
            }
        }
        if(structure != null)
            return true;
        else return false;
    }
    private bool processNonMoveToAction(string text)
    {
        bool processed = false;
        if(this.produceGoldTextRef.text.Equals(text))
        {
            this.selectedAction = ActionType.ProduceGold;
            processed = true;
        }
        else if(!processed && this.produceUnitsTextRef.text.Equals(text))
        {
            this.selectedAction = ActionType.ProduceSoldiers;
            processed=true;
        }
        else if(!processed && this.upgradeStructureTextRef.text.Equals(text))
        {
            if(checkIfCanUpgradeStructure(this.currentlySelectedStructure))
            {
                this.selectedAction = ActionType.UpgradeStructure;
                processed=true;
            }
            else
            {
                toastRef.displayMessage("Brak złota na ulepszenie",false);
            }
        }
        return processed;
          
    }
    public void processStructSelection(string inputText)
    {
        if(processIfWordInDictionary(inputText, this.selectionStringDictionary))
            {
                nextInputPhase();
                updateActionTexts();
                updateDisplayTextToWrite(getCurrentTypingPhase());
                this.inputField.getCurrentPhaseWords(collectCurrentPhaseWords(getCurrentTypingPhase()), getCurrentTypingPhase());
            }
    }
    public void processActionSelection(string inputText)
    {
        if(processIfWordInDictionary(inputText, this.moveToStringDictionary) || processNonMoveToAction(inputText))
            {
                nextInputPhase();
                updateActionTexts();
                this.inputField.getCurrentPhaseWords(collectCurrentPhaseWords(getCurrentTypingPhase()),getCurrentTypingPhase());
                updateDisplayTextToWrite(getCurrentTypingPhase());
            }
        
    }
    public void processActionExecution(string inputText)
    {
        if(this.selectedAction != ActionType.MoveToCommand && this.selectedAction != ActionType.None)
        {
            this.currentlySelectedStructure.executeAction(this.selectedAction);
            setInputPhaseToAnfang();
            this.inputField.getCurrentPhaseWords(collectCurrentPhaseWords(getCurrentTypingPhase()), getCurrentTypingPhase());
            updateDisplayTextToWrite(getCurrentTypingPhase());
            this.inputField.clearInputField();
            this.inputField.setTextToType("");
            Debug.Log("WYKONAŁEM AKCJĘ!");
        }
        else
        {
            this.inputField.enterUnitAmountMode(true);
            this.inputField.setTextToType("Wprowadź liczbę żołnierzy, których chcesz wysłać. Maksymalnie można wysłać "+this.currentlySelectedStructure.stationingUnits+ " jednostek.");
            nextInputPhase();
        }   
        
    }
    public void issueSendToOrderForSourceStructure(string inputText)
    {
        int numberOfSoldiers = Int32.Parse(inputText);
        if(numberOfSoldiers<0)
            this.toastRef.displayMessage("Przynajmniej 1 żołnierz musi zostać wysłany",true);
        else if(numberOfSoldiers>this.currentlySelectedStructure.stationingUnits)
            this.toastRef.displayMessage("Nie możesz wysłać więcej, niż jest w strukturze!", false);
        else
        {
            this.currentlySelectedStructure.issueSendToOrder(this.destinationSendToStructure, numberOfSoldiers); 
            this.inputField.enterUnitAmountMode(false);
            setInputPhaseToAnfang();
            this.inputField.clearInputField();
            this.inputField.setTextToType("Wybierz strukturę");
            this.inputField.getCurrentPhaseWords(collectCurrentPhaseWords(getCurrentTypingPhase()), getCurrentTypingPhase());
            updateDisplayTextToWrite(getCurrentTypingPhase());
        }
    }

   
    //get arrays of strings depending on phase for the InputFieldManager to check
    private string[] collectCurrentPhaseWords(int currentInputPhase)
    {
        string[] arrayOfWords = null;
        switch(currentInputPhase)
        {
            case 1: arrayOfWords = getSortedArrayOfStringsToPlayerOwnedStructures();
            break;
            case 2: arrayOfWords = getSortedArrayOfActionSelectionStrings();
            break;
            case 3: arrayOfWords = new string[1]{this.wordManagerReference.getActionExecutionString()};
            break;
        }

        return arrayOfWords;
    }
    private string[] getSortedArrayOfStringsToPlayerOwnedStructures()
    {
        List<string> sortedListOfPlayerOwnedStructuresSelectionStrings = new List<string>();
            foreach(string str in this.selectionStringDictionary.Keys)
                if(this.selectionStringDictionary[str].owner.playerType.Equals(PlayerType.Human))
                    sortedListOfPlayerOwnedStructuresSelectionStrings.Add(str);

        sortedListOfPlayerOwnedStructuresSelectionStrings.Sort();
        return sortedListOfPlayerOwnedStructuresSelectionStrings.ToArray();  
    }
    private string[] getSortedArrayOfActionSelectionStrings()
    {
        List<string> sortedListOfActionSelectionStrings = new List<string>();
            foreach(string str in this.moveToStringDictionary.Keys)
                if(this.moveToStringDictionary[str]!=this.currentlySelectedStructure)
                    sortedListOfActionSelectionStrings.Add(str);
        sortedListOfActionSelectionStrings.Add(this.produceGoldTextRef.text);
        sortedListOfActionSelectionStrings.Add(this.produceUnitsTextRef.text);
        if(!this.currentlySelectedStructure.isMaxLevelUpgrade())
            sortedListOfActionSelectionStrings.Add(this.upgradeStructureTextRef.text);

        sortedListOfActionSelectionStrings.Sort();

        return sortedListOfActionSelectionStrings.ToArray();  
    }
   

    //wordManager get strings methods
    private string getActionExecutionString()
    { return this.wordManagerReference.getActionExecutionString();}
    private string getActionSelectionString()
    { return this.wordManagerReference.getActionSelectionString();}

    private string getUnusedActionSelectionString()
    {
        string newString;
        do
        {
            newString = getActionSelectionString();
        } while(this.moveToStringDictionary.Keys.Contains(newString) || doActionSelectionTextsContainString(newString));
        return newString;
    }

    private bool  doActionSelectionTextsContainString(string newString)
    {
        if(this.produceGoldTextRef.text.Equals(newString))
            return true;
        else if(this.produceUnitsTextRef.text.Equals(newString))
            return true;
        else if(this.upgradeStructureTextRef.text.Equals(newString))
            return true;
        else return false;
    }
    private string getSelectionString()
    { return this.wordManagerReference.getStructSelectionString();}

    private void setInputPhaseToAnfang()
    {   
        resetActionSelectionPhaseVariables();
        resetSelectedStructures();
        this.inputPhase = 1; 
    }
    private void previousInputPhase()
    {
        switch(this.inputPhase)
        {
            case 1: break;
            case 2: this.inputPhase = 1; break;
            case 3: this.inputPhase = 2; break;
            case 4: this.inputPhase = 2; break;
        }
    }
    private void nextInputPhase()
    {
        if(this.inputPhase<5) // && actionType != SendToOrder
        {
            this.inputPhase++;
            this.inputField.clearInputField();
        }
        else Debug.Log("Próba wejścia w fazę 5");
            
    }

    private bool isSelectionPhase()
    { return this.inputPhase ==1;}
    
    private void resetSelectedStructures()
    {
        this.currentlySelectedStructure = null;
        this.destinationSendToStructure = null;
    }

    public bool isInputEmpty()
    { return this.inputField.isInputEmpty();}

    public void updateActionTexts()
    {
        if(this.produceGoldTextRef.text != null)
        this.produceGoldTextRef.text = getUnusedActionSelectionString();
        else
        this.produceGoldTextRef.text = getActionSelectionString();
        if(this.produceUnitsTextRef.text != null)
        this.produceUnitsTextRef.text = getUnusedActionSelectionString();
        else
        this.produceUnitsTextRef.text = getActionSelectionString();
        
        if(!this.currentlySelectedStructure.isMaxLevelUpgrade())
        {
            this.upgradeStructureTextRef.text = getUnusedActionSelectionString();
            this.upgradeCostRef.text = this.currentlySelectedStructure.calculateUpgradeCost().ToString();
        }
        if(this.currentlySelectedStructure == null)
            Debug.Log("Nie ma prawa nie być wybrana struktura w tym miejscu");
    }

    public void resetSelectionPhaseVariables()
    {
        this.currentlySelectedStructure = null;
    }
    public void resetActionSelectionPhaseVariables()
    {
        this.selectedAction = ActionType.None;
        this.destinationSendToStructure = null;
    }

    private bool checkIfCanUpgradeStructure(Structure structure)
    {
        return structure.owner.totalGold > structure.calculateUpgradeCost();
    }

    public ActionType getCurrentlySelectedAction()
    { return this.selectedAction; }

    public Structure getCurrentlySelectedStructure()
    { return this.currentlySelectedStructure;}

    
}
