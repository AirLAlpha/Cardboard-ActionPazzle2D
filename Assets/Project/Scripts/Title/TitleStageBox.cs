/**********************************************
 * 
 *  TitleStageBox.cs 
 *  �X�e�[�W�Z���N�g�̊e�X�e�[�W�̃{�b�N�X�̏������L�q
 * 
 *  ����ҁF���� ���m
 *  ������F2023/04/04
 * 
 **********************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleStageBox : MonoBehaviour
{
	private bool enableTaskSelect;

	//	�R���|�[�l���g
	[Header("�R���|�[�l���g")]
	[SerializeField]
	private Animator		stageClearStatusAnim;
	[SerializeField]
	private Animator		boxAnim;
	[SerializeField]
	private TitleTasks		tasks;

	//	�n�R�̊J��
	[Header("�J��")]
	[SerializeField]
	private bool			isOpen;			//	�J�t���O
	[SerializeField]
	private float			openSpeed;		//	�n�R�̊J���x
	
	private float			openProgress;

	[Header("�w�i")]
	[SerializeField]
	private Transform		taskBackground;
	[SerializeField]
	private float			bgZoomSpeed;
	[SerializeField]
	private Vector2			bgIdleScale;
	[SerializeField]
	private Vector2			bgZoomScale;

	//	�v���p�e�B
	public bool IsOpen { get { return isOpen; } set { isOpen = value; } }
	public int	SelectedTaskIndex => tasks.SelectedTaskIndex;

	//	�X�V����
	private void Update()
	{
		BoxOpenUpdate();
		BackgroundZoomUpdate();

		//	�N���A�󋵂̃A�j���[�V�����p�����[�^��ύX
		stageClearStatusAnim.SetBool("Enable", isOpen);
	}

	/*--------------------------------------------------------------------------------
	|| �n�R�̊J����
	--------------------------------------------------------------------------------*/
	private void BoxOpenUpdate()
	{
		int progressDir = isOpen ? 1 : -1;
		openProgress += Time.deltaTime * openSpeed * progressDir;
		openProgress = Mathf.Clamp01(openProgress);

		boxAnim.SetFloat("OpenProgress", EasingFunctions.EaseInOutCubic(openProgress));
	}

	/*--------------------------------------------------------------------------------
	|| �w�i�̃Y�[������
	--------------------------------------------------------------------------------*/
	private void BackgroundZoomUpdate()
	{
		Vector2 targetScale = enableTaskSelect ? bgZoomScale : bgIdleScale;
		taskBackground.localScale = Vector2.Lerp(taskBackground.localScale, targetScale, Time.deltaTime * bgZoomSpeed);
	}

	/*--------------------------------------------------------------------------------
	|| �X�e�[�W�̑I������
	--------------------------------------------------------------------------------*/
	public void SelectBox()
	{
		stageClearStatusAnim.SetFloat("AnimationSpeed", 0.0f);
		tasks.StartTaskSelect();
		enableTaskSelect = true;
	}

	/*--------------------------------------------------------------------------------
	|| �L�����Z������
	--------------------------------------------------------------------------------*/
	public void CancelBox()
	{
		enableTaskSelect = false;
		stageClearStatusAnim.SetFloat("AnimationSpeed", 1.0f);
		tasks.EndTaskSelect();
	}

}
