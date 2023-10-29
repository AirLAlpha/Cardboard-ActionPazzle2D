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
		[SerializeField]
		private SpriteRenderer[] labelSprites;
		[SerializeField]
		private BlazingShaderController[] burnShaderController;        //	BlazingShaderController

		private SpriteRenderer	spriteRenderer;			//	SpriteRrender
		private Rigidbody2D		rb;						//	Rigidbody2D
		private SoundPlayer		soundPlayer;

		public SpriteRenderer	SpriteRenderer	{ get { return spriteRenderer; } }
		public Rigidbody2D		Rigidbody2D		{ get { return rb; } }
		public SpriteRenderer[] LabelSprites	{ get { return labelSprites; } }
		public SoundPlayer		SoundPlayer		{ get { return soundPlayer; } }

		//	ステート
		[SerializeField]
		private NonPackedBox	nonPackedBox;		//	未梱包のハコ
		[SerializeField]
		private DefaultBox		defaultBox;         //	通常のハコ
		[SerializeField]
		private BreakableBox	breakableBox;       //	割れ物注意のハコ
		[SerializeField]
		private RightSideUpBox	rightSideUpBox;		//	天地無用のハコ


		private CardboardBoxState currentState;	//	現在のステート

		public DefaultBox		DefaultBox		{ get{ return defaultBox; } }
		public BreakableBox		BreakableBox	{ get { return breakableBox; } }
		public RightSideUpBox	RightSideUpBox	{ get { return rightSideUpBox; } }

		//	マテリアル
		[Header("デフォルトマテリアル")]
		[SerializeField]
		private Material	burnMaterial;


		//	設置判定
		[Header("設置")]
		[SerializeField]
		private LayerMask tryCheckMask;       //	設置前判定用レイヤーマスク

		//	梱包
		[Header("梱包")]
		[SerializeField]
		private Color[] stateColors;        //	各ステートにおける色

		public bool IsPacked { get { return currentState != nonPackedBox; } }

		//	めり込み判定
		[SerializeField]
		private LayerMask destroyMask;

		//	ポーズ
		private Vector2 posedVelocity;
		private float posedAnglerVelocity;

		//	破壊
		private bool isBruned;      //	破壊済みフラグ
		public bool IsBurned { get { return isBruned; } }

		//	実行前初期化処理
		private void Awake()
		{
			//	コンポーネントの取得
			rb = GetComponent<Rigidbody2D>();
			spriteRenderer = GetComponent<SpriteRenderer>();

			//	ステートの初期化
			nonPackedBox = new NonPackedBox(nonPackedBox, this);
			defaultBox = new DefaultBox(defaultBox, this);
			breakableBox = new BreakableBox(breakableBox, this);
			rightSideUpBox = new RightSideUpBox(rightSideUpBox, this);

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
			//	現在のステートを更新
			if (currentState != null)
				currentState.StateUpdate();

			if (rb.simulated == true)
			{
				//	箱の内部に衝突したら潰されていると判定する
				ContactFilter2D filter = new ContactFilter2D();
				filter.layerMask = destroyMask;
				filter.useLayerMask = true;
				filter.useTriggers = false;
				Collider2D[] result = new Collider2D[1];
				int hit = Physics2D.OverlapBox(transform.position, Vector2.one * 0.01f, 0.0f, filter, result);
				if (hit != 0)
					Burn();
			}
		}

		private void FixedUpdate()
		{
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

			//	マテリアルの変更
			spriteRenderer.material = burnMaterial;
			foreach (var item in labelSprites)
			{
				item.material = burnMaterial;
			}


			foreach (var bsc in burnShaderController)
			{
				bsc.IsBurning = true;
			}

			if (rb != null)
			{
				rb.isKinematic = true;
				rb.velocity = Vector2.zero;
			}

			Instantiate(burnEffect, transform.position, Quaternion.identity);

			soundPlayer.PlaySound(8);
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
		|| 梱包処理
		--------------------------------------------------------------------------------*/
		public void Packing(CardboardType type, IPackable packable)
		{
			if (currentState == nonPackedBox)
			{
				nonPackedBox.Packing(type, packable);
			}
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

		public void SetSoundPlayer(SoundPlayer soundPlayer)
		{
			this.soundPlayer = soundPlayer;
		}
	}
}