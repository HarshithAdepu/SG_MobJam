using UnityEngine;

public class BlockController : MonoBehaviour
{
	[SerializeField] private float dragThreshold = 30f;
	
	private GridManager gridManager;
	private Vector3 startMousePos;
	private bool isDragging = false;
	private Vector3 mouseDelta;
	public void SetGridManager(GridManager manager)
	{
		gridManager = manager;
	}

	private void OnMouseDown()
	{
		startMousePos = Input.mousePosition;
		isDragging = true;
	}

	private void OnMouseUp()
	{
		isDragging = false;
	}

	private void Update()
	{
		if (!isDragging) return;

		Vector3 currentMousePos = Input.mousePosition;
		mouseDelta = currentMousePos - startMousePos;

		if (mouseDelta.magnitude > dragThreshold)
		{
			Vector2Int direction = Vector2Int.zero;
			if (Mathf.Abs(mouseDelta.x) > Mathf.Abs(mouseDelta.y))
			{
				direction = new Vector2Int(mouseDelta.x > 0 ? 1 : -1, 0);
			}
			else
			{
				direction = new Vector2Int(0, mouseDelta.y > 0 ? 1 : -1);
			}

			if (gridManager.TryMoveBlock(transform.parent.gameObject, direction))
			{
				startMousePos = Input.mousePosition;
			}
		}
	}
}