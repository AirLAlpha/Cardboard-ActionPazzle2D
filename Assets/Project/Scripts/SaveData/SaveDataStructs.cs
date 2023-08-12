/**********************************************
 * 
 *  SaveDataStructs.cs 
 *  セーブデータに使用する構造体を記述
 * 
 *  製作者：牛丸 文仁
 *  制作日：2023/04/14
 * 
 **********************************************/

//	1ステージ分の構造体
[System.Serializable]
public struct TaskScore
{
	public float clearTime;         //	クリア時間
	public int usedBoxCount;        //	使用した箱の数

	//	コンストラクタ
	public TaskScore(float time, int usedBoxCount)
	{
		this.clearTime = time;
		this.usedBoxCount = usedBoxCount;
	}
}
//	ステージすべての構造体
[System.Serializable]
public struct StageScore
{
	public TaskScore[] scores;
	//	コンストラクタ
	public StageScore(TaskScore[] scores) { this.scores = scores; }
}
//	ゲーム全体のセーブデータの構造体
[System.Serializable]
public struct SaveData
{
	//	各ステージのスコア
	public StageScore[] stageScores;

	public int lastSelectStage;
	//	コンストラクタ
	public SaveData(StageScore[] scores, int lastSelectStage) { this.stageScores = scores; this.lastSelectStage = lastSelectStage; }
}
