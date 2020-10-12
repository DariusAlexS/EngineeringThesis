using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
public class Unit : MonoBehaviour
{
    public GameObject soldier;
    public int numberOfSoldiers;
    public float movementSpeed;
    private float maxSpeed;
    public TextMeshProUGUI textNumberOfSoldiers;
    public Player owner;
    public Structure sendingStructure;
    public Structure destinationStructure;
    private float xDirection;
    private float yDirection;
    public float baseSizeScale;
    private bool isSlowedDown;
    private bool wasPunished;
    private float currentSlowingFactor;
    void Start()
    {
        this.wasPunished= false;
        this.maxSpeed = this.movementSpeed;
        this.isSlowedDown = false;
        this.currentSlowingFactor = 0f;
        textNumberOfSoldiers.text = numberOfSoldiers.ToString(); 
        float scaleModifier = calculateScaleBasedOnAmountOfSoldiers(this.numberOfSoldiers);
        this.soldier.transform.localScale = new Vector3(this.soldier.transform.localScale.x*scaleModifier, this.soldier.transform.localScale.y*scaleModifier, this.soldier.transform.localScale.z);
        calculateMovementDirections();
        addUnitToListOfAttackingUnitsForAttackedPlayer(this.destinationStructure.owner);
    }

    void FixedUpdate()
    { 
       moveUnitTowardsDestination();
    }

    private void moveUnitTowardsDestination()
    {
        soldier.transform.position = new Vector3(this.soldier.transform.position.x + xDirection * this.movementSpeed, this.soldier.transform.position.y + yDirection * this.movementSpeed, this.soldier.transform.position.z);
    }

    private void calculateMovementDirections()
    {
        double xStart, yStart, xEnd, yEnd, xDistance, yDistance, shortestDistance;
        xStart = this.sendingStructure.transform.position.x;
        yStart = this.sendingStructure.transform.position.y;
        xEnd   = this.destinationStructure.transform.position.x;
        yEnd   = this.destinationStructure.transform.position.y;
        xDistance = xEnd-xStart;
        yDistance = yEnd-yStart;
        shortestDistance = Math.Sqrt(Math.Pow(xDistance,2)+Math.Pow(yDistance,2));
        xDirection = (float)(xDistance/shortestDistance);
        yDirection = (float)(yDistance/shortestDistance);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.GetComponent<Structure>()!=null && other.gameObject.GetComponent<Structure>().Equals(this.destinationStructure))
        {
            if(this.destinationStructure.owner.Equals(this.owner))
                    this.destinationStructure.stationingUnits+=this.numberOfSoldiers;
            else attackStructure(this.destinationStructure, this.numberOfSoldiers, this.owner);
            Destroy(this.soldier);
        }
            
    }

    private void attackStructure(Structure attackedStructure, int attackingSoldiers, Player attacker)
    {
        removeUnitFromListOfAttackingUnitsForAttackedPlayer(attackedStructure.owner);
        if(attackedStructure.stationingUnits<attackingSoldiers)
            {
                attackedStructure.stationingUnits = attackingSoldiers - attackedStructure.stationingUnits;
                attackedStructure.owner.removeOwnedStructure(attackedStructure);
                attackedStructure.owner = attacker;
                attackedStructure.updateStructureColorToOwnerColor();
                owner.addOwnedStructure(attackedStructure);
                GameManagerScript gameManager = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
                if(attacker.playerType == PlayerType.Human)
                    gameManager.updateInputAfterConquering(attackedStructure);
                attackedStructure.setDisplayTextToWrite(gameManager.getCurrentTypingPhase());
                
            }
        else attackedStructure.stationingUnits -= attackingSoldiers;
        owner.removeUnitFromUnitsOnMove(this);
        
    }

    public void slowSpeed(float slowingFactor)
    {
        if(!this.isSlowedDown)
        {
            this.isSlowedDown = true;
            this.movementSpeed = this.movementSpeed * (1-slowingFactor);
        }
        else if(this.currentSlowingFactor!=slowingFactor)
        {
            this.movementSpeed = this.maxSpeed;
            this.movementSpeed = this.movementSpeed * (1 -slowingFactor);
        }
    }

    public void restoreMaxSpeed()
    { if(this.isSlowedDown)
        {
            this.movementSpeed = this.maxSpeed;
            this.isSlowedDown = false;
        }
        
    }
    private float calculateScaleBasedOnAmountOfSoldiers(int amountOfSoldiers)
    {
        return this.baseSizeScale + (float) Math.Log((double)amountOfSoldiers, 100.0);
    }

    public Structure getDestinationStructure()
    { return this.destinationStructure; }

    public int getNumberOfSoldiersInUnit()
    { return this.numberOfSoldiers; }

    public void permanentSlowDown(double slowDownFactor)
    {
        if(!this.wasPunished)
        {
            this.maxSpeed = (float) (this.maxSpeed * slowDownFactor);
            this.movementSpeed = (float) (this.movementSpeed * slowDownFactor);
            this.wasPunished = true;
        }
        
    }

    public bool wasUnitPunished()
    {return this.wasPunished;}

    public void addUnitToListOfAttackingUnitsForAttackedPlayer(Player attackedPlayer)
    {
        attackedPlayer.addUnitToListOfAttackingUnits(this);
    }

    public void removeUnitFromListOfAttackingUnitsForAttackedPlayer(Player attackedPlayer)
    {
        attackedPlayer.removeUnitFromListOfAttackingUnits(this);
    }
    
}
