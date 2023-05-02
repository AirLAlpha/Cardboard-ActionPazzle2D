/**********************************************
 * 
 *  StageDetailMenu.cs 
 *  ステージエディタでステージの詳細を変更する処理
 * 
 *  製作者：牛丸 文仁
 *  制作日：2023/04/26
 * 
 **********************************************/
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;

public class StageDetailMenu : MonoBehaviour
{
	public enum DetailMenuItem
	{
		BACKGROUND_TYPE,
		USABLE_BOX_COUNT,
		TARGET_BOX_COUNT,

		OVER_ID
	}
	//	現在選択中の項目
	private DetailMenuItem	currentSelectItem;

	private DetailMenuItem	saveSelectItem;

	[SerializeField]
	private StageBackgroundDatabase bgDatabase;

	//	有効
	[Header("アクティブ")]
	[SerializeField]
	private RectTransform menuRoot;
	[SerializeField]
	private Vector2 activePos;
	[SerializeField]
	private Vector2 deactivePos;
	[SerializeField]
	private float menuMoveSpeed;

	public bool IsActive { get; set; }      //	メニューの有効フラグ

	[Header("カーソル")]
	[SerializeField]
	private RectTransform menuCursor;                 //	メニューカーソル
	[SerializeField]
	private Vector2 cursorOffset;               //	カーソルのズレ
	[SerializeField]
	private Vector2 cursorSpeed;                //	カーソルの速度
	[SerializeField]
	private float cursorMaxWidth;               //	カーソルの横幅

	[Header("メニュー項目")]
	[SerializeField]
	private RectTransform[]	menuItemTransform;
	[SerializeField]
	private RectTransform[]	textMasks;
	[SerializeField]
	private TextMeshProUGUI[] counters;

	//	入力
	[Header("入力")]
	[SerializeField]
	private float inputYInterval;
	[SerializeField]
	private float inputXInterval;

	private float inputYTimer;
	private float inputXTimer;
	private int inputY;                     //	上下入力
	private int inputX;						//	左右入力
	private bool inputConfirm;              //	決定

	//	カウンター
	public int UsableBoxCount { get; set; }
	public int TargetBoxCount { get; set; }
	public int BackgroundType { get; set; }


	//	実行前初期化処理
	private void Awake()
	{
	}

	//	初期化処理
	private void Start()
	{
	}

	//	更新処理
	private void Update()
	{
		MenuActivateUpdate();

		if (!IsActive)
			return;

		InputUpdate();
		CounterUpdate();
		SelectUpdate();
		MenuCursorUpdate();
	}

	/*--------------------------------------------------------------------------------
	|| 入力処理
	--------------------------------------------------------------------------------*/
	private void InputUpdate()
	{
		//	上下入力
		float vertical = Input.GetAxisRaw("Vertical") == 0 ? 0 : Mathf.Sign(Input.GetAxisRaw("Vertical"));
		float y = vertical + Input.GetAxisRaw("D-PadY");
		//	Y軸の入力がなくなったら入力を初期化し、タイマーをリセット
		if (y == 0)
		{
			inputY = 0;
			inputYTimer = 0;
		}
		//	タイマーのカウントが終了しているときは入力を受け付け、タイマーをセット
		else if (inputYTimer <= 0)
		{
			inputY = (int)y;
			inputYTimer = inputYInterval;
		}
		//	タイマーのカウント中は入力を初期化し、カウントする
		else if (inputYTimer > 0)
		{
			inputY = 0;
			inputYTimer -= Time.deltaTime;
		}

		//	左右入力
		float horizontal = Input.GetAxisRaw("Horizontal") == 0 ? 0 : Mathf.Sign(Input.GetAxisRaw("Horizontal"));
		float x = horizontal + Input.GetAxisRaw("D-PadX");
		//	Y軸の入力がなくなったら入力を初期化し、タイマーをリセット
		if (x == 0)
		{
			inputX = 0;
			inputXTimer = 0;
		}
		//	タイマーのカウントが終了しているときは入力を受け付け、タイマーをセット
		else if (inputXTimer <= 0)
		{
			inputX = (int)x;
			inputXTimer = inputXInterval;

			//print("InputX");
		}
		//	タイマーのカウント中は入力を初期化し、カウントする
		else if (inputXTimer > 0)
		{
			inputX = 0;
			inputXTimer -= Time.deltaTime;
		}


		//	決定ボタンの入力
		inputConfirm = Input.GetButtonDown("Jump");
	}

