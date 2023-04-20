/**********************************************
 * 
 *  PlayerMove.cs 
 *  プレイヤーの移動に関する処理を記述
 * 
 *  製作者：牛丸 文仁
 *  制作日：2023/04/02
 * 
 **********************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMove : MonoBehaviour
{
	//	コンポーネント
	[Header("コンポーネント")]
	[SerializeField]
	private SpriteRenderer	spriteRenderer; //	SpriteRenderer
	[SerializeField]
	private Animator		anim;           //	Animator

	private Rigidbody2D		rb;             //	Rigidbody2D

	//	入力
	public bool		DisableInput { get; set; }

	private Vector2 inputVec;               //	移動入力
	private bool	inputJump;				//	ジャンプ入力	


	//	移動
	[Header("移動")]
	[SerializeField] 
	private float		moveSpeed;					//	移動速度
	[SerializeField]
	private float		speedChangeRate;            //	移動速度の適応速度

	private float		velX;						//	移動ベクトルX

	public Direction	CurrentDir	{ get; private set; }       //	現在の方向
	public bool			CantMove	{ get; set; }				//	移動不可能フラグ（true:移動不可）

	//	ジャンプ
	[Header("ジャンプ")]
	[SerializeField]
	private float	jumpPower;                  //	ジャンプ力

	private bool	isFreeFall;					//	自由落下フラグ
	private bool	isJumping;					//	ジャンプ中フラグ

	//	接地
	[Header("接地")]
	[SerializeField]
	private Vector2		groundCheckArea;        //	地面の確認範囲
	[SerializeField]
	private Vector2		groundCheckOffset;		//	範囲のズレ
	[SerializeField]
	private LayerMask	groundMask;             //	地面とするレイヤー

	//	地面の確認範囲の中心座標
	private Vector2		GroundCheckCenter => new Vector2(transform.position.x, transform.position.y) + groundCheckOffset;

	private bool		isGrounded;             //	接地フラグ

	//	移動アニメーション
	[Header("回転アニメーション")]
	[SerializeField]
	private Transform			spriteRoot;
	[SerializeField]
	private SpriteRenderer		cap;
	[SerializeField]
	private AnimationCurve		rotateCurve;
	[SerializeField]
	private float				rotateSpeed;
	[SerializeField]
	private float				capJumpHeight;

	private float				rotateInterval;
	private float				currentAngle;
	private bool				isRotate;               //	回転中
	private int					rotateCount;

	//	プロパティ
	public float MoveSpeed			{ get { return moveSpeed; } set { moveSpeed = value; } }
	public float RotateSpeed		{ get { return rotateSpeed; } set { rotateSpeed = value; } }
	public float SpeedChangeRate	{ get { return speedChangeRate; } }


#if UNITY_EDITOR
	//	デバッグ
	[Header("デバッグ")]
	[SerializeField]
	private bool		drawGizmos;				//	ギズモの描画フラグ
#endif

	//	実行前初期化処理
	private void Awake()
	{
		//	コンポーネントの取得
		rb = GetComponent<Rigidbody2D>();       //	Rigidbody2D

		//	向きの初期化
		CurrentDir = Direction.RIGHT;
	}

	//	更新処理
	private void Update()
	{
		InputUpdate();			//	入力処理
		CheckGround();          //	接地確認処理

		DirectionUpdate();		//	向きの更新処理
		JumpUpdate();			//	ジャンプの更新処理
		RotateUpdate();         //	回転処理

		AnimationUpdate();		//	アニメーションの更新処理
	}

	//	等間隔更新処理
	private void FixedUpdate()
	{
		MoveUpdate();		//	移動処理
	}

	/*--------------------------------------------------------------------------------
	|| 入力処理
	--------------------------------------------------------------------------------*/
	private void InputUpdate()
	{
		//	入力をリセット
		inputVec = Vector2.zero;

		if (DisableInput)
			return;

		//	移動入力
		inputVec.x = Input.GetAxisRaw("Horizontal") + Input.GetAxisRaw("D-PadX");        //	X
		inputVec.y = Input.GetAxisRaw("Vertical") + Input.GetAxisRaw("D-PadY");          //	Y

		inputVec.x = Mathf.Clamp(inputVec.x, -1, 1);
		inputVec.y = Mathf.Clamp(inputVec.y, -1, 1);


		//	ジャンプ入力
		inputJump = Input.GetButtonDown("Jump");			//	ジャンプ
	}

	/*--------------------------------------------------------------------------------
	|| 向きの更新処理
	--------------------------------------------------------------------------------*/
	private void DirectionUpdate()
	{
		if (inputVec.x > 0)
			CurrentDir = Direction.RIGHT;
		else if (inputVec.x < 0)
			CurrentDir = Direction.LEFT;

		//	帽子が設定されていないときは処理しない
		if (cap == null)
			return;

		//	向いている方向に応じて帽子を反転させる
		if (CurrentDir == Direction.RIGHT)
			cap.flipX = false;
		else
			cap.flipX = true;
	}

	/*--------------------------------------------------------------------------------
	|| 移動処理
	--------------------------------------------------------------------------------*/
	private void MoveUpdate()
	{
		//	移動不可のときは処理しない
		if (CantMove)
			return;

		//	重力を含むY軸のベクトルを保持しておく
		float velY = rb.velocity.y;
		//	X軸のベクトルを作成し、現在のベクトルから徐々に遷移させる
		float targetVelX = inputVec.x * moveSpeed;
		velX = Mathf.Lerp(velX, targetVelX, Time.deltaTime * speedChangeRate);

		//	移動ベクトルを作成
		Vector2 moveVec = Vector2.right * velX + Vector2.up * velY;

		//	移動の適応
		rb.velocity = moveVec;
	}

	/*--------------------------------------------------------------------------------
	|| ジャンプ処理
	--------------------------------------------------------------------------------*/
	private void JumpUpdate()
	{
		//	移動不可のときは処理しない
		if (CantMove)
			return;

		//	ジャンプ入力がない or 接地していないときは処理しない
		if (!inputJump || !isGrounded) 
			return;

		//	ベクトルの加算
		rb.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
		//	ジャンプフラグの有効化
		isJumping = true;
	}

	/*--------------------------------------------------------------------------------
	|| 接地確認処理
	--------------------------------------------------------------------------------*/
	private void CheckGround()
	{
		//	指定した範囲内にコライダーがあるか確認
		var hitResult = Physics2D.OverlapBox(
			GroundCheckCenter,						//	中心座標
			groundCheckArea / 2,					//	サイズの半分
			0.0f,									//	回転角度
			groundMask								//	レイヤーマスク
			);

		if (hitResult == null)
		{
			//	なににも衝突していない
			isGrounded = false;

			isFreeFall = true;
		}
		else
		{
			//	何かしらに衝突した
			isGrounded = true;

			//	各フラグをリセット
			isJumping = false;
			isFreeFall = false;
		}
	}

	/*--------------------------------------------------------------------------------
	|| 回転処理
	--------------------------------------------------------------------------------*/
	private void RotateUpdate()
	{
		if (Mathf.Abs(inputVec.x) > 0 &&
			isGrounded)
			isRotate = true;

		if (!isRotate)
			return;

		rotateInterval += Time.deltaTime * rotateSpeed;
		if(rotateInterval >= 1.0f)
		{
			rotateInterval = 0.0f;

			isRotate = false;

			rotateCount++;
			if (rotateCount >= 4)
				rotateCount = 0;
		}


		//float curvePos = rotateCurve.Evaluate(rotateInterval);
		float curvePos = Ease(rotateInterval);
		float rot = (Mathf.PI / 2) * curvePos;

		//currentAngle = Mathf.Repeat((rotateCount * Mathf.PI / 2) + rot, Mathf.PI * 2);
		currentAngle = rot + rotateCount * Mathf.PI / 2 ;

		//	回転を適応
		spriteRenderer.transform.rotation = Quaternion.Euler(new Vector3(0, 0, -currentAngle * Mathf.Rad2Deg * (int)CurrentDir));

		//	進行度からキャップを動かす
		float capY = (-Mathf.Cos(rotateInterval * Mathf.PI * 2) + 1.0f) / 2;
		cap.transform.localPosition = Vector2.up + Vector2.up * capY * capJumpHeight;
	}
	private float Ease(float x)
	{
		return x * x;
	}

	/*--------------------------------------------------------------------------------
	|| アニメーションの更新処理
	--------------------------------------------------------------------------------*/
	private void AnimationUpdate()
	{
		//	接地フラグ
		anim.SetBool("OnGround", isGrounded);
		//	ジャンプ中フラグ
		anim.SetBool("Jump", isJumping);
		//	自由落下フラグ
		anim.SetBool("FreeFall", isFreeFall);
	}

#if UNITY_EDITOR
	/*--------------------------------------------------------------------------------
	|| ギズモの描画処理
	--------------------------------------------------------------------------------*/
	private void OnDrawGizmosSelected()
	{
		//	フラグが向こうなときは処理しない
		if (!drawGizmos)
			return;

		//	接地確認

		//	色の変更
		const float ALPHA = 0.5f;
		Color col = isGrounded ? Color.cyan : Color.red;
		col.a = ALPHA;
		Gizmos.color = col;

		//	CheckAreaの描画
		Gizmos.DrawCube(GroundCheckCenter, groundCheckArea);
	}
#endif
}
