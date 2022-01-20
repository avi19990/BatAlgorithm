using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BatHuntState : IBatState
{
    private BatManager manager;

    private List<float> batsFitness;
    private int bestBatIndex;

    public BatStateType BatStateType => BatStateType.BatHuntState;

    public BatHuntState(BatManager manager)
    {
        Start(manager);
    }

    public void Start(BatManager manager)
    {
        this.manager = manager;

        batsFitness = new List<float>();
        for (int i = 0; i < manager.BatsCount; ++i)
            batsFitness.Add(Evaluation(manager.Bats[i].position));

        bestBatIndex = batsFitness.IndexOf(batsFitness.Min());
    }

    public bool Update()
    {
        Vector3 newSolution;
        for (int i = 0; i < manager.BatsCount; ++i)
        {
            manager.Bats[i].frequency = manager.FrequencyMin + (manager.FrequencyMax - manager.FrequencyMin) * Random.Range(0.0f, 1.0f);
            manager.Bats[i].velocity = (manager.Bats[bestBatIndex].position - manager.Bats[i].position) * manager.Bats[i].frequency;
            newSolution = manager.Bats[i].position + manager.Bats[i].velocity;

            if (Random.Range(0.0f, 1.0f) < manager.Bats[i].pulseRate)
            {
                float sumLoudness = 0.0f;
                for (int j = 0; j < manager.BatsCount; ++j)
                    sumLoudness += manager.Bats[j].loudness;
                float averageLoudness = sumLoudness / manager.BatsCount;

                Vector3 off = averageLoudness * Random.Range(-1.0f, 1.0f) * Random.onUnitSphere;
                newSolution += off;
            }

            newSolution = new Vector3(
                Mathf.Clamp(newSolution.x, manager.AreaMin, manager.AreaMax),
                Mathf.Clamp(newSolution.y, manager.AreaMin, manager.AreaMax),
                Mathf.Clamp(newSolution.z, manager.AreaMin, manager.AreaMax)
                );

            float evalValue = Evaluation(newSolution);
            if (evalValue < batsFitness[i])
            {
                batsFitness[i] = evalValue;
                manager.Bats[i].position = newSolution;

                manager.Bats[i].loudness = manager.Alpha * manager.Bats[i].loudness;
                manager.Bats[i].pulseRate = manager.PulseRateMax * (1 - Mathf.Exp(-manager.Gamma * manager.SimulationIterations));

                if (evalValue < batsFitness[bestBatIndex])
                    bestBatIndex = i;
            }
        }

        return false;
    }

    private float Evaluation(Vector3 position)
    {
        float bestEval = float.MaxValue;
        for (int i = 0; i < manager.Prey.Count; ++i)
        {
            float distance = Vector3.Distance(position, manager.Prey[i].position) / manager.PreyWeights[i] + 10.0f * (1.0f - manager.PreyWeights[i]);
            if (distance < bestEval)
                bestEval = distance;
        }

        return bestEval;
    }
}
