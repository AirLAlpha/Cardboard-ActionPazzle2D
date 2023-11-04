/**********************************************
 * 
 *  SpringGimmick.cs 
 *  バネのギミック
 * 
 *  製作者：牛丸 文仁
 *  制作日：2023/05/01
 * 
 **********************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringGimmick : Gimmick
{
	[Header("バネ")]
	[SerializeField]
	private float		springPower;        //	反発力
	[SerializeField]
	private float		springSpeed;        //	バネの動きの速さ
	[SerializeField]
	private float		reverseSpeed;       //	もとに戻る速度s
	[SerializeField]
	private float		maxVelocity;		//	最大の速度


	[Header("スプライト")]
	[SerializeField]
	private Transform		spriteRoot;         //	ルートオブジェクト
	[SerializeField]
	private SpriteRenderer	springTop;
	[SerializeField]
	private SpriteRenderer	springButtom;
	[SerializeField]
	private Transform		springMiddle;
	[SerializeField]
	private Vector2			pressedPos;
	[SerializeField]
	private Vector2			nonPressedPos;
	[SerializeField]
	private Vector2			pressedScale;
	[SerializeField]
	private Vector2			nonPressedScale;

	private BoxCollider2D	boxCollider;

	[Header("判定")]
	[SerializeField]
	private LayerMask	checkMask;
	[SerializeField]
	private Vector2		checkAreaSize;		//	判定エリア
	[SerializeField]
	private Vector2		checkAreaOffset;

	private Vector2		CheckAreaCenter => springTop.transform.position + new Vector3(checkAreaOffset.x, checkAreaOffset.y);

	private bool		isPressed;          //	オブジェクトの接触フラグ
	private bool		saveIsPressed;		//	前回処理時のオブジェクト接触フラグ
	private bool		isReversing;		//	もとに戻っているフラグ
	private float		springProgress;     //	バネの進行度

	private Rigidbody2D targetRb;           //	対象のRigidbody2D
	private SoundPlayer soundPlayer;

#if UNITY_EDITOR
	[Header("デバッグ")]
	[SerializeField]
	private bool		drawGizmos;
#endif

	//	実行前初期化処理
	private void Awake()
	{
		spriteRoot.TryGetComponent<BoxCollider2D>(out boxCollider);
		transform.parent.TryGetComponent<SoundPlayer>(out soundPlayer);
	}

	//	初期化処理
	private void Start()
	{
		
	}

	//	更新処理
	private void Update()
	{
		CheckHitSpring();
		SpringPressUpdate();
	}

	/*--------------------------------------------------------------------------------
	|| 衝突判定
	--------------------------------------------------------------------------------*/
	private void CheckHitSpring()
	{
		saveIsPressed = isPressed;

		var hit = Physics2D.OverlapBox(CheckAreaCenter, checkAreaSize, transform.eulerAngles.z, checkMask);
		if (hit == null)
		{
			isPressed = false;

			if (hit != null)
				hit = null;

			return;
		}

		isPressed = true;

		if (isPressed && !saveIsPressed)
			hit.transform.TryGetComponent<Rigidbody2D>(out targetRb);
	}

	/*--------------------------------------------------------------------------------
	|| バネの可動処理
	--------------------------------------------------------------------------------*/
	private void SpringPressUpdate()
	{
		//	投下状態に応じて進行度を更新する
		if(isPressed && !isReversing)
		{
			springProgress += Time.deltaTime * springSpeed;
		}
		else
		{
			springProgress -= Time.deltaTime * reverseSpeed;

			if (isReversing && springProgress <= 0.0f)
				isReversing = false;
		}
		springProgress = Mathf.Clamp01(springProgress);

		//	バネの進行度が1を超え、もとに戻っていないなら、元に戻す
		if(springProgress >= 1.0 && !isReversing)
		{
			isReversing = true;

			if (targetRb == null)
				return;

			//	最大速度を超えていたら処理しない
			if (targetRb.velocity.sqrMagnitude >= maxVelocity * maxVelocity)
				return;

			//	乗っているターゲットに力を加える
			//targetRb.AddForce(spriteRoot.up * springPower * targetRb.mass, ForceMode2D.Impulse);
			targetRb.velocity = Vector2.up * springPower;
			targetRb = null;

			//	SEの再生
			soundPlayer.PlaySound(1);
		}

		//	各パーツの座標を更新する
		springTop.transform.localPosition = Vector2.Lerp(nonPressedPos, pressedPos, springProgress);
		springMiddle.transform.localPosition = (springTop.transform.localPosition + springButtom.transform.localPosition) * 0.5f;
		springMiddle.transform.localScale = Vector2.Lerp(nonPressedScale, pressedScale, springProgress);
		boxCollider.offset = springMiddle.localPosition;
		boxCollider.size = springMiddle.localScale + Vector3.up * 0.1f;
	}

	/*--------------------------------------------------------------------------------
	|| 固有の設定を設定する
	--------------------------------------------------------------------------------*/
	public override void SetExtraSetting(string json)
	{
	}

	/*--------------------------------------------------------------------------------
	|| 固有の設定の取得
	--------------------------------------------------------------------------------*/
	public override string GetExtraSetting()
	{
		return null;
	}

#if UNITY_EDITOR
	private void OnDrawGizmosSelected()
	{
		if (!drawGizmos)
			return;

		Gizmos.color = new Color(1, 0, 0, 0.5f);
		
		Gizmos.DrawCube(CheckAreaCenter, checkAreaSize);
	}
#endif
}
