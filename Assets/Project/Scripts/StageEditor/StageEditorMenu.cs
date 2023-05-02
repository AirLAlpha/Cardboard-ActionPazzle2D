using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class StageEditorMenu : MonoBehaviour
{
	//	メニューの項目
	private enum EditorMenuItem
	{
		EXPORT,		//	書き出し
		LOAD,		//	ロード
		RESET,		//	リセット
		EXIT,		//	終了

		OVER_ID
	}
	//	現在選択中の項目
	private EditorMenuItem currentSelectItem;

	private EditorMenuItem saveSelectItem;

	//	コンポーネント
	[Header("コンポーネント")]
	[SerializeField]
	private StageLoader		stageLoader;
	[SerializeField]
	private StageExporter	stageExporter;
	[SerializeField]
	private StageEditorManager editorManager;

	//	有効
	[Header("アクティブ")]
	[SerializeField]
	private RectTransform	menuRoot;
	[SerializeField]
	private Vector2			activePos;
	[SerializeField]
	private Vector2			deactivePos;
	[SerializeField]
	private float			menuMoveSpeed;

	public bool			IsActive { get; set; }      //	メニューの有効フラグ

	[Header("カーソル")]
	[SerializeField]
	private RectTransform	menuCursor;                 //	メニューカーソル
	[SerializeField]
	private Vector2			cursorOffset;				//	カーソルのズレ
	[SerializeField]
	private Vector2			cursorSpeed;                //	カーソルの速度
	[SerializeField]
	private float			cursorMaxWidth;				//	カーソルの横幅

	[Header("メニュー項目")]
	[Space, SerializeField]
	private RectTransform[] menuItemTransform;			//	各項目のRectTransform
	[SerializeField]
	private RectTransform[] textMasks;					//	テキストのマスク

	//	入力
	[Header("入力")]
	[SerializeField]
	private float			inputYInterval;

	private float			inputYTimer;
	private int				inputY;                     //	上下入力
	private bool			inputConfirm;				//	決定


	//	実行前初期化処理
	private void Awake()
	{
		
	}

	//	更新処理
	private void Update()
	{
		MenuActivateUpdate();

		if (!IsActive)
			return;

		MenuCursorUpdate();
		InputUpdate();
		SelectUpdate();

		if (inputConfirm)
			Confirm();
	}


	/*--------------------------------------------------------------------------------
	|| 入力処理
	--------------------------------------------------------------------------------*/
	private void InputUpdate()
	{
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
		nextSelect = (int)Mathf.Repeat(nextSelect, (int)EditorMenuItem.OVER_ID);

		//	値を保持
		saveSelectItem = currentSelectItem;
		//	選択を適応
		currentSelectItem = (EditorMenuItem)nextSelect;
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
		if(currentSelectItem != saveSelectItem)
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
	|| 決定時処理
	--------------------------------------------------------------------------------*/
	private void Confirm()
	{
		switch (currentSelectItem)
		{
			case EditorMenuItem.EXPORT:
				stageExporter.OpenSaveDialog();
				break;

			case EditorMenuItem.LOAD:
				stageLoader.OpenLoadDialog();
				break;

			case EditorMenuItem.RESET:
				stageLoader.ResetStage(false);
				break;

			case EditorMenuItem.EXIT:
				//editorManager.ExitEditor();
#if UNITY_EDITOR
				EditorApplication.isPlaying = false;
#else
				Application.Quit();
#endif

				break;

			default:
				break;
		}
	}

}
