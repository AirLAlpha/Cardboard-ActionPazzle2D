/**********************************************
 * 
 *  StageLoaderEditor.cs 
 *  StageLoaderクラスのインスペクタ拡張
 * 
 *  製作者：牛丸 文仁
 *  制作日：2023/04/19
 * 
 **********************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(StageLoader))]
public class StageLoaderEditor : Editor
{
	//	インスペクターの拡張
	public override void OnInspectorGUI()
	{
		var loader = target as StageLoader;

		//	元のインスペクターを表示する
		base.OnInspectorGUI();

		//	隙間を開ける
		GUILayout.Space(10);

		//	ステージのリセットを実行するボタンを表示
		if (GUILayout.Button("Reset", GUILayout.Height(30)))
		{
			//	クリックされたらステージのリセットを実行
			loader.ResetStage();
		}

		//	ロード処理を実行するボタンを表示
		if (GUILayout.Button("Load", GUILayout.Height(30)))
		{
			//	クリックされたらロード処理を実行
			loader.LoadStage();
		}
	}
}
