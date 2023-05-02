/**********************************************
 * 
 *  StageBaseLoader.cs 
 *  Stage��ǂݍ��ލۂɎ����I��StageBase��ǂݍ���
 * 
 *  ����ҁF���� ���m
 *  ������F2024/04/05
 * 
 **********************************************/
#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class StageBaseLoader
{
	//	�^�C�g���V�[���̖��O
	static readonly string[] NOT_LOAD_MANAGER_NAME =
	{
		"TitleScene",
		"StageBase",
		"PlayScene"
	};

	//	StageBase�V�[��
	static readonly string STAGE_BASE_NAME = "StageBase";
	//	Stage�̃f�B���N�g��
	static readonly string STAGE_DIRECTORY = "Assets/Project/Scenes/";

	/*--------------------------------------------------------------------------------
	|| ����������
	--------------------------------------------------------------------------------*/
	[InitializeOnLoadMethod]
	private static void Init()
	{
		//EditorSceneManager.sceneOpened += OnSceneLoaded;
	}

	/*--------------------------------------------------------------------------------
	|| �V�[�����ǂݍ��܂ꂽ�Ƃ��̏���
	--------------------------------------------------------------------------------*/
	private static void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEditor.SceneManagement.OpenSceneMode mode)
	{
		//	�V�[�������擾
		string currentSceneName = scene.name;

		//	�X�e�[�W�x�[�X��ǂݍ��ނ��ǂ����𔻒f����
		for (int i = 0; i < NOT_LOAD_MANAGER_NAME.Length; i++)
		{
			if (NOT_LOAD_MANAGER_NAME[i] == currentSceneName)
				return;
		}

		EditorSceneManager.OpenScene(STAGE_DIRECTORY + STAGE_BASE_NAME + ".unity", OpenSceneMode.Additive);
	}
}
#endif