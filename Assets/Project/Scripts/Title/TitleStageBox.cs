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
	private Animator		stageClearStatusAnim;
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

	[Header("背景")]
	[SerializeField]
	private Transform		taskBackground;
	[SerializeField]
	private float			bgZoomSpeed;
	[SerializeField]
	private Vector2			bgIdleScale;
	[SerializeField]
	private Vector2			bgZoomScale;

	//	プロパティ
	public bool IsOpen { get { return isOpen; } set { isOpen = value; } }
	public int	SelectedTaskIndex => tasks.SelectedTaskIndex;

	//	更新処理
	private void Update()
	{
		BoxOpenUpdate();
		BackgroundZoomUpdate();

		//	クリア状況のアニメーションパラメータを変更
		stageClearStatusAnim.SetBool("Enable", isOpen);
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
	|| 背景のズーム処理
	--------------------------------------------------------------------------------*/
	private void BackgroundZoomUpdate()
	{
		Vector2 targetScale = enableTaskSelect ? bgZoomScale : bgIdleScale;
		taskBackground.localScale = Vector2.Lerp(taskBackground.localScale, targetScale, Time.deltaTime * bgZoomSpeed);
	}

	/*--------------------------------------------------------------------------------
	|| ステージの選択処理
	--------------------------------------------------------------------------------*/
	public void SelectBox()
	{
		stageClearStatusAnim.SetFloat("AnimationSpeed", 0.0f);
		tasks.StartTaskSelect();
		enableTaskSelect = true;
	}

	/*--------------------------------------------------------------------------------
	|| キャンセル処理
	--------------------------------------------------------------------------------*/
	public void CancelBox()
	{
		enableTaskSelect = false;
		stageClearStatusAnim.SetFloat("AnimationSpeed", 1.0f);
		tasks.EndTaskSelect();
	}

}
