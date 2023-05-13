/**********************************************
 * 
 *  WaterEnemy.cs 
 *  水の敵の処理を記述
 * 
 *  製作者：牛丸 文仁
 *  制作日：2023/05/02
 * 
 **********************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterEnemy : Enemy, IPackable
{
	[Header("コンポーネント")]
	[SerializeField]
	private SpriteRenderer				spriteRenderer;     //	スプライトレンダー

	private MaterialPropertyBlock		propertyBlock;      //	プロパティブロック

	[Header("シェーダー")]
	[SerializeField, Range(0,1)]
	private float		targetProgress;       //	進行度

	private float		progress;

	[Header("攻撃")]
	[SerializeField]
	private FireBall	waterObj;
	[SerializeField]
	private int			attackObjCount;
	[SerializeField]
	private float		attackAngle;
	[SerializeField]
	private Vector3		attackObjInitOffset;//	攻撃オブジェクトの生成オフセット
	[SerializeField]
	private float		attackInterval;		//	攻撃間隔
	[SerializeField]
	private int			attackableCount;    //	攻撃可能回数
	[SerializeField]
	private float		rechargeSpeed;      //	リチャージ速度

	private float	attackTimeCount;		//	攻撃してからの経過時間
	private bool	isRecharging;           //	リチャージ中フラグ
	private bool	isAttacking;			//	攻撃中フラグ
	private bool	attackFlag;				//	攻撃フラグ（トリガーのためすぐfalseにする）
	private int		attackCount;			//	現在の攻撃回数


	//	初期化処理
	private void Start()
	{
		propertyBlock = new MaterialPropertyBlock();
	}

	//	更新処理
	protected override void Update()
	{
		if (disableUpdate)
			return;

		StateUpdate();              //	状態の更新処理

		//	リチャージ中
		if (isRecharging)
		{
			RechargeUpdate();
		}
		//	それ以外
		else
		{
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

		AnimationUpdate();          //	アニメーションの更新処理


	}

	/*--------------------------------------------------------------------------------
	|| 待機中の更新処理
	--------------------------------------------------------------------------------*/
	protected override void IdleUpdate()
	{
		if (attackTimeCount < attackInterval)
			attackTimeCount += Time.deltaTime;
	}

	/*--------------------------------------------------------------------------------
	|| 警戒中の更新処理
	--------------------------------------------------------------------------------*/
	protected override void AttentionUpdate()
	{
		if (attackTimeCount < attackInterval)
			attackTimeCount += Time.deltaTime;
	}

	/*--------------------------------------------------------------------------------
	|| 攻撃中の更新処理
	--------------------------------------------------------------------------------*/
	protected override void AttackUpdate()
	{
		//	攻撃した回数が一定数を超えていればリチャージに移行
		if(attackCount >= attackableCount)
		{
			isRecharging = true;
			return;
		}

		if(attackTimeCount < attackInterval)
		{
			attackTimeCount += Time.deltaTime;
		}
		else if(!isAttacking)
		{
			if(!isAttacking)
				StartCoroutine(Attack());
			attackTimeCount = 0;
		}
	}

	/*--------------------------------------------------------------------------------
	|| リチャージ中の更新処理
	--------------------------------------------------------------------------------*/
	private void RechargeUpdate()
	{
		//	リチャージのアニメーションじゃないときは処理しない
		if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Recharge@WaterEnemy"))
			return;

		//	リチャージを進行させる
		if(progress > 0)
		{
			targetProgress -= Time.deltaTime * rechargeSpeed;
		}
		//	進行度が0以下になったら初期化
		else
		{
			targetProgress = 0;
			attackCount = 0;
			isRecharging = false;
		}
	}

	/*--------------------------------------------------------------------------------
	|| 攻撃処理
	--------------------------------------------------------------------------------*/
	private IEnumerator Attack()
	{
		const int ATTACK_START_FRAME = 5;		//	攻撃開始フレーム
		const int ATTACKING_FRAME = 10;			//	攻撃中フレーム数

		//	攻撃中フラグを有効化
		isAttacking = true;

		//	アニメーション用のトリガーを有効化
		attackFlag = true;
		//	攻撃回数を加算
		attackCount++;
		//	シェーダーの進行度を更新
		targetProgress = (float)attackCount / attackableCount;

		//	攻撃開始フレームまで待機
		yield return new WaitForSeconds(ATTACK_START_FRAME / 60.0f);

		//	攻撃の生成
		for (int i = 0; i <= attackObjCount; i++)
		{
			float rad = Mathf.Lerp(Mathf.PI - Mathf.PI * (attackAngle / 2), Mathf.PI * (attackAngle / 2), i / (float)attackObjCount);
			float x = Mathf.Cos(rad);
			float y = Mathf.Sin(rad);

			var newFireBall = Instantiate(waterObj, anim.transform.position + attackObjInitOffset, Quaternion.identity);
			newFireBall.Direction = new Vector2(x, y);
			//	親を設定
			newFireBall.Parent = transform;

			//	一つの弾分の時間待機させる（攻撃の総時間 / 攻撃回数）
			yield return new WaitForSeconds((ATTACKING_FRAME / 60.0f) / (attackObjCount + 1));
		}

		//	攻撃中フラグを無効化
		isAttacking = false;
	}

	/*--------------------------------------------------------------------------------
	|| アニメーションの更新処理
	--------------------------------------------------------------------------------*/
	protected override void AnimationUpdate()
	{
		bool attention = currentState != State.IDLE;
		anim.SetBool("Attention", attention);

		anim.SetBool("Recharge", isRecharging);

		//	攻撃のトリガー
		if(attackFlag)
		{
			anim.SetTrigger("Attack");
			attackFlag = false;
		}

		ShaderUpdate();
	}

	/*--------------------------------------------------------------------------------
	|| シェーダーの更新処理
	--------------------------------------------------------------------------------*/
	private void ShaderUpdate()
	{
		progress = Mathf.Lerp(progress, targetProgress, Time.deltaTime * 10.0f);

		spriteRenderer.GetPropertyBlock(propertyBlock);
		propertyBlock.SetVector("_RootPosition", transform.position);
		propertyBlock.SetFloat("_Progress", this.progress);
		spriteRenderer.SetPropertyBlock(propertyBlock);
	}

	/*--------------------------------------------------------------------------------
	|| 梱包処理
	--------------------------------------------------------------------------------*/
	public CardboardType Packing()
	{
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
