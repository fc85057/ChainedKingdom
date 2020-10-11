using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PrerequisiteEnum { MinFood, MinGold, MinEquipment, MinPopulation,
MinFarmer, MinMerchant, MinBlacksmith, MinSoldier, MinIdle}

[CreateAssetMenu(fileName = "NewScenario", menuName = "New Scenario")]
public class Scenario : ScriptableObject
{

    [SerializeField] public PrerequisiteEnum Prerequisite;
    [SerializeField] public int Minimum;

    [SerializeField] public string ScenarioText;
    [SerializeField] public string ChoiceOneText;
    [SerializeField] public string ChoiceTwoText;

    [SerializeField] public Outcome[] ChoiceOneOutcomes;
    [SerializeField] public Outcome[] ChoiceTwoOutcomes;

}