	/*--------------------------------------------------------------------------------
	|| 項目の選択処理
	--------------------------------------------------------------------------------*/
	private void SelectUpdate()
	{
		//	次の選択対象
		int nextSelect = (int)currentSelectItem - inputY;
		//	範囲外にならないようにクランプ
		nextSelect = (int)Mathf.Repeat(nextSelect, (int)DetailMenuItem.OVER_ID);

		//	値を保持
		saveSelectItem = currentSelectItem;
		//	選択を適応
		currentSelectItem = (DetailMenuItem)nextSelect;
	}

	/*--------------------------------------------------------------------------------
	|| カウンターの更新処理
	--------------------------------------------------------------------------------*/
	private void CounterUpdate()
	{
		//	テキストの更新に使用する値
		int counterNum = -1;

		//	現在選択中の値を加算
		switch (currentSelectItem)
		{
			case DetailMenuItem.BACKGROUND_TYPE:
				BackgroundType += inputX;
				BackgroundType = Mathf.Clamp(BackgroundType, 0, bgDatabase.DataCount - 1);
				counterNum = BackgroundType;
				break;

			case DetailMenuItem.USABLE_BOX_COUNT:
				UsableBoxCount += inputX;
				UsableBoxCount = Mathf.Clamp(UsableBoxCount, 0, 99);
				counterNum = UsableBoxCount;
				break;

			case DetailMenuItem.TARGET_BOX_COUNT:
				TargetBoxCount += inputX;
				TargetBoxCount = Mathf.Clamp(TargetBoxCount, 1, 99);
				counterNum = TargetBoxCount;
				break;

			default:
				break;
		}

		//	選択中のカウンターのテキストを更新
		counters[(int)currentSelectItem].text = counterNum.ToString("D2");
	}

	/*--------------------------------------------------------------------------------
	|| メニューのアクティブ処理
	--------------------------------------------------------------------------------*/
	private void MenuActivateUpdate()
	{
		Vector2 targetPos = IsActive ? activePos : deactivePos;

		Vector2 pos = menuRoot.anchoredPosition;
		menuRoot.anchoredPosition = Vector2.Lerp(pos, targetPos, Time.deltaTime * menuMoveSpeed);
	}

	/*--------------------------------------------------------------------------------
	|| カーソルの更新処理
	--------------------------------------------------------------------------------*/
	private void MenuCursorUpdate()
	{
		//	項目が切り替わった処理
		if (currentSelectItem != saveSelectItem)
		{
			//	カーソルの横幅を0にする
			menuCursor.sizeDelta = Vector2.up * menuCursor.sizeDelta.y;

			//	テキストマスクの幅を0にする
			for (int i = 0; i < textMasks.Length; i++)
			{
				textMasks[i].sizeDelta = Vector2.up * textMasks[i].sizeDelta.y;
			}
		}

		//	カーソルの目標座標
		Vector2 targetPos = new Vector2(menuCursor.anchoredPosition.x, menuItemTransform[(int)currentSelectItem].anchoredPosition.y + cursorOffset.y);
		//	カーソルを徐々に移動させる
		menuCursor.anchoredPosition = Vector2.Lerp(menuCursor.anchoredPosition, targetPos, Time.deltaTime * cursorSpeed.y);
		//	カーソルを徐々に大きくする
		menuCursor.sizeDelta = Vector2.Lerp(menuCursor.sizeDelta, new Vector2(cursorMaxWidth, menuCursor.sizeDelta.y), Time.deltaTime * cursorSpeed.x);
		//	テキストマスクを大きくする
		textMasks[(int)currentSelectItem].sizeDelta = menuCursor.sizeDelta - Vector2.right * 35;
	}

	/*--------------------------------------------------------------------------------
	|| テキストを一括で更新する
	--------------------------------------------------------------------------------*/
	public void CounterTextUpdateAll()
	{
		counters[(int)DetailMenuItem.BACKGROUND_TYPE].text = BackgroundType.ToString("D2");
		counters[(int)DetailMenuItem.USABLE_BOX_COUNT].text = UsableBoxCount.ToString("D2");
		counters[(int)DetailMenuItem.TARGET_BOX_COUNT].text = TargetBoxCount.ToString("D2");
	}

}
