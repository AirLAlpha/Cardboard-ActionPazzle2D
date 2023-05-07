using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

//	参考：http://anndoroido.blogspot.com/2016/03/unity.html
public class TitleBackgroundLoader : MonoBehaviour
{
	private SpriteRenderer spriteRenderer;

	//	実行前初期化処理
	private void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();

		string path = UnityEngine.Application.streamingAssetsPath;
		path = path.Replace("/", "\\");
		path = Path.Combine(path, "title_background.png");

		//	外部ファイルを読み込む
		byte[] result;
		try
		{
			FileStream fs = new FileStream(path, FileMode.Open);
			BinaryReader br = new BinaryReader(fs);
			result = br.ReadBytes((int)br.BaseStream.Length);
			br.Close();
			fs.Close();
		}
		catch
		{
			spriteRenderer.sprite = null;
			return;
		}

		//	テクスチャの作成
		Texture2D tex = new Texture2D(0, 0);
		tex.LoadImage(result);
		tex.filterMode = FilterMode.Point;

		Rect rect = new Rect(0, 0, tex.width, tex.height);
		var sprite = Sprite.Create(tex, rect, new Vector2(0.5f,0.5f), 32.0f);

		this.spriteRenderer.sprite = sprite;
	}

	//	初期化処理
	private void Start()
	{
		
	}

	//	更新処理
	private void Update()
	{
		
	}
}
