using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum OutcomeEnum { AddFood, RemoveFood, AddGold, RemoveGold,
AddEquipment, RemoveEquipment, IncreasePop, DecreasePop,
RemoveFarmer, RemoveMerchant, RemoveBlacksmith, RemoveSoldier,
FarmerDie, MerchantDie, BlacksmithDie, SoldierDie,
IdleDie, AllStopWorking}

[CreateAssetMenu(fileName = "NewOutcome", menuName = "New Outcome")]
public class Outcome : ScriptableObject
{

    [SerializeField] public PrerequisiteEnum Prerequisite;
    [SerializeField] public int Minimum;

    [SerializeField] public OutcomeEnum OutcomeType;
    [SerializeField] public int OutcomeNumber;
    [SerializeField] public string OutcomeText;

}
