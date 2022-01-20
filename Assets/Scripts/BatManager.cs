using System.Collections.Generic;
using UnityEngine;

public class BatManager : MonoBehaviour
{
    [Header("Bat cnofig")]
    [SerializeField]
    private int batsCount;
    [SerializeField]
    private float areaMin, areaMax;
    [SerializeField]
    private float batSpeed;

    [SerializeField]
    private float frequencyMin, frequencyMax;
    [SerializeField]
    private float pulseRateMax;
    [SerializeField]
    private float loudnessMax;
    [SerializeField]
    private float alpha, gamma;

    private List<Bat> bats;

    [Header("Spawn config")]
    [SerializeField]
    private float batSpawnRadius;
    [SerializeField]
    private Transform batSpawnPoint;
    [SerializeField]
    private Transform batPrefab;

    [Header("Prey config")]
    [SerializeField]
    private Transform preyPrefab;
    [SerializeField]
    private int preyCount;

    private List<float> preyWeights;
    private List<Transform> prey;

    [Header("Simulation config")]
    [SerializeField]
    private DayNightCycle dayNightCycle;

    [Header("Simulation data")]
    [SerializeField]
    private int simulationIterations = 0;

    public int BatsCount => batsCount;
    public float AreaMin => areaMin;
    public float AreaMax => areaMax;
    public float BatSpeed => batSpeed;

    public float FrequencyMin => frequencyMin;
    public float FrequencyMax => frequencyMax;
    public float PulseRateMax => pulseRateMax;
    public float LoudnessMax => loudnessMax;
    public float Alpha => alpha;
    public float Gamma => gamma;

    public List<Bat> Bats => bats;

    public float BatSpawnRadius => batSpawnRadius;
    public Transform BatSpawnPoint => batSpawnPoint;
    public Transform BatPrefab => batPrefab;

    public List<float> PreyWeights => preyWeights;
    public List<Transform> Prey => prey;
    public Transform PreyPrefab => preyPrefab;
    public int PreyCount => preyCount;

    public int SimulationIterations => simulationIterations;

    private IBatState batState;
    private List<Transform> outputBats;
    private bool moveComplete = true;

    private void Awake()
    {
        AssignCallback();
    }

    private void Start()
    {
        bats = new List<Bat>();
        ChangeBatState(BatStateType.BatDistributionState);

        prey = new List<Transform>();
        preyWeights = new List<float>();
        for (int i = 0; i < preyCount; ++i)
        {
            preyWeights.Add(Random.Range(0.0f, 1.0f));

            Transform tempTransform = Instantiate(PreyPrefab, new Vector3(Random.Range(AreaMin, AreaMax), Random.Range(AreaMin, AreaMax), Random.Range(AreaMin, AreaMax)), Quaternion.identity, transform);
            tempTransform.localScale = new Vector3(1.0f + preyWeights[i] * 5.0f, 1.0f + preyWeights[i] * 5.0f, 1.0f + preyWeights[i] * 5.0f);
            prey.Add(tempTransform);
        }

        outputBats = new List<Transform>();
        for (int i = 0; i < BatsCount; ++i)
            outputBats.Add(Instantiate(BatPrefab, Bats[i].position, Quaternion.identity, transform));
    }

    private void Update()
    {
        if (moveComplete)
        {
            if (batState.Update())
                SetNextBatState();

            simulationIterations += 1;
            moveComplete = false;
        }

        int batsComplete = 0;
        for (int i = 0; i < BatsCount; ++i)
        {
            Vector3 offset = Vector3.MoveTowards(outputBats[i].position, Bats[i].position, BatSpeed * Time.deltaTime);
            outputBats[i].position = offset;

            if (offset == Bats[i].position)
                batsComplete += 1;
        }

        if (batsComplete >= BatsCount / 2)
            moveComplete = true;
    }

    private void ChangeBatState(BatStateType stateType)
    {
        switch (stateType)
        {
            case BatStateType.BatDistributionState:
                batState = new BatDistributionState(this);
                Debug.Log("DistributionState");
                break;
            case BatStateType.BatHuntState:
                batState = new BatHuntState(this);
                Debug.Log("HuntState");
                break;
            case BatStateType.BatReturnState:
                batState = new BatReturnState(this);
                Debug.Log("ReturnState");
                break;
        }
    }

    private void SetNextBatState()
    {
        switch (batState.BatStateType)
        {
            case BatStateType.BatDistributionState:
                ChangeBatState(BatStateType.BatHuntState);
                break;
            case BatStateType.BatHuntState:
                ChangeBatState(BatStateType.BatReturnState);
                break;
        }
    }

    private void dayEnd()
    {
        ChangeBatState(BatStateType.BatDistributionState);
    }

    private void dayStart()
    {
        ChangeBatState(BatStateType.BatReturnState);
    }

    private void AssignCallback()
    {
        dayNightCycle.dayStart += dayStart;
        dayNightCycle.dayEnd += dayEnd;
    }
}
