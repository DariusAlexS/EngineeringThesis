using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Player : MonoBehaviour
{
    
    public PlayerType playerType;
    public int totalGold;
    
    public int totalUnits;
    public Color playerColor;
    public List<Structure> playerStructures;
    public List<Unit> unitsOnMove;
    public List<Unit> listOfAttackingUnits;
    public Structure startingStructure;
    public double bonusModifier;
    public void Start()
    {
        setPlayerBonusModifier(1.0);
        this.playerStructures = new List<Structure>(3);
        if(!playerType.Equals(PlayerType.Neutral))
            playerStructures.Add(startingStructure);
        else
        {
            foreach(GameObject gObj in GameObject.FindGameObjectsWithTag("Structure"))
                if(gObj.GetComponent<Structure>().owner.playerType.Equals(PlayerType.Neutral))
                    playerStructures.Add(gObj.GetComponent<Structure>());
        }
        this.unitsOnMove = new List<Unit>(6);
        this.listOfAttackingUnits = new List<Unit>(5);
    }
    public void Update()
    {
        this.totalUnits = calculateTotalUnits();
    }

    protected int calculateTotalUnits()
    {
        int numberOfTotalUnits = 0;
        foreach(Structure ownedStructure in this.playerStructures)
            numberOfTotalUnits += ownedStructure.stationingUnits;

        foreach(Unit unit in unitsOnMove)
            numberOfTotalUnits += unit.numberOfSoldiers;

        return numberOfTotalUnits;
    }   


    public double getPlayerBonusModifier()
    { return this.bonusModifier;}

    public void setPlayerBonusModifier(double modifier)
    { this.bonusModifier = modifier;}

    public void addUnitToUnitsOnMove(Unit unit)
    { this.unitsOnMove.Add(unit);}

    public void removeUnitFromUnitsOnMove(Unit unit)
    { this.unitsOnMove.Remove(unit);}

    public void addOwnedStructure(Structure s)
    { this.playerStructures.Add(s);}

    public void removeOwnedStructure(Structure s)
    {  this.playerStructures.Remove(s);    }

    public void addUnitToListOfAttackingUnits(Unit u)
    { this.listOfAttackingUnits.Add(u);}

    public void removeUnitFromListOfAttackingUnits(Unit u)
    {  this.listOfAttackingUnits.Remove(u); }

}
