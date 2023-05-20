using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDead : MonoBehaviour
{
	//	コンポーネント
	[Header("コンポーネント")]
	[SerializeField]
	private PlayerDamageReciver		damageReciver;
	[SerializeField]
	private CameraShake				cameraShake;
	[SerializeField]
	private CameraZoom				cameraZoom;
	[SerializeField]
	private CanvasAlphaController	canvasAlphaController;
	[SerializeField]
	private CanvasAlphaController	buttonHintAlpha;

	//	待機時間
	[Header("待機時間")]
	[SerializeField]
	private float		cameraShakeWait;		//	カメラ揺れの待機時間
	[SerializeField]
	private float		zoomInWait;				//	ズームイン時の待機時間
	[SerializeField]
	private float		playerBurnWait;			//	プレイヤーの炎上アニメーションの待機時間

	//	実行前初期化処理
	private void Awake()
	{
		
	}

	//	初期化処理
	private void Start()
	{
		
	}

	//	更新処理
	private void Update()
	{
		
	}

	/*--------------------------------------------------------------------------------
	|| 死亡処理
	--------------------------------------------------------------------------------*/
	public void Dead()
	{
		StartCoroutine(DeadCoroutine());
	}

	/*--------------------------------------------------------------------------------
	|| 死亡時のコルーチン
	--------------------------------------------------------------------------------*/
	private IEnumerator DeadCoroutine()
	{
		//	カメラ揺れを実行
		cameraShake.StartShake(0.2f);
		//	待つ
		yield return new WaitForSeconds(cameraShakeWait);

		//	カメラをズームインする
		cameraZoom.ZoomIn = true;
		//	UIの表示を消す
		if (canvasAlphaController != null)
			canvasAlphaController.TargetAlpha = 0.0f;
		if (buttonHintAlpha != null)
			buttonHintAlpha.TargetAlpha = 0.0f;
		//	待つ
		yield return new WaitForSeconds(zoomInWait);

		//	レイヤーをアニメーションさせる
		damageReciver.StartBurnAnimation();
		//	待つ
		yield return new WaitForSeconds(playerBurnWait);

		//	すべてが終了したらステージを再読込する
		StageManager.Instance.ResetStage();
	}
}
