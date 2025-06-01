using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

public class NPC : MonoBehaviour
{
	public GameObject target;
	public float attackDelay = 1f;
	public float damage = 5f;
	public bool npcDieOnKill = false;
	public float movementSpeed = 2f;
	public float towerAttackDistance = 3f;
	public float guardAttackDistance = 1f;

	private bool canAttack = true;
	private NPCManager npcManager;
	private NavMeshAgent agent;

	private bool isAnimating = true;
	
	public void InitializeNPC(NPCManager manager)
	{
		npcManager = manager;
		agent = GetComponent<NavMeshAgent>();
		
		transform.localScale = Vector3.zero;
		transform.DOScale( 0.25f, 0.2f ).SetEase( Ease.OutBack ).OnComplete( () => isAnimating = false );
	}

	private void Update()
	{
		if ( isAnimating )
		{
			return;
		}
		
		if ( target == null )
		{
			UpdateTarget();
		}
		
		if (target != null)
		{
			CheckAttackProximity();
		}
	}

	private void OnDestroy()
	{
		npcManager.RemoveNPC( this );
	}

	private void CheckAttackProximity()
	{
		if ( target.CompareTag( "Guard" ) )
		{
			if ( Vector3.Distance( transform.position, target.transform.position ) < guardAttackDistance )
			{
				if ( canAttack )
				{
					Destroy(target);
					target = null;
					
					if ( npcDieOnKill )
					{
						Destroy( gameObject );
						return;
					}
					UpdateTarget();

					StartCoroutine( AttackCoolDownCoroutine() );
				}
			}
		}
		else if ( target.CompareTag( "Tower" ) )
		{
			if ( Vector3.Distance( transform.position, target.transform.position ) < towerAttackDistance )
			{
				if ( canAttack )
				{
					Tower tower = target.GetComponent<Tower>();
					tower.TakeDamage(damage);
					
					StartCoroutine( AttackCoolDownCoroutine() );
				}
				
			}
		}
	}

	private void UpdateTarget()
	{
		if ( target != null )
		{
			return;
		}
		
		Guard foundGuard = npcManager.FindAvailableGuard();
		
		if (foundGuard != null)
		{
			target = foundGuard.gameObject;
		}
		else
		{
			Tower tower = npcManager.GetTower();
			if(tower != null)
			{
				target = npcManager.GetTower().gameObject;
			}
		}
		
		if(target != null)
		{
			agent.SetDestination(target.transform.position);
		}

		agent.speed = movementSpeed;
	}

	IEnumerator AttackCoolDownCoroutine()
	{
		canAttack = false;

		yield return new WaitForSeconds( attackDelay );

		canAttack = true;
	}
}