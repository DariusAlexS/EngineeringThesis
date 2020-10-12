using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class HumanPlayer : Player
{
    public Text totalGoldText;
    public Text totalUnitsText;
    public ToastManager toastRef;
    public double goldReductionFactor; 
    public double goldIncreaseFactor; 
    public double upgradeRefundFactor; 
    public double armyKillingFactor; 
    public double armyIncreaseFactor; 
    public double positiveBonusReductionFactor; 
    public double negativeBonusReductionFactor;

    public double playerBonusIncreaseFlat;

    public double minimumBonus;
    public double maximumBonus;
    new void Start()
    {
        base.Start();
        this.toastRef = GameObject.Find("ToastManager").GetComponent<ToastManager>();
    }
    new void Update()
    {
        base.Update();
        this.totalGoldText.text = totalGold.ToString();
        this.totalUnitsText.text = this.totalUnits.ToString();
        if(this.playerStructures.Count==0)
            GameObject.Find("GameStateManager").GetComponent<GameStateManager>().humanPlayerLost();
    }

    public void increasePlayerBonus(int bonusInRow)
    {
        double newBonus = this.getPlayerBonusModifier() + this.playerBonusIncreaseFlat * bonusInRow;
        if(newBonus<this.maximumBonus)
            this.setPlayerBonusModifier(newBonus);
        else this.setPlayerBonusModifier(maximumBonus);

        this.toastRef.displayMessage("Bonus!",false);
        this.toastRef.changeToFontColorToPositive();
    }
    public void reducePlayerBonus()
    {
        double realBonus = this.getPlayerBonusModifier() - 1;
        if(realBonus>1.0)
            setPlayerBonusModifier(realBonus * this.positiveBonusReductionFactor + 1.0);
        else
            setPlayerBonusModifier(this.getPlayerBonusModifier() - this.getPlayerBonusModifier()*this.negativeBonusReductionFactor);

        if(this.getPlayerBonusModifier()<this.minimumBonus)
            this.setPlayerBonusModifier(this.minimumBonus);
    
    }
    public void issuePunishment()
    {
        int punishmentType = new System.Random().Next(0,5);
        switch(punishmentType)
        {
            case 1: generalPunishment(); break;
            case 2: killUnitsOnMove(); break;
            case 3: loseGold(); break;
            case 4: reverseStructureUpgrade(); break;
        }
        reducePlayerBonus();
    }
    
    // units related punishment methods
    private void killUnitsOnMove()
    {
        bool issuedPunishment = false;
        foreach(Unit u in unitsOnMove)
        {
            if(willUnitConquerDestStructure(u))
            {
                u.numberOfSoldiers = (int) (u.destinationStructure.stationingUnits * this.armyKillingFactor);
                if(u.numberOfSoldiers == 0)
                    u.numberOfSoldiers = 1;
                issuedPunishment = true;
                break;
            }  
        }
        if(!issuedPunishment)
            killUnitsInStructure(findStructureWithMostStationingUnits());
        
    }

    private bool willUnitConquerDestStructure(Unit u)
    {
        bool conquering = false;
        if(u.destinationStructure.owner != this && u.destinationStructure.stationingUnits<u.numberOfSoldiers)
            return true;
        else return false;
    }

    private Structure findStructureWithMostStationingUnits()
    {
        int indexOfStrongest = 0;
        for(int i=0;i<this.playerStructures.Count;i++)
            if(this.playerStructures[indexOfStrongest].stationingUnits<this.playerStructures[i].stationingUnits)
                indexOfStrongest = i;
        return this.playerStructures[indexOfStrongest];
    }

    private void killUnitsInStructure(Structure s)
    {
        int unitLoss= s.stationingUnits;
        s.stationingUnits= (int) (s.stationingUnits * this.armyKillingFactor);
        unitLoss -= s.stationingUnits;
        this.toastRef.displayMessage("Za karę zabito "+unitLoss+" jednostek w strukturze!",false);
    }

    // economy related punishment methods
    private void loseGold()
    {
        int goldLoss = this.totalGold;
        this.totalGold = (int)(this.totalGold * this.goldReductionFactor);
        goldLoss -=this.totalGold;
        this.toastRef.displayMessage("Za karę zabrano "+goldLoss+" sztuk złota!", false);
    }

    private Structure findStructureWithBestUpgrade()
    {
        int indexOfBestUpgrade = 0;
        for(int i=0;i<this.playerStructures.Count;i++)
            if(this.playerStructures[indexOfBestUpgrade].upgradeLevel<this.playerStructures[i].upgradeLevel)
                indexOfBestUpgrade = i;
        return this.playerStructures[indexOfBestUpgrade];
    }

    private void reverseStructureUpgrade()
    {
        Structure s= findStructureWithBestUpgrade();
        if(s.upgradeLevel>1)
        {
             s.reverseUpgrade();
             this.toastRef.displayMessage("Za karę zmniejszono poziom struktury!", false);
        } 
        else killUnitsInStructure(s);
    }

    // general punishments

    private void generalPunishment()
    {
        bool wasPunished=false;
        if(this.unitsOnMove.Count>0)
        {
            foreach(Unit u in this.unitsOnMove)
            {
                if(!u.wasUnitPunished())
                {
                    u.permanentSlowDown(0.2);
                    this.toastRef.displayMessage("Za karę spowolniono szybkość jednostkę o 90% !",true);
                    wasPunished = true;
                    break;
                }
            }
        }
        else if(!wasPunished && this.totalGold>400)
                loseGold();
        else killUnitsInStructure(findStructureWithMostStationingUnits());
            
    }

    // bonuses
    public void refundUpgrade(int goldCost)
    {
        this.totalGold+= (int) (this.totalGold * this.upgradeRefundFactor);
        this.toastRef.displayMessage("Zwrócono " + this.upgradeRefundFactor.ToString("P")+ " kosztów za ulepszenie",false);
    }

    public void bonusGold(int producedGold)
    {
        int bonusGold = (int)(producedGold * this.goldIncreaseFactor);
        this.totalGold+= bonusGold;
        this.toastRef.displayMessage("Wyprodukowano "+ bonusGold.ToString() + " dodatkowych sztuk złota", false);
    }

    public void bonusSoldiers(int soldiersProduced, Structure source)
    {
        int bonusSoldiersProduced = (int)(soldiersProduced * this.armyIncreaseFactor);
        source.stationingUnits +=bonusSoldiersProduced;
        this.toastRef.displayMessage("Wyprodukowano "+ bonusSoldiersProduced.ToString() + " dodatkowych żołnierzy!",false);
    }

    public void bonusSoldiers(int soldiersSend, Unit u)
    {
        int bonusSoldiersSend = (int)(soldiersSend * this.armyIncreaseFactor);
        u.numberOfSoldiers +=bonusSoldiersSend;
        this.toastRef.displayMessage("Do oddziału dołączyło "+ bonusSoldiersSend.ToString() + " dodatkowych żołnierzy!",false);
    }

    public void deduceBonusForBackspaceUse()
    { if(getPlayerBonusModifier()>this.minimumBonus)
        this.setPlayerBonusModifier(getPlayerBonusModifier()-0.01);}
}
