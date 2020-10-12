using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;
using TMPro;

public class InputFieldManager : MonoBehaviour
{

    private Dictionary<KeyCode, char> lowerCaseKeys = new Dictionary<KeyCode, char>()
    {
        [KeyCode.E] ='e', 
        [KeyCode.T] ='t',
        [KeyCode.A]='a',
        [KeyCode.O]='o',
        [KeyCode.I]='i',
        [KeyCode.N]='n',
        [KeyCode.S]='s',
        [KeyCode.R]='r',
        [KeyCode.H]='h',
        [KeyCode.D]='d',
        [KeyCode.L]='l',
        [KeyCode.U]='u',
        [KeyCode.C]='c',
        [KeyCode.M]='m',
        [KeyCode.F]='f',
        [KeyCode.Y]='y',
        [KeyCode.W]='w',
        [KeyCode.G]='g',
        [KeyCode.P]='p',
        [KeyCode.B]='b',
        [KeyCode.V]='v',
        [KeyCode.K]='k',
        [KeyCode.X]='x',
        [KeyCode.Q]='q',
        [KeyCode.J]='j',
        [KeyCode.Z]='z',
        [KeyCode.Alpha0]='0',
        [KeyCode.Alpha1]='1',
        [KeyCode.Alpha2]='2',
        [KeyCode.Alpha3]='3',
        [KeyCode.Alpha4]='4',
        [KeyCode.Alpha5]='5',
        [KeyCode.Alpha6]='6',
        [KeyCode.Alpha7]='7',
        [KeyCode.Alpha8]='8',
        [KeyCode.Alpha9]='9',
        [KeyCode.Minus] = '-',
        [KeyCode.Equals] = '=',
        [KeyCode.LeftBracket] = '[',
        [KeyCode.RightBracket] = ']',
        [KeyCode.Semicolon] = ';',
        [KeyCode.Quote] = '\'',
        [KeyCode.Backslash] = '\\',
        [KeyCode.Comma]=',',
        [KeyCode.Period]='.',
        [KeyCode.Slash]='/'
        };

    private Dictionary<KeyCode, char> upperCaseKeys = new Dictionary<KeyCode, char>()
    {
        
        [KeyCode.E] ='E', 
        [KeyCode.T] ='T',
        [KeyCode.A]='A',
        [KeyCode.O]='O',
        [KeyCode.I]='I',
        [KeyCode.N]='N',
        [KeyCode.S]='S',
        [KeyCode.R]='R',
        [KeyCode.H]='H',
        [KeyCode.D]='D',
        [KeyCode.L]='L',
        [KeyCode.U]='U',
        [KeyCode.C]='C',
        [KeyCode.M]='M',
        [KeyCode.F]='N',
        [KeyCode.Y]='Y',
        [KeyCode.W]='W',
        [KeyCode.G]='G',
        [KeyCode.P]='P',
        [KeyCode.B]='B',
        [KeyCode.V]='V',
        [KeyCode.K]='K',
        [KeyCode.X]='X',
        [KeyCode.Q]='Q',
        [KeyCode.J]='J',
        [KeyCode.Z]='Z',
        [KeyCode.Alpha0]='!',
        [KeyCode.Alpha1]='@',
        [KeyCode.Alpha2]='#',
        [KeyCode.Alpha3]='$',
        [KeyCode.Alpha4]='%',
        [KeyCode.Alpha5]='^',
        [KeyCode.Alpha6]='&',
        [KeyCode.Alpha7]='*',
        [KeyCode.Alpha8]='(',
        [KeyCode.Alpha9]=')',
        [KeyCode.Minus] = '_',
        [KeyCode.Equals] = '+',
        [KeyCode.LeftBracket] = '{',
        [KeyCode.RightBracket] = '}',
        [KeyCode.Semicolon] = ':',
        [KeyCode.Quote] = '"',
        [KeyCode.Backslash] = '|',
        [KeyCode.Comma]='<',
        [KeyCode.Period]='>',
        [KeyCode.Slash]='?'
    };
    
