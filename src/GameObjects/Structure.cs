using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
public class Structure : MonoBehaviour
{
    private class DistanceComparer : IComparer<Structure>
    {
        Structure sourceStruct;
        public DistanceComparer(Structure sourceStruct)
        {
            this.sourceStruct = sourceStruct;
        }

        public int Compare(Structure firstStructure, Structure secondStructure)
        {
            float distToFirst = sourceStruct.calculateDistanceTo(firstStructure);
            float distToSecond = sourceStruct.calculateDistanceTo(secondStructure);
            if(distToFirst > distToSecond)
                return 1;
            else if(distToFirst< distToSecond)
                return -1;
            else return 0;
        }
    }

    public StructureType structureType;
    public int stationingUnits;
    private int passiveProductionSoldierLimit;
    public int[] upgradeLevelSoldierLimits;

    public TextMeshProUGUI textStationingUnits;
    public Text textToWrite;
    public TextMeshProUGUI coordinates;
    public Player owner;
    public Structure structure;
    public double baseGoldProduction;
    public double baseUnitsProduction;
    public int upgradeLevel;
    public int[] upgradeCosts;
    public double activeGoldProductionModifier;
    public double activeUnitsProductionModifier;
    public int productionInterval;
    private bool isCurrentlySelected;
    public string selectionString;
    public string moveToStructString;
    public GameObject soldier;
    GameObject ownedSoldier;
    public List<Structure> structuresSortedByDistanceFromThisStructure;

    public static double requiredBonusForAdditionalBenefits;
    public static double bonusChance;

   
    // Start is called before the first frame update
    void Start()
    {
        requiredBonusForAdditionalBenefits = 2.0;
        bonusChance = 0.3;
        structuresSortedByDistanceFromThisStructure = new List<Structure>();
        populateStructureDistanceMap(structuresSortedByDistanceFromThisStructure);
        structuresSortedByDistanceFromThisStructure.Sort(new DistanceComparer(this));
        this.isCurrentlySelected = false;
        this.passiveProductionSoldierLimit = this.upgradeLevelSoldierLimits[this.upgradeLevel-1];
        InvokeRepeating("passivelyProduceResources", 5, this.productionInterval);
        if(this.coordinates != null)
            this.coordinates.text = "x: "+this.transform.localPosition.x.ToString("N0") +",y: "+this.transform.localPosition.y.ToString("N0");
        //Structure dest = GameObject.Find("CastleNearPlayer1").GetComponent<Structure>();
        //issueSendToOrder(dest, 29);
    }

    // Update is called once per frame
    void Update()
    {
        updateStructureColorToOwnerColor();
        updateStationingUnitsCountText();
    }

    public void executeAction(ActionType action)
    {
        switch(action)
        {
            case ActionType.ProduceGold: produceActiveGold();
            break;
            case ActionType.ProduceSoldiers: produceActiveUnits();
            break;
            case ActionType.UpgradeStructure: upgradeThisStructure(); break;
        }
    }

    public void updateStructureColorToOwnerColor()
    {  
         if(structureType == StructureType.Castle){
            Image structureImage = gameObject.GetComponent<Image>();
            structureImage.color = owner.playerColor;
        }
        else
        {
            GameObject gObj = this.transform.Find("Flags").gameObject;
            Image structureImage = gObj.GetComponent<Image>();
            structureImage.color = owner.playerColor;
        }
    }

    private void updateStationingUnitsCountText()
    {   
        if(!this.owner.playerType.Equals(PlayerType.Neutral))
            this.textStationingUnits.text = this.stationingUnits.ToString()+"/"+this.passiveProductionSoldierLimit;
        else  this.textStationingUnits.text = this.stationingUnits.ToString();
    }

    private void passivelyProduceResources()
    {
        if(owner.playerType != PlayerType.Neutral)
        {
            producePassiveGold();
            producePassiveUnits();
        }
    }

