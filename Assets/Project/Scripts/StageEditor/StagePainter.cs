/**********************************************
 * 
 *  StagePainiter.cs 
 *  ステージにオブジェクトを配置する処理を記述
 * 
 *  製作者：牛丸 文仁
 *  制作日：2023/04/23
 * 
 **********************************************/
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Rendering.HybridV2;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class StagePainter : MonoBehaviour
{
	//	モード
	private enum EditorMode
	{
		PAINTER,		//	オブジェクトの配置、削除
		GIMMICK,		//	ギミックの紐づけ
	}
	private EditorMode mode;

	[Header("データベース")]
	[SerializeField]
	private StageObjectDatabase database;       //	ステージオブジェクトのデータベース

	[Header("コンポーネント")]
	[SerializeField]
	private StageObjectPallet	pallet;

	private LineRenderer		lineRenderer;

	[Header("カメラ")]
	[SerializeField]
	private Camera			editorCamera;       //	エディタ用のカメラ

	[Header("プレイヤー")]
	[SerializeField]
	private PlayerMove		player;

	[Header("ステージ")]
	[SerializeField]
	private Tilemap			tilemap;			//	ステージのグリッド
	[SerializeField]
	private Transform		enemyRoot;			//	敵のルートオブジェクト
	[SerializeField]
	private Transform		gimmickRoot;        //	ギミックのルートオブジェクト

	[Header("編集")]
	[SerializeField]
	private bool			editActive;         //	編集可能フラグ
	[SerializeField]
	private RectInt			editableArea;       //	編集可能範囲
	[SerializeField]
	private bool			drawGizmos;

	[Header("選択")]
	[SerializeField]
	private Image			cursor;				//	カーソル
	[SerializeField]
	private Transform		gridCursor;         //	グリッドのカーソル
	[SerializeField]
	private float			cursorSpeed;        //	カーソルの移動速度
	[SerializeField]
	private Color			paintModeCursorColor;
	[SerializeField]
	private Color			gimmickModeCurosrColor;
	[SerializeField]
	private LayerMask		eraseMask;

	private RectTransform	cursorTransform;    
	private Vector3Int		selectedCell;       //	選択中のマス
	private Vector2			selectedCellCenter;	//	選択中のマスの中心

	//	グリッドカーソルのズレ（グリッドに合わせる）
	static readonly Vector3 GRID_CURSOR_OFFSET = new Vector3(0.5f, 0.5f, 0.0f);

	//	入力
	[Header("入力")]
	private Vector2			inputVec;

	private bool			inputConfirm;		//	決定キーの入力（設置）
	private bool			inputCancel;        //	キャンセルキーの入力（削除）
	private bool			inputToggleMode;    //	モード切替の入力
	private int				inputLR;			//	LRボタンの入力(-1:L, 0:None, 1:R)

	private bool			saveInputConfirm;	//	保持用
	private bool			saveInputCancel;	//	保持用

	private bool			enableMouse;        //	マウスの有効化

	//	ギミック
	[Header("ギミック")]
	[SerializeField]
	private GimmickSettings gimmickSettings;

	private Gimmick			firstGimmick;
	private Gimmick			secondGimmick;

	//	実行前初期化処理
	private void Awake()
	{
		//	コンポーネントの取得
		this.lineRenderer = GetComponent<LineRenderer>();
		lineRenderer.SetPositions(new Vector3[2]);
		lineRenderer.enabled = false;

		//	マウスカーソルを非表示に設定
		Cursor.visible = false;

		//	カーソルのトランスフォームを取得
		if (cursor != null)
			cursorTransform = cursor.transform as RectTransform;
	}

	//	初期化処理
	private void Start()
	{
		
	}

	//	更新処理
	private void Update()
	{
		//	編集モードでなければ処理しない
		if (!editActive)
			return;

		//	ギミックの設定中は処理しない
		if (gimmickSettings.IsActive)
			return;

		//	入力処理
		InputUpdate();

		//	カーソルの更新処理
		CursorUpdate();

		switch (mode)
		{
			case EditorMode.PAINTER:		//	オブジェクトの設置、削除
				PainterModeUpdate();
				break;

			case EditorMode.GIMMICK:		//	ギミックの接続
				GimmickModeUpdate();
				break;
		}

		//	モードの切替
		if(inputToggleMode)
		{
			switch (mode)
			{
				case EditorMode.PAINTER:
					mode = EditorMode.GIMMICK;
					cursor.color = gimmickModeCurosrColor;
					pallet.IsActive = false;
					break;

				case EditorMode.GIMMICK:
					mode = EditorMode.PAINTER;
					cursor.color = paintModeCursorColor;
					pallet.IsActive = true;
					break;
			}
		}

		saveInputConfirm = inputConfirm;
		saveInputCancel = inputCancel;
	}

	/*--------------------------------------------------------------------------------
	|| ペイントモードの更新処理
	--------------------------------------------------------------------------------*/
	private void PainterModeUpdate()
	{
		if (inputConfirm)
		{
			//	押された瞬間にギミックを取得する
			if (!saveInputConfirm)
			{
				firstGimmick = GetCursorHitGimmick();

				//	ギミックが見つかればパレットを固定
				if (firstGimmick != null)
				{
					//	パレットの入力を無効化しカーソルを非表示
					pallet.DisableInput = true;
					SetCursorActive(false);
				}
			}

			//	ギミックがあれば回転の処理をする
			if (firstGimmick != null &&
				firstGimmick.Rotatable)
			{
				RotateObject(firstGimmick.transform);
			}
			//	ギミックがなければ設置処理へ
			else
			{
				SetObject(pallet.SelectedIndex);
			}
		}
		//	クリックを解除したら初期化
		else if(saveInputConfirm)
		{
			//	ギミックを初期化
			firstGimmick = null;
			secondGimmick = null;

			//	パレットの入力無効を解除
			pallet.DisableInput = false;
			//	カーソルを表示
			SetCursorActive(true);
		}

		if (inputCancel)
			EraseObject();
	}

	/*--------------------------------------------------------------------------------
	|| ギミックモードの更新処理
	--------------------------------------------------------------------------------*/
	private void GimmickModeUpdate()
	{
		SetGimmickSender();
		SetGimmickExSetting();
	}

	/*--------------------------------------------------------------------------------
	|| 入力の更新処理
	--------------------------------------------------------------------------------*/
	private void InputUpdate()
	{
		//	上下左右の入力を取得
		inputVec.x = Input.GetAxis("Horizontal") + Input.GetAxisRaw("D-PadX");
		inputVec.y = Input.GetAxis("Vertical") + Input.GetAxisRaw("D-PadY");

		inputVec.x = Mathf.Clamp(inputVec.x, -1, 1);
		inputVec.y = Mathf.Clamp(inputVec.y, -1, 1);

		//	マウスの入力
		Vector2 mouseInput = Vector2.zero;
		mouseInput.x = Input.GetAxis("Mouse X");
		mouseInput.y = Input.GetAxis("Mouse Y");

		//	マウスが有効でないときにマウスが入力されたら、マウスを有効化する
		if(!enableMouse && Mathf.Abs(mouseInput.sqrMagnitude) > 0.0f)
		{
			enableMouse = true;
		}

		//	マウスが有効なときにキー入力がされたら、マウスを無効化する
		if(enableMouse && Mathf.Abs(inputVec.sqrMagnitude) > 0.0f)
		{
			enableMouse = false;
		}

		//	決定キーの入力
		inputConfirm = Input.GetButton("Jump") || Input.GetMouseButton(0);
		//	キャンセルキーの入力
		inputCancel = Input.GetButton("Fire1") || Input.GetMouseButton(1);
		//	モード切替の入力
		inputToggleMode = Input.GetButtonDown("ToggleEditorMode");

		//	LRボタンの入力
		int lr = 0;
		if (Input.GetButtonDown("EditorPalletPlus"))
			lr += 1;
		if (Input.GetButtonDown("EditorPalletMinus"))
			lr -= 1;
		inputLR = lr;
	}

	/*--------------------------------------------------------------------------------
	|| カーソルの更新処理
	--------------------------------------------------------------------------------*/
	private void CursorUpdate()
	{
		//	カーソルの座標を取得
		Vector3 cursorPos = cursorTransform.localPosition;

		//	マウスが有効なときはマウスに追従させる
		if (enableMouse)
		{
			//	マウスの座標を取得
			cursorPos = Input.mousePosition;
			//	カーソルの「ワールド座標」に適応する
			cursorTransform.position = cursorPos;
		}
		else
		{
			//	カーソルに入力を適応
			cursorPos += new Vector3(inputVec.x, inputVec.y).normalized * cursorSpeed * Time.deltaTime;
			float w = Screen.width * 0.5f;
			float h = Screen.height * 0.5f;
			cursorPos.x = Mathf.Clamp(cursorPos.x, -w, w);
			cursorPos.y = Mathf.Clamp(cursorPos.y, -h, h);

			//	カーソルの位置を更新
			cursorTransform.localPosition = cursorPos;
		}

		//	カーソルの座標から選択中のセルを更新
		Vector3 cursorWorldPos = editorCamera.ScreenToWorldPoint(cursorTransform.position - GRID_CURSOR_OFFSET);
		selectedCell = Vector3Int.FloorToInt(cursorWorldPos);
		selectedCell.z = 0;
		//	選択中のセルの中心座標を更新
		selectedCellCenter = new Vector3(selectedCell.x, selectedCell.y, 0) + GRID_CURSOR_OFFSET;

		//	グリッドカーソルの座標を更新
		gridCursor.position = selectedCellCenter;
	}

	/*--------------------------------------------------------------------------------
	|| オブジェクトの配置処理
	--------------------------------------------------------------------------------*/
	private void SetObject(int dbIndex)
	{
		//	カーソルが編集可能範囲外にあれば処理しない
		if (editableArea.x > selectedCell.x || selectedCell.x > editableArea.width - Mathf.Abs(editableArea.x) ||
			editableArea.y > selectedCell.y || selectedCell.y > editableArea.height - Mathf.Abs(editableArea.y))
			return;

		//	データベースからオブジェクトを取得
		StageObject obj = database.StageObject[dbIndex];
		//	カーソルの下にあるオブジェクトを取得
		var onCursorObject = Physics2D.OverlapBox(selectedCellCenter, Vector2.one * 0.5f, 0);
		
		//	タイル
		if(obj.type == ObjectType.TILE)
		{
			//	カーソルの下にオブジェクトがあり、タイルマップじゃなければ処理しない
			if (onCursorObject != null && onCursorObject.gameObject != tilemap.gameObject)
				return;

				tilemap.SetTile(selectedCell, obj.prefab as Tile);
			return;
		}

		//	カーソルの下にオブジェクトがある時点で処理しない
		if (onCursorObject != null)
			return;
		//	オブジェクトの設置 or 移動
		switch (obj.type)
		{
			case ObjectType.GIMMICK:
				GameObject newGimmick = Instantiate(obj.prefab, selectedCellCenter, Quaternion.identity, gimmickRoot) as GameObject;
				if(newGimmick.TryGetComponent<IPauseable>(out IPauseable gimmickPause))
				{
					gimmickPause.Pause();
				}
				break;

			case ObjectType.ENEMY:
				GameObject newEnemy = Instantiate(obj.prefab, selectedCellCenter, Quaternion.identity, enemyRoot) as GameObject;
				if (newEnemy.TryGetComponent<IPauseable>(out IPauseable enemyPause))
				{
					enemyPause.Pause();
				}
				break;

			case ObjectType.PLAYER:
				player.transform.position = new Vector3(selectedCellCenter.x, selectedCellCenter.y);
				break;
		}
	}

	/*--------------------------------------------------------------------------------
	|| オブジェクトの回転処理
	--------------------------------------------------------------------------------*/
	private void RotateObject(Transform gimmick)
	{
		gimmick.rotation *= Quaternion.AngleAxis(90.0f * -inputLR, Vector3.forward);
	}

	/*--------------------------------------------------------------------------------
	|| オブジェクトの削除処理
	--------------------------------------------------------------------------------*/
	private void EraseObject()
	{
		//	削除するオブジェクトがあるか確認する
		var obj = Physics2D.OverlapBox(selectedCellCenter, Vector2.one * 0.5f, 0, eraseMask);
		if (obj == null)
			return;

		if(obj.gameObject == tilemap.gameObject)
		{
			this.tilemap.SetTile(selectedCell, null);
			return;
		}

		//	ルートオブジェクトのときは処理しない
		if (obj.gameObject == enemyRoot.gameObject ||
			obj.gameObject == gimmickRoot.gameObject)
			return;

		string tag = obj.transform.tag;
		switch (tag)
		{
			//	ギミック
			case "Gimmick":
				Destroy(obj.transform.parent.gameObject);
				break;

			//	敵
			case "Enemy":
				Destroy(obj.transform.gameObject);
				break;

			default:
				return;
		}
	}

	/*--------------------------------------------------------------------------------
	|| ギミックの紐づけ
	--------------------------------------------------------------------------------*/
	private void SetGimmickSender()
	{
		//	ボタンが入力された瞬間にギミックの取得を行う
		if (inputCancel && !saveInputCancel)
		{
			Gimmick gim = GetCursorHitGimmick();
			firstGimmick = gim;

			//	ギミックが見つかればLineRenderを有効化
			if (firstGimmick != null &&
				firstGimmick.Type != GimmickType.EVENT_NONE)
			{
				lineRenderer.enabled = true;
				lineRenderer.SetPosition(0, firstGimmick.transform.position);
			}
		}

		//	ボタンが押されている間
		if(inputCancel)
		{
			//	ギミックが設定されているなら線を描画する
			if (firstGimmick != null)
			{
				//	接続先のオブジェクトを取得する
				Gimmick gim = GetCursorHitGimmick();
				secondGimmick = gim;

				Vector3 linePos = Vector3.zero;
				if (secondGimmick != null &&
					secondGimmick.Type != GimmickType.EVENT_NONE)
				{
					linePos = secondGimmick.transform.position;
				}
				else
				{
					linePos = editorCamera.ScreenToWorldPoint(cursorTransform.position - GRID_CURSOR_OFFSET);
				}

				lineRenderer.SetPosition(1, linePos);
			}
		}

		//	キーが離された瞬間
		if(!inputCancel && saveInputCancel)
		{
			//	LineRenderを無効化
			lineRenderer.enabled = false;

			//	接続元がなければ処理しない
			if (firstGimmick == null)
				return;

			//	なければ処理しない	
			if(secondGimmick == null)
			{
				firstGimmick = null;
				return;
			}

			//	どちらも同じギミックのときはオブジェクト固有の設定を表示する
			if(firstGimmick == secondGimmick)
			{
				print("Configurate");
			}

			//	接続できるか確認して、可能なら接続する
			if(CheckConnectableGimmick(out ReceiveGimmick receive, out SendGimmick send))
			{
				//	どちらか片方でも取得できていなければ処理しない
				if (receive == null || send == null)
					return;

				receive.Sender = send;

				Debug.Log(receive.name + " : " + send.name + " を接続しました。");
			}

			//	ギミックのリセット
			firstGimmick = null;
			secondGimmick = null;
		}
	}

	/*--------------------------------------------------------------------------------
	|| ギミック固有の設定
	--------------------------------------------------------------------------------*/
	private void SetGimmickExSetting()
	{
		if(inputConfirm && !saveInputConfirm)
		{
			var gim = GetCursorHitGimmick();
			if (gim == null)
				return;

			gimmickSettings.MenuActivate(gim);
		}
	}

	/*--------------------------------------------------------------------------------
	|| カーソル上のギミックを取得
	--------------------------------------------------------------------------------*/
	private Gimmick GetCursorHitGimmick()
	{
		var cursorHitObj = Physics2D.OverlapBox(selectedCellCenter, Vector2.one * 0.5f, 0, eraseMask);
		if (cursorHitObj == null)
			return null;
		if (cursorHitObj.tag != "Gimmick")
			return null;
		if (!cursorHitObj.transform.parent.TryGetComponent<Gimmick>(out Gimmick gimmick))
			return null;

		return gimmick;
	}

	/*--------------------------------------------------------------------------------
	|| ギミックを接続できるか確認する
	--------------------------------------------------------------------------------*/
	private bool CheckConnectableGimmick(out ReceiveGimmick receive, out SendGimmick send)
	{
		receive = null;
		send = null;

		if(firstGimmick.TryGetComponent<ReceiveGimmick>(out receive))
		{
			if(!secondGimmick.TryGetComponent<SendGimmick>(out send))
			{
				return false;
			}
		}
		else if(secondGimmick.TryGetComponent<ReceiveGimmick>(out receive))
		{
			if(!firstGimmick.TryGetComponent<SendGimmick>(out send))
			{
				return false;
			}
		}

		return true;
	}

	/*--------------------------------------------------------------------------------
	|| 未接続のギミックが無いか確認する
	--------------------------------------------------------------------------------*/
	public bool CheckConnectedGimmick()
	{
		List<GameObject> gimmickObjects = new List<GameObject>();
		gimmickObjects.AddRange(GameObject.FindGameObjectsWithTag("Gimmick"));
		gimmickObjects.AddRange(GameObject.FindGameObjectsWithTag("GimmickEnemy"));

		foreach (var obj in gimmickObjects)
		{
			if(obj.TryGetComponent<ReceiveGimmick>(out ReceiveGimmick gimmick))
			{
				if (gimmick.Type != GimmickType.EVENT_RECEIVE)
					continue;

				if (gimmick.Sender == null)
					return false;
			}
		}

		return true;
	}

	/*--------------------------------------------------------------------------------
	|| 編集モードに入ったときの初期化処理
	--------------------------------------------------------------------------------*/
	public void Enable()
	{
		if (mode == EditorMode.GIMMICK)
			pallet.IsActive = false;

		//	カーソルを有効化
		cursorTransform.gameObject.SetActive(true);
		gridCursor.gameObject.SetActive(true);

		//	編集状態にする
		editActive = true;
	}

	/*--------------------------------------------------------------------------------
	|| プレイモードに入ったときの初期化処理
	--------------------------------------------------------------------------------*/
	public void Disable()
	{
		//	編集状態を無効化
		editActive = false;

		//	カーソルを無効化
		cursorTransform.gameObject.SetActive(false);
		gridCursor.gameObject.SetActive(false);
	}

	/*--------------------------------------------------------------------------------
	|| カーソルの切り替え処理
	--------------------------------------------------------------------------------*/
	private void SetCursorActive(bool active)
	{
		//	カーソルの切り替え
		cursorTransform.gameObject.SetActive(active);
		gridCursor.gameObject.SetActive(active);
	}

#if UNITY_EDITOR
	private void OnDrawGizmosSelected()
	{
		if (!drawGizmos)
			return;

		Gizmos.color = new Color(1, 0, 0, 0.5f);

		Vector2 center = editableArea.center;
		Vector2 size = editableArea.size;
		Gizmos.DrawCube(center, size);
	}
#endif
}
