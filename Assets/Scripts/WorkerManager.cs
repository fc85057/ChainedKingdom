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
    [SerializeField] float increasePopulationInterval = 15f;
    [SerializeField] float decreasePopulationInterval = 5f;

    int population;
    int idle;
    int farmers;
    int merchants;
    int blacksmiths;
    int soldiers;

    int food;
    bool populationChanging;

    private void Awake()
    {
        WorkerButton.OnWorkerChanged += HandleOnWorkerChanged;
        ResourceManager.OnFoodChanged += UpdateFood;
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
        ResourceManager.OnFoodChanged -= UpdateFood;
    }

    #region Food

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
        else if (food > 100 && !populationChanging)
        {
            Debug.Log("Calling increasing population");
            populationChanging = true;
            StopCoroutine(GraduallyDecreasePopulation());
            StartCoroutine(GraduallyIncreasePopulation());

        }
        else if (food > 1 && food < 100 && populationChanging) // need to stop this from resetting
        {
            Debug.Log("Stopping all coroutines");
            populationChanging = false;
            StopCoroutine(GraduallyIncreasePopulation());
            StopCoroutine(GraduallyDecreasePopulation());
        }

    }

    #endregion

    #region Population

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
            yield return new WaitForSeconds(decreasePopulationInterval);
        }
    }

    void IncreasePopulation(int amount)
    {
        population += amount;
        OnPopulationChanged(population);
    }

    void DecreasePopulation(int amount)
    {
        population -= amount;
        OnPopulationChanged(population);
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

            idle -= 1;

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

        OnIdleChanged(idle);
    }

    #endregion

    #region Farmers

    public void AddFarmer(int amount)
    {
        farmers += amount;
        OnFarmersChanged(farmers);
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
    }

    #endregion

    #region Merchants

    public void AddMerchant(int amount)
    {
        merchants += amount;
        OnMerchantsChanged(merchants);
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
    }

    #endregion

    #region Blacksmiths

    public void AddBlacksmith(int amount)
    {
        blacksmiths += amount;
        OnBlacksmithsChanged(blacksmiths);
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
    }

    #endregion

    #region Soldiers

    public void AddSoldier(int amount)
    {
        soldiers += amount;
        OnSoldiersChanged(soldiers);
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
    }

    #endregion

}
