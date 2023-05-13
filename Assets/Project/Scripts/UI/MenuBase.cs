using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public abstract class MenuBase : MonoBehaviour
{
	//	入力
	[Header("入力")]
	[SerializeField]
	private bool	activateX;          //	X軸入力の有効化フラグ
	[SerializeField]
	private bool	activateY;			//	Y軸入力の有効化フラグ
	[SerializeField]
	private float	inputXInterval;
	[SerializeField]
	private float	inputYInterval;

	private float inputYTimer;
	private float inputXTimer;

	private Vector2 inputVec;       //	軸の入力
	private bool	inputConfirm;   //	決定
	private bool	inputCancel;		//	キャンセル

	//	インデックス
	private int currentIndex;       //	現在選択中のインデックス

	//	カーソル
	[Header("カーソル")]
	[SerializeField]
	private RectTransform	menuCursor;			//	メニューカーソル
	[SerializeField]
	private float			cursorSpeed;		//	カーソル速度
	[SerializeField]
	private Vector2			cursorOffset;		//	カーソルのズレ
	[SerializeField]
	private float			animationMinSize;	//	アニメーションの最小サイズ
	[SerializeField]
	private float			animationMaxSize;	//	アニメーションの最大サイズ
	[SerializeField]
	private float			animationSpeed;     //	アニメーション速度

	private float			animProgress;		//	アニメーション進行度

	//	メニュー項目
	[Header("メニュー項目")]
	[SerializeField]
	private Sprite			selectedSprite;		//	選択中のスプライト
	[SerializeField]
	private Sprite			nonselectedSprite;	//	非選択中のスプライト
	[SerializeField]
	private Image[]			menuItem;			//	メニュー項目

	public int		MaxIndex => menuItem.Length;

	//	プロパティ
	public int		CurrentIndex	{ get { return currentIndex; } }
	public Vector2	InputVec		{ get { return inputVec; } }
	public bool		InputConfirm	{ get { return inputConfirm; } }
	public bool		InputCancel		{ get { return inputCancel; } }

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
		InputUpdate();      //	入力処理
		SelectUpdate();     //	選択処理

		MenuUpdate();       //	メニューの更新処理

		CursorAnimationUpdate();	//	カーソルのアニメーション処理
	}

	/*--------------------------------------------------------------------------------
	|| 入力処理
	--------------------------------------------------------------------------------*/
	private void InputUpdate()
	{
		//	左右入力
		if (activateX)
		{
			inputVec.x = MenuAxisInput(inputXInterval, ref inputXTimer, "Horizontal", "D-PadX");
		}
		else
		{
			inputVec.x = 0;
		}

		//	上下入力
		if (activateY)
		{
			inputVec.y = MenuAxisInput(inputYInterval, ref inputYTimer, "Vertical", "D-PadY");
		}
		else
		{
			inputVec.y = 0;
		}

		//	決定
		inputConfirm = Input.GetButtonDown("Jump");
		//	キャンセル
		inputCancel = Input.GetButtonDown("Restart");
	}

	/*--------------------------------------------------------------------------------
	|| 軸の入力処理
	--------------------------------------------------------------------------------*/
	private int MenuAxisInput(float interval, ref float timer, params string[] buttonNames)
	{
		//	入力を取得
		float x = Input.GetAxisRaw(buttonNames[0]) == 0 ? 0 : Mathf.Sign(Input.GetAxisRaw(buttonNames[0]));
		for (int i = 1; i < buttonNames.Length; i++)
		{
			x += Input.GetAxisRaw(buttonNames[i]);
		}

		int ret = 0;		//	戻り値

		//	入力がなくなったらタイマーをリセット
		if (x == 0)
		{
			timer = 0;
		}
		//	タイマーのカウントが終了しているときは入力を受け付け、タイマーをセット
		else if (timer <= 0)
		{
			ret = (int)x;
			timer = interval;
		}
		//	タイマーのカウント中は入力を初期化し、カウントする
		else if (timer > 0)
		{
			timer -= Time.deltaTime;
		}

		return ret;
	}

	/*--------------------------------------------------------------------------------
	|| 選択処理
	--------------------------------------------------------------------------------*/
	private void SelectUpdate()
	{
		currentIndex -= (int)inputVec.y;								//	上下入力を加算
		currentIndex = (int)Mathf.Repeat(currentIndex, MaxIndex);       //	リピートする

		//	選択している項目のスプライトを切り替える
		for (int i = 0; i < menuItem.Length; i++)
		{
			Sprite sprite = currentIndex == i ? selectedSprite : nonselectedSprite;
			menuItem[i].sprite = sprite;
		}

		//	カーソルの座標を更新
		Vector3 targetPos = menuItem[currentIndex].transform.localPosition + (Vector3)cursorOffset;
		menuCursor.transform.localPosition = Vector3.Lerp(menuCursor.localPosition, targetPos, Time.deltaTime * cursorSpeed);
	}

	/*--------------------------------------------------------------------------------
	|| カーソルのアニメーション処理
	--------------------------------------------------------------------------------*/
	private void CursorAnimationUpdate()
	{
		//	進行度を進める
		animProgress += Time.deltaTime * animationSpeed;
		animProgress = Mathf.Repeat(animProgress, Mathf.PI * 2);

		//	sin波を 0 ~ 1 の範囲に収める
		float sin = (Mathf.Sin(animProgress) + 1.0f) / 2;
		//	進行度でサイズを補間する
		float size = Mathf.Lerp(animationMinSize, animationMaxSize, sin);

		//	カーソルのスケールを変更
		menuCursor.localScale = Vector3.one * size;
	}

	/*--------------------------------------------------------------------------------
	|| メニューの更新処理
	--------------------------------------------------------------------------------*/
	protected abstract void MenuUpdate();

	/*--------------------------------------------------------------------------------
	|| 決定時処理
	--------------------------------------------------------------------------------*/
	protected abstract void ConfirmUpdate();

}
