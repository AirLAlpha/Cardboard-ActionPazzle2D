using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(StageExporter))]
public class StageExporterEditor : Editor
{
	public override void OnInspectorGUI()
	{
		var exporter = target as StageExporter;

		//	元のインスペクターを表示
		base.OnInspectorGUI();

		//	隙間を開ける
		GUILayout.Space(10);

		//	ステージの書き出し処理を実行するボタンを描画
		if(GUILayout.Button("Export", GUILayout.Height(30)))
		{
			//	クリックされたときに書き出し処理を実行する
			exporter.ExportJson();
		}
	}
}