    private void producePassiveUnits()
    {
        if(this.passiveProductionSoldierLimit>this.stationingUnits)
        {
            this.stationingUnits += calculatePassiveUnitProduction();
            if(this.stationingUnits>this.passiveProductionSoldierLimit)
                this.stationingUnits = this.passiveProductionSoldierLimit;
        }

    }
    private void producePassiveGold()
    {   
        
        this.owner.totalGold+=calculatePassiveGoldProduction();
    }

    private void produceActiveGold()
    {
        int producedGold = calculateActiveGoldProduction();
        this.owner.totalGold += producedGold;
        if(this.owner.playerType.Equals(PlayerType.Human))
        {
             if(this.owner.getPlayerBonusModifier()>requiredBonusForAdditionalBenefits)
             {
                 System.Random rand = new System.Random();
                 if(bonusChance>rand.NextDouble())
                    (this.owner as HumanPlayer).bonusGold(producedGold);
                else GameObject.Find("ToastManager").GetComponent<ToastManager>().displayMessage("Wyprodukowano "+producedGold+" sztuk złota." ,false);
             }
             else GameObject.Find("ToastManager").GetComponent<ToastManager>().displayMessage("Wyprodukowano "+producedGold+" sztuk złota." ,false);
        }
            
    }
    private void produceActiveUnits()
    {   
        int producedSoldiers = calculateActiveUnitProduction();
        this.stationingUnits += producedSoldiers;
         if(this.owner.playerType.Equals(PlayerType.Human))
         {
             if(this.owner.getPlayerBonusModifier()>requiredBonusForAdditionalBenefits)
            {
                System.Random rand = new System.Random();
                if(bonusChance>rand.NextDouble())
                    (this.owner as HumanPlayer).bonusSoldiers(producedSoldiers, this);
                else  GameObject.Find("ToastManager").GetComponent<ToastManager>().displayMessage("Wyprodukowano "+producedSoldiers +" żołnierzy." ,false);
            }
            else GameObject.Find("ToastManager").GetComponent<ToastManager>().displayMessage("Wyprodukowano "+producedSoldiers +" żołnierzy." ,false);
         }
           

    }

    public int calculateActiveGoldProduction()
    { return (int) (calculatePassiveGoldProduction() * this.activeGoldProductionModifier);}

    public int calculateActiveUnitProduction()
    { return (int) (calculatePassiveUnitProduction() * this.activeUnitsProductionModifier);}

    private void upgradeThisStructure()
    {
        int upgradeCost = calculateUpgradeCost();
        if(this.owner.totalGold>=upgradeCost && !isMaxLevelUpgrade())
        {
            this.owner.totalGold -= upgradeCost;
            this.upgradeLevel++;
            this.passiveProductionSoldierLimit = this.upgradeLevelSoldierLimits[this.upgradeLevel-1];
            this.transform.localScale = new Vector3(this.transform.localScale.x*1.25f,this.transform.localScale.y*1.25f,this.transform.localScale.z);
            if(this.owner.playerType.Equals(PlayerType.Human))
            {
                GameObject.Find("ToastManager").GetComponent<ToastManager>().displayMessage("Ulepszono strukturę",false);
                if(this.owner.getPlayerBonusModifier()>requiredBonusForAdditionalBenefits)
                {
                    System.Random rand = new System.Random();
                    if(bonusChance>rand.NextDouble())
                        (this.owner as HumanPlayer).refundUpgrade(upgradeCost);
                }
            }
            
        }
    }
    public void reverseUpgrade()
    {
        if(this.upgradeLevel>1)
        {
             this.upgradeLevel--;
            this.passiveProductionSoldierLimit=this.upgradeLevelSoldierLimits[this.upgradeLevel-1];
            this.transform.localScale = new Vector3(this.transform.localScale.x*0.8f,this.transform.localScale.y*0.8f,this.transform.localScale.z);
        }
       
    }
    public void issueSendToOrder(Structure destination, int numberOfSendSoldiers)
    {
            if(this.stationingUnits >= numberOfSendSoldiers)
            {
                GameObject newObj = Instantiate(this.soldier, this.structure.GetComponent<RectTransform>().position, Quaternion.identity) as GameObject;
                Unit newUnit = newObj.GetComponent<Unit>();
                newUnit.soldier = newObj;
                newUnit.owner = this.owner;
                newUnit.sendingStructure = this.GetComponent<Structure>();
                newUnit.destinationStructure = destination;
                newUnit.numberOfSoldiers = numberOfSendSoldiers;
                newObj.GetComponent<Image>().color = this.owner.playerColor;
                newObj.transform.SetParent(this.transform.parent.transform);
                addSoldierToOwnersUnitsOnMove(newUnit);

                this.stationingUnits -= numberOfSendSoldiers;
            }

    }
    
