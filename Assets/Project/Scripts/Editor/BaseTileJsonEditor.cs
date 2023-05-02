using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BaseTileJson))]
public class BaseTileJsonEditor : Editor
{
	public override void OnInspectorGUI()
	{
		var tileJson = target as BaseTileJson;

		//	元のインスペクターを表示
		base.OnInspectorGUI();

		//	隙間を開ける
		GUILayout.Space(10);

		//	ステージの書き出し処理を実行するボタンを描画
		if (GUILayout.Button("Export Json", GUILayout.Height(30)))
		{
			tileJson.ExportJson();
		}
	}
}
