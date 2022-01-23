using System.Collections.Generic;
using UnityEngine;

public class BatManager : MonoBehaviour
{
    [Header("Bat cnofig")]
    [SerializeField]
    private int batsCount;
    [SerializeField]
    private Vector3 areaMin, areaMax;
    [SerializeField]
    private float batSpeed;
    [SerializeField]
    private float batWanderSpeed;

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
    private FireflyGenerator preyPrefab;
    [SerializeField]
    private int preyCount;

    private List<float> preyWeights;
    private List<FireflyGenerator> prey;

    [Header("Simulation config")]
    [SerializeField]
    private DayNightCycle dayNightCycle;

    [Header("Simulation data")]
    [SerializeField]
    private int simulationIterations = 0;

    public int BatsCount => batsCount;
    public Vector3 AreaMin => areaMin;
    public Vector3 AreaMax => areaMax;
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
    public List<FireflyGenerator> Prey => prey;
    public FireflyGenerator PreyPrefab => preyPrefab;
    public int PreyCount => preyCount;

    public int SimulationIterations => simulationIterations;

    private IBatState batState;
    private List<BatOutput> outputBats;
    private bool moveComplete = true;

    private void Awake()
    {
        AssignCallback();
    }

    private void Start()
    {
        bats = new List<Bat>();
        for (int i = 0; i < batsCount; ++i)
        {
            Bat tempBat = new Bat();

            tempBat.position = new Vector3();
            tempBat.velocity = new Vector3();
            tempBat.pulseRate = 0.0f;
            tempBat.loudness = 0.0f;
            tempBat.frequency = 0.0f;

            bats.Add(tempBat);
        }

        prey = new List<FireflyGenerator>();
        preyWeights = new List<float>();
        for (int i = 0; i < preyCount; ++i)
        {
            preyWeights.Add(0.0f);

            FireflyGenerator tempPrey = Instantiate(PreyPrefab, new Vector3(), Quaternion.identity, transform);
            tempPrey.gameObject.SetActive(false);
            prey.Add(tempPrey);
        }

        outputBats = new List<BatOutput>();
        for (int i = 0; i < BatsCount; ++i)
        {
            BatOutput tempBat = new BatOutput();
            tempBat.transform = Instantiate(BatPrefab, new Vector3(), Quaternion.identity, transform);
            tempBat.transform.gameObject.SetActive(false);

            tempBat.position = new Vector3();

            tempBat.wanderPoint = new Vector3();
            tempBat.wanderOffset = new Vector3();

            outputBats.Add(tempBat);
        }
    }

    private void Update()
    {
        if (batState == null)
            return;

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
            float maxWanderRadius;
            if (batState.BatStateType == BatStateType.BatDistributionState)
                maxWanderRadius = 5.0f;
            else if (batState.BatStateType == BatStateType.BatHuntState)
                maxWanderRadius = 5.0f;
            else
                maxWanderRadius = (50.0f / (1 + Mathf.Exp(-(Vector3.Distance(outputBats[i].position, batSpawnPoint.position) + 10.0f) / 25.0f))) - 25.0f;

            Vector3 oldPosition = outputBats[i].transform.position;

            outputBats[i].wanderOffset = Vector3.MoveTowards(outputBats[i].wanderOffset, outputBats[i].wanderPoint, batWanderSpeed * Time.deltaTime);
            if (outputBats[i].wanderOffset == outputBats[i].wanderPoint || outputBats[i].wanderPoint.magnitude > maxWanderRadius)
                outputBats[i].wanderPoint = Random.onUnitSphere * Random.Range(1.0f, maxWanderRadius);

            Vector3 newPosition = Vector3.MoveTowards(outputBats[i].position, bats[i].position, batSpeed * Time.deltaTime);
            Vector3 offset = newPosition + outputBats[i].wanderOffset;

            outputBats[i].position = newPosition;
            outputBats[i].transform.position = offset;
            outputBats[i].transform.rotation = Quaternion.LookRotation(oldPosition - offset);
            if (newPosition == bats[i].position)
                batsComplete += 1;
        }

        if (batsComplete >= batsCount / 2)
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
        }
    }

    private void dayEnd()
    {
        simulationIterations = 0;

        ChangeBatState(BatStateType.BatDistributionState);

        for (int i = 0; i < BatsCount; ++i)
        {
            outputBats[i].position = bats[i].position;
            outputBats[i].transform.gameObject.SetActive(true);
        }

        for (int i = 0; i < preyCount; ++i)
        {
            preyWeights[i] = Random.Range(0.0f, 1.0f);

            prey[i].transform.position = new Vector3(Random.Range(AreaMin.x, AreaMax.x), Random.Range(AreaMin.y, AreaMax.y), Random.Range(AreaMin.z, AreaMax.z));
            prey[i].GenerateFireflies((int)(preyWeights[i] * 50.0f));
            //prey[i].localScale = new Vector3(1.0f + preyWeights[i] * 20.0f, 1.0f + preyWeights[i] * 20.0f, 1.0f + preyWeights[i] * 20.0f);
            prey[i].gameObject.SetActive(true);
        }
    }

    private void dayStart()
    {
        ChangeBatState(BatStateType.BatReturnState);

        for (int i = 0; i < preyCount; ++i)
            prey[i].gameObject.SetActive(false);
    }

    private void AssignCallback()
    {
        dayNightCycle.dayStart += dayStart;
        dayNightCycle.dayEnd += dayEnd;
    }
}
