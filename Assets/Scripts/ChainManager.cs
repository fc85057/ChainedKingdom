using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChainManager : MonoBehaviour
{

    [SerializeField] GameObject choicePanel;
    [SerializeField] GameObject outcomePanel;

    [SerializeField] Scenario[] scenarios;
    [SerializeField] Text scenarioText;
    [SerializeField] Text choiceOneText;
    [SerializeField] Text choiceTwoText;

    [SerializeField] int chanceOfWar = 40;
    [SerializeField] Scenario warScenario;
    [SerializeField] Text outcomeText;

    Scenario currentScenario;
    Outcome currentOutcome;

    ResourceManager resourceManager;
    WorkerManager workerManager;

    int population;
    int idle;
    int farmers;
    int merchants;
    int blacksmiths;
    int soldiers;

    int year;
    int food;
    int gold;
    int equipment;

    #region Setup

    private void Awake()
    {
        GameManager.OnYearChanged += HandleOnYearChanged;

        WorkerManager.OnPopulationChanged += UpdatePopulation;
        WorkerManager.OnIdleChanged += UpdateIdle;
        WorkerManager.OnFarmersChanged += UpdateFarmers;
        WorkerManager.OnMerchantsChanged += UpdateMerchants;
        WorkerManager.OnBlacksmithsChanged += UpdateBlacksmiths;
        WorkerManager.OnSoldiersChanged += UpdateSoldiers;

        ResourceManager.OnFoodChanged += UpdateFood;
        ResourceManager.OnGoldChanged += UpdateGold;
        ResourceManager.OnEquipmentChanged += UpdateEquipment;
    }

    private void Start()
    {
        resourceManager = GetComponent<ResourceManager>();
        workerManager = GetComponent<WorkerManager>();
    }

    private void OnDestroy()
    {
        GameManager.OnYearChanged -= HandleOnYearChanged;

        WorkerManager.OnPopulationChanged -= UpdatePopulation;
        WorkerManager.OnIdleChanged -= UpdateIdle;
        WorkerManager.OnFarmersChanged -= UpdateFarmers;
        WorkerManager.OnMerchantsChanged -= UpdateMerchants;
        WorkerManager.OnBlacksmithsChanged -= UpdateBlacksmiths;
        WorkerManager.OnSoldiersChanged -= UpdateSoldiers;

        ResourceManager.OnFoodChanged -= UpdateFood;
        ResourceManager.OnGoldChanged -= UpdateGold;
        ResourceManager.OnEquipmentChanged -= UpdateEquipment;
    }

    #endregion

    #region UpdateValues

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

    void UpdateFood(int amount)
    {
        food = amount;
    }

    void UpdateGold(int amount)
    {
        gold = amount;
    }

    void UpdateEquipment(int amount)
    {
        equipment = amount;
    }

    void UpdateYear(int currentYear)
    {
        year = currentYear;
    }

    #endregion

    #region ChooseScenarios

    void HandleOnYearChanged(int year)
    {
        // if (year == 1) return;
        UpdateYear(year);
        currentScenario = DrawScenario();
        HandleScenario();
    }

    Scenario DrawScenario()
    {
        int chance = Random.Range(0, 100);
        if (chance < chanceOfWar)
        {
            currentScenario = warScenario;
        }
        else
        {
            currentScenario = scenarios[Random.Range(0, scenarios.Length)];
            bool prerequisiteMet = false;
            prerequisiteMet = CheckPrerequisite(currentScenario.Prerequisite, currentScenario.Minimum);
            if (!prerequisiteMet)
            {
                DrawScenario();
            }
        }
        return currentScenario;
    }

    bool CheckPrerequisite(PrerequisiteEnum prequisite, int minimum)
    {
        switch (prequisite)
        {
            case (PrerequisiteEnum.MinBlacksmith):
                if (blacksmiths >= minimum)
                    return true;
                break;
            case (PrerequisiteEnum.MinEquipment):
                if (equipment >= minimum)
                    return true;
                break;
            case (PrerequisiteEnum.MinFarmer):
                if (farmers >= minimum)
                    return true;
                break;
            case (PrerequisiteEnum.MinFood):
                if (food >= minimum)
                    return true;
                break;
            case (PrerequisiteEnum.MinGold):
                if (gold >= minimum)
                    return true;
                break;
            case (PrerequisiteEnum.MinIdle):
                if (idle >= minimum)
                    return true;
                break;
            case (PrerequisiteEnum.MinMerchant):
                if (merchants >= minimum)
                    return true;
                break;
            case (PrerequisiteEnum.MinPopulation):
                if (population >= minimum)
                    return true;
                break;
            case (PrerequisiteEnum.MinSoldier):
                if (soldiers >= minimum)
                    return true;
                break;
            default:
                return false;
        }
        return false;
    }

    #endregion

    #region PlayScenarios

    void HandleScenario()
    {
        scenarioText.text = currentScenario.ScenarioText;
        choiceOneText.text = currentScenario.ChoiceOneText;
        choiceTwoText.text = currentScenario.ChoiceTwoText;

        choicePanel.SetActive(true);
        Time.timeScale = 0;
    }

    #endregion

    #region War

    void HandleWar(int choice)
    {
        if (choice == 1)
        {
            Fight();
        }
        else
        {
            DontFight();
        }
    }

    void Fight()
    {
        if (soldiers <= 0)
        {
            LoseAllResources();
            return;
        }

        int enemySoldiers = (int)(year / 2.0f + 1);
        Debug.Log("The number of enemy soldiers is " + enemySoldiers.ToString());
        if (soldiers >= enemySoldiers)
        {
            WinWar();
        }
        else
        {
            LoseWar();
        }
    }

    void LoseAllResources()
    {
        outcomeText.text = "You decided to fight.\n" +
            "It was the shortest battle in history since...we don't have an army.\n" +
            "The enemy storms their way throughout the kingdom. " +
            "They take everything that isn't nailed down.\n" +
            "Actually, even some of the stuff that was nailed down.";
        choicePanel.SetActive(false);
        outcomePanel.SetActive(true);
        resourceManager.RemoveGold(100);
        resourceManager.RemoveEquipment(100);
    }

    void WinWar()
    {
        outcomeText.text = "You decided to fight.\n" +
            "Our soldiers put up a valiant effort." +
            "The enemy is no match for our army." +
            "In other words, we kicked their ass." +
            "We live to fight another day.";
        choicePanel.SetActive(false);
        outcomePanel.SetActive(true);
    }

    void LoseWar()
    {
        int soldiersLost = Random.Range(1, soldiers);
        outcomeText.text = "You decided to fight.\n" +
            "Our soldiers fought as hard as they could." +
            "Alas, the enemy was just too strong." +
            "In other words, we got our ass handed to us." +
            soldiersLost + " of our soldiers died.";
        workerManager.RemoveSoldier(soldiersLost);
        workerManager.DecreasePopulation(soldiersLost);
        choicePanel.SetActive(false);
        outcomePanel.SetActive(true);
    }

    // Should handle all no fighting scenarios
    // Need to add all don't fight options to choice two

    void DontFight()
    {
        DrawOutcome(2);
    }

    #endregion

    #region Outcomes

    public void HandleOutcome(int choice)
    {
        if (currentScenario == warScenario)
        {
            HandleWar(choice);
        }
        else if (choice == 1)
        {
            DrawOutcome(1);
        }
        else
        {
            DrawOutcome(2);
        }
    }

    void DrawOutcome(int choice)
    {
        bool prerequisiteMet = false;

        if (choice == 1)
        {
            currentOutcome = currentScenario.ChoiceOneOutcomes[Random.Range(0, currentScenario.ChoiceOneOutcomes.Length)];
        }
        else
        {
            currentOutcome = currentScenario.ChoiceTwoOutcomes[Random.Range(0, currentScenario.ChoiceTwoOutcomes.Length)];
        }

        prerequisiteMet = CheckPrerequisite(currentOutcome.Prerequisite, currentOutcome.Minimum);
        if (!prerequisiteMet)
        {
            DrawOutcome(choice);
        }
        else
        {
            outcomeText.text = currentOutcome.OutcomeText;
            choicePanel.SetActive(false);
            outcomePanel.SetActive(true);
            ResolveOutcome();
        }

    }

    void ResolveOutcome()
    {
        switch (currentOutcome.OutcomeType)
        {
            case (OutcomeEnum.AddEquipment):
                resourceManager.AddEquipment(currentOutcome.OutcomeNumber);
                break;
            case (OutcomeEnum.AddFood):
                resourceManager.AddFood(currentOutcome.OutcomeNumber);
                break;
            case (OutcomeEnum.AddGold):
                resourceManager.AddGold(currentOutcome.OutcomeNumber);
                break;
            case (OutcomeEnum.AllStopWorking):
                workerManager.RemoveFarmer(farmers);
                workerManager.RemoveMerchant(merchants);
                workerManager.RemoveBlacksmith(blacksmiths);
                workerManager.RemoveSoldier(soldiers);
                break;
            case (OutcomeEnum.BlacksmithDie):
                workerManager.RemoveBlacksmith(currentOutcome.OutcomeNumber);
                workerManager.DecreasePopulation(currentOutcome.OutcomeNumber);
                break;
            case (OutcomeEnum.DecreasePop):
                workerManager.DecreasePopulation(currentOutcome.OutcomeNumber);
                break;
            case (OutcomeEnum.FarmerDie):
                workerManager.RemoveFarmer(currentOutcome.OutcomeNumber);
                workerManager.DecreasePopulation(currentOutcome.OutcomeNumber);
                break;
            case (OutcomeEnum.IdleDie):
                workerManager.DecreasePopulation(idle);
                break;
            case (OutcomeEnum.IncreasePop):
                workerManager.IncreasePopulation(currentOutcome.OutcomeNumber);
                break;
            case (OutcomeEnum.MerchantDie):
                workerManager.RemoveMerchant(currentOutcome.OutcomeNumber);
                workerManager.DecreasePopulation(currentOutcome.OutcomeNumber);
                break;
            case (OutcomeEnum.RemoveBlacksmith):
                workerManager.RemoveBlacksmith(currentOutcome.OutcomeNumber);
                workerManager.DecreasePopulation(currentOutcome.OutcomeNumber);
                break;
            case (OutcomeEnum.RemoveEquipment):
                resourceManager.RemoveEquipment(currentOutcome.OutcomeNumber);
                break;
            case (OutcomeEnum.RemoveFarmer):
                workerManager.RemoveFarmer(currentOutcome.OutcomeNumber);
                break;
            case (OutcomeEnum.RemoveFood):
                resourceManager.RemoveFood(currentOutcome.OutcomeNumber);
                break;
            case (OutcomeEnum.RemoveGold):
                resourceManager.RemoveGold(currentOutcome.OutcomeNumber);
                break;
            case (OutcomeEnum.RemoveMerchant):
                workerManager.RemoveMerchant(currentOutcome.OutcomeNumber);
                break;
            case (OutcomeEnum.RemoveSoldier):
                workerManager.RemoveSoldier(currentOutcome.OutcomeNumber);
                break;
            case (OutcomeEnum.SoldierDie):
                workerManager.RemoveSoldier(currentOutcome.OutcomeNumber);
                workerManager.DecreasePopulation(currentOutcome.OutcomeNumber);
                break;
            default:
                return;
        }

    }

    public void ConcludeScenario()
    {
        Time.timeScale = 1f;
        outcomePanel.SetActive(false);
    }

    #endregion



}