    private KeyCode[] availableKeys = {
        KeyCode.E,KeyCode.T,KeyCode.A,KeyCode.O,KeyCode.I,KeyCode.N,KeyCode.S,KeyCode.R,KeyCode.H,KeyCode.D,
    KeyCode.L,KeyCode.U,KeyCode.C,KeyCode.M,KeyCode.F,KeyCode.Y,KeyCode.W,KeyCode.G,KeyCode.P,KeyCode.B,
    KeyCode.V,KeyCode.K,KeyCode.X,KeyCode.Q,KeyCode.J,KeyCode.Z,KeyCode.Alpha0,KeyCode.Alpha1,KeyCode.Alpha2,
    KeyCode.Alpha3,KeyCode.Alpha4,KeyCode.Alpha5,KeyCode.Alpha6,KeyCode.Alpha7,KeyCode.Alpha8,KeyCode.Alpha9,
    KeyCode.Minus,KeyCode.Equals,KeyCode.LeftBracket,KeyCode.RightBracket,KeyCode.Semicolon, KeyCode.Quote,
    KeyCode.Backslash,KeyCode.Comma, KeyCode.Period, KeyCode.Slash};
    
    // gameObject References
    public InputField inputF;
    public ToastManager infoToast;
    public MeasurementManager measurementManagerRef;
    public GameManagerScript gameManager;
    public HumanPlayer humanPlayer;
    public TextMeshProUGUI textToType;

    //performance related variables
    public int punishMentFrequency;
    private List<char> wronglyPressedKeys;

    // words processed in current phase
    private string[] currentPhaseWords;
    private int lowerAvailableWordsBounds;
    private int upperAvailableWordsBounds;
    private int longestWordLengthInCurrentPhaseWords;
    private bool enterUnitsMode;

    private bool isMeasuring;
    private double measureStartTime;
    private double lastPressedKeyTime;
    private double measureEndTime;
    private int keysTypedDuringMeasurmentTime;

    private DateTime startDateTime = new DateTime (1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
   
    
    void Start()
    {
        this.keysTypedDuringMeasurmentTime = 0;
        this.lastPressedKeyTime = 0;
        //this.keysPressedSinceLastMistake = 0;
        this.isMeasuring = false;
        this.wronglyPressedKeys = new List<char>(200);

        this.enterUnitsMode = false;
        this.lowerAvailableWordsBounds = 0;
        this.upperAvailableWordsBounds = 0;
        this.measurementManagerRef = getMeasurementManagerReference();
        this.gameManager = getGameManagerReference();
        this.infoToast = getToastManagerReference();
        inputF.ActivateInputField();

    }
    void Update()
    {
        
       if(Input.anyKeyDown)
       {
               if(Input.GetKeyDown(KeyCode.F1))
                {
                    if(GameObject.Find("WordHolder")!=null)
            
                    Destroy(GameObject.Find("WordHolder"));
                    UnityEngine.SceneManagement.SceneManager.LoadScene("MenuScreen");
                }
                else if(Input.GetKeyDown(KeyCode.Return))
                {
                    if(!this.enterUnitsMode)
                    {
                        endMeasuringTheTime();
                        if(checkIfEnteredTextMatchesOneOfAvailableSequences(this.inputF.text))
                        {
                            gameManager.processInputFieldText(this.inputF.text);
                        }
                        else resetKeysSinceLastMistake();
                    }
                    else gameManager.processInputFieldText(this.inputF.text);

                } 
                else if (!this.enterUnitsMode)
                {
                    if(Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Backspace))
                    {
                        this.measurementManagerRef.incrementBackspaceTimesPressed();
                        clearInputField();
                        recheckAvailableWordsBounds();
                        endMeasuringTheTime();
                    
                    }
                    else if(Input.GetKeyDown(KeyCode.Backspace))
                    {
                        increaseBackspaceTimesPressedIfInputFieldNotEmpty();
                        recheckAvailableWordsBounds();
                        if(isInputEmpty())
                            endMeasuringTheTime();
                    }
                    else if(Input.GetKeyDown(KeyCode.Escape))
                    {
                        this.gameManager.moveToPreviousPhase();
                        this.infoToast.displayMessage("Poprzedni wybór...",false);
                        endMeasuringTheTime();
                    }
                    else if(Input.GetKeyDown(KeyCode.Space)) 
                    {
                        processLastPressedKey(' ');
                    }
                
                    else if(Input.GetKey(KeyCode.LeftShift))
                    {
                        foreach(KeyCode k in this.availableKeys)
                        {
                            if(Input.GetKeyDown(k))
                            {
                                processLastPressedKey(this.upperCaseKeys[k]);
                                break;
                            }
                        }
                    }
                    else
                    {
                        foreach(KeyCode k in this.availableKeys)
                        {
                            if(Input.GetKeyDown(k))
                            {
                                processLastPressedKey(this.lowerCaseKeys[k]);
                                break;
                            }
                        }
                    }
                
                }
               
               
            
        }
        
    }    
    void FixedUpdate()
    {
        if(inputF.isFocused == false)
        {
            inputF.ActivateInputField();
        }
        inputF.caretPosition = inputF.text.Length;
        
        
    }
    void OnGUI()
    {
      
    }
   

