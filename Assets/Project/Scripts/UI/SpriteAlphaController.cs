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
	private float alpha = 1.0f;

	public float Alpha { get { return alpha;} set { alpha = value; } }

	//	更新処理
	private void Update()
	{
		foreach (var item in sprites)
		{
			SetAlpha(item);
		}
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
