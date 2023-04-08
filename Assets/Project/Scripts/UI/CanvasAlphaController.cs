/**********************************************
 * 
 *  CanvasAlphaController.cs 
 *  自身の子に影響するアルファを設定する
 * 
 *  製作者：牛丸 文仁
 *  制作日：2023/04/07
 * 
 **********************************************/
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class CanvasAlphaController : MonoBehaviour
{
	//	コンポーネント
	private CanvasGroup			group;

	//	アルファ
	[SerializeField, Range(0.0f,1.0f)]
	private float				targetAlpha;
	[SerializeField]
	private float				alphaSpeed;

	public float				TargetAlpha { get { return targetAlpha; } set { targetAlpha = value; } }


	//	実行前初期化処理
	private void Awake()
	{
		group = GetComponent<CanvasGroup>();
	}

	//	更新処理
	private void Update()
	{
		targetAlpha = Mathf.Clamp01(targetAlpha);
		group.alpha = Mathf.Lerp(group.alpha, targetAlpha, Time.deltaTime * alphaSpeed);

		if (group.alpha >= 0.9999999f && group.alpha < 1.0f)
			group.alpha = 1.0f;
		if (group.alpha <= 0.0000001f && group.alpha > 0.0f)
			group.alpha = 0.0f;
	}
}
