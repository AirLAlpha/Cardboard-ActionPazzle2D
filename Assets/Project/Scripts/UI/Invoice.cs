using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Invoice : MonoBehaviour
{
	[Header("データベース")]
	[SerializeField]
	private StageImageDatabase imageDatabase;

	//	コンポーネント
	[SerializeField]
	private SpriteRenderer		spriteRenderer;

	[Header("ステージ")]
	[SerializeField]
	private TextMeshPro			stageNumText;
	[SerializeField]
	private SpriteRenderer		stageImage;

	[Header("リザルト")]
	[SerializeField]
	private TextMeshPro			timeText;
	[SerializeField]
	private TextMeshPro			boxCountText;
	[SerializeField]
	private SpriteRenderer		clearStamp;


	//	プロパティ
	public SpriteRenderer SpriteRenderer { get { return spriteRenderer; } }

	private void Awake()
	{
	}

	/*--------------------------------------------------------------------------------
	|| ステージ番号の設定
	--------------------------------------------------------------------------------*/
	public void SetStageNum(int stageNum, int taskNum)
	{
		stageNumText.text = stageNum + "-" + (taskNum + 1);

		spriteRenderer.sprite = imageDatabase.StageData[stageNum - 1].Invoice;
		stageImage.sprite = imageDatabase.StageData[stageNum - 1].TaskImage[taskNum];
	}

	/*--------------------------------------------------------------------------------
	|| 時間の設定（戻り値：値の変更の有無）
	--------------------------------------------------------------------------------*/
	public bool SetTime(int min, int sec)
	{
		string m, s;
		if(min < 0 && sec < 0)
		{
			m = "--";
			s = "--";
		}
		else
		{
			m = min.ToString("D2");
			s = sec.ToString("D2");
		}
		
		if(timeText.text == m + ":" + s)
		{
			return false;
		}
		else
		{
			timeText.text = m + ":" + s;
			return true;
		}
	}
	public void SetTime(float t)
	{
		int intT = (int)t;
		int min = intT / 60;
		int sec = intT % 60;

		SetTime(min, sec);
	}

	/*--------------------------------------------------------------------------------
	|| 使用した箱の数を設定（戻り値；値の変更の有無）
	--------------------------------------------------------------------------------*/
	public bool SetBoxCount(int count)
	{
		string c;
		if(count < 0)
		{
			c = "--";
		}
		else
		{
			c = count.ToString("D2");
		}

		if(boxCountText.text == c)
		{
			return false;
		}
		else
		{
			boxCountText.text = c;
			return true;
		}
	}

	/*--------------------------------------------------------------------------------
	|| クリアスタンプの切り替え
	--------------------------------------------------------------------------------*/
	public void SetStampEnable(bool enable)
	{
		clearStamp.gameObject.SetActive(enable);
	}

	/*--------------------------------------------------------------------------------
	|| スコアの設定
	--------------------------------------------------------------------------------*/
	public void SetScore(TaskScore score)
	{
		//	未クリアの時
		if (Mathf.Approximately(score.clearTime, 0))
		{
			SetStampEnable(false);
			SetTime(-1, -1);
			SetBoxCount(-1);
			return;
		}

		SetTime(score.clearTime);
		SetBoxCount(score.usedBoxCount);
		SetStampEnable(true);
	}

}
