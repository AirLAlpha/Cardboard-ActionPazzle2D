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
public class CardboardBox : MonoBehaviour, IBurnable
{
	//	コンポーネント
	[SerializeField]
	private ParticleSystem	particle;

	private SpriteRenderer	spriteRenderer;		//	SpriteRrender
	private Rigidbody2D		rb;					//	Rigidbody2D
	private BoxCollider2D	collider;			//	BoxCollider2D

	//	設置判定
	[Header("設置")]
	[SerializeField]
	private LayerMask		tryCheckMask;       //	設置前判定用レイヤーマスク
	[SerializeField]
	private float			putSpeed;

	private float			putProgress;
	private bool			isMoving;
	private Vector2			startPos;
	private Vector2			targetPos;

	//	梱包
	[SerializeField]
	private Transform		label;				//	ラベル

	private bool			isPacked;           //	パッキング済み

	public bool				IsPacked { get { return isPacked; } }

	//	炎上
	[SerializeField]
	private float			burnSpeed;			//	炎上し切る速度
	[SerializeField, Range(0.0f, 1.0f)]
	private float			burnProgress;       //	燃える進行度

	private bool					isBurning;			//	炎上フラグ
	private MaterialPropertyBlock	propertyBlock;


	//	実行前初期化処理
	private void Awake()
	{
		//	コンポーネントの取得
		rb				= GetComponent<Rigidbody2D>();
		collider		= GetComponent<BoxCollider2D>();
		spriteRenderer	= GetComponent<SpriteRenderer>();

		propertyBlock = new MaterialPropertyBlock();
	}

	//	初期化処理
	private void Start()
	{
		
	}

	//	更新処理
	private void Update()
	{
		if (isBurning)
		{
			BurnUpdate();
			return;
		}

		if (!isMoving)
			return;

		putProgress = Mathf.Clamp01(putProgress + Time.deltaTime * putSpeed);
		if (putProgress >= 1.0f)
			isMoving = false;

		float x = Mathf.Lerp(startPos.x, targetPos.x, EaseX(putProgress));
		float y = Mathf.Lerp(startPos.y, targetPos.y, putProgress);

		transform.localPosition = new Vector2(x, y);
		transform.localScale = Vector3.one;

	}

	private float EaseX(float x)
	{
		return 1 - Mathf.Pow(1 - x, 3);
	}
	private float EaseY(float x)
	{
		return x * x * x * x;
	}

	/*--------------------------------------------------------------------------------
	|| 衝突判定
	--------------------------------------------------------------------------------*/
	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (isPacked || isBurning)
			return;

		if (collision.transform.tag == "Enemy")
			Packing(collision.transform);
	}

	/*--------------------------------------------------------------------------------
	|| 梱包処理
	--------------------------------------------------------------------------------*/
	private void Packing(Transform packingTarget)
	{
		Destroy(packingTarget.gameObject);

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
		isBurning = true;
	}

	/*--------------------------------------------------------------------------------
	|| 炎上の更新処理
	--------------------------------------------------------------------------------*/
	private void BurnUpdate()
	{
		burnProgress = Mathf.Clamp01(burnProgress + Time.deltaTime * burnSpeed);

		Material mat = spriteRenderer.material;

		spriteRenderer.GetPropertyBlock(propertyBlock);
		propertyBlock.SetFloat("_Progress", burnProgress);
		spriteRenderer.SetPropertyBlock(propertyBlock);

		if (burnProgress >= 1.0f)
			Destroy(gameObject);
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
		Vector2 checkAreaSize = collider.size;
		//	設置先を確認
		var checkResult = Physics2D.OverlapBox(checkPos, checkAreaSize, 0.0f, tryCheckMask);
		if(checkResult != null)
		{
			//	敵がいた場合は梱包を行う
			if (checkResult.tag == "Enemy")
			{
				Packing(checkResult.transform);
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


}
