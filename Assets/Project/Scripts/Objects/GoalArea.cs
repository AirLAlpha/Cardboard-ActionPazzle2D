using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalArea : MonoBehaviour
{
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if(collision.transform.TryGetComponent<CardboardBox>(out CardboardBox box))
		{
			if (box.IsPacked)
				StageManager.Instance.CompleteBoxCount++;

				Destroy(box.gameObject);
		}
	}
}
