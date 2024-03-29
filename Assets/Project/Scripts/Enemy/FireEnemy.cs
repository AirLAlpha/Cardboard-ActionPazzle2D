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

public class FireEnemy : Enemy, IPackable
{
	//	攻撃
	[Header("攻撃")]
	[SerializeField]
	private FireBall		fireBall;			//	ファイヤボール
	[SerializeField]
	private float			attackInterval;		//	攻撃間隔

	private float			attackIntervalCount;    //	攻撃間隔のカウンター

	public Sprite LabelSprite { get { return labelSprite; } }

	//	初期化処理
	private void Start()
	{
		deadSeIndex = 1;
	}

	/*--------------------------------------------------------------------------------
	|| 待機中の更新処理
	--------------------------------------------------------------------------------*/
	protected override void IdleUpdate()
	{
	}

	/*--------------------------------------------------------------------------------
	|| 警戒中の更新処理
	--------------------------------------------------------------------------------*/
	protected override void AttentionUpdate()
	{
		//	攻撃間隔のカウントをリセット
		attackIntervalCount = 0.0f;
	}

	/*--------------------------------------------------------------------------------
	|| 攻撃の更新処理
	--------------------------------------------------------------------------------*/
	protected override void AttackUpdate()
	{
		if (fireBall == null)
			return;

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
		//	親を自身に設定
		newFireBall.Parent = transform;
		//	サウンドを設定
		newFireBall.SetSound(soundPlayer, 1);

		//	攻撃SEの再生
		soundPlayer.PlaySound(0);
	}

	/*--------------------------------------------------------------------------------
	|| アニメーションの更新処理
	--------------------------------------------------------------------------------*/
	protected override void AnimationUpdate()
	{
		bool attention = currentState != State.IDLE;
		anim.SetBool("Attention", attention);

		bool open = currentState == State.ATTACK;
		anim.SetBool("Open", open);
	}

	/*--------------------------------------------------------------------------------
	|| 梱包時処理
	--------------------------------------------------------------------------------*/
	public CardboardType Packing()
	{
		//	自身を削除する
		Destroy(gameObject);

		return this.packedType;
	}

	/*--------------------------------------------------------------------------------
	|| 衝突判定
	--------------------------------------------------------------------------------*/
	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.transform.tag == "Player")
			collision.transform.GetComponent<IBurnable>().Burn();
	}
}
