using System;
using System.Collections;
using UnityEngine;

public enum Worker { Farmer, Merchant, Blacksmith, Soldier}

public class WorkerManager : MonoBehaviour
{
    public static event Action<int> OnPopulationChanged = delegate { };
    public static event Action<int> OnIdleChanged = delegate { };

    public static event Action<int> OnFarmersChanged = delegate { };
    public static event Action<int> OnMerchantsChanged = delegate { };
    public static event Action<int> OnBlacksmithsChanged = delegate { };
    public static event Action<int> OnSoldiersChanged = delegate { };

    [SerializeField] int startingPopulation = 10;
    [SerializeField] int foodForGrowth = 100;
    [SerializeField] float increasePopulationInterval = 15f;
    [SerializeField] float decreasePopulationInterval = 5f;

    int population;
    int idle;
    int farmers;
    int merchants;
    int blacksmiths;
    int soldiers;

    int food;
    int gold;
    int equipment;
    bool populationChanging;

    private void Awake()
    {
        WorkerButton.OnWorkerChanged += HandleOnWorkerChanged;

        ResourceManager.OnHappinessChanged += SetPopulationGrowth; // happiness
        ResourceManager.OnFoodChanged += UpdateFood;
        ResourceManager.OnGoldChanged += UpdateGold;
        ResourceManager.OnEquipmentChanged += UpdateEquipment;
    }

    private void Start()
    {
        populationChanging = false;

        population = startingPopulation;
        idle = population;

        OnPopulationChanged(population);
        OnIdleChanged(idle);
        OnFarmersChanged(farmers);
        OnMerchantsChanged(merchants);
        OnBlacksmithsChanged(blacksmiths);
        OnSoldiersChanged(soldiers);
    }

    private void OnDestroy()
    {
        WorkerButton.OnWorkerChanged -= HandleOnWorkerChanged;

        ResourceManager.OnHappinessChanged -= SetPopulationGrowth; // happiness
        ResourceManager.OnFoodChanged -= UpdateFood;
        ResourceManager.OnGoldChanged -= UpdateGold;
        ResourceManager.OnEquipmentChanged -= UpdateEquipment;
    }

    #region Food
    /*
    private void UpdateFood(int amount)
    {
        food = amount;
        if (food < 1 && !populationChanging)
        {
            Debug.Log("Calling decreasing population");
            populationChanging = true;
            StopCoroutine(GraduallyIncreasePopulation());
            StartCoroutine(GraduallyDecreasePopulation());
        }
        else if (food > foodForGrowth && !populationChanging)
        {
            Debug.Log("Calling increasing population");
            populationChanging = true;
            StopCoroutine(GraduallyDecreasePopulation());
            StartCoroutine(GraduallyIncreasePopulation());

        }
        else if (food > 0 && food < 100 && populationChanging)
        {
            Debug.Log("Stopping all coroutines");
            populationChanging = false;
            StopAllCoroutines();
        }

    }
    */
    private void UpdateFood(int amount)
    {
        food = amount;
    }

        #endregion

    #region Gold

        void UpdateGold(int amount)
    {
        gold = amount;
        if (gold < 1)
        {
            RemoveBlacksmith(blacksmiths);
        }
    }

    #endregion

    #region Equipment

    void UpdateEquipment(int amount)
    {
        equipment = amount;
        if (equipment < 1)
        {
            RemoveSoldier(soldiers);
        }
    }

    #endregion

    #region Population

    // happiness
    void SetPopulationGrowth(Happiness happiness)
    {
        if (Time.time < 1f) return;

        if (happiness == Happiness.Happy && !populationChanging)
        {
            Debug.Log("Happy so increasing population");
            populationChanging = true;
            StopCoroutine(GraduallyDecreasePopulation());
            StartCoroutine(GraduallyIncreasePopulation());
        }
        else if (happiness == Happiness.Unhappy && !populationChanging)
        {
            Debug.Log("Unhappy so decreasing population");
            populationChanging = true;
            StopCoroutine(GraduallyIncreasePopulation());
            StartCoroutine(GraduallyDecreasePopulation());
        }
        else if (happiness == Happiness.Neutral && populationChanging)
        {
            Debug.Log("Happiness neutral, stopping population decline/growth.");
            populationChanging = false;
            StopAllCoroutines();
        }

        /* using siwtch statement for happiness does not consider changing population
        switch(happiness)
        {
            case (Happiness.Happy):
                Debug.Log("Happy so increasing population");
                populationChanging = true;
                StopCoroutine(GraduallyDecreasePopulation());
                StartCoroutine(GraduallyIncreasePopulation());
                break;
            case (Happiness.Unhappy):
                Debug.Log("Unhappy so decreasing population");
                populationChanging = true;
                StopCoroutine(GraduallyIncreasePopulation());
                StartCoroutine(GraduallyDecreasePopulation());
                break;
            case (Happiness.Neutral):
                Debug.Log("Happiness neutral, stopping population decline/growth.");
                populationChanging = false;
                StopAllCoroutines();
                break;
        }
        */
    }

