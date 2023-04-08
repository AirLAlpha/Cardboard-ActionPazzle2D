using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class StampColorChanger : MonoBehaviour
{
	[SerializeField]
	private SpriteRenderer targetRender;

	private SpriteRenderer render;


	private void OnEnable()
	{
		render = GetComponent<SpriteRenderer>();
	}

	//	更新処理
	private void Update()
	{
		if (targetRender == null || render == null)
			return;

		render.color = targetRender.color;	
	}
}
