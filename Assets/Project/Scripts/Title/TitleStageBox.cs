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
	//	�R���|�[�l���g
	[Header("�R���|�[�l���g")]
	[SerializeField]
	private Animator		stageClearStatusAnim;
	[SerializeField]
	private TitleManager	titleManager;

	//	�X�e�[�W���
	[Header("�X�e�[�W���")]
	[SerializeField]
	private int stageIndex;

	//	�I��
	[Header("�I��")]
	[SerializeField]
	private Rect			selectArea;     //	�I�𒆂Ƃ���͈�
	[SerializeField]
	private LayerMask		selectMask;		//	�I���ł��郌�C���[

	//	�I�𒆂Ƃ���͈͂̒��S���W
	private Vector3 SelectAreaCenter => new Vector3(selectArea.x, selectArea.y) + transform.position;

	private bool			inRange;		//	�͈͓��t���O

#if UNITY_EDITOR
	[Header("�f�o�b�O")]
	[SerializeField]
	private bool drawGizmos;
#endif


	//	�X�V����
	private void Update()
	{
		CheckArea();
		SelectUpdate();

		//	�N���A�󋵂̃A�j���[�V�����p�����[�^��ύX
		stageClearStatusAnim.SetBool("Enable", inRange);
	}

	/*--------------------------------------------------------------------------------
	|| �͈͔���
	--------------------------------------------------------------------------------*/
	private void CheckArea()
	{
		inRange = Physics2D.OverlapBox(SelectAreaCenter, selectArea.size, 0, selectMask);
	}

	/*--------------------------------------------------------------------------------
	|| �I���̓K������
	--------------------------------------------------------------------------------*/
	private void SelectUpdate()
	{
		//	�͈͊O
		if(!inRange)
		{
			//	���g�̕ێ�����X�e�[�W�ԍ��̂Ƃ��̓��Z�b�g����
			if (titleManager.SelectedStage == this.stageIndex)
				titleManager.SelectedStage = -1;
		}
		//	�͈͓�
		else
		{
			//	�I�𒆂̃X�e�[�W�Ƃ��Đݒ肷��
			titleManager.SelectedStage = this.stageIndex;
		}
	}


#if UNITY_EDITOR
	/*--------------------------------------------------------------------------------
	|| �M�Y���̕`�揈��
	--------------------------------------------------------------------------------*/
	private void OnDrawGizmosSelected()
	{
		//	�t���O���L���łȂ��Ƃ��͏������Ȃ�
		if (!drawGizmos)
			return;

		//	�F�̐ݒ�
		Color col = Color.yellow;
		col.a = 0.5f;
		Gizmos.color = col;
		//	�͈͂̕`��
		Gizmos.DrawCube(SelectAreaCenter, selectArea.size);
	}
#endif
}
