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

namespace CardboardBox
{
	[RequireComponent(typeof(Rigidbody2D))]
	[RequireComponent(typeof(SpriteRenderer))]
	[RequireComponent(typeof(BlazingShaderController))]
	public class CardboardBox : MonoBehaviour, IPauseable, IBurnable
	{
		//	コンポーネント
		[SerializeField]
		private ParticleSystem particle;
		[SerializeField]
		private ParticleSystem burnEffect;

		private SpriteRenderer spriteRenderer;      //	SpriteRrender
		private Rigidbody2D rb;                 //	Rigidbody2D
		private new BoxCollider2D collider;           //	BoxCollider2D
		private BlazingShaderController bsc;                //	BlazingShaderController

		public SpriteRenderer SpriteRenderer { get { return spriteRenderer; } }

		//	ステート
		[SerializeField]
		private NonPackedBox	nonPackedBox;		//	未梱包のハコ
		[SerializeField]
		private DefaultBox		defaultBox;         //	通常のハコ
		[SerializeField]
		private BreakableBox	breakableBox;		//	割れ物注意のハコ

		private CardboardBoxState currentState;	//	現在のステート

		public DefaultBox DefaultBox		{ get{ return defaultBox; } }
		public BreakableBox BreakableBox	{ get { return breakableBox; } }

		//	設置判定
		[Header("設置")]
		[SerializeField]
		private LayerMask tryCheckMask;       //	設置前判定用レイヤーマスク
		[SerializeField]
		private float putSpeed;         //	設置速度

		private bool isMoving;
		private float putProgress;
		private Vector2 startPos;           //	設置の開始座標
		private Vector2 targetPos;          //	設置先の座標

		public Vector2 StartPos		{ get { return startPos; } }
		public Vector2 TargetPos	{ get { return targetPos; } }

		//	梱包
		[Header("梱包")]
		[SerializeField]
		private Color[] stateColors;        //	各ステートにおける色

		public bool IsPacked { get { return currentState != nonPackedBox; } }

		//	めり込み判定
		[SerializeField]
		private LayerMask mask;

		//	ポーズ
		private Vector2 posedVelocity;
		private float posedAnglerVelocity;

		//	破壊
		private bool isBruned;      //	破壊済みフラグ


		//	実行前初期化処理
		private void Awake()
		{
			//	コンポーネントの取得
			rb = GetComponent<Rigidbody2D>();
			collider = GetComponent<BoxCollider2D>();
			spriteRenderer = GetComponent<SpriteRenderer>();
			bsc = GetComponent<BlazingShaderController>();

			//	ステートの初期化
			nonPackedBox = new NonPackedBox(nonPackedBox, this);
			defaultBox = new DefaultBox(this);
			breakableBox = new BreakableBox(breakableBox, this);

			//	現在のステートを初期化
			currentState = nonPackedBox;
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

			//	現在のステートを更新
			if (currentState != null)
				currentState.StateUpdate();

			if (putProgress >= 1.0f)
			{
				//	箱の内部に衝突したら潰されていると判定する
				var hit = Physics2D.OverlapBox(transform.position, Vector2.one * 0.1f, 0.0f, mask);
				if (hit != null)
					Burn();
			}
		}

		//*--------------------------------------------------------------------------------
		//|| 箱の移動処理
		//--------------------------------------------------------------------------------*/
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

		/*--------------------------------------------------------------------------------
		|| 衝突判定
		--------------------------------------------------------------------------------*/
		private void OnCollisionEnter2D(Collision2D collision)
		{
			//	現在のステートの衝突判定
			if (currentState != null)
				currentState.OnCollisionEnter(collision);
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
			if (checkResult != null)
			{
				//	敵がいた場合は梱包を行う
				if (checkResult.TryGetComponent<IPackable>(out var hit))
				{
					//	相手の梱包処理を行う
					CardboardType type = hit.Packing();
					//	自身の梱包時処理を実行
					nonPackedBox.Packing(type);
				}
				//	それ以外は設置不能として処理
				else
				{
					return false;
				}
			}

			isMoving = true;
			//	座標の更新
			startPos = transform.position;
			targetPos = transform.TransformPoint(localPos);
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


		/*--------------------------------------------------------------------------------
		|| ステートの設定
		--------------------------------------------------------------------------------*/
		public void SetState(CardboardBoxState state)
		{
			//	ステートを抜ける処理
			currentState.OnExitState();

			//	ステートの設定
			currentState = state;

			//	ステートに入ったときの処理
			currentState.OnEnterState();
		}
	}
}