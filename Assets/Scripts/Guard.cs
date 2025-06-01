using System;
using UnityEngine;

public class Guard : MonoBehaviour
{
    public bool isTargeted = false;

    private TowerManager towerManager;
    public void InitializeGuard(TowerManager manager)
    {
        towerManager = manager;
    }

    private void OnDestroy()
    {
        towerManager.RemoveGuard( this );
    }
}
