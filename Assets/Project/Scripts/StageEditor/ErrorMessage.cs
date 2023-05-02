using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ErrorMessage : MonoBehaviour
{
	//	コンポーネント
	[SerializeField]
	private TextMeshProUGUI		errorText;      //	メッセージを表示するテキスト

	private Image				image;

	[SerializeField]
	private Color				flashColor;     //	エラー時の色
	[SerializeField]
	private Color				normalColor;    //	通常時の色

	[SerializeField]
	private float				flashSpeed;     //	エラー時の色が変化する速度
	[SerializeField]
	private float				dispTime;       //	表示する時間
	[SerializeField]
	private float				alphaSpeed;

	private bool				isActive;
	private float				flashProgress;	//	フラッシュの進行度
	private float				dispedTimer;    //	表示する時間
	private float				alpha;


	//	実行前初期化処理
	private void Awake()
	{
		image = GetComponent<Image>();
	}

	//	初期化処理
	private void Start()
	{
		
	}

	//	更新処理
	private void Update()
	{
		if (!isActive)
			return;

		flashProgress += Time.deltaTime * flashSpeed;
		flashProgress = Mathf.Clamp01(flashProgress);
		Color col = Color.Lerp(flashColor, normalColor, flashProgress);
		col.a = alpha;
		image.color = col;

		Color textCol = errorText.color;
		textCol.a = alpha;
		errorText.color = textCol;

		if (dispedTimer < dispTime)
		{
			dispedTimer += Time.deltaTime;
		}
		else if(alpha > 0)
		{
			alpha -= Time.deltaTime * alphaSpeed;
		}
		else
		{
			isActive = false;
		}
	}

	//	無効化時処理
	private void OnDisable()
	{
		errorText.text = "";
		errorText.color = new Color(1, 1, 1, 0);
		image.color = new Color(flashColor.r, flashColor.g, flashColor.b, 0);
		alpha = 0;
		isActive = false;
	}

	/*--------------------------------------------------------------------------------
	|| エラーメッセージの表示
	--------------------------------------------------------------------------------*/
	public void DispErrorMessage(string message)
	{
		errorText.text = message;
		errorText.color = Color.white;
		image.color = flashColor;
		alpha = 1;
		flashProgress = 0.0f;
		dispedTimer = 0.0f;
		isActive = true;
	}

}
