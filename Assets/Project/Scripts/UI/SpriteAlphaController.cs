/**********************************************
 * 
 *  SpriteAlphaController.cs 
 *  設定されたSpriteRendererのAlphaを一括で設定
 * 
 *  製作者：牛丸 文仁
 *  制作日：2023/04/08
 * 
 **********************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SpriteAlphaController : MonoBehaviour
{
	[System.Serializable]
	struct SpriteData
	{
		public SpriteRenderer renderer;
		[Range(0.0f,1.0f)]
		public float alphaMin;
		[Range(0.0f,1.0f)]
		public float alphaMax;
	}

	[SerializeField]
	private SpriteData[] sprites;

	[SerializeField, Range(0.0f,1.0f)]
	private float targetAlpha = 1.0f;

	[SerializeField]
	private float alphaSpeed;

	private float alpha;

	public float Alpha			{ get { return alpha; } set { alpha = value; targetAlpha = value; } }
	public float TargetAlpha	{ get { return targetAlpha; } set { targetAlpha = value; } }

	//	更新処理
	private void Update()
	{
		if (sprites == null ||
			sprites.Length <= 0)
			return;

		alpha = Mathf.Lerp(alpha, TargetAlpha, Time.deltaTime * alphaSpeed);
		alpha = Mathf.Clamp01(alpha);

		foreach (var item in sprites)
		{
			SetAlpha(item);
		}

		if (Input.GetKeyDown(KeyCode.T))
			targetAlpha = 1;
		if (Input.GetKeyDown(KeyCode.Y))
			targetAlpha = 0;
	}

	/*--------------------------------------------------------------------------------
	|| アルファの設定
	--------------------------------------------------------------------------------*/
	private void SetAlpha(SpriteData sprite)
	{
		Color col = sprite.renderer.color;
		col.a = Mathf.Lerp(sprite.alphaMin, sprite.alphaMax, alpha);
		sprite.renderer.color = col;
	}
}
