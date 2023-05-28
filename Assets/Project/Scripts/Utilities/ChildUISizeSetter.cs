using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildUISizeSetter : MonoBehaviour
{
	[SerializeField]
	private float minSize;
	[SerializeField]
	private float maxSize;

	[ContextMenu("SetSize")]
	public void SetSize()
	{
		int childCount = transform.childCount;

		for (int i = 0; i < childCount; i++)
		{
			float size = Mathf.Lerp(minSize, maxSize, ((float)i + 1) / childCount);

			var t = transform.GetChild(i) as RectTransform;
			t.sizeDelta = Vector2.one * size;
		}
	}
}
