using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class NPCManager : MonoBehaviour
{
    public float spawnDelay = 0.2f;
    public float attackDelay = 1f;
    public float movementSpeed = 2f;
    public float damage = 5f;
    public float guardAttackDistance = 1f;
    public float towerAttackDistance = 3f;
    
    private List<NPC> npcs = new List<NPC>();

    private GameManager gameManager;
    
    public void InitializeNPCManager(GameManager manager)
    {
        gameManager = manager;
    }

    public void SpawnNPCs(int value, Vector3 pos)
    {
        StartCoroutine( SpawnNPCCoroutine( value, pos ) );
    }

    private IEnumerator SpawnNPCCoroutine(int value, Vector3 pos)
    {
        for (int i = 0; i < value; i++)
        {
            NPC npc = Instantiate(gameManager.CurrentLevelData.npcPrefab, pos, Quaternion.identity).GetComponent<NPC>();
            npc.attackDelay = attackDelay;
            npc.damage = damage;
            npc.npcDieOnKill = gameManager.CurrentLevelData.npcDieOnKill;
            npc.movementSpeed = movementSpeed;
            npc.guardAttackDistance = guardAttackDistance;
            npc.towerAttackDistance = towerAttackDistance;
            npc.InitializeNPC(this);
            npcs.Add(npc);

            yield return new WaitForSeconds( spawnDelay );
        }
    }

    public Guard FindAvailableGuard()
    {
        return gameManager.towerManager.FindAvailableGuard();
    }

    public Tower GetTower()
    {
        return gameManager.towerManager.GetTower();
    }

    public int GetNPCCount()
    {
        return npcs.Count;
    }

    public void RemoveNPC( NPC npc )
    {
        npcs.Remove( npc );
    }
}