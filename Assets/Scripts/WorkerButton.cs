using System;
using UnityEngine;

public class WorkerButton : MonoBehaviour
{

    public static event Action<Worker, bool> OnWorkerChanged = delegate { };

    [SerializeField] Worker workerType;
    [SerializeField] bool isAdd;

    public void ChangeWorker()
    {
        OnWorkerChanged(workerType, isAdd);
    }

}
