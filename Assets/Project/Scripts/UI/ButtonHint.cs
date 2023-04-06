using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ButtonHint : MonoBehaviour
{
	//	画面に表示するヒント一つ文の構造体
	[System.Serializable]
	public struct HintItem
	{
		public TextMeshProUGUI	text;
		public Image			image;
		public Image			negativeImage;
	};

	//	データベース
	[Header("データベース")]
	[SerializeField]
	private ButtonHintDetabase		dataBase;

	//	表示
	[Header("表示")]
	[SerializeField]
	private HorizontalLayoutGroup	layout;
	[SerializeField]
	private float					menuButtonHeight;
	[SerializeField]
	private int[]					displayNameIndex;

	//	操作UI
	[Header("操作UI")]
	[SerializeField]
	private HintItem[] controlHints;	//	コントロール
	[SerializeField]
	private HintItem menuHint;          //	メニュー

	static readonly string[] CONTROL_NAMES =
	{
		"Horizontal",
		"Jump",
		"Fire1",
		"Restart",
	};
	static readonly string MENU_NAME = "Menu";

	//	実行前初期化処理
	private void Awake()
	{
		if (dataBase == null)
			Debug.LogError("データベースが設定されていません。");

		displayNameIndex = new int[controlHints.Length];
	}

	//	初期化処理
	private void Start()
	{
		ControlButtons(ControllerCecker.Instance.ControllerConnected);
		DisplayNameUpdate();
	}

	//	更新処理
	private void Update()
	{
	}

	/*--------------------------------------------------------------------------------
	|| 各キーの表示
	--------------------------------------------------------------------------------*/
	public void ControlButtons(bool isConnected)
	{
		for (int i = 0; i < controlHints.Length; i++)
		{
			int index = dataBase.FindIndex(CONTROL_NAMES[i]);

			Sprite newSprite = null;
			Sprite newNegativeSprite = null;
			//	接続時
			if (isConnected)
			{
				//	画像の差し替え
				newSprite = dataBase.Buttons[index].controllerSprite;
			}
			//	接続解除時
			else
			{
				//	画像の差し替え
				newSprite = dataBase.Buttons[index].positiveKeySprite;
				newNegativeSprite = dataBase.Buttons[index].negativeKeySprite;
			}

			//	どちらも設定されていないときは処理しない
			if (newSprite == null &&
				newNegativeSprite == null)
				return;

			//	変更がないときは処理しない
			if (controlHints[i].image?.sprite == newSprite)
				return;

			//	ボタンのスプライトを設定
			if (newSprite != null)
				controlHints[i].image.sprite = newSprite;

			const float OFFSET_X = 90.0f;
			//	negativeSpriteの設定があるときは変更する
			if (dataBase.Buttons[index].negativeKeySprite != null)
			{
				//	親のRectTransformを取得
				var parentRectTransform = controlHints[i].negativeImage.transform.parent as RectTransform;
				//	画像が設定されるとき
				if (newNegativeSprite != null)
				{
					//	キャンバスのサイズを大きくする
					parentRectTransform.sizeDelta += Vector2.right * OFFSET_X;

					//	negativeImageを有効化し画像を設定
					controlHints[i].negativeImage.gameObject.SetActive(true);
					controlHints[i].negativeImage.sprite = newNegativeSprite;

					//	各オブジェクトをずらす
					controlHints[i].text.rectTransform.localPosition += Vector3.right * OFFSET_X;
					controlHints[i].image.rectTransform.localPosition += Vector3.right * OFFSET_X;
					controlHints[i].negativeImage.rectTransform.localPosition += Vector3.right * OFFSET_X;
				}
				else
				{
					//	キャンバスのサイズを小さくする
					parentRectTransform.sizeDelta -= Vector2.right * OFFSET_X;

					//	negativeImageを無効化
					controlHints[i].negativeImage.gameObject.SetActive(false);

					//	各オブジェクトをずらす
					controlHints[i].text.rectTransform.localPosition -= Vector3.right * OFFSET_X;
					controlHints[i].image.rectTransform.localPosition -= Vector3.right * OFFSET_X;
					controlHints[i].negativeImage.rectTransform.localPosition -= Vector3.right * OFFSET_X;
				}

				//	LyaoutGroupの再計算
				LayoutRebuilder.MarkLayoutForRebuild(parentRectTransform);
			}
		}

		DisplayNameUpdate();
	}

	/*--------------------------------------------------------------------------------
	|| メニューキーの表示
	--------------------------------------------------------------------------------*/
	public void MenuButton(bool isConnected)
	{
		int index = dataBase.FindIndex(MENU_NAME);

		Sprite newSprite = null;
		if(isConnected)
		{
			//	画像の差し替え
			newSprite =  dataBase.Buttons[index].controllerSprite;
		}
		else
		{
			//	画像の差し替え
			newSprite = dataBase.Buttons[index].positiveKeySprite;
		}

		if (newSprite == null)
			return;

		menuHint.image.sprite = newSprite;
		//	画像から比率を取得し、描画のサイズを変更
		float w = newSprite.rect.width;
		float h = newSprite.rect.height;

		RectTransform rt = menuHint.image.transform as RectTransform;
		rt.sizeDelta = new Vector2(w * (menuButtonHeight / h), menuButtonHeight);
	}

	/*--------------------------------------------------------------------------------
	|| DisplayNameIndexの変更
	--------------------------------------------------------------------------------*/
	public void SetDisplayNameIndex(string inputName, int newIndex)
	{
		List<string> controlNameList = new List<string>(CONTROL_NAMES);
		int i = controlNameList.FindIndex(a => a == inputName);
		displayNameIndex[i] = newIndex;

		DisplayNameUpdate();
	}

	/*--------------------------------------------------------------------------------
	|| 表示名の更新処理
	--------------------------------------------------------------------------------*/
	private void DisplayNameUpdate()
	{
		for (int i = 0; i < CONTROL_NAMES.Length; i++)
		{
			int index = dataBase.FindIndex(CONTROL_NAMES[i]);

			controlHints[i].text.text = dataBase.Buttons[index].displayNames[displayNameIndex[i]];
		}
	}
}
