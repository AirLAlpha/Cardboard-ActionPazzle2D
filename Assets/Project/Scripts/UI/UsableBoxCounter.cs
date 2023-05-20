using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UsableBoxCounter : MonoBehaviour
{
	[Header("透明")]
	[SerializeField]
	private Vector2			invisibleArea;
	[SerializeField]
	private Vector2			areaOffset;
	[SerializeField]
	private LayerMask		invisibleAreaMask;
	[SerializeField]
	private Image[]			alphaTargets;
	[SerializeField]
	private float			onAreaAlpha;
	[SerializeField]
	private float			alphaSpeed;

	private float alpha;
	private Vector2 AreaWorldCenter => Camera.main.ScreenToWorldPoint(transform.position + (Vector3)areaOffset);

	//	コンポーネント
	[Header("コンポーネント")]
	[SerializeField]
	private NumberDrawer			currentCount;	//	現在の個数を描画するテキスト
	[SerializeField]
	private NumberDrawer			buffCount;    //	バッファ用のテキスト
	[SerializeField]
	private Image			symbol;		//	乗算記号

	private RectTransform	rootRect;	//	自身のRectTransform

	private RectTransform	currentCountTransform;
	private RectTransform	buffCountTransform;

	//	表示
	[Header("表示")]
	[SerializeField]
	private string countBeforeText;		//	数字の前につける文字列
	[SerializeField]
	private string countAfterText;      //	数字の後ろに付け足す文字列

	//	アニメーション
	[Header("アニメーション")]
	[SerializeField]
	private float	animSpeed;          //	アニメーション速度
	[SerializeField]
	private Vector3 amplitudeBasePosition;
	[SerializeField]
	private float	amplitudeSpeed;     //	エラー時の振幅速度
	[SerializeField]
	private float	amplitudePower;		//	エラー時の振れ幅

	private bool	isChangeCountAnim;		//	数値変更のアニメーションフラグ
	private bool	isRemainingAnim;		//	残数がなかったときのアニメーションフラグ
	private float	changeCountProgress;    //	数値変更のアニメーション進行度
	private float	remainingProgress;      //	残数がなかったときのアニメーション進行度


	//	実行前初期化処理
	private void Awake()
	{
		//	コンポーネントの取得
		rootRect = transform as RectTransform;

		currentCountTransform	= currentCount.transform as RectTransform;
		buffCountTransform		= buffCount.transform as RectTransform;
	}

	//	初期化処理
	private void Start()
	{
		alpha = 1.0f;
	}

	//	更新処理
	private void Update()
	{
		InvisibleUpdate();

		ChangeCountAnimation();
		NonRemainingAnimation();
	}

	/*--------------------------------------------------------------------------------
	|| 数値が変化したときに呼び出される処理
	--------------------------------------------------------------------------------*/
	public void ChangeCount()
	{
		//	残数の取得
		int count = StageManager.Instance.RemainingBoxCount;
		//	バッファに現在の文字列を保持
		buffCount.Number = currentCount.Number;
		//	文字列の更新
		currentCount.Number = count;

		//	文字列の座標を更新する
		buffCountTransform.localPosition = currentCountTransform.position;
		currentCountTransform.localPosition += Vector3.up * 150;

		//	アニメーション用の変数を初期化
		changeCountProgress = 0.0f;
		//	アニメーションフラグを有効化
		isChangeCountAnim = true;
	}

	/*--------------------------------------------------------------------------------
	|| 残数がなかったときに呼び出される
	--------------------------------------------------------------------------------*/
	public void NonRemaining()
	{
		//	文字色を変更
		currentCount.Color = Color.red;
		buffCount.Color = Color.red;
		symbol.color = Color.red;

		//	アニメーション用の変数を初期化
		remainingProgress = 0.0f;
		//	アニメーションフラグを有効化
		isRemainingAnim = true;
	}

	/*--------------------------------------------------------------------------------
	|| アニメーションの更新処理
	--------------------------------------------------------------------------------*/
	private void ChangeCountAnimation()
	{
		//	アニメーション中でないときは処理しない
		if (!isChangeCountAnim)
			return;

		//	進行度を更新する
		changeCountProgress = Mathf.Clamp01(changeCountProgress + Time.deltaTime * animSpeed);

		//	座標を更新する
		float y = Mathf.Lerp(150.0f, 0.0f, EaseOutBounce(changeCountProgress));
		currentCountTransform.localPosition = new Vector3(100, y);
		buffCountTransform.localPosition = new Vector3(100, -150 + y);

		//	進行度が1に到達したらアニメーションを終了
		if (changeCountProgress >= 1.0f)
			isChangeCountAnim = false;
	}

	/*--------------------------------------------------------------------------------
	|| 箱の残数がなかったときのアニメーション処理
	--------------------------------------------------------------------------------*/
	private void NonRemainingAnimation()
	{
		//	フラグが有効でないときは処理しない
		if (!isRemainingAnim)
			return;

		//	進行度を更新する
		remainingProgress = Mathf.Clamp01(remainingProgress + Time.deltaTime * amplitudeSpeed);
		//	座標を計算
		float x = Mathf.Sin(remainingProgress * Mathf.PI * 2 * 5) * amplitudePower * (1.0f - remainingProgress);
		//	座標を更新
		rootRect.localPosition = amplitudeBasePosition + Vector3.right * x;

		//	文字色を変更
		currentCount.Color = Color.Lerp(Color.red, Color.white, remainingProgress);
		buffCount.Color = Color.Lerp(Color.red, Color.white, remainingProgress);
		symbol.color = Color.Lerp(Color.red, Color.white, remainingProgress);

		//	進行度が1に到達したらアニメーションを終了
		if (remainingProgress >= 1.0f)
			isRemainingAnim = false;
	}

	/*--------------------------------------------------------------------------------
	|| プレイヤーが近づいたら透明にする処理
	--------------------------------------------------------------------------------*/
	private void InvisibleUpdate()
	{
		var hit = Physics2D.OverlapBox(AreaWorldCenter, invisibleArea, 0.0f, invisibleAreaMask);
		if (hit != null)
			alpha -= Time.deltaTime * alphaSpeed;
		else
			alpha += Time.deltaTime * alphaSpeed;
		alpha = Mathf.Clamp(alpha, onAreaAlpha, 1.0f);

		foreach (var item in alphaTargets)
		{
			Color col = item.color;
			col.a = alpha;
			item.color = col;
		}
	}

	/*--------------------------------------------------------------------------------
	|| イージング
	--------------------------------------------------------------------------------*/
	private float EaseOutBounce(float x)
	{
		const float n1 = 7.5625f;
		const float d1 = 2.75f;

		if (x < 1 / d1)
		{
			return n1 * x * x;
		}
		else if (x < 2 / d1)
		{
			return n1 * (x -= 1.5f / d1) * x + 0.75f;
		}
		else if (x < 2.5 / d1)
		{
			return n1 * (x -= 2.25f / d1) * x + 0.9375f;
		}
		else
		{
			return n1 * (x -= 2.625f / d1) * x + 0.984375f;
		}
	}

#if UNITY_EDITOR
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = new Color(0, 1, 0, 0.5f);

		Gizmos.DrawCube(AreaWorldCenter, invisibleArea);
	}
#endif
}
