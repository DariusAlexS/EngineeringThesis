using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierObstacle : MonoBehaviour
{
    public float slowingFactor;
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag=="Soldier")
        {
            Unit walkingUnit = other.gameObject.GetComponent<Unit>();
            walkingUnit.slowSpeed(this.slowingFactor);
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if(other.gameObject.tag=="Soldier")
        {
            Unit walkingUnit = other.gameObject.GetComponent<Unit>();
            walkingUnit.restoreMaxSpeed();
        }
    }
}
