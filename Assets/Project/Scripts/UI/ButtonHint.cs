/**********************************************
 * 
 *  ButtonHint.cs 
 *  ボタンヒントに関する処理を記述
 * 
 *  製作者：牛丸 文仁
 *  制作日：2023/04/06
 * 
 **********************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//	TODO : リファクタリング

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
		"Vertical",
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
		/////////////////////////////////////////////////////////////////////
		///	注意：controlHintの配列の並びは、データベースの並びに依存する ///
		/////////////////////////////////////////////////////////////////////

		for (int i = 0; i < dataBase.Buttons.Count; i++)
		{
			//	設定されたヒントの数を超えたら終了する
			if (controlHints.Length <= i)
				return;

			//	データを一つ取り出す
			ButtonContainor containor = dataBase.Buttons[i];

			//	スプライトの設定
			//	コントローラー接続時
			if (isConnected)
			{
				//	コントローラーのボタン画像を設定
				controlHints[i].image.sprite = containor.controllerSprite;

				//	ネガティブキーは非アクティブに設定
				if (controlHints[i].negativeImage != null)
					controlHints[i].negativeImage.gameObject.SetActive(false);
			}
			//	コントローラー接続解除時
			else
			{
				//	ポジティブキーに画像を設定
				controlHints[i].image.sprite = containor.positiveKeySprite;

				//	ネガティブキーが存在するときは設定する
				if (controlHints[i].negativeImage != null)
				{
					//	アクティブに設定
					controlHints[i].negativeImage.gameObject.SetActive(true);
					//	画像を割り当てる
					controlHints[i].negativeImage.sprite = containor.negativeKeySprite;
				}
			}


			//	ネガティブボタンが存在する場合は配置を整える
			if(controlHints[i].negativeImage != null)
			{
				//	ネガティブボタンの画像のアクティブ状態を取得
				bool negativeActive = controlHints[i].negativeImage.gameObject.activeSelf;

				//	親（画像やテキストをまとめる）のRectTransformを取得
				RectTransform parentRectTransform = controlHints[i].negativeImage.transform.parent as RectTransform;

				//	アクティブなとき
				if(negativeActive)
				{
					//	横幅を 2010 に設定
					parentRectTransform.sizeDelta = new Vector2(2010, parentRectTransform.sizeDelta.y);

					//	ポジティブボタンの画像をずらす
					controlHints[i].image.rectTransform.anchoredPosition = new Vector3(213, -1000);
					//	テキストをずらす
					controlHints[i].text.rectTransform.anchoredPosition = new Vector3(309, -1015);
				}
				//	非アクティブなとき
				else
				{
					//	横幅を 2010 に設定
					parentRectTransform.sizeDelta = new Vector2(1920, parentRectTransform.sizeDelta.y);

					//	ポジティブボタンの画像をずらす
					controlHints[i].image.rectTransform.anchoredPosition = new Vector3(123, -1000);
					//	テキストをずらす
					controlHints[i].text.rectTransform.anchoredPosition = new Vector3(229, -1015);
				}

				//	LyaoutGroupの再計算
				LayoutRebuilder.MarkLayoutForRebuild(parentRectTransform);
			}
		}

		//	文字の更新
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
		int i = dataBase.FindIndex(inputName);
		if (i == -1)
			return;

		displayNameIndex[i] = newIndex;
		DisplayNameUpdate();
	}

	/*--------------------------------------------------------------------------------
	|| 表示名の更新処理
	--------------------------------------------------------------------------------*/
	private void DisplayNameUpdate()
	{
		for (int i = 0; i < dataBase.Buttons.Count; i++)
		{
			if (displayNameIndex.Length <= i)
				return;

			int nameIndex = displayNameIndex[i];
			controlHints[i].text.text = dataBase.Buttons[i].displayNames[nameIndex];
		}
	}

	/*--------------------------------------------------------------------------------
	|| 表示、非表示の切り替え
	--------------------------------------------------------------------------------*/
	public void SetActive(string inputName, bool active)
	{
		int i = dataBase.FindIndex(inputName);
		if (i == -1)
			return;

		if (controlHints.Length == i)
			menuHint.image.transform.parent.gameObject.SetActive(active);
		else
			controlHints[i].image.transform.parent.gameObject.SetActive(active);
	}
}
