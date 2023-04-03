/**********************************************
 * 
 *  FireBall.cs 
 *  炎の丸に関する処理を記述
 * 
 *  製作者：牛丸 文仁
 *  制作日：2023/02/01
 * 
 **********************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//	燃やすことが出来るオブジェクトに付与するインターフェース
public interface IBurnable
{
	public abstract void Burn();		//	燃えたときに呼ばれる処理
}

[RequireComponent(typeof(Rigidbody2D))]
public class FireBall : MonoBehaviour
{
	//	コンポーネント
	private Rigidbody2D rb;         //	Rigidbody2D

	//	移動
	[Header("移動")]
	[SerializeField]
	private float		moveSpeed;					//	移動速度

	public Vector2		Direction { get; set; }		//	移動方向ベクトル


	//	実行前初期化処理
	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
	}

	//	更新処理
	private void Update()
	{
		rb.velocity = Direction * moveSpeed * Time.deltaTime;
	}

	/*--------------------------------------------------------------------------------
	|| 衝突判定
	--------------------------------------------------------------------------------*/
	private void OnTriggerEnter2D(Collider2D collision)
	{
		//	衝突したオブジェクトにIBurnableが実装されていたら処理を行う
		if (collision.TryGetComponent<IBurnable>(out var hit))
		{
			//	相手を燃やす処理
			hit.Burn();
			//	自身を削除する
			Destroy(gameObject);
		}

	}
}