    IEnumerator GraduallyIncreasePopulation()
    {
        while (true)
        {
            Debug.Log("Increasing population by 1");
            IncreasePopulation(1);
            yield return new WaitForSeconds(increasePopulationInterval);
        }
    }

    IEnumerator GraduallyDecreasePopulation()
    {
        while (true)
        {
            Debug.Log("Decreasing population by 1");
            DecreasePopulation(1);
            // AdjustWorkers(1);
            yield return new WaitForSeconds(decreasePopulationInterval);
        }
    }

    public void IncreasePopulation(int amount)
    {
        population += amount;
        idle += amount;
        OnPopulationChanged(population);
        OnIdleChanged(idle);
    }

    public void DecreasePopulation(int amount)
    {
        population -= amount;
        OnPopulationChanged(population);
        AdjustWorkers(amount);
    }

    void AdjustWorkers(int amount)
    {
        int workersToRemove = amount;
        while (workersToRemove != 0)
        {
            if (idle > 0)
            {
                idle--;
                workersToRemove--;
                OnIdleChanged(idle);
            }
            else if (soldiers > 0)
            {
                soldiers--;
                workersToRemove--;
                OnSoldiersChanged(soldiers);
            }
            else if (blacksmiths > 0)
            {
                blacksmiths--;
                workersToRemove--;
                OnBlacksmithsChanged(blacksmiths);
            }
            else if (merchants > 0)
            {
                merchants--;
                workersToRemove--;
                OnMerchantsChanged(merchants);
            }
            else if (farmers > 0)
            {
                farmers--;
                workersToRemove--;
                OnFarmersChanged(farmers);
            }

        }
    }

    #endregion

    #region HandleButton

    void HandleOnWorkerChanged(Worker workerType, bool isAdd)
    {

        if (isAdd)
        {
            if (idle < 1)
            {
                return;
            }

            switch (workerType)
            {
                case (Worker.Blacksmith):
                    {
                        AddBlacksmith(1);
                        break;
                    }
                case (Worker.Farmer):
                    {
                        AddFarmer(1);
                        break;
                    }
                case (Worker.Merchant):
                    {
                        AddMerchant(1);
                        break;
                    }
                case (Worker.Soldier):
                    {
                        AddSoldier(1);
                        break;
                    }
                default:
                    return;
            }

            // idle -= 1;
            // OnIdleChanged(idle);
        }
        else
        {
            switch (workerType)
            {
                case (Worker.Blacksmith):
                    {
                        RemoveBlacksmith(1);
                        break;
                    }
                case (Worker.Farmer):
                    {
                        RemoveFarmer(1);
                        break;
                    }
                case (Worker.Merchant):
                    {
                        RemoveMerchant(1);
                        break;
                    }
                case (Worker.Soldier):
                    {
                        RemoveSoldier(1);
                        break;
                    }
                default:
                    return;
            }

        }
    }

    #endregion

    #region Farmers

    public void AddFarmer(int amount)
    {
        farmers += amount;
        idle -= amount;
        OnFarmersChanged(farmers);
        OnIdleChanged(idle);
    }

    public void RemoveFarmer(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            if (farmers != 0)
            {
                farmers--;
                idle++;
            }
        }
        OnFarmersChanged(farmers);
        OnIdleChanged(idle);
    }

    #endregion

    #region Merchants

    public void AddMerchant(int amount)
    {
        merchants += amount;
        idle -= amount;
        OnMerchantsChanged(merchants);
        OnIdleChanged(idle);
    }

    public void RemoveMerchant(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            if (merchants != 0)
            {
                merchants--;
                idle++;
            }
        }
        OnMerchantsChanged(merchants);
        OnIdleChanged(idle);
    }

    #endregion

    #region Blacksmiths

    public void AddBlacksmith(int amount)
    {
        if (gold < 1)
            return;
        blacksmiths += amount;
        idle -= amount;
        OnBlacksmithsChanged(blacksmiths);
        OnIdleChanged(idle);
    }

    public void RemoveBlacksmith(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            if (blacksmiths != 0)
            {
                blacksmiths--;
                idle++;
            }
        }
        OnBlacksmithsChanged(blacksmiths);
        OnIdleChanged(idle);
    }

    #endregion

    #region Soldiers

    public void AddSoldier(int amount)
    {
        if (equipment < 1)
            return;
        soldiers += amount;
        idle -= amount;
        OnSoldiersChanged(soldiers);
        OnIdleChanged(idle);
    }

    public void RemoveSoldier(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            if (soldiers != 0)
            {
                soldiers--;
                idle++;
            }
        }
        OnSoldiersChanged(soldiers);
        OnIdleChanged(idle);
    }

    #endregion

}
