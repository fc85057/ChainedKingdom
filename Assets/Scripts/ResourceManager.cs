using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{

    public static event Action<int> OnFoodChanged = delegate { };
    public static event Action<int> OnGoldChanged = delegate { };
    public static event Action<int> OnEquipmentChanged = delegate { };

    [SerializeField] int workerFoodCost = 1;
    [SerializeField] float foodCalculationInterval = 5f;
    [SerializeField] float foodReductionInterval = 5f;
    [SerializeField] float workerProductionInterval = 1f;

    [SerializeField] int startingFood = 50;
    [SerializeField] int startingGold = 50;
    [SerializeField] int startingEquipment = 50;

    [SerializeField] int foodGenerated = 1;
    [SerializeField] int goldGenerated = 1;
    [SerializeField] int equipmentGenerated = 1;

    int population;
    int idle;
    int farmers;
    int merchants;
    int blacksmiths;
    int soldiers;

    int food;
    int gold;
    int equipment;

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

        // StartCoroutine(IncreaseFood());
        // StartCoroutine(DecreaseFood());
        StartCoroutine(ChangeFood());
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
    }

    #endregion

    #region Food

    // Should test this out before doing it for other resources

    IEnumerator ChangeFood()
    {
        while (true)
        {
            Debug.Log("Food before calculation: " + food.ToString());
            food += ((farmers * foodGenerated) - (population * workerFoodCost));
            OnFoodChanged(food);
            Debug.Log("Food after calculation: " + food.ToString());
            yield return new WaitForSeconds(foodCalculationInterval);
        }
    }

    // Not thread safe so this won't work

    IEnumerator IncreaseFood()
    {
        while (true)
        {
            food += (farmers * foodGenerated);
            OnFoodChanged(food);
            yield return new WaitForSeconds(workerProductionInterval);
        }
    }

    IEnumerator DecreaseFood()
    {
        while (true)
        {
            food -= (population * workerFoodCost);
            OnFoodChanged(food);
            yield return new WaitForSeconds(foodReductionInterval);
        }
    }

    #endregion

}
