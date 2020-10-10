using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    [SerializeField] Text foodText;
    [SerializeField] Text goldText;
    [SerializeField] Text equipmentText;

    [SerializeField] Text popText;
    [SerializeField] Text idleText;

    [SerializeField] Text farmerText;
    [SerializeField] Text merchantText;
    [SerializeField] Text blacksmithText;
    [SerializeField] Text soldierText;

    private void Awake()
    {
        WorkerManager.OnPopulationChanged += ChangePopText;
        WorkerManager.OnIdleChanged += ChangeIdleText;

        WorkerManager.OnFarmersChanged += ChangeFarmerText;
        WorkerManager.OnMerchantsChanged += ChangeMerchantText;
        WorkerManager.OnBlacksmithsChanged += ChangeBlacksmithText;
        WorkerManager.OnSoldiersChanged += ChangeSoldierText;

        ResourceManager.OnFoodChanged += ChangeFoodText;
        ResourceManager.OnGoldChanged += ChangeGoldText;
        ResourceManager.OnEquipmentChanged += ChangeEquipmentText;
    }

    private void OnDestroy()
    {
        WorkerManager.OnPopulationChanged -= ChangePopText;
        WorkerManager.OnIdleChanged -= ChangeIdleText;

        WorkerManager.OnFarmersChanged -= ChangeFarmerText;
        WorkerManager.OnMerchantsChanged -= ChangeMerchantText;
        WorkerManager.OnBlacksmithsChanged -= ChangeBlacksmithText;
        WorkerManager.OnSoldiersChanged -= ChangeSoldierText;

        ResourceManager.OnFoodChanged -= ChangeFoodText;
        ResourceManager.OnGoldChanged -= ChangeGoldText;
        ResourceManager.OnEquipmentChanged -= ChangeEquipmentText;
    }

    void ChangeFoodText(int amount)
    {
        foodText.text = amount.ToString();
    }

    void ChangeGoldText(int amount)
    {
        goldText.text = amount.ToString();
    }

    void ChangeEquipmentText(int amount)
    {
        equipmentText.text = amount.ToString();
    }

    void ChangePopText(int amount)
    {
        popText.text = "Population: " + amount.ToString();
    }

    void ChangeIdleText(int amount)
    {
        idleText.text = "Idle: " + amount.ToString();
    }

    void ChangeFarmerText(int amount)
    {
        farmerText.text = amount.ToString();
    }

    void ChangeMerchantText(int amount)
    {
        merchantText.text = amount.ToString();
    }
    
    void ChangeBlacksmithText(int amount)
    {
        blacksmithText.text = amount.ToString();
    }

    void ChangeSoldierText(int amount)
    {
        soldierText.text = amount.ToString();
    }

}
