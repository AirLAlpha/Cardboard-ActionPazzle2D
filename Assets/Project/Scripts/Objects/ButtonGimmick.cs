/**********************************************
 * 
 *  ButtonGimmick.cs 
 *  ステージ上にあるボタンの処理を記述
 * 
 *  製作者：牛丸 文仁
 *  制作日：2023/04/04
 * 
 **********************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(SpriteRenderer))]
public class ButtonGimmick : MonoBehaviour
{
	//	コンポーネント
	private SpriteRenderer spriteRenderer;

	//	スプライト
	[Header("スプライト")]
	[SerializeField]
	private Sprite releasedSprite;
	[SerializeField]
	private Sprite pressedSprite;

	//	判定
	[Header("判定")]
	[SerializeField]
	private Vector2				checkArea;      //	判定エリア
	[SerializeField]
	private Vector2				checkOffset;
	[SerializeField]
	private LayerMask			pushableMask;   //	ボタンを押すことが出来るレイヤー

	//	判定エリアの中心座標
	private Vector3 CheckCenterPos => transform.position + new Vector3(checkOffset.x, checkOffset.y);

	//	投下
	public bool					IsPressed	{ get; private set; }       //	ボタンの投下フラグ
	private bool				saveIsPressed;							//	前回処理時のボタンの投下フラグ

	[Header("イベント")]
	[SerializeField]
	public 	UnityEvent			OnPressed;      //	ボタンが押されたタイミングで呼ばれる処理		
	[SerializeField]
	public UnityEvent			OnReleased;     //	ボタンが離されたタイミングで呼ばれる処理

#if UNITY_EDITOR
	[Header("デバッグ")]
	[SerializeField]
	private bool				drawGizmos;     //	ギズモの描画フラグ
#endif


	//	実行前初期化処理
	private void Awake()
	{
		//	コンポーネントの取得
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	//	初期化処理
	private void Start()
	{
		OnPressed.AddListener(()=> { Debug.Log("OnPressed"); });
		OnReleased.AddListener(()=> { Debug.Log("OnReleased"); });
	}

	//	更新処理
	private void Update()
	{
		CheckPressed();
	}

	/*--------------------------------------------------------------------------------
	|| 投下確認
	--------------------------------------------------------------------------------*/
	private void CheckPressed()
	{
		var a = Physics2D.OverlapBox(CheckCenterPos, checkArea, 0.0f, pushableMask);
		if (a == null)
		{
			IsPressed = false;

			spriteRenderer.sprite = releasedSprite;
		}
		else
		{
			IsPressed = true;

			spriteRenderer.sprite = pressedSprite;
		}


		if(IsPressed != saveIsPressed)
		{
			if(IsPressed)
			{
				OnPressed.Invoke();
			}
			else
			{
				OnReleased.Invoke();
			}

			saveIsPressed = IsPressed;
		}
	}

#if UNITY_EDITOR
	/*--------------------------------------------------------------------------------
	|| ギズモの描画処理
	--------------------------------------------------------------------------------*/
	private void OnDrawGizmosSelected()
	{
		//	ギズモの描画フラグが有効でないときは処理しない
		if (!drawGizmos)
			return;

		Color col = IsPressed ? Color.cyan : Color.red;
		col.a = 0.5f;
		Gizmos.color = col;

		Gizmos.DrawCube(CheckCenterPos, checkArea);
	}
#endif
}
