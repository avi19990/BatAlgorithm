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
            Vector3 randPosition = manager.BatSpawnPoint.position + Random.onUnitSphere * manager.BatSpawnRadius;

            manager.Bats[i].position = randPosition;
            manager.Bats[i].velocity = new Vector3();
            manager.Bats[i].pulseRate = 0.0f;
            manager.Bats[i].loudness = manager.LoudnessMax;
            manager.Bats[i].frequency = 0.0f;
        }
    }

    public bool Update()
    {
        for (int i = 0; i < manager.BatsCount; ++i)
            manager.Bats[i].position = new Vector3(Random.Range(manager.AreaMin.x, manager.AreaMax.x), Random.Range(manager.AreaMin.y, manager.AreaMax.y), Random.Range(manager.AreaMin.z, manager.AreaMax.z));

        return true;
    }
}