    private GameManagerScript getGameManagerReference()
    {   return GameObject.Find("GameManager").GetComponent<GameManagerScript>(); }
    private ToastManager getToastManagerReference()
    { return GameObject.Find("ToastManager").GetComponent<ToastManager>();}

    private MeasurementManager getMeasurementManagerReference()
    { return GameObject.Find("MeasurementManager").GetComponent<MeasurementManager>();}
    private void processLastPressedKey(char pressedKey)
    {   
       
        if(!this.isMeasuring && this.inputF.caretPosition>0)
            startMeasuringTheTime();
        if(this.isMeasuring)
        {
            updateMeasureEndTime();
            incrementKeysPressedDuringMeasurement();
        }

        bool matches = checkIfKeyMatchesOneOfAvailableWords(pressedKey);
        if(!matches)
        {
            resetKeysSinceLastMistake();
            this.infoToast.displayMessage("Błąd!", true);
            if(pressedKey == ' ')
                incrementMistakesMade("spacja");
            else incrementMistakesMade(pressedKey.ToString());
        }
        else incrementKeysSinceLastMistake();   
    }
    
    private bool checkIfKeyMatchesOneOfAvailableWords(char key)
    {
        bool matches = false;
        
        if(this.longestWordLengthInCurrentPhaseWords>this.inputF.caretPosition-1 && this.inputF.caretPosition>0)
        {
            for(int i = this.lowerAvailableWordsBounds; i<=this.upperAvailableWordsBounds && !matches;i++)
            {
                
                if(key==' ')
                {
                    if(char.IsWhiteSpace( (currentPhaseWords[i])[this.inputF.caretPosition-1] ) )
                        matches=true;
                    
                }
                else if(this.inputF.caretPosition-1<currentPhaseWords[i].Length && (currentPhaseWords[i])[this.inputF.caretPosition-1].Equals(key))
                {
                    matches = true;
                    if(this.lowerAvailableWordsBounds!=this.upperAvailableWordsBounds)
                        recheckAvailableWordsBounds();
                }

            }
        }
        
        return matches;
    }

 
    public void getCurrentPhaseWords(string[] words, int typingPhase)
    {
        this.currentPhaseWords = words;
        this.lowerAvailableWordsBounds = 0;
        this.upperAvailableWordsBounds = this.currentPhaseWords.Length-1;
        this.longestWordLengthInCurrentPhaseWords = getLongestWordInWordArray(words);

        switch(typingPhase)
        {
            case 1: setTextToType("Wybierz strukturę na mapie"); break;
            case 2: setTextToType("Wybierz akcję lub strukturę do zaatakowania"); break;
            case 3: setTextToType(words[0]); break;
        }

        for(int i=0;i<this.currentPhaseWords.Length;i++)
            Debug.Log(this.currentPhaseWords[i]+" - "+this.currentPhaseWords[i].Length);

    }

    private void recheckAvailableWordsBounds()
    {
        bool lowerBoundSet=false;
        bool upperBoundSet=false;
        if(this.inputF.caretPosition==0)
        {
            this.lowerAvailableWordsBounds = 0;
            this.upperAvailableWordsBounds = currentPhaseWords.Length-1;
            lowerBoundSet = true;
            upperBoundSet = true;
        }
        if(this.currentPhaseWords.Length>1)
        {
            for(int i=0;i<this.currentPhaseWords.Length && !lowerBoundSet;i++)
            {
                if(!lowerBoundSet && this.currentPhaseWords[i].StartsWith(this.inputF.text))
                {
                    this.lowerAvailableWordsBounds = i;
                    lowerBoundSet = true;
                }
            }

            for(int i=this.currentPhaseWords.Length-1;!upperBoundSet && i>this.lowerAvailableWordsBounds; i--)
            {
                 if(!upperBoundSet && this.currentPhaseWords[i].StartsWith(this.inputF.text))
                {
                    this.upperAvailableWordsBounds = i;
                    upperBoundSet = true;
                }
            }
            if(!upperBoundSet)
                this.upperAvailableWordsBounds = this.currentPhaseWords.Length-1;
        }
    }

