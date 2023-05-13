/**********************************************
 * 
 *  PauseActivator.cs 
 *  ポーズメニューの有効化を行う
 * 
 *  製作者：牛丸 文仁
 *  制作日：2023/05/11
 * 
 **********************************************/
using UnityEngine;

[RequireComponent(typeof(PauseManager))]
public class PauseActivator : MonoBehaviour
{
	[Header("コンポーネント")]
	[SerializeField]
	private PauseMenu		pauseMenu;          //	ポーズメニュー

	private PauseManager	pauseManager;		//	ポーズマネージャー

	//	実行前初期化処理
	private void Awake()
	{
		pauseManager = GetComponent<PauseManager>();
	}

	//	更新処理
	private void Update()
	{
		//	ポーズ中は処理しない
		if (pauseManager.IsPose)
			return;

		PauseUpdate();		//	ポーズの有効化処理
	}

	/*--------------------------------------------------------------------------------
	|| ポーズの有効化処理
	--------------------------------------------------------------------------------*/
	private void PauseUpdate()
	{
		if(Input.GetButtonDown("Menu"))
		{
			//	ポーズを実行
			pauseManager.Pause();

			//	ポーズメニューを有効化
			pauseMenu.ActivatePauseMenu();
		}
	}

}
