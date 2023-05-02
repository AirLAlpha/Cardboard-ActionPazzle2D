using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public abstract class GimmickSettingBase : MonoBehaviour
{
	public RectTransform Cursor { get; set; }

	[SerializeField]
	protected RectTransform[] menuItems;
	[SerializeField]
	protected TextMeshProUGUI[] valueNameTexts;
	[SerializeField]
	protected TextMeshProUGUI[] valueTexts;

	protected int ValueTextsCount => valueTexts.Length;
	protected int[] values;

	[SerializeField]
	protected int currentSelect;

	/*--------------------------------------------------------------------------------
	|| 設定対象の設定
	--------------------------------------------------------------------------------*/
	public abstract void SetTarget<T>(T target);

	/*--------------------------------------------------------------------------------
	|| 項目を選択する処理
	--------------------------------------------------------------------------------*/
	public virtual void SelectUpdate(float inputY)
	{
		currentSelect -= (int)inputY;
		currentSelect = Mathf.Clamp(currentSelect, 0, menuItems.Length - 1);
		Cursor.position = new Vector2(Cursor.position.x, menuItems[currentSelect].position.y);

		for (int i = 0; i < menuItems.Length; i++)
		{
			bool isSelected = currentSelect == i;
			Color col = isSelected ? Color.black : Color.white;

			valueNameTexts[i].color = col;
			valueTexts[i].color = col;
		}
	}

	/*--------------------------------------------------------------------------------
	|| 項目の値を変更する処理
	--------------------------------------------------------------------------------*/
	public virtual void ValueChangeUpdate(float inputX)
	{
		values[currentSelect] += (int)inputX;
		valueTexts[currentSelect].text = values[currentSelect].ToString("D2");
	}

	/*--------------------------------------------------------------------------------
	|| 値の適応処理
	--------------------------------------------------------------------------------*/
	public abstract void ApplyValues();

	/*--------------------------------------------------------------------------------
	|| テキストをすべて書き換える処理
	--------------------------------------------------------------------------------*/
	protected virtual void ApplyTextAll()
	{
		for (int i = 0; i < valueTexts.Length; i++)
		{
			valueTexts[i].text = values[i].ToString("D2");
		}
	}

}


public class GimmickSettings : MonoBehaviour
{
	//	設定をまとめる構造体
	[System.Serializable]
	private struct GimmickSetting
	{
		[SerializeField]
		private string		settingName;	//	設定の名前
		[SerializeField]
		private GameObject itemPrefab;		//	設定のプレファブ

		public string		SettingName { get { return settingName; } }
		public GameObject	ItemPrefab	{ get { return itemPrefab; } }
	}

	//	アクティブ
	public bool IsActive { get; set; }

	//	入力
	[Header("入力")]
	[SerializeField]
	private float inputYInterval;
	[SerializeField]
	private float inputXInterval;

	private float inputYTimer;
	private float inputXTimer;
	private int inputY;                     //	上下入力
	private int inputX;                     //	左右入力
	private bool inputConfirm;              //	決定
	private bool inputCancel;

	//	ギミック
	private Gimmick				target;
	private GimmickSettingBase	targetSettings;

	//	テキスト
	[Header("テキスト")]
	[SerializeField]
	private TextMeshProUGUI windowTitleText;

	//	ルート
	[Header("ルート")]
	[SerializeField]
	private RectTransform windowRoot;
	//	カーソル
	[Header("カーソル")]
	[SerializeField]
	private RectTransform cursor;


	//	設定のプレファブ
	[Header("設定のプレファブ")]
	[SerializeField]
	private GimmickSetting doorSetting;     //	ドア
	[SerializeField]
	private GimmickSetting conveyorSetting;     //	ベルトコンベア


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
		InputUpdate();

		if (IsActive && inputCancel)
			MenuDeactivate();

		if (targetSettings == null)
			return;

		targetSettings.SelectUpdate(inputY);
		targetSettings.ValueChangeUpdate(inputX);
		targetSettings.ApplyValues();
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
		inputCancel = Input.GetButtonDown("Restart");
	}

	/*--------------------------------------------------------------------------------
	|| メニューの有効化
	--------------------------------------------------------------------------------*/
	public void MenuActivate(Gimmick target)
	{
		if(target.TryGetComponent<DoorGimmick>(out var door))
		{
			windowTitleText.text = doorSetting.SettingName;
			var obj = Instantiate(doorSetting.ItemPrefab, windowRoot) as GameObject;
			if(obj.TryGetComponent<GimmickSettingBase>(out targetSettings))
			{
				targetSettings.SetTarget(door);
			}
		}
		else if(target.TryGetComponent<BeltConveyorGimmick>(out var conveyor))
		{
			windowTitleText.text = conveyorSetting.SettingName;
			var obj = Instantiate(conveyorSetting.ItemPrefab, windowRoot);
			if(obj.TryGetComponent<GimmickSettingBase>(out targetSettings))
			{
				targetSettings.SetTarget(conveyor);
			}
		}
		else
		{
			//	それ以外なら処理しない
			return;
		}

		targetSettings.Cursor = cursor;

		this.IsActive = true;
		this.target = target;

		windowRoot.gameObject.SetActive(true);
	}

	/*--------------------------------------------------------------------------------
	|| メニューの無効化
	--------------------------------------------------------------------------------*/
	private void MenuDeactivate()
	{
		windowRoot.gameObject.SetActive(false);

		this.target = null;
		Destroy(targetSettings.gameObject);
		targetSettings = null;

		IsActive = false;
	}

}
