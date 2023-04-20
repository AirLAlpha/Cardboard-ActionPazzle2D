using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UsableBoxCounter : MonoBehaviour
{
	//	コンポーネント
	[Header("コンポーネント")]
	[SerializeField]
	private Text currentCountText;		//	現在の個数を描画するテキスト
	[SerializeField]
	private Text buffCountText;         //	バッファ用のテキスト
	[SerializeField]
	private Text symbolText;			//	乗算記号

	private RectTransform rootRect;		//	自身のRectTransform

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
	private float	amplitudeSpeed;		//	エラー時の振幅速度

	private bool	isChangeCountAnim;		//	数値変更のアニメーションフラグ
	private bool	isRemainingAnim;		//	残数がなかったときのアニメーションフラグ
	private float	changeCountProgress;    //	数値変更のアニメーション進行度
	private float	remainingProgress;      //	残数がなかったときのアニメーション進行度


	//	実行前初期化処理
	private void Awake()
	{
		//	コンポーネントの取得
		rootRect = transform as RectTransform;
	}

	//	初期化処理
	private void Start()
	{
	}

	//	更新処理
	private void Update()
	{
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
		buffCountText.text = currentCountText.text;
		//	文字列の更新
		currentCountText.text = countBeforeText + count.ToString() + countAfterText;

		//	文字列の座標を更新する
		buffCountText.rectTransform.localPosition = currentCountText.rectTransform.position;
		currentCountText.rectTransform.localPosition += Vector3.up * 150;

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
		currentCountText.color = Color.red;
		buffCountText.color = Color.red;
		symbolText.color = Color.red;

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
		currentCountText.rectTransform.localPosition = new Vector3(100, y);
		buffCountText.rectTransform.localPosition = new Vector3(100, -150 + y);

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
		float x = Mathf.Sin(remainingProgress * Mathf.PI * 2 * 5) * 30 * (1.0f - remainingProgress);
		//	座標を更新
		rootRect.localPosition = new Vector2(-456, 417) + Vector2.right * x;

		//	文字色を変更
		currentCountText.color = Color.Lerp(Color.red, Color.white, remainingProgress);
		buffCountText.color = Color.Lerp(Color.red, Color.white, remainingProgress);
		symbolText.color = Color.Lerp(Color.red, Color.white, remainingProgress);

		//	進行度が1に到達したらアニメーションを終了
		if (remainingProgress >= 1.0f)
			isRemainingAnim = false;
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
}
