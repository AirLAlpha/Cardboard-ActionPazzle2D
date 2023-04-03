/**********************************************
 * 
 *  FireEnemy.cs 
 *  炎の敵に関する処理を記述
 * 
 *  製作者：牛丸 文仁
 *  制作日：2023/04/03
 * 
 **********************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireEnemy : Enemy
{
	//	状態
	public enum State
	{
		IDLE,			//	待機
		ATTENTION,		//	警戒
		ATTACK,			//	攻撃
	}

	//	コンポーネント
	[Header("コンポーネント")]
	[SerializeField]
	private Animator anim;          //	アニメーション

	//	状態
	private State currentState;     //	現在の状態

	//	攻撃
	[Header("攻撃")]
	[SerializeField]
	private FireBall	fireBall;			//	ファイヤボール
	[SerializeField]
	private float		attackInterval;		//	攻撃間隔
	[SerializeField]
	private float		attentionRange;		//	警戒範囲
	[SerializeField]
	private float		attackRange;        //	攻撃範囲
	[SerializeField]
	private Vector2		rangeOffset;        //	範囲のズレ
	[SerializeField]
	private LayerMask	detectionLayer;     //	検出するレイヤー

	private float		attackIntervalCount;	//	攻撃間隔のカウンター
	private Collider2D[] inAttackRrange;		//	攻撃範囲内のコライダー

	//	範囲の中心座標
	private Vector2 RangeCenter => transform.position + new Vector3(rangeOffset.x, rangeOffset.y);

#if UNITY_EDITOR
	[Header("デバッグ")]
	[SerializeField]
	private bool drawGizmos;            //	ギズモの描画フラグ
#endif


	//	実行前初期化処理
	private void Awake()
	{
		//	コンポーネントの取得
		rb = GetComponent<Rigidbody2D>();

		//	状態の初期化
		currentState = State.IDLE;
	}

	//	初期化処理
	private void Start()
	{
		
	}

	//	更新処理
	private void Update()
	{
		StateUpdate();				//	状態の更新処理

		switch (currentState)
		{
			case State.IDLE:		//	待機
				break;	

			case State.ATTENTION:	//	警戒
				break;

			case State.ATTACK:      //	攻撃
				AttackUpdate();	
				break;
		}

		AnimationUpdate();			//	アニメーションの更新処理
	}

	/*--------------------------------------------------------------------------------
	|| 状態の更新処理
	--------------------------------------------------------------------------------*/
	private void StateUpdate()
	{
		//	警戒範囲
		var attentionResult = Physics2D.OverlapCircle(RangeCenter, attentionRange, detectionLayer);

		if(attentionResult == null)
		{
			//	待機
			currentState = State.IDLE;
		}
		else
		{
			//	攻撃範囲
			inAttackRrange = Physics2D.OverlapCircleAll(RangeCenter, attackRange, detectionLayer);

			if(inAttackRrange.Length > 0)
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
	|| 攻撃の更新処理
	--------------------------------------------------------------------------------*/
	private void AttackUpdate()
	{
		attackIntervalCount += Time.deltaTime;

		if(attackIntervalCount >= attackInterval)
		{
			float dist = attentionRange * attentionRange;
			var nearestTarget = inAttackRrange[0];
			for (int i = 0; i < inAttackRrange.Length; i++)
			{
				Vector3 vec = inAttackRrange[i].transform.position - transform.position;
				float d = vec.x * vec.x + vec.y * vec.y;
				if (d >= dist)
					continue;

				nearestTarget = inAttackRrange[i];
				dist = d;
			}
			Attack(nearestTarget.transform);

			//	カウントをリセット
			attackIntervalCount = 0.0f;
		}
	}

	/*--------------------------------------------------------------------------------
	|| 攻撃処理
	--------------------------------------------------------------------------------*/
	private void Attack(Transform target)
	{
		//	新たなファイヤボールの作成
		var newFireBall = Instantiate(fireBall, transform.position, Quaternion.identity);
		//	ターゲットに向けたベクトルを作成
		var toTargetVec = (target.position - transform.position).normalized;
		//	ベクトルを設定
		newFireBall.Direction = toTargetVec;
	}

	/*--------------------------------------------------------------------------------
	|| アニメーションの更新処理
	--------------------------------------------------------------------------------*/
	private void AnimationUpdate()
	{
		bool attention = currentState != State.IDLE;
		anim.SetBool("Attention", attention);

		bool open = currentState == State.ATTACK;
		anim.SetBool("Open", open);
	}


#if UNITY_EDITOR
	/*--------------------------------------------------------------------------------
	|| ギズモの描画処理
	--------------------------------------------------------------------------------*/
	private void OnDrawGizmosSelected()
	{
		//	描画フラグが有効でないときは処理しない
		if (!drawGizmos)
			return;

		Color ATTENTION_COLOR	= Color.yellow + new Color(0, 0, 0, -0.5f);		//	警戒範囲の色
		Color ATTACK_COLOR		= Color.red + new Color(0, 0, 0, -0.5f);        //	攻撃範囲の色

		//	警戒範囲
		Gizmos.color = ATTENTION_COLOR;
		Gizmos.DrawSphere(RangeCenter, attentionRange);

		//	攻撃範囲
		Gizmos.color = ATTACK_COLOR;
		Gizmos.DrawSphere(RangeCenter, attackRange);

	}
#endif
}
