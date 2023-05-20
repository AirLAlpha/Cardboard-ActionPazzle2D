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
using System.Diagnostics.CodeAnalysis;

public class ButtonHint : MonoBehaviour
{
	//	画面に表示するヒント一つ文の構造体
	[System.Serializable]
	public struct HintItem
	{
		public Image			text;
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
	private Vector2					buttonBasePosition;		//	ボタン全体のズレ
	[SerializeField]
	private float					buttonsOffsetX;			//	ボタン（メニューを除く）の間隔
	[SerializeField]
	private float					menuButtonHeight;		//	ボタン画像の高さ
	[SerializeField]
	private float					textSpriteHeight;		//	文字画像の高さ
	[SerializeField]
	private int[]					displayNameIndex;		//	文字画像の番号

	//	操作UI
	[Header("操作UI")]
	[SerializeField]
	private HintItem[] controlHints;	//	コントロール
	[SerializeField]
	private HintItem menuHint;          //	メニュー

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
		bool controllerConnected = ControllerCecker.Instance.ControllerConnected;
		ControlButtons(controllerConnected);
		MenuButton(controllerConnected);
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

			//	整列させる
			AlignmentButtons();
			//	文字の更新
			DisplayNameUpdate();
		}
	}

	/*--------------------------------------------------------------------------------
	|| ボタンを整列させる
	--------------------------------------------------------------------------------*/
	[ContextMenu("AlignmentButtons")]
	public void AlignmentButtons()
	{
		int nonActiveCount = 0;
		float negativeOffsetX = 0.0f;

		for (int i = 0; i < controlHints.Length; i++)
		{
			//	親のアクティブを取得
			bool buttonActive = controlHints[i].text.transform.parent.gameObject.activeSelf;
			//	親がアクティブでなければカウンターを更新して次へ
			if (!buttonActive)
			{
				nonActiveCount++;
				continue;
			}

			//	親のRectTransformを取得
			RectTransform rt = controlHints[i].text.transform.parent as RectTransform;

			//	ボタンの位置を更新する
			rt.anchoredPosition = (Vector2.right * buttonsOffsetX * (i - nonActiveCount)) + (Vector2.left * negativeOffsetX);

			//	ネガティブボタンが存在して、アクティブでないときは左へずらす
			if (controlHints[i].negativeImage != null &&
				!controlHints[i].negativeImage.gameObject.activeSelf)
			{
				//	オフセットを計算
				float offset = controlHints[i].negativeImage.rectTransform.sizeDelta.x;
				//	次以降のオフセットを加算
				negativeOffsetX += offset;
				rt.anchoredPosition += Vector2.left * offset;
			}
		}
	}

	/*--------------------------------------------------------------------------------
	|| メニューキーの表示
	--------------------------------------------------------------------------------*/
	public void MenuButton(bool isConnected)
	{
		var menuButtonData = dataBase.Buttons[dataBase.Buttons.Count - 1];

		Sprite newSprite = null;
		if(isConnected)
		{
			//	画像の差し替え
			newSprite = menuButtonData.controllerSprite;
		}
		else
		{
			//	画像の差し替え
			newSprite = menuButtonData.positiveKeySprite;
		}

		if (newSprite == null)
			return;

		menuHint.image.sprite = newSprite;
		//	画像から比率を取得し、描画のサイズを変更
		//	ボタン画像の大きさを変更
		RectTransform rt = menuHint.image.transform as RectTransform;
		rt.sizeDelta = new Vector2(newSprite.rect.width * (menuButtonHeight / newSprite.rect.height), menuButtonHeight);

		//	テキスト画像の比率を計算
		Rect menuButtonTextRect = menuHint.text.sprite.rect;
		menuHint.text.rectTransform.sizeDelta = new Vector2(menuButtonTextRect.width * (textSpriteHeight / menuButtonTextRect.height), textSpriteHeight);
	}

	/*--------------------------------------------------------------------------------
	|| DisplayNameIndexの変更
	--------------------------------------------------------------------------------*/
	public void SetDisplayNameIndex(string inputName, int newIndex)
	{
		int i = dataBase.FindIndex(inputName);
		if (i == -1 || i >= displayNameIndex.Length)
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
			Sprite newTextSprite = dataBase.Buttons[i].displayNames[nameIndex];
			controlHints[i].text.sprite = newTextSprite;

			RectTransform rt = controlHints[i].text.rectTransform;
			Rect size = newTextSprite.rect;
			rt.sizeDelta = new Vector2(size.width * textSpriteHeight / size.height, textSpriteHeight);
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
		{
			menuHint.image.transform.parent.gameObject.SetActive(active);
		}
		else
		{
			controlHints[i].image.transform.parent.gameObject.SetActive(active);
		}

		//	整列させる
		AlignmentButtons();
	}
}
