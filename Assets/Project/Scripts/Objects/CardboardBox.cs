/**********************************************
 * 
 *  CardboardBox.cs 
 *  段ボール箱に関する処理を記述
 * 
 *  製作者：牛丸 文仁
 *  制作日：2023/04/02
 * 
 **********************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BlazingShaderController))]
public class CardboardBox : MonoBehaviour, IPauseable, IBurnable
{
	//	コンポーネント
	[SerializeField]
	private ParticleSystem	particle;
	[SerializeField]
	private ParticleSystem	burnEffect;

	private SpriteRenderer			spriteRenderer;		//	SpriteRrender
	private Rigidbody2D				rb;					//	Rigidbody2D
	private new BoxCollider2D		collider;           //	BoxCollider2D
	private BlazingShaderController bsc;                //	BlazingShaderController

	//	設置判定
	[Header("設置")]
	[SerializeField]
	private LayerMask		tryCheckMask;       //	設置前判定用レイヤーマスク
	[SerializeField]
	private float			putSpeed;			//	設置速度

	private float			putProgress;		//	設置の進行度
	private bool			isMoving;			//	設置中フラグ
	private Vector2			startPos;			//	設置の開始座標
	private Vector2			targetPos;          //	設置先の座標

	//	梱包
	[Header("梱包")]
	[SerializeField]
	private Color[]			stateColors;		//	各ステートにおける色
	[SerializeField]
	private Transform		label;              //	ラベル

	private CardboardType	type;				//	梱包ステート
	private bool			isPacked;           //	パッキング済み

	public bool				IsPacked { get { return isPacked; } }

	//	割れ物注意
	[Header("割れ物注意")]
	[SerializeField]
	private float			breakingVelocity;     //	破壊されてしまう高さ

	//	めり込み判定
	[SerializeField]
	private LayerMask		mask;

	//	ポーズ
	private Vector2			posedVelocity;
	private float			posedAnglerVelocity;

	//	破壊
	private bool			isBruned;		//	破壊済みフラグ


	//	実行前初期化処理
	private void Awake()
	{
		//	コンポーネントの取得
		rb				= GetComponent<Rigidbody2D>();
		collider		= GetComponent<BoxCollider2D>();
		spriteRenderer	= GetComponent<SpriteRenderer>();
		bsc				= GetComponent<BlazingShaderController>();
	}

	//	初期化処理
	private void Start()
	{
		
	}

	//	更新処理
	private void Update()
	{
		//	箱の移動処理
		MoveUpdate();

		if (putProgress < 1)
			return;

		//	箱の内部に衝突したら潰されていると判定する
		var hit = Physics2D.OverlapBox(transform.position, Vector2.one * 0.1f, 0.0f, mask);
		if (hit != null)
			Burn();
	}

	/*--------------------------------------------------------------------------------
	|| 箱の移動処理
	--------------------------------------------------------------------------------*/
	private void MoveUpdate()
	{
		if (!isMoving)
			return;

		putProgress = Mathf.Clamp01(putProgress + Time.deltaTime * putSpeed);
		if (putProgress >= 1.0f)
			isMoving = false;

		float x = Mathf.Lerp(startPos.x, targetPos.x, putProgress);
		float y = Mathf.Lerp(startPos.y, targetPos.y, EasingFunctions.EaseInExpo(putProgress));

		transform.localPosition = new Vector2(x, y);
		transform.localScale = Vector3.one;
	}
	private float EaseX(float x)
	{
		return 1 - Mathf.Pow(1 - x, 3);
	}

	/*--------------------------------------------------------------------------------
	|| 衝突判定
	--------------------------------------------------------------------------------*/
	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (type == CardboardType.BREAKABLE)
		{
			float hitVel = collision.relativeVelocity.sqrMagnitude;
			if (hitVel >= breakingVelocity * breakingVelocity)
				Burn();

			Debug.Log(collision.relativeVelocity);
		}

		if (isPacked)
			return;

		if (collision.transform.TryGetComponent<IPackable>(out var hit))
		{
			//	相手の梱包処理を実行
			CardboardType type = hit.Packing();
			//	自身の梱包時処理を実行
			Packing(type);
		}
	}

	/*--------------------------------------------------------------------------------
	|| 梱包処理
	--------------------------------------------------------------------------------*/
	private void Packing(CardboardType type)
	{
		if(type == CardboardType.NONPACKABLE)
		{
			Burn();
			return;
		}

		//	タイプを保持する
		this.type = type;
		//	対応する色に変化させる
		spriteRenderer.color = stateColors[(int)type];

		//	パッキング済みにする
		isPacked = true;

		//	ラベルの有効化
		label.gameObject.SetActive(true);
		//	パーティクルの再生
		particle.Play();
	}

	/*--------------------------------------------------------------------------------
	|| 炎上した際の処理
	--------------------------------------------------------------------------------*/
	public void Burn()
	{
		if (isBruned)
			return;

		isBruned = true;

		bsc.IsBurning = true;
		if (rb != null)
		{
			rb.isKinematic = true;
			rb.velocity = Vector2.zero;
		}

		Instantiate(burnEffect, transform.position, Quaternion.identity);
	}

	/*--------------------------------------------------------------------------------
	|| Rigidbodyのアクティブを設定
	--------------------------------------------------------------------------------*/
	public void SetRigidbodyActive(bool active)
	{
		rb.bodyType = active ? RigidbodyType2D.Dynamic : RigidbodyType2D.Kinematic;
		rb.simulated = active ? true : false;
	}

	/*--------------------------------------------------------------------------------
	|| ハコの設置を試みる
	--------------------------------------------------------------------------------*/
	public bool TryPut(Vector2 localPos)
	{
		//	確認する中心座標
		Vector2 checkPos = transform.TransformPoint(localPos);
		//	確認する範囲
		Vector2 checkAreaSize = Vector2.one * 0.9f;
		//	設置先を確認
		var checkResult = Physics2D.OverlapBox(checkPos, checkAreaSize, 0.0f, tryCheckMask);
		if(checkResult != null)
		{
			//	敵がいた場合は梱包を行う
			if (checkResult.TryGetComponent<IPackable>(out var hit))
			{
				//	相手の梱包処理を行う
				CardboardType type = hit.Packing();
				//	自身の梱包時処理を実行
				Packing(type);
			}
			//	それ以外は設置不能として処理
			else
			{
				return false;
			}
		}

		//	座標の更新
		startPos = transform.position;
		targetPos = transform.TransformPoint(localPos);
		putProgress = 0.0f;
		isMoving = true;
		//	正常に完了
		return true;
	}

	public void Pause()
	{
		posedVelocity = rb.velocity;
		posedAnglerVelocity = rb.angularVelocity;

		this.rb.isKinematic = true;
		rb.velocity = Vector2.zero;
		rb.angularVelocity = 0.0f;
	}

	public void Resume()
	{
		if (this == null)
			return;

		this.rb.isKinematic = false;
		rb.velocity = posedVelocity;
		rb.angularVelocity = posedAnglerVelocity;
	}
}
