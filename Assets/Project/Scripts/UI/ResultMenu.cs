/**********************************************
 * 
 *  ResultMenu.cs 
 *  リザルトメニューに関する処理を記述
 * 
 *  製作者：牛丸 文仁
 *  制作日：2023/04/10
 * 
 **********************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultMenu : MonoBehaviour
{
	private enum MenuItems
	{
		NEXT_TASK,
		RETRY,
		RETURN_TITLE,

		OVER_ID
	}
	private MenuItems			currentSelect;          //	現在選択中の項目
	private MenuItems			saveSelect;             //	前回処理時の項目

	//	項目
	[Header("メニュー項目")]
	[SerializeField]
	private RectTransform[]		menuItemTransforms;

	private bool notActiveNext;		//	

	//	カーソル
	[Header("カーソル")]

	[SerializeField]		
	private RectTransform		cursour;                //	カーソル
	[SerializeField]
	private Vector2				cursourSpeed;           //	カーソルの伸縮速度
	[SerializeField]
	private float				cursourMaxWidth;        //	カーソルの最大幅
	[SerializeField]
	private Vector2				cursourOffset;

	//	カーソルエフェクト
	[Header("カーソルエフェクト")]
	[SerializeField]
	private Image				cursourEffect;          //	カーソルエフェクト
	[SerializeField]
	private float				effectSpeed;
	[SerializeField]
	private float				effectMinAlpha;
	[SerializeField]
	private float				effectMaxAlpha;
	[SerializeField]
	private Vector2				effectMaxScale;

	private float sin;

	//	テキストマスク
	[Header("テキストマスク")]
	[SerializeField]
	private RectTransform[]		menuTextMasks;

	//	入力
	[Header("入力")]
	[SerializeField]
	private float				inputYInterval;

	private float				inputY;                 //	Y軸入力
	private bool				inputConfirm;           //	決定

	private float				inputYTimer;

	public bool					DisableInput { get; set; }		//	入力の無効化フラグ

	//	実行前初期化処理
	private void Awake()
	{
		
	}

	//	初期化処理
	private void Start()
	{
		//	ステージの最大タスクを超えていたら「NEXT TASK」の選択肢を消す
		if (StageManager.Instance.TaskIndex >= 4)
		{
			//	オブジェクトを無効化し、フラグを立てる
			menuItemTransforms[(int)MenuItems.NEXT_TASK].gameObject.SetActive(false);
			notActiveNext = true;

			//	選択中の項目をリトライに合わせる
			currentSelect = MenuItems.RETRY;
			//	カーソルの座標を合わせる
			cursour.anchoredPosition =
				new Vector2(cursour.anchoredPosition.x + cursourOffset.x, menuItemTransforms[(int)currentSelect].anchoredPosition.y + cursourOffset.y);
		}
	}

	//	更新処理
	private void Update()
	{
		InputUpdate();      //	入力処理
		SelectUpdate();     //	選択処理

		CursourUpdate();    //	カーソルの更新処理
		CursourEffectUpdate();

		//	決定時処理
		if (inputConfirm)
			Confirm();
	}

	/*--------------------------------------------------------------------------------
	|| 入力処理
	--------------------------------------------------------------------------------*/
	private void InputUpdate()
	{
		//	入力が無効化されているときは処理しない
		if (DisableInput)
			return;

		//	上下入力
		float vertical = Input.GetAxisRaw("Vertical") == 0 ? 0 : Mathf.Sign(Input.GetAxisRaw("Vertical"));
		float y = vertical + Input.GetAxisRaw("D-PadY");
		if (y == 0)
		{
			inputY = 0;
			inputYTimer = 0;
		}
		else if (inputYTimer <= 0)
		{
			inputY = y;
			inputYTimer = inputYInterval;
		}
		else if (inputYTimer > 0)
		{
			inputY = 0;
			inputYTimer -= Time.deltaTime;
		}

		//	決定入力
		inputConfirm = Input.GetButtonDown("Jump");
	}

	/*--------------------------------------------------------------------------------
	|| 選択処理
	--------------------------------------------------------------------------------*/
	private void SelectUpdate()
	{
		saveSelect = currentSelect;

		int nextItem = (int)currentSelect - (int)inputY;
		nextItem = (int)Mathf.Repeat(nextItem, (int)MenuItems.OVER_ID);

		if(notActiveNext && nextItem == (int)MenuItems.NEXT_TASK)
		{
			if (inputY > 0)
				nextItem = (int)MenuItems.RETURN_TITLE;
			else if (inputY < 0)
				nextItem = (int)MenuItems.RETRY;
		}

		currentSelect = (MenuItems)nextItem;
	}

	/*--------------------------------------------------------------------------------
	|| カーソル処理
	--------------------------------------------------------------------------------*/
	private void CursourUpdate()
	{
		//	選択中の項目が切り替わったときの処理
		if(saveSelect != currentSelect)
		{
			//	カーソルの横幅を 0 にする
			cursour.sizeDelta = new Vector2(0.0f, cursour.sizeDelta.y);

			//	選択中のマスク以外を 0 にする
			for (int i = 0; i < menuTextMasks.Length; i++)
			{
				if (i == (int)currentSelect)
					continue;

				menuTextMasks[i].sizeDelta = new Vector2(0, menuTextMasks[i].sizeDelta.y);
			}

			//	エフェクトの進行度を0にする
			sin = 0.0f;
		}

		//	現在選択中の項目に徐々に移動させる
		Vector2 currentSelectPos = new Vector2(cursour.anchoredPosition.x + cursourOffset.x, menuItemTransforms[(int)currentSelect].anchoredPosition.y + cursourOffset.y);
		cursour.anchoredPosition = Vector2.Lerp(cursour.anchoredPosition, currentSelectPos, Time.deltaTime * cursourSpeed.y);
		//	カーソルの横幅を広げる
		cursour.sizeDelta = Vector2.Lerp(cursour.sizeDelta, new Vector2(cursourMaxWidth, cursour.sizeDelta.y), Time.deltaTime * cursourSpeed.x);
		//	現在選択中の項目のマスクを広げる
		menuTextMasks[(int)currentSelect].sizeDelta = cursour.sizeDelta - Vector2.right * 50;
	}

	/*--------------------------------------------------------------------------------
	|| カーソルエフェクトの更新処理
	--------------------------------------------------------------------------------*/
	private void CursourEffectUpdate()
	{
		sin += Time.deltaTime * effectSpeed;
		sin = Mathf.Repeat(sin, Mathf.PI * 2);

		//	エフェクトの横幅を変更
		RectTransform rt = cursourEffect.transform as RectTransform;
		rt.anchoredPosition = cursour.anchoredPosition + Vector2.right * rt.sizeDelta.x / 2;
		rt.sizeDelta = cursour.sizeDelta;
		//	枠のスケールを変更
		float absRad = Mathf.Abs(sin / 2);
		float t = 1.0f - Mathf.Cos(Mathf.Repeat(absRad, Mathf.PI / 2));
		cursourEffect.transform.localScale = Vector3.Lerp(Vector2.one, effectMaxScale, t);
		//	枠の透明度を変更
		Color col = Color.white;
		col.a = Mathf.Lerp(effectMinAlpha, effectMaxAlpha, t);
		cursourEffect.color = col;
	}

	/*--------------------------------------------------------------------------------
	|| 項目の選択処理
	--------------------------------------------------------------------------------*/
	private void Confirm()
	{
		switch (currentSelect)
		{
			case MenuItems.NEXT_TASK:
				StageManager.Instance.LoadNextStage();
				break;

			case MenuItems.RETRY:
				StageManager.Instance.ResetStage();
				break;

			case MenuItems.RETURN_TITLE:
				StageManager.Instance.ReturnTitle();
				break;

			default:
				break;
		}
	}

}
