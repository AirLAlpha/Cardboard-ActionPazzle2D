/**********************************************
 * 
 *  Enemy.cs 
 *  敵の基底クラス
 * 
 *  製作者：牛丸 文仁
 *  制作日：2023/04/02
 * 
 **********************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Enemy : MonoBehaviour, IPoseable
{
	//	状態
	public enum State
	{
		IDLE,           //	待機
		ATTENTION,      //	警戒
		ATTACK,         //	攻撃
	}
	//	状態
	protected State			currentState;	//	現在の状態

	//	コンポーネント
	protected Rigidbody2D	rb;				//	Rigidbody2D

	//	移動
	protected Vector2		acce;			//	加速度
	protected Direction		currentDir;     //	現在向いている方向

	//	範囲
	[Header("範囲")]
	[SerializeField]
	protected float attentionRange;					//	警戒範囲
	[SerializeField]
	protected float attackRange;					//	攻撃範囲
	[SerializeField]
	protected Vector2 rangeOffset;					//	範囲のズレ
	[SerializeField]
	protected LayerMask detectionLayer;				//	検出するレイヤー

	protected Collider2D[] inAttackRrange;          //	攻撃範囲内のコライダー

	//	範囲の中心座標
	protected Vector2 RangeCenter => transform.position + new Vector3(rangeOffset.x, rangeOffset.y);

	[Header("梱包")]
	[SerializeField]
	protected CardboardType		packedType;         //	梱包後のダンボールタイプ

	//	ポーズ
	protected bool				disableUpdate;      //	アップデートの無効化
	private Vector2				posedVelocity;

#if UNITY_EDITOR
	[Header("デバッグ")]
	[SerializeField]
	private bool drawGizmos;            //	ギズモの描画フラグ
#endif


	//	実行前初期化処理
	protected virtual void Awake()
	{
		//	コンポーネントの取得
		rb = GetComponent<Rigidbody2D>();

		//	状態の初期化
		currentState = State.IDLE;
	}

	//	更新処理
	protected virtual void Update()
	{
		if (disableUpdate)
			return;

		StateUpdate();              //	状態の更新処理

		switch (currentState)
		{
			case State.IDLE:        //	待機
				IdleUpdate();
				break;

			case State.ATTENTION:   //	警戒
				AttentionUpdate();
				break;

			case State.ATTACK:      //	攻撃
				AttackUpdate();
				break;
		}
	}

	/*--------------------------------------------------------------------------------
	|| 状態の更新処理
	--------------------------------------------------------------------------------*/
	protected virtual void StateUpdate()
	{
		//	警戒範囲
		var attentionResult = Physics2D.OverlapCircle(RangeCenter, attentionRange, detectionLayer);

		if (attentionResult == null)
		{
			//	待機
			currentState = State.IDLE;
		}
		else
		{
			//	攻撃範囲
			inAttackRrange = Physics2D.OverlapCircleAll(RangeCenter, attackRange, detectionLayer);

			if (inAttackRrange.Length > 0)
			{
				//	攻撃
				currentState = State.ATTACK;
			}
			else
			{
				//	警戒
				currentState = State.ATTENTION;
			}
		}
	}

	/*--------------------------------------------------------------------------------
	|| 待機中の更新処理
	--------------------------------------------------------------------------------*/
	protected abstract void IdleUpdate();

	/*--------------------------------------------------------------------------------
	|| 警戒中の更新処理
	--------------------------------------------------------------------------------*/
	protected abstract void AttentionUpdate();

	/*--------------------------------------------------------------------------------
	|| 攻撃の処理
	--------------------------------------------------------------------------------*/
	protected abstract void AttackUpdate();

	/*--------------------------------------------------------------------------------
	|| ポーズ時処理
	--------------------------------------------------------------------------------*/
	public void Pose()
	{
		disableUpdate = true;

		posedVelocity = rb.velocity;
		rb.isKinematic = true;
		rb.velocity = Vector2.zero;
	}
	/*--------------------------------------------------------------------------------
	|| 再開時処理
	--------------------------------------------------------------------------------*/
	public void Resume()
	{
		disableUpdate = false;

		rb.isKinematic = true;
		rb.velocity = posedVelocity;
	}


#if UNITY_EDITOR
	/*--------------------------------------------------------------------------------
	|| ギズモの描画処理
	--------------------------------------------------------------------------------*/
	protected virtual void OnDrawGizmosSelected()
	{
		//	描画フラグが有効でないときは処理しない
		if (!drawGizmos)
			return;

		Color ATTENTION_COLOR = Color.yellow + new Color(0, 0, 0, -0.5f);       //	警戒範囲の色
		Color ATTACK_COLOR = Color.red + new Color(0, 0, 0, -0.5f);        //	攻撃範囲の色

		//	警戒範囲
		Gizmos.color = ATTENTION_COLOR;
		Gizmos.DrawSphere(RangeCenter, attentionRange);

		//	攻撃範囲
		Gizmos.color = ATTACK_COLOR;
		Gizmos.DrawSphere(RangeCenter, attackRange);

	}
#endif

}
