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
using UnityEngine.Assertions.Must;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SoundPlayer))]
public class PlayerMove : MonoBehaviour, IPauseable
{
	//	コンポーネント
	[Header("コンポーネント")]
	[SerializeField]
	private SpriteRenderer	spriteRenderer; //	SpriteRenderer
	[SerializeField]
	private Animator		anim;           //	Animator

	private SoundPlayer		soundPlayer;
	private Rigidbody2D		rb;             //	Rigidbody2D

	public Rigidbody2D Rigidbody2D { get { return rb; } }

	//	入力
	public bool		DisableInput { get; set; }

	private Vector2 inputVec;               //	移動入力
	private bool	inputJump;              //	ジャンプ入力	
	private bool	saveInputJump;          //	ジャンプ入力の保持用変数

	//	移動
	[Header("移動")]
	[SerializeField] 
	private float		moveSpeed;					//	移動速度
	[SerializeField]
	private float		speedChangeRate;            //	移動速度の適応速度

	private float		velX;                       //	移動ベクトルX

	public Direction	CurrentDir	{ get; private set; }       //	現在の方向
	public bool			CantMove	{ get; set; }				//	移動不可能フラグ（true:移動不可）

	public Vector2		SecondaryVelocity { get; set; }

	//	ジャンプ
	[Header("ジャンプ")]
	[SerializeField]
	private float	jumpPower;                  //	ジャンプ力
	[SerializeField]
	private float	gravityScale;               //	通常時の重力
	[SerializeField]
	private float	heighJumpGravityScale;		//	高ジャンプ時の重力

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
	public bool			IsGrounded { get { return isGrounded; } }

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

	//	設置できなかったときのフラグ（トリガー）
	private bool				cantPut;

	//	ポーズ
	private Vector2				posedVelocity;
	private bool				disableAnimation;

	//	プロパティ
	public float MoveSpeed			{ get { return moveSpeed; } set { moveSpeed = value; } }
	public float RotateSpeed		{ get { return rotateSpeed; } set { rotateSpeed = value; } }
	public float SpeedChangeRate	{ get { return speedChangeRate; } }
	public bool CantPut				{ get { return cantPut; } set { cantPut = value; } }

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
		soundPlayer = GetComponent<SoundPlayer>();

		//	向きの初期化
		CurrentDir = Direction.RIGHT;

		Pause();
	}

	//	更新処理
	private void Update()
	{
		InputUpdate();			//	入力処理
		CheckGround();          //	接地確認処理

		DirectionUpdate();      //	向きの更新処理
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
		saveInputJump = inputJump;						//	保持しておく
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
		rb.velocity = moveVec + SecondaryVelocity;
	}

	/*--------------------------------------------------------------------------------
	|| ジャンプ処理
	--------------------------------------------------------------------------------*/
	private void JumpUpdate()
	{
		//	移動不可のときは処理しない
		if (CantMove)
			return;

		////	入力を解除していなく、入力があれば重力を軽くしておく
		//if(!inputJumpReleased && inputJump)
		//{
		//	rb.gravityScale = heighJumpGravityScale;
		//}
		//else
		//{
		//	rb.gravityScale = gravityScale;
		//}
		////	前フレームにジャンプ入力があった or ジャンプ入力がない or 接地していないときは処理しない
		//if (saveInputJump || !inputJump || !isGrounded) 
		//	return;

		if (!inputJump ||
			!isGrounded)
			return;

		//	ベクトルの加算
		//rb.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
		rb.velocity = Vector2.up * jumpPower;
		//	ジャンプフラグの有効化
		isJumping = true;

		//	移動SEの再生
		soundPlayer.PlaySound(0);
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
		{
			isRotate = true;
		}

		if (!isRotate)
			return;

		if(rotateInterval <= 0.0f)
		{
			//	移動SEの再生
			soundPlayer.PlaySound(2);
		}

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
		if (anim == null)
			return;

		if (disableAnimation)
			return;

		//	接地フラグ
		anim.SetBool("OnGround", isGrounded);
		//	ジャンプ中フラグ
		anim.SetBool("Jump", isJumping);
		//	自由落下フラグ
		anim.SetBool("FreeFall", isFreeFall);

		if (cantPut)
		{
			anim.SetTrigger("CantPut");
			cantPut = false;
		}
	}

	/*--------------------------------------------------------------------------------
	|| ポーズ処理
	--------------------------------------------------------------------------------*/
	public void Pause()
	{
		//	入力を無効化
		DisableInput = true;
		inputVec = Vector2.zero;
		inputJump = false;

		CantMove = true;
		disableAnimation = true;

		SecondaryVelocity = Vector2.zero;
		posedVelocity = rb.velocity;
		rb.velocity = Vector3.zero;
		rb.isKinematic = true;
		rb.velocity = Vector3.zero;

		//	アニメーションを停止
		if (anim != null)
			anim.SetFloat("AnimationSpeed", 0.0f);
	}
	/*--------------------------------------------------------------------------------
	|| 再開処理
	--------------------------------------------------------------------------------*/	
	public void Resume()
	{
		//	入力を有効化
		DisableInput = false;
		disableAnimation = false;

		CantMove = false;
		rb.isKinematic = false;
		rb.velocity = posedVelocity;

		//	アニメーションを再開
		if (anim != null)
			anim.SetFloat("AnimationSpeed", 1.0f);
	}

	/*--------------------------------------------------------------------------------
	|| ステージリセット時の処理
	--------------------------------------------------------------------------------*/
	public void OnStageReset()
	{
		if (!Application.isPlaying)
			return;

		rb.velocity = Vector2.zero;

		if (anim != null)
			anim.Play("Idle@Player");
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
