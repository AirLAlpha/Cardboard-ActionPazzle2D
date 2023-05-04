/**********************************************
 * 
 *  StageObjectPallet.cs 
 *  エディタでステージオブジェクトを選択するUI
 * 
 *  製作者：牛丸 文仁
 *  制作日：2023/04/24
 * 
 **********************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class StageObjectPallet : MonoBehaviour
{
	//	パレットの要素
	private struct PalletItem
	{
		public int dbIndex;
		public int typeId;
	}

	//	カードの移動方向
	enum CardMoveDirection
	{
		LEFT = -1,
		NONE = 0,
		RIGHT = 1,
	}
	private CardMoveDirection cardMoveDir;

	[Header("データベース")]
	[SerializeField]
	private StageObjectDatabase database;

	private IOrderedEnumerable<PalletItem> palletDatabase;

	[Header("カード")]
	[SerializeField]
	private Transform			cardsRoot;
	[SerializeField]
	private float				cardsRootPosY;
	[SerializeField]
	private RectTransform[]		cards;                  //	オブジェクト１つを描画するカード
	[SerializeField]
	private Image[]				cardImages;				//	カードの画像
		
	[Header("スケール")]
	[SerializeField]
	private Vector2				selectCardScale;        //	選択されているカードのスケール
	[SerializeField]
	private Vector2				nonSelectCardScale;     //	選択されていないカードのスケール
	[Space]
	[SerializeField]
	private float				selectCardImageScale;
	[SerializeField]
	private float				nonSelectCardImageScale;

	[Header("選択")]
	[SerializeField]
	private float				cardMoveSpeed;
	[SerializeField]
	private int					selectCardIndex;

	[Header("表示切替")]
	[SerializeField]
	private float				activateSpeed;
	[SerializeField]
	private Vector2				deactivePos;
	[SerializeField]
	private Vector2				activePos;

	private bool				isActive;
	public bool					IsActive { get { return isActive; } set{ isActive = value; } }

	//	入力
	public bool					DisableInput { get; set; }      //	入力の無効化

	[SerializeField]
	private int					skipMoveStep;
	[SerializeField]
	private int					skipStartCount;
	[SerializeField]
	private float				inputWait;

	private int					consecutiveCount;               //	連続で入力された数
	private float				inputWaitTime;

	private int					index;					//	選択されているインデックス
	private float				moveProgress;           //	移動の進行度

	//	選択中のデータベースのインデックス番号を取得
	public int					SelectedIndex { get { return palletDatabase.ElementAt(index).dbIndex; } }

	//	実行前初期化処理
	private void Awake()
	{
		//	データベースから受け取る用のリストを作成
		var db = new List<PalletItem>();
		//	データベースよりリストの番号とタイプを設定
		for(int i = 0; i < database.StageObject.Count; i++)
		{
			db.Add(new PalletItem { dbIndex = i, typeId = (int)database.StageObject[i].type });
		}
		//	タイプで並び替え、タイプ内でインデックス順に並び替える
		palletDatabase = db.OrderBy(value => { return value.typeId; }).
			ThenBy(value => { return value.dbIndex; });
	}

	//	初期化処理
	private void Start()
	{
		ResetCards();
		SetCardImage();

		if (cards.Length != cardImages.Length)
			Debug.LogError("カードと画像の数が一致しません。");
	}

	//	更新処理
	private void Update()
	{
		PalletMoveUpdate();

		if (!isActive)
			return;

		InputUpdate();

		CardsUpdate();
	}

	/*--------------------------------------------------------------------------------
	|| 入力処理
	--------------------------------------------------------------------------------*/
	private void InputUpdate()
	{
		if (DisableInput ||
			cardMoveDir != CardMoveDirection.NONE)
			return;

		//	入力を取得
		int inputX = 0;
		if (Input.GetButton("EditorPalletMinus") || Input.mouseScrollDelta.y > 0)
			inputX++;
		if (Input.GetButton("EditorPalletPlus") || Input.mouseScrollDelta.y < 0)
			inputX--;

		//	入力がなくなったら連続入力を解除
		if (inputX == 0)
		{
			consecutiveCount = 0;
			inputWaitTime = 0;

			return;
		}

		//	入力をカードの動きに適応
		if (consecutiveCount < skipStartCount)
		{
			cardMoveDir = (CardMoveDirection)inputX;
			consecutiveCount++;
		}
		//	上の条件に入らなかった時
		else
		{
			//	入力待ちの時間に満たない時
			if (inputWaitTime < inputWait)
			{
				inputWaitTime += Time.deltaTime;
				return;
			}

			if (inputX > 0)
			{
				cardMoveDir = CardMoveDirection.RIGHT;

				inputWaitTime = 0;
			}
			else if (inputX < 0)
			{
				cardMoveDir = CardMoveDirection.LEFT;

				inputWaitTime = 0;
			}

			ResetCards();
		}
	}

	/*--------------------------------------------------------------------------------
	|| カードの更新処理
	--------------------------------------------------------------------------------*/
	private void CardsUpdate()
	{
		if (cardMoveDir == CardMoveDirection.NONE)
		{
			ResetCards();
			return;
		}

		//	進行度を加算し、イージング関数を適応する
		moveProgress += Time.deltaTime * cardMoveSpeed;
		float easedProgress = EasingFunctions.EaseOutSine(moveProgress);

		//	カード全体を左右に動かす
		Vector3 pos = cardsRoot.localPosition;
		pos.x = Mathf.Lerp(0.0f, 110.0f * (int)cardMoveDir, easedProgress);
		cardsRoot.localPosition = pos;

		switch (cardMoveDir)
		{
			case CardMoveDirection.LEFT:
				//	左から2つ目（temp含む）のカードを小さくし、右のtempカードを大きくする
				cards[cards.Length - 1].localScale = Vector2.one * Mathf.Lerp(0.0f, 1.0f, easedProgress);
				cards[1].localScale = Vector2.one * Mathf.Lerp(1.0f, 0.0f, moveProgress);

				//	真ん中のカードを小さくし、真ん中の１つ右のカードを大きくする
				cards[selectCardIndex].sizeDelta = Vector2.Lerp(selectCardScale, nonSelectCardScale, easedProgress);
				cards[selectCardIndex + 1].sizeDelta = Vector2.Lerp(nonSelectCardScale, selectCardScale, easedProgress);

				//	画像の大きさを変更する
				cardImages[selectCardIndex].transform.localScale = Vector2.Lerp(Vector2.one * selectCardImageScale, Vector2.one * nonSelectCardImageScale, easedProgress);
				cardImages[selectCardIndex + 1].transform.localScale = Vector2.Lerp(Vector2.one * nonSelectCardImageScale, Vector2.one * selectCardImageScale, easedProgress);

				break;

			case CardMoveDirection.RIGHT:
				//	左から2つ目（temp含む）のカードを小さくし、左のtempカードを大きくする
				cards[cards.Length - 2].localScale = Vector2.one * Mathf.Lerp(1.0f, 0.0f, easedProgress);
				cards[0].localScale = Vector2.one * Mathf.Lerp(0.0f, 1.0f, moveProgress);

				//	真ん中のカードを小さくし、真ん中の１つ左のカードを大きくする
				cards[selectCardIndex].sizeDelta = Vector2.Lerp(selectCardScale, nonSelectCardScale, easedProgress);
				cards[selectCardIndex - 1].sizeDelta = Vector2.Lerp(nonSelectCardScale, selectCardScale, easedProgress);

				//	画像の大きさを変更する
				cardImages[selectCardIndex].transform.localScale = Vector2.Lerp(Vector2.one * selectCardImageScale, Vector2.one * nonSelectCardImageScale, easedProgress);
				cardImages[selectCardIndex - 1].transform.localScale = Vector2.Lerp(Vector2.one * nonSelectCardImageScale, Vector2.one * selectCardImageScale, easedProgress);
				break;
		}

		//	進行度が1超えたらリセット
		if (moveProgress >= 1.0f)
		{
			ResetCards();
		}

	}

	/*--------------------------------------------------------------------------------
	|| カードをリセット
	--------------------------------------------------------------------------------*/
	private void ResetCards()
	{
		//	カードの大きさを元に戻す
		cards[cards.Length - 2].localScale = Vector2.one * 1;
		cards[cards.Length - 1].localScale = Vector2.one * 0;
		cards[0].localScale = Vector2.one * 0;
		cards[1].localScale = Vector2.one * 1;

		//	カードの幅を戻す
		cards[selectCardIndex].sizeDelta = selectCardScale;
		cards[selectCardIndex + 1].sizeDelta = nonSelectCardScale;
		cards[selectCardIndex - 1].sizeDelta = nonSelectCardScale;

		//	画像の大きさを戻す
		cardImages[selectCardIndex].transform.localScale = Vector2.one * selectCardImageScale;
		cardImages[selectCardIndex + 1].transform.localScale = Vector2.one * nonSelectCardImageScale;
		cardImages[selectCardIndex - 1].transform.localScale = Vector2.one * nonSelectCardImageScale;

		//	進行度をリセットし、状態を移動なしに変更
		moveProgress = 0.0f;

		//	インデックスを進める
		if(cardMoveDir != CardMoveDirection.NONE)
		{
			//	カード全体の位置を戻す
			cardsRoot.localPosition = new Vector3(0, cardsRootPosY);

			int indexMod = consecutiveCount >= skipStartCount ? skipMoveStep : 1;

			index = (int)Mathf.Repeat(index - (int)cardMoveDir * indexMod, palletDatabase.Count());
			SetCardImage();
		}
		cardMoveDir = CardMoveDirection.NONE;
	}

	/*--------------------------------------------------------------------------------
	|| カードの中身の画像を設定する
	--------------------------------------------------------------------------------*/
	[ContextMenu("SetCardImage")]
	private void SetCardImage()
	{
		//	選択済みとするの画像を設定する
		cardImages[selectCardIndex].sprite = database.StageObject[palletDatabase.ElementAt(index).dbIndex].sprite;

		int roopCount = 0;

		for (int i = selectCardIndex + 1; i < cardImages.Length; i++)
		{
			roopCount++;
			int index = (int)Mathf.Repeat(roopCount + this.index, database.StageObject.Count);

			cardImages[i].sprite = database.StageObject[palletDatabase.ElementAt(index).dbIndex].sprite;
		}
		roopCount = 0;

		for (int i = selectCardIndex - 1; i >= 0; i--)
		{
			roopCount++;
			int index = (int)Mathf.Repeat(this.index - roopCount, database.StageObject.Count);
			cardImages[i].sprite = database.StageObject[palletDatabase.ElementAt(index).dbIndex].sprite;
		}
	}

	/*--------------------------------------------------------------------------------
	|| パレットの移動処理
	--------------------------------------------------------------------------------*/
	private void PalletMoveUpdate()
	{
		Vector2 targetPos = isActive ? activePos : deactivePos;

		cardsRoot.localPosition = Vector2.Lerp(cardsRoot.localPosition, targetPos, Time.deltaTime * activateSpeed);
	}

}
