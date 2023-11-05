using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public abstract class MenuBase : MonoBehaviour
{
	//	キャンバス
	[Header("キャンバス")]
	[SerializeField]
	private Canvas					menuItemsCanvas;
	[SerializeField]
	private VerticalLayoutGroup		layoutGroup;
	[SerializeField]
	private Canvas					selectedItemCanvas;

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

	private Vector2 inputVec;			//	軸の入力
	private bool	inputConfirm;		//	決定
	private bool	inputCancel;        //	キャンセル
	private bool	disableInput;		//	入力の無効化フラグ

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

	protected RectTransform MenuCursor { get { return menuCursor; } }

	//	メニュー項目
	[Header("メニュー項目")]
	[SerializeField]
	private Sprite			selectedSprite;		//	選択中のスプライト
	[SerializeField]
	private Sprite			nonselectedSprite;	//	非選択中のスプライト
	[SerializeField]
	private Image[]			menuItem;           //	メニュー項目
	[SerializeField]
	private float			selectedScale;      //	選択中の大きさ
	[SerializeField]
	private float			nonselectedScale;   //	非選択中の大きさ
	[SerializeField]
	private float			scaleChangeRate;    //	大きさの変更速度
	[SerializeField]
	private float			selectedRot;        //	選択中の角度
	[SerializeField]
	private float			nonselectedRot;     //	非選択中の角度

	public int		MaxIndex => menuItem.Length;

	//	サウンド
	[Header("サウンド")]
	[SerializeField]
	protected SoundPlayer		soundPlayer;

	//	プロパティ
	public int		CurrentIndex	{ get { return currentIndex; } set { currentIndex = value; } }
	public Vector2	InputVec		{ get { return inputVec; } }
	public bool		InputConfirm	{ get { return inputConfirm; } }
	public bool		InputCancel		{ get { return inputCancel; } }
	public bool		DisableInput	{ get { return disableInput; } set { disableInput = value; } }

	//	実行前初期化処理
	private void Awake()
	{

	}

	//	初期化処理
	private void Start()
	{
	}

	private void OnEnable()
	{
		currentIndex = 0;
	}

	//	更新処理
	protected virtual void Update()
	{
		InputUpdate();      //	入力処理
		SelectUpdate();     //	選択処理

		MenuUpdate();       //	メニューの更新処理

		CursorAnimationUpdate();	//	カーソルのアニメーション処理
	}

	/*--------------------------------------------------------------------------------
	|| 入力処理
	--------------------------------------------------------------------------------*/
	protected void InputUpdate()
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
	protected void SelectUpdate()
	{
		if(!Mathf.Approximately(inputVec.y, 0))
		{
			soundPlayer.PlaySound(1);
		}

		currentIndex -= (int)inputVec.y;                                //	上下入力を加算
		currentIndex = (int)Mathf.Repeat(currentIndex, MaxIndex);       //	リピートする
		//	有効でないときは次の項目へ
		if (!menuItem[currentIndex].gameObject.activeSelf)
		{
			currentIndex -= (int)inputVec.y;
		}
		currentIndex = (int)Mathf.Repeat(currentIndex, MaxIndex);       //	リピートする

		for (int i = 0; i < menuItem.Length; i++)
		{
			bool isSeleceted = currentIndex == i;

			//	スプライトを切り替える
			Sprite sprite = isSeleceted ? selectedSprite : nonselectedSprite;
			menuItem[i].sprite = sprite;

			//	親を切り替える（表示順が切り替わる）
			Transform parent = isSeleceted ? selectedItemCanvas.transform : menuItemsCanvas.transform;
			menuItem[i].transform.SetParent(parent);
			//	大きさを変更する
			float targetScale = isSeleceted ? selectedScale : nonselectedScale;
			menuItem[i].transform.localScale = Vector3.Lerp(menuItem[i].transform.localScale, Vector3.one * targetScale, Time.deltaTime * scaleChangeRate);
			//	角度を変更する
			float targetRot = isSeleceted ? selectedRot : nonselectedRot;
			menuItem[i].transform.localEulerAngles = Vector3.Lerp(menuItem[i].transform.localEulerAngles, Vector3.forward * targetRot, Time.deltaTime * scaleChangeRate);

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

	/*--------------------------------------------------------------------------------
	|| 項目の有効切り替え
	--------------------------------------------------------------------------------*/
	public void SetItemActive(int index, bool active)
	{
		if (index < 0 ||
			index >= menuItem.Length)
			return;

		//	項目の切り替え
		menuItem[index].gameObject.SetActive(active);

		//	親を一旦設定
		if (menuItem[currentIndex].transform.parent == selectedItemCanvas.transform)
			menuItem[currentIndex].rectTransform.SetParent(menuItemsCanvas.transform);

		//	親を戻す
		//if (menuItem[currentIndex].transform.parent != selectedItemCanvas.transform)
		//	menuItem[currentIndex].rectTransform.SetParent(selectedItemCanvas.transform);

		//	有効でないときは次の項目へ
		if (!menuItem[currentIndex].gameObject.activeSelf)
		{
			currentIndex++;
		}
		currentIndex = (int)Mathf.Repeat(currentIndex, MaxIndex);       //	リピートする
		Vector3 targetPos = menuItem[currentIndex].transform.localPosition + (Vector3)cursorOffset;
		menuCursor.transform.localPosition = targetPos;

		//	並び替え
		/*		layoutGroup.enabled = true;
				LayoutRebuilder.MarkLayoutForRebuild(layoutGroup.transform as RectTransform);
				layoutGroup.enabled = false;*/

		layoutGroup.enabled = true;
		layoutGroup.CalculateLayoutInputHorizontal();
		layoutGroup.CalculateLayoutInputVertical();
		layoutGroup.SetLayoutHorizontal();
		layoutGroup.SetLayoutVertical();
		layoutGroup.enabled = false;
	}
}
