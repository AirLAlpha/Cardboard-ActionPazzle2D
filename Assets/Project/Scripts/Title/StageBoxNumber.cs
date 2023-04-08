using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[ExecuteInEditMode]
public class StageBoxNumber : MonoBehaviour
{
	//	コンポーネント
	[SerializeField]
	private SpriteRenderer	render;

	private TextMeshPro		textMesh;

	//	実行前初期化処理
	private void OnEnable()
	{
		textMesh = GetComponent<TextMeshPro>();
	}

	//	更新処理
	private void Update()
	{
		if (render == null || textMesh == null)
			return;

		textMesh.color = render.color;
		textMesh.sortingOrder = render.sortingOrder + 1;
	}
}
