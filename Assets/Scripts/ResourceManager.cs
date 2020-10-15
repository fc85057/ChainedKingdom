using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// happiness
public enum Happiness { Happy, Neutral, Unhappy }
public class ResourceManager : MonoBehaviour
{

    public static event Action<Happiness> OnHappinessChanged = delegate { }; // happiness
    public static event Action<int> OnFoodChanged = delegate { };
    public static event Action<int> OnGoldChanged = delegate { };
    public static event Action<int> OnEquipmentChanged = delegate { };

    [SerializeField] int workerFoodCost = 1;
    [SerializeField] int blacksmithGoldCost = 1;
    [SerializeField] int soldierEquipmentCost = 1;

    [SerializeField] float foodCalculationInterval = 5f;
    [SerializeField] float goldCalculationInterval = 5f;
    [SerializeField] float equipmentCalculationInterval = 5f;

    [SerializeField] int startingFood = 50;
    [SerializeField] int startingGold = 50;
    [SerializeField] int startingEquipment = 50;

    [SerializeField] int foodGenerated = 1;
    [SerializeField] int goldGenerated = 1;
    [SerializeField] int equipmentGenerated = 1;

    [SerializeField] int foodForGrowth = 50;
    [SerializeField] int soldiersForGrowth = 10;

    int population;
    int idle;
    int farmers;
    int merchants;
    int blacksmiths;
    int soldiers;

    int food;
    int gold;
    int equipment;

    Happiness happiness;

    private void Awake()
    {
        WorkerManager.OnPopulationChanged += UpdatePopulation;
        WorkerManager.OnIdleChanged += UpdateIdle;
        WorkerManager.OnFarmersChanged += UpdateFarmers;
        WorkerManager.OnMerchantsChanged += UpdateMerchants;
        WorkerManager.OnBlacksmithsChanged += UpdateBlacksmiths;
        WorkerManager.OnSoldiersChanged += UpdateSoldiers;
    }

    private void OnDestroy()
    {
        WorkerManager.OnPopulationChanged -= UpdatePopulation;
        WorkerManager.OnIdleChanged -= UpdateIdle;
        WorkerManager.OnFarmersChanged -= UpdateFarmers;
        WorkerManager.OnMerchantsChanged -= UpdateMerchants;
        WorkerManager.OnBlacksmithsChanged -= UpdateBlacksmiths;
        WorkerManager.OnSoldiersChanged -= UpdateSoldiers;
    }

    private void Start()
    {
        food = startingFood;
        gold = startingGold;
        equipment = startingEquipment;
        OnFoodChanged(food);
        OnGoldChanged(gold);
        OnEquipmentChanged(equipment);

        StartCoroutine(ChangeFood());
        StartCoroutine(ChangeGold());
        StartCoroutine(ChangeEquipment());
    }

    #region UpdateWorkers

    void UpdatePopulation(int amount)
    {
        population = amount;
    }

    void UpdateIdle(int amount)
    {
        idle = amount;
    }

    void UpdateFarmers(int amount)
    {
        farmers = amount;
    }

    void UpdateMerchants(int amount)
    {
        merchants = amount;
    }

    void UpdateBlacksmiths(int amount)
    {
        blacksmiths = amount;
    }

    void UpdateSoldiers(int amount)
    {
        soldiers = amount;
        CalculateHappiness(); // happiness
    }

    #endregion

    #region Happiness

    void CalculateHappiness()
    {
        int soldiersPercentage;
        if (population == 0)
        {
            soldiersPercentage = 0;
        }
        else
        {
            soldiersPercentage = ((soldiers * 100) / population);
        }
        Debug.Log("Soldiers percentage is " + soldiersPercentage);
        if ((soldiersPercentage >= soldiersForGrowth) && (food >= foodForGrowth))
        {
            happiness = Happiness.Happy;
            OnHappinessChanged(happiness);
        }
        else if ((soldiersPercentage < soldiersForGrowth) && (food < foodForGrowth))
        {
            happiness = Happiness.Unhappy;
            OnHappinessChanged(happiness);
        }
        else
        {
            happiness = Happiness.Neutral;
            OnHappinessChanged(happiness);
        }
    }

    #endregion

    #region Food

    IEnumerator ChangeFood()
    {
        while (true)
        {
            food += ((farmers * foodGenerated) - (population * workerFoodCost));
            if (food < 0)
            {
                food = 0;
            }
            OnFoodChanged(food);
            CalculateHappiness(); // happiness
            yield return new WaitForSeconds(foodCalculationInterval);
        }
    }

    public void AddFood(int amount)
    {
        food += amount;
        OnFoodChanged(food);
        // need to see if thread safety is an issue
    }

    public void RemoveFood(int amount)
    {
        food -= amount;
        if (food < 0)
            food = 0;
        OnFoodChanged(food);
    }

    #endregion

    #region Gold

    IEnumerator ChangeGold()
    {
        while (true)
        {
            gold += ((merchants * goldGenerated) - (blacksmiths * blacksmithGoldCost));
            if (gold < 0)
            {
                gold = 0;
            }
            OnGoldChanged(gold);
            yield return new WaitForSeconds(goldCalculationInterval);
        }
    }

    public void AddGold(int amount)
    {
        gold += amount;
        OnGoldChanged(gold);
        // thread safety
    }

    public void RemoveGold(int amount)
    {
        gold -= amount;
        if (gold < 0)
            gold = 0;
        OnGoldChanged(gold);
        // thread safety
    }

    #endregion

    #region Equipment

    IEnumerator ChangeEquipment()
    {
        while (true)
        {
            equipment += ((blacksmiths * equipmentGenerated) - (soldiers * soldierEquipmentCost));
            if (equipment < 0)
            {
                equipment = 0;
            }
            OnEquipmentChanged(equipment);
            yield return new WaitForSeconds(equipmentCalculationInterval);
        }

    }

    public void AddEquipment(int amount)
    {
        equipment += amount;
        OnEquipmentChanged(equipment);
        // thread safety
    }

    public void RemoveEquipment(int amount)
    {
        equipment -= amount;
        if (equipment < 0)
            equipment = 0;
        OnEquipmentChanged(equipment);
        // thread safety
    }

    #endregion

}
