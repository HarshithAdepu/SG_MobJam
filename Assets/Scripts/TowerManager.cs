using UnityEngine;
using System.Collections.Generic;

public class TowerManager : MonoBehaviour
{
	public float initialSpawnOffset = 0.75f;
	public float spawnOffset = 1f;
	public float zDistance = 20f;
	private Tower tower;
	private List<Guard> guards = new List<Guard>();
	public GameManager gameManager;
	
	public void InitializeTower(GameManager manager)
	{
		gameManager = manager;

		tower = Instantiate(gameManager.CurrentLevelData.towerPrefab, transform.position, Quaternion.identity).GetComponent<Tower>();
		tower.transform.position = new Vector3( Camera.main.transform.position.x, transform.position.y, zDistance );
		tower.InitializeTower( this );
		tower.SetHealth(gameManager.CurrentLevelData.towerHealth);

		Vector3 towerPos = tower.transform.position;
		float totalWidth = (gameManager.CurrentLevelData.guardGrid.x - 1) * spawnOffset;
		float totalHeight = (gameManager.CurrentLevelData.guardGrid.y - 1) * spawnOffset;
		Vector3 startPos = new Vector3(towerPos.x - totalWidth * 0.5f, 0f, towerPos.z - initialSpawnOffset );

		for (int x = 0; x < gameManager.CurrentLevelData.guardGrid.x; x++)
		{
			for (int z = 0; z < gameManager.CurrentLevelData.guardGrid.y; z++)
			{
				Vector3 spawnPos = startPos + new Vector3(x * spawnOffset, 0f, -z * spawnOffset);
				Guard guard = Instantiate(gameManager.CurrentLevelData.guardPrefab, spawnPos, Quaternion.identity).GetComponent<Guard>();
				guard.InitializeGuard( this );
				guard.tag = "Guard";
				guards.Add(guard);
			}
		}
	}

	public float GetTowerHealth()
	{
		return tower.GetHealth();
	}

	public float GetTowerMaxHealth()
	{
		return gameManager.CurrentLevelData.towerHealth;
	}
	
	public Guard FindAvailableGuard()
	{
		Guard availableGuard = guards.Find( x => x.isTargeted == false );
		if ( availableGuard != null )
		{
			availableGuard.isTargeted = true;
		}
		else if ( availableGuard == null && guards.Count > 0 )
		{
			availableGuard = guards[Random.Range( 0, guards.Count )];
		}
		else
		{
			return null;
		}

		return availableGuard;
	}
	
	public List<Guard> GetGuards()
	{
		return guards;
	}

	public void RemoveGuard( Guard guard )
	{
		guards.Remove(guard);
	}
	
	public Tower GetTower()
	{
		return tower;
	}
}