    public void setDisplayTextToWrite(int inputPhase)
    {
        if(inputPhase == 1)
        {
            if(owner.playerType == PlayerType.Human)
                setTextToWriteSelect();
            else setTextToWriteEmpty();
        }
        else if(inputPhase == 2)
        {
             if(this.isCurrentlySelected)
                setTextToWriteEmpty();
            else
                setTextToWriteSendTo();
        }
        else setTextToWriteEmpty();
    }
    public void setTextToWriteEmpty()
    { this.textToWrite.text = " ";}
    public void setTextToWriteSelect()
    { this.textToWrite.text = this.selectionString; }
    public void setTextToWriteSendTo()
    { this.textToWrite.text = this.moveToStructString;}

    public int getTextToWriteLength()
    { return this.textToWrite.text.Length;}

    public int calculateUpgradeCost()
    {   if(!isMaxLevelUpgrade())
            return this.upgradeCosts[this.upgradeLevel-1]; 
        else return 0;

    }

    public int calculatePassiveUnitProduction()
    {
        double upgradeLevelFactor;
        if(this.upgradeLevel==1)
            upgradeLevelFactor=1.0;
        else upgradeLevelFactor= (double)this.upgradeLevel * (double)this.upgradeLevel * 0.7;

        int passiveUnitProd = (int)(this.baseUnitsProduction * upgradeLevelFactor * owner.getPlayerBonusModifier());
        if(passiveUnitProd==0)
            return 1;
        else return passiveUnitProd;
    }
    public int calculatePassiveGoldProduction()
    {
        double upgradeLevelFactor;
        if(this.upgradeLevel==1)
            upgradeLevelFactor=1.0;
        else upgradeLevelFactor= (double)this.upgradeLevel * (double)this.upgradeLevel * 0.7;

        int passiveGoldProd =(int)(this.baseGoldProduction * upgradeLevelFactor * owner.getPlayerBonusModifier());
        if(passiveGoldProd==0)
            return 1;
        else return passiveGoldProd;
    }

    private void addSoldierToOwnersUnitsOnMove(Unit unit)
    { this.owner.addUnitToUnitsOnMove(unit);}

    public bool canSendUnits(int amount)
    { return this.stationingUnits>=amount;}

    public float calculateDistanceTo(Structure anotherStructure)
    {
        float x1 = this.transform.position.x;
        float y1 = this.transform.position.y;
        float x2 = anotherStructure.transform.position.x;
        float y2 = anotherStructure.transform.position.y;

        return (float) Math.Sqrt( Math.Pow((x2-x1),2) + Math.Pow((y2-y1),2) );
    }

    private void populateStructureDistanceMap(List<Structure> listOfStructures)
    {
        foreach(GameObject obj in GameObject.FindGameObjectsWithTag("Structure"))
            if(obj!=this.gameObject)
                listOfStructures.Add(obj.GetComponent<Structure>());
    }

    public bool isMaxLevelUpgrade()
    { return this.upgradeLevel == this.upgradeCosts.Length;}
}
