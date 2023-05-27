/**********************************************
 * 
 *  TitleStageBox.cs 
 *  ステージセレクトの各ステージのボックスの処理を記述
 * 
 *  製作者：牛丸 文仁
 *  制作日：2023/04/04
 * 
 **********************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleStageBox : MonoBehaviour
{
	private bool enableTaskSelect;

	//	コンポーネント
	[Header("コンポーネント")]
	[SerializeField]
	private Animator		boxAnim;
	[SerializeField]
	private TitleTasks		tasks;

	//	ハコの開閉
	[Header("開閉")]
	[SerializeField]
	private bool			isOpen;			//	開閉フラグ
	[SerializeField]
	private float			openSpeed;		//	ハコの開閉速度
	
	private float			openProgress;

	[Header("背景ルート")]
	[SerializeField]
	private Transform		bgRoot;                         //	背景ルート
	[SerializeField]
	private SpriteAlphaController bgRootAlphaController;	//	背景の透明度
	[SerializeField]
	private float			bgRootAnimSpeed;				//	背景アニメーションの速度
	[SerializeField]
	private Vector2			bgRootMoveRange;				//	背景の上下に揺れる幅
	[SerializeField]
	private Vector2			bgRootEnableScale;				//	背景が表示されるときのスケール
	[SerializeField]
	private Vector2			bgRootOffset;                   //	背景が上下に揺れる際ののオフセット
	[SerializeField]
	private Vector2			bgRootTaskSelectOffset;			//	タスク選択時の背景のオフセット

	private float			bgRootSine;						//	背景の上下に揺れるsin波

	[Header("背景")]
	[SerializeField]
	private SpriteRenderer	taskBackground;
	[SerializeField]
	private	SpriteRenderer	bgArrow;
	[SerializeField]
	private float			bgZoomSpeed;
	[SerializeField]
	private Vector2			bgIdleSize;
	[SerializeField]
	private Vector2			bgZoomSize;
	[SerializeField]
	private Vector2			bgIdleOffset;
	[SerializeField]
	private Vector2			bgZoomOffset;
	[SerializeField]
	private Vector2			bgArrowIdleOffset;
	[SerializeField]
	private Vector2			bgArrowZoomOffset;

	[Header("文字")]
	[SerializeField]
	private SpriteRenderer stageText;
	[SerializeField]
	private Vector2			textIdleOffset;
	[SerializeField]
	private Vector2			textZoomOffset;
	[SerializeField]
	private Vector2			textIdleScale;
	[SerializeField]
	private Vector2			textZoomScale;


	//	プロパティ
	public bool IsOpen { get { return isOpen; } set { isOpen = value; } }
	public int	SelectedTaskIndex => tasks.SelectedTaskIndex;

	//	更新処理
	private void Update()
	{
		BoxOpenUpdate();
		BackgroundAnimationUpdate();

		//	クリア状況のアニメーションパラメータを変更
		//stageClearStatusAnim.SetBool("Enable", isOpen);

	}

	/*--------------------------------------------------------------------------------
	|| ハコの開閉処理
	--------------------------------------------------------------------------------*/
	private void BoxOpenUpdate()
	{
		int progressDir = isOpen ? 1 : -1;
		openProgress += Time.deltaTime * openSpeed * progressDir;
		openProgress = Mathf.Clamp01(openProgress);

		boxAnim.SetFloat("OpenProgress", EasingFunctions.EaseInOutCubic(openProgress));
	}

	/*--------------------------------------------------------------------------------
	|| 背景のアニメーション処理
	--------------------------------------------------------------------------------*/
	private void BackgroundAnimationUpdate()
	{
		//	スケール
		Vector2 targetScale = isOpen ? bgRootEnableScale : Vector2.zero;
		bgRoot.localScale = Vector2.Lerp(bgRoot.localScale, targetScale, Time.deltaTime * bgZoomSpeed);
		//	座標
		Vector2 targetPos = Vector2.zero;
		if (enableTaskSelect)
		{
			targetPos = bgRootTaskSelectOffset;
		}
		else if(isOpen)
		{
			bgRootSine += Time.deltaTime * bgRootAnimSpeed;
			bgRootSine = Mathf.Repeat(bgRootSine, Mathf.PI * 2);
			float sin = (Mathf.Sin(bgRootSine) + 1.0f) / 2.0f;
			Vector2 pos = Vector2.Lerp(-bgRootMoveRange, bgRootMoveRange, sin);
			targetPos = pos + bgRootOffset;
		}
		bgRoot.localPosition = Vector2.Lerp(bgRoot.localPosition, targetPos, Time.deltaTime * bgRootAnimSpeed);
		//	透明度
		bgRootAlphaController.TargetAlpha = isOpen ? 1.0f : 0.0f;

		//	SpriteRendererのサイズ
		Vector2 targetSize = enableTaskSelect ? bgZoomSize : bgIdleSize;
		taskBackground.size = Vector2.Lerp(taskBackground.size, targetSize, Time.deltaTime * bgZoomSpeed);
		//	Spriteのオフセット
		Vector2 targetOffset = enableTaskSelect ? bgZoomOffset : bgIdleOffset;
		taskBackground.transform.localPosition = Vector2.Lerp(taskBackground.transform.localPosition, targetOffset, Time.deltaTime * bgZoomSpeed);
		//	矢印の座標
		Vector2 targetArrowPos = enableTaskSelect ? bgArrowZoomOffset : bgArrowIdleOffset;
		bgArrow.transform.localPosition = Vector2.Lerp(bgArrow.transform.localPosition, targetArrowPos, Time.deltaTime * bgZoomSpeed);

		//	文字
		//	座標
		Vector2 textTargetPos = enableTaskSelect ? textZoomOffset : textIdleOffset;
		stageText.transform.localPosition = Vector2.Lerp(stageText.transform.localPosition, textTargetPos, Time.deltaTime * bgZoomSpeed);
		//	スケール
		Vector2 textTargetScale = enableTaskSelect ? textZoomScale : textIdleScale;
		stageText.transform.localScale = Vector2.Lerp(stageText.transform.localScale, textTargetScale, Time.deltaTime * bgZoomSpeed);
	}

	/*--------------------------------------------------------------------------------
	|| ステージの選択処理
	--------------------------------------------------------------------------------*/
	public void SelectBox()
	{
		tasks.StartTaskSelect();
		enableTaskSelect = true;
	}

	/*--------------------------------------------------------------------------------
	|| キャンセル処理
	--------------------------------------------------------------------------------*/
	public void CancelBox()
	{
		enableTaskSelect = false;
		tasks.EndTaskSelect();
	}

	/*--------------------------------------------------------------------------------
	|| セーブデータの設定処理
	--------------------------------------------------------------------------------*/
	public void SetStageScore(StageScore score)
	{
		tasks.SetStageScore(score);
	}

}
