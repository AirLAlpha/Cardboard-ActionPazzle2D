using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleBackgroundManager : MonoBehaviour
{
	[SerializeField]
	private SpriteRenderer[]		renderers;

	MaterialPropertyBlock			propertyBlock;

	//	実行前初期化処理
	private void Awake()
	{
	}

	//	初期化処理
	private void Start()
	{
		propertyBlock = new MaterialPropertyBlock();

		foreach (var render in renderers)
		{
			//	プロパティを取得
			render.GetPropertyBlock(propertyBlock);
			//	時間のオブセットを乱数で設定
			propertyBlock.SetFloat("_TimeOffset", Random.Range(0, 10000.0f));
			//	プロパティを設定
			render.SetPropertyBlock(propertyBlock);
		}
		
	}

	//	更新処理
	private void Update()
	{
		
	}
}
