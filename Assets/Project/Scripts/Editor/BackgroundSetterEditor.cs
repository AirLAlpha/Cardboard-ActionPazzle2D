using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BackgroundSetter))]
public class BackgroundSetterEditor : Editor
{
	public override void OnInspectorGUI()
	{
		var setter = target as BackgroundSetter;

		//	元のインスペクターを表示
		base.OnInspectorGUI();

		//	隙間を開ける
		GUILayout.Space(10);

		//	ステージの書き出し処理を実行するボタンを描画
		if (GUILayout.Button("Set background", GUILayout.Height(30)))
		{
			setter.SetBackground();
		}

		using (new GUILayout.HorizontalScope())
		{
			//	ステージの書き出し処理を実行するボタンを描画
			if (GUILayout.Button("Before", GUILayout.Height(30)))
			{
				setter.SetBackground(setter.Index - 1);
			}
			//	ステージの書き出し処理を実行するボタンを描画
			if (GUILayout.Button("Next", GUILayout.Height(30)))
			{
				setter.SetBackground(setter.Index + 1);
			}
		}

	}
}
