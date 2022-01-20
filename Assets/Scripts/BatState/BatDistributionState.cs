using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatDistributionState : IBatState
{
    private BatManager manager;

    public BatStateType BatStateType => BatStateType.BatDistributionState;

    public BatDistributionState(BatManager manager)
    {
        Start(manager);
    }

    public void Start(BatManager manager)
    {
        this.manager = manager;

        for (int i = 0; i < manager.BatsCount; ++i)
        {
            Bat tempBat = new Bat();

            Vector3 randPosition = manager.BatSpawnPoint.position + Random.onUnitSphere * manager.BatSpawnRadius;

            tempBat.position = randPosition;
            tempBat.velocity = new Vector3();
            tempBat.pulseRate = 0.0f;
            tempBat.loudness = manager.LoudnessMax;
            tempBat.frequency = 0.0f;

            manager.Bats.Add(tempBat);
        }
    }

    public bool Update()
    {
        for (int i = 0; i < manager.BatsCount; ++i)
            manager.Bats[i].position = new Vector3(Random.Range(manager.AreaMin, manager.AreaMax), Random.Range(manager.AreaMin, manager.AreaMax), Random.Range(manager.AreaMin, manager.AreaMax));

        return true;
    }
}
