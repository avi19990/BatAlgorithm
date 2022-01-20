using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatReturnState : IBatState
{
    private BatManager manager;

    public BatStateType BatStateType => BatStateType.BatReturnState;

    public BatReturnState(BatManager manager)
    {
        Start(manager);
    }

    public void Start(BatManager manager)
    {
        this.manager = manager;
    }

    public bool Update()
    {
        for (int i = 0; i < manager.BatsCount; ++i)
            manager.Bats[i].position = manager.BatSpawnPoint.position;

        return true;
    }
}
