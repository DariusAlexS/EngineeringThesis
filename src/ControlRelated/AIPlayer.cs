using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayer : Player
{
    // comparators and others
    private class StructureImportanceComparer : IComparer<Structure>
    {
        public int Compare(Structure first, Structure second)
        {
            if(first.upgradeLevel > second.upgradeLevel)
                return 1;
            else if(first.upgradeLevel < second.upgradeLevel)
                return -1;
                else if(first.structureType.Equals (StructureType.GoldMine) && second.structureType.Equals (StructureType.Castle))
                        return 1;
                    else if(first.structureType.Equals (StructureType.Castle) && second.structureType.Equals ( StructureType.GoldMine))
                        return -1;
                    else return 0;
        }
    }

    private class StructureStationingSoldiersComparer : IComparer<Structure>
    {
        public int Compare(Structure first, Structure second)
        {
            if(first.stationingUnits>second.stationingUnits)
                return 1;
            else if(first.stationingUnits<second.stationingUnits)
                return -1;
            else return 0;
        }
    }
    
    private class StructureSoldiersDistanceComparer : IComparer<Structure>
    {
        Structure attackedStructure;
        public StructureSoldiersDistanceComparer(Structure attackedStruct)
        {
            this.attackedStructure = attackedStruct;
        }

        public int Compare(Structure first, Structure second)
        {
            if(  (float)first.stationingUnits * (second.calculateDistanceTo(attackedStructure)/first.calculateDistanceTo(attackedStructure)) > (float )second.stationingUnits * (first.calculateDistanceTo(attackedStructure)/second.calculateDistanceTo(attackedStructure)) )
                return 1;
            else if ((float)first.stationingUnits * (second.calculateDistanceTo(attackedStructure)/first.calculateDistanceTo(attackedStructure)) < (float)second.stationingUnits * (first.calculateDistanceTo(attackedStructure)/second.calculateDistanceTo(attackedStructure)) )
               return -1;
            else return 0;
        }

    }
    private class StructureSoldierProductionCapabilities : IComparer<Structure>
    {
        public int Compare(Structure first, Structure second)
        {
            if(first.calculateActiveUnitProduction()>second.calculateActiveUnitProduction())
                return 1;
            else if(first.calculateActiveUnitProduction()<second.calculateActiveUnitProduction())
                return -1;
            else return 0;
        }
    }
    
    // references
    private MeasurementManager playerMeasurementsRef;
    private wordManager wordManagerRef;
  

    // algorithm related
    private AIAction currentlyPerformedAction;

    // gameState
    private bool hasLost;

    // AI typing related
    private int typingPhase;
    private double lengthOfCurrentWordToType; 
    private double alreadyTypedCharacters;
    public double typedCharsUpdateFrequency; 
    private Structure currentlySelectedStructure;
    private Structure destinationStructure;
   
    // AI typing speed management
    public double AITypingSpeedRatioComparedToPlayer; 
    public double AITypingSpeedBias; 
    private double AITypingSpeed; 

    // AI mainLoopControl
    private bool isMainLoopBeingExecuted;


    // AI control parameters
    private double defaultAttackFactor; 
    public double prefferedAttackFactor; 


    private int numberOfStructuresOnMap;
    //private double percentageOfOwnedStructures;
    private bool pickedFirstAction;
    //public float searchRange;
    /* INHERITED FROM PLAYER */
    // List<Unit> unitsOnMove 
    // List<Structure> playerStructures;
    // int totalUnits;
    // int totalGold;
    
    // currentAction
    new void Start()
    {
        base.Start();
        this.pickedFirstAction = false;
        this.listOfAttackingUnits = new List<Unit>(5);
        this.typingPhase = 1;
        this.defaultAttackFactor = 1.0;
        this.numberOfStructuresOnMap = GameObject.FindGameObjectsWithTag("Structure").Length;
        this.playerMeasurementsRef = GameObject.Find("MeasurementManager").GetComponent<MeasurementManager>();
        this.wordManagerRef = GameObject.Find("WordManager").GetComponent<wordManager>();
        InvokeRepeating("updateAITypingSpeed",0,5);
        
        executeMainLoop();
        

    }
    
    new void Update()
    {

        if(this.playerStructures.Count==0)
        {
            GameObject.Find("ToastManager").GetComponent<ToastManager>().displayMessage("Gracz SI odpadl z gry...",false);
            GameObject.Find("GameStateManager").GetComponent<GameStateManager>().AIPLayerHasLost(this);
            Destroy(this.gameObject);
        }
    }

    public IEnumerator AIMainLoop()
    {   
    
        while(!hasPlayerLost())
        {
            this.currentlyPerformedAction = pickNextActionToPerform(); 
            this.pickedFirstAction = true;
            yield return StartCoroutine(simulateTyping());
            executeAction(this.currentlyPerformedAction);
          
        }
   
    }
/* 
    private void updatePercentageOfOwnedStructures()
    {
        this.percentageOfOwnedStructures = (double)this.playerStructures.Count/(double)this.numberOfStructuresOnMap;
    }*/
    public IEnumerator simulateTyping()
    {
        while(this.typingPhase<4)
        {
            this.lengthOfCurrentWordToType = getCurrentPhaseStringLength(this.typingPhase);
            yield return StartCoroutine(simulateCurrentPhaseTyping(this.lengthOfCurrentWordToType, this.typedCharsUpdateFrequency));
            increaseTypingPhase();
            resetAlreadyTypedStringLength();
        }
        resetAlreadyTypedStringLength();
        resetTypingPhase();
    }
    private IEnumerator simulateCurrentPhaseTyping(double lengthOfWord, double updateFreq)
    {
        while(lengthOfWord>this.alreadyTypedCharacters)
        {
          this.alreadyTypedCharacters += this.AITypingSpeed*updateFreq/12.0;
          yield return new WaitForSeconds((float)updateFreq);   
        }
    }
    private double getCurrentPhaseStringLength(int typingPhase)
    {
        double stringLength = 3.0;
        switch(typingPhase)
        {
            case 1: stringLength = (double)getStringLength(this.wordManagerRef.getStructSelectionString()); break;
            case 2: stringLength = (double)getStringLength(this.wordManagerRef.getActionSelectionString()); break;
            case 3: stringLength = (double)getStringLength(this.wordManagerRef.getActionExecutionString()); break;
        }
        return stringLength;
    }
    private void increaseTypingPhase()
    { 
        this.typingPhase++;
    }
    private void resetAlreadyTypedStringLength()
    { this.alreadyTypedCharacters = 0;}

    private void resetTypingPhase() 
    {
        this.typingPhase = 1;
    }

    private void resetAIParamsOnSelectedStructureBeingConquered()
    {
        resetAlreadyTypedStringLength();
        resetTypingPhase();
    }
    private void updateAITypingSpeed()
    {this.AITypingSpeed = this.playerMeasurementsRef.getPlayerWPM() * this.AITypingSpeedRatioComparedToPlayer + this.AITypingSpeedBias;}
    public void executeMainLoop()
    {
        
        StartCoroutine(AIMainLoop()); 
    }
    
   
    private AIAction pickNextActionToPerform()
    {
        AIAction nextAction = null;
        Structure selectedStructure = findUpgradeableStructure(this.playerStructures);
        Structure targetStructure = findRandomEnemyStructureInProxmity(this.startingStructure);
        if(selectedStructure!=null)
        {
            nextAction = new AIAction(ActionType.UpgradeStructure, selectedStructure, 100);
        }
        else if(canConquerStructureWithAnyOwnedStructure(targetStructure))
        {
            
            selectedStructure = findStructureWhichCanConquer(this.playerStructures, targetStructure);
            nextAction = queueAttackActionForConquerableStructure(selectedStructure, targetStructure);
        }
        else 
        {
            selectedStructure = pickBestStructureToBuildSoldiers(this.playerStructures);
            nextAction = new AIAction(ActionType.ProduceSoldiers, selectedStructure, 100);
        }

        return nextAction;
       
    }

    private Structure findStructureWhichCanConquer(List<Structure> structureList, Structure targetStructure)
    {
        Structure structThatCanConquer = null;
        structureList.Sort(new StructureSoldiersDistanceComparer(targetStructure));
        foreach(Structure s in structureList)
            if(s.stationingUnits * this.prefferedAttackFactor>targetStructure.stationingUnits)
            {
                structThatCanConquer = s;
                break;
            }

        return structThatCanConquer;

    }
    private Structure findUpgradeableStructure(List<Structure> structureList)
    {
        Structure upgradeableStruct = null;
        foreach(Structure s in structureList)
            if(s.calculateUpgradeCost()<this.totalGold)
            {
                upgradeableStruct = s;
                break;
            }
                
        return upgradeableStruct;
    }
    private bool canConquerStructureWithAnyOwnedStructure(Structure targetStructure)
    {
        bool canConquer = false;
        foreach(Structure s in this.playerStructures)
            if((double)s.stationingUnits * this.prefferedAttackFactor > targetStructure.stationingUnits)
            {
                canConquer=true;
                break;
            }
              
        return canConquer;
    }

    private AIAction queueAttackActionForConquerableStructure(Structure selectedStructure, Structure targetStructure)
    {
        return new AIAction(ActionType.MoveToCommand, selectedStructure, targetStructure, MoveToAIActionType.Attack, 100);
    }


    private Structure findRandomEnemyStructureInProxmity(Structure startingStructure)
    {
        int checkedStructuresCount = 3;
        List<Structure> potentiallyAttacked = new List<Structure>();
        foreach(Structure s in startingStructure.structuresSortedByDistanceFromThisStructure)
            if(s.owner != this && s!=startingStructure)
            {
                potentiallyAttacked.Add(s);
                checkedStructuresCount--;
                if(checkedStructuresCount==0)
                    break;
            }
        int randomNumber = new System.Random().Next(0,potentiallyAttacked.Count);

        return potentiallyAttacked[randomNumber];

    }


    private Structure pickBestStructureToBuildSoldiers(List<Structure> playerStructures)
    {
        playerStructures.Sort(new StructureSoldierProductionCapabilities());
        if(playerStructures.Count>0)
            return playerStructures[0];
        else return null;
    }
    

    private void executeAction(AIAction executedAction)
    {
        switch(executedAction.aiActionType)
        {
            case ActionType.MoveToCommand: performSendToAction(executedAction); break;
            case ActionType.ProduceGold: performGoldProductionAction(executedAction.sourceStructure); break;
            case ActionType.ProduceSoldiers: performSoldierProductionAction(executedAction.sourceStructure);  break;
            case ActionType.UpgradeStructure: performStructureUpgradeAction(executedAction.sourceStructure);break;
        }
    }


    // attack and reinforcement methods
    private void performSendToAction(AIAction sendToAction)
    {
        switch(sendToAction.moveToActionType)
        {
            case MoveToAIActionType.Reinforcements: sendReinforcements(sendToAction.sourceStructure, sendToAction.destinationStructure);
            break;
            case MoveToAIActionType.Attack: sendSoldiersToConquer(sendToAction.sourceStructure, sendToAction.destinationStructure); 
            break;
        }
    }
   
    
    private void sendReinforcements(Structure sendingStruct, Structure destination)
    {
        int requiredReinforcements = calculateRequiredReinforcements(sendingStruct, destinationStructure);
        sendingStruct.issueSendToOrder(destination, requiredReinforcements);

    }
    private int calculateRequiredReinforcements(Structure sendingStruct, Structure destinationStructure)
    {
        int numOfReinforcements = 0;
        int numberOfAttackingSoldiers = calculateNumberOfSoldiersAttackingStructure(destinationStructure);
        if(!destinationStructure.owner.Equals(this))
           numOfReinforcements = calculateAmountRequiredToConquer(sendingStruct, destinationStructure);
        else numOfReinforcements = sendingStruct.stationingUnits - numberOfAttackingSoldiers;

        if(numOfReinforcements<=0)
            numOfReinforcements = (int)(sendingStruct.stationingUnits * this.prefferedAttackFactor);
        return numOfReinforcements;
    }
    private void sendSoldiersToConquer(Structure sendingStruct, Structure destination)
    {
        int requiredSoldiers = 0;
        if(isStructureBeingAttacked(sendingStruct))
        {
            requiredSoldiers = calculateAvailableSoldiersToSendIfUnderAttack(sendingStruct, destination);
            if(requiredSoldiers<1)
                sendingStruct.issueSendToOrder(destination, (int)sendingStruct.stationingUnits);
            else
                sendingStruct.issueSendToOrder(destination, requiredSoldiers);

        }
        else 
        {
            requiredSoldiers = calculateAmountRequiredToConquer(sendingStruct, destination);
            sendingStruct.issueSendToOrder(destination, requiredSoldiers);
        }
    }
    private int calculateAmountRequiredToConquer(Structure sendingStruct, Structure destination)
    {
        int minimalRequired = 0;
        if(destination.stationingUnits>=sendingStruct.stationingUnits)
            minimalRequired = (int)(sendingStruct.stationingUnits * this.defaultAttackFactor);
        else
        {
            if(destination.stationingUnits>= (int)(sendingStruct.stationingUnits * this.prefferedAttackFactor))
                minimalRequired = (int)(sendingStruct.stationingUnits * this.defaultAttackFactor);
            else minimalRequired = (int)(sendingStruct.stationingUnits * this.prefferedAttackFactor);
        }
        return minimalRequired;
    }
    private int calculateAvailableSoldiersToSendIfUnderAttack(Structure sendingStruct, Structure destination)
    {
        int availableSoldiers = 0;
        int numberOfSoldiersAttackingThisStructure = calculateNumberOfSoldiersAttackingStructure(sendingStruct);
       
        if(destination.stationingUnits>=sendingStruct.stationingUnits-numberOfSoldiersAttackingThisStructure)
            availableSoldiers = sendingStruct.stationingUnits-numberOfSoldiersAttackingThisStructure;
        else if(availableSoldiers <1)
        {
            if(destination.stationingUnits>= (int)(sendingStruct.stationingUnits * this.prefferedAttackFactor)-numberOfSoldiersAttackingThisStructure)
                availableSoldiers = sendingStruct.stationingUnits - numberOfSoldiersAttackingThisStructure;
            else availableSoldiers = (int)(sendingStruct.stationingUnits * this.prefferedAttackFactor)-numberOfSoldiersAttackingThisStructure;
        }
        return availableSoldiers;
    }
    private int calculateNumberOfSoldiersAttackingStructure(Structure attackedStruct)
    {
        int attackingUnits = 0;
        foreach(Unit u in listOfAttackingUnits)
            attackingUnits+=u.numberOfSoldiers;
        
        return attackingUnits;
    }
    private bool isStructureBeingAttacked(Structure attacked)
    {
        bool isAttacked=false;
        foreach(Unit u in listOfAttackingUnits)
            if(u.destinationStructure == attacked)
            {
                isAttacked = true;
                break;
            }
        return isAttacked;
    }
   
   
    // PRODUCTION/ECONOMY ACTIONS
    private void performGoldProductionAction(Structure producingStructure)
    {
        producingStructure.executeAction(ActionType.ProduceGold);
    }
    private void performSoldierProductionAction(Structure producingStructure)
    {
        producingStructure.executeAction(ActionType.ProduceSoldiers);
    }
    private void performStructureUpgradeAction(Structure upgradedStructure)
    {
        if(this.totalGold>=upgradedStructure.calculateUpgradeCost())
            upgradedStructure.executeAction(ActionType.UpgradeStructure);
    }
   
   
   
    private Structure checkIfStructuresRankedByImportanceAreAttacked(List<Structure> listOfStructures)
    {   
        sortStructuresByImportance(listOfStructures);
        Structure attackedStructure = null;
        foreach(Structure s in listOfStructures)
        {
            foreach(GameObject gObj in GameObject.FindGameObjectsWithTag("Soldier"))
            {
                if(gObj.GetComponent<Unit>().getDestinationStructure()==s && isEnemyUnit(gObj.GetComponent<Unit>()))
                    return attackedStructure;
            } 
        }
        return attackedStructure;
    }



    private void attackedStructureCheck()
    {
        Structure attacked = null;
        attacked = checkIfStructuresRankedByImportanceAreAttacked(this.playerStructures);
        if(attacked!=null)
            attackedStructureHandling(attacked);
        else attacked = null;
    }
    private int getStringLength(string s)
    { return s.Length;}

    private bool isEnemyUnit(Unit u)
    { return this == u.owner;}

    public bool hasPlayerLost()
    { return this.hasLost;}

    private void sortStructuresByImportance(List<Structure> listOfStructures)
    { listOfStructures.Sort(new StructureImportanceComparer());}

    private Structure lookForStructureToAttack()
    {
        return null;
    }

    private void attackedStructureHandling(Structure attackedStruct)
    {

    }

    public bool hasAILost()
    { return this.hasLost; }
}