    public void setTextToType(string s)
    { this.textToType.text = s;}

    // before being send to gameManager
    private bool checkIfEnteredTextMatchesOneOfAvailableSequences(string text)
    {
        bool matches = false;
        for(int i=this.lowerAvailableWordsBounds;i<=this.upperAvailableWordsBounds && !matches;i++)
        {
            if(text.Equals(currentPhaseWords[i]))
            {
                matches = true;
            }
                
        }
        if(!matches)
        {
            this.infoToast.displayMessage("Błąd. Wpisuj od nowa.", true);
            resetKeysSinceLastMistake();
            clearInputField();
        }
        else incrementKeysSinceLastMistake();
        return matches;
            
    }

    public void enterUnitAmountMode(bool isUnitEnterMode)
    {
        if(isUnitEnterMode)
        {
            this.enterUnitsMode = isUnitEnterMode;
            this.inputF.contentType = InputField.ContentType.IntegerNumber;
        }
        else
        {
            this.enterUnitsMode = isUnitEnterMode;
            this.inputF.contentType = InputField.ContentType.Standard;
        }
    }

    private int getLongestWordInWordArray(string[] words)
    {
        int currentLongestIndex=0;
        for(int i=0;i<words.Length;i++)
        {
            if(words[currentLongestIndex].Length<=words[i].Length)
                currentLongestIndex=i;
        }
        return words[currentLongestIndex].Length;
    }

    public void clearInputField()
    {
        this.inputF.text = "";
        if(this.currentPhaseWords != null)
        {
            this.lowerAvailableWordsBounds = 0;
            if(this.currentPhaseWords.Length>1)
                this.upperAvailableWordsBounds = this.currentPhaseWords.Length;
            else this.upperAvailableWordsBounds = 0;
        }
        
    }

    public bool isInputEmpty()
    {
        return this.inputF.caretPosition == 0;
    }

    private void incrementKeysPressedDuringMeasurement()
    {
         this.measurementManagerRef.incrementTotalCharsTyped(); 
         this.keysTypedDuringMeasurmentTime++;
    }
    private void incrementMistakesMade(string errorMade)
    { this.measurementManagerRef.incrementTotalErrorsMade(errorMade);}

    private void startMeasuringTheTime()
    {
        this.isMeasuring = true;
        this.measureStartTime = currentTime();
    }

    private void endMeasuringTheTime()
    {
        if(isMeasuring)
        {
             this.isMeasuring = false;
            sendMeasuresToMeasurementManager();
            resetMeasureVariables();
        }
       
    }

    private double calculateTypingTime(double start, double end)
    {
        return end - start;
    }

    private void sendMeasuresToMeasurementManager()
    {
        this.measurementManagerRef.addTotalMeasurmentTime(calculateTypingTime(this.measureStartTime, this.measureEndTime));
    }

    private void updateMeasureEndTime()
    {
        this.measureEndTime = currentTime();
    }

    private void resetMeasureVariables()
    { 
        this.keysTypedDuringMeasurmentTime = 0;
        this.lastPressedKeyTime = 0;
        this.measureStartTime = 0;
        this.measureEndTime = 0;
    }

    private void incrementKeysSinceLastMistake()
    { 
        this.measurementManagerRef.incrementKeysPressedSinceLastMistake();
    }

    private void resetKeysSinceLastMistake()
    { 
        this.measurementManagerRef.resetKeysPressedSinceLastMistake();
    }
    public double currentTime()
    {   
        return (DateTime.UtcNow - this.startDateTime).TotalMilliseconds;}

    public void increaseBackspaceTimesPressedIfInputFieldNotEmpty()
    {
        if(this.inputF.caretPosition>0)
            this.measurementManagerRef.incrementBackspaceTimesPressed();
    }
}
