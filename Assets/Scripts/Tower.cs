using DG.Tweening;
using UnityEngine;

public class Tower : MonoBehaviour
{
	private float health;
	private TowerManager towerManager;

	private bool isAnimating = false;
	
	public void InitializeTower(TowerManager manager)
	{
		towerManager = manager;
	}

	public void SetHealth(float initialHealth)
	{
		health = initialHealth;
	}

	public float GetHealth()
	{
		return health;
	}

	public void TakeDamage(float damage)
	{
		health -= damage;

		Sequence bounceSequence = DOTween.Sequence();
		
		if ( !isAnimating )
		{
			bounceSequence.Append( transform.DOScale(1.75f, 0.1f).SetEase(Ease.OutBack) );
			bounceSequence.Append( transform.DOScale(2.0f, 0.1f).SetEase(Ease.OutBack) );
			bounceSequence.Play();
			
			bounceSequence.OnComplete( () => isAnimating = false );
		}
		
		if (health <= 0)
		{
			towerManager.gameManager.CompleteLevel();
			Destroy(gameObject);
		}
	}
}