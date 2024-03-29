/**********************************************
 * 
 *  CardboardBoxType.cs 
 *  ダンボール箱のタイプの列挙を記述
 * 
 *  製作者：牛丸 文仁
 *  制作日：2023/04/17
 * 
 **********************************************/

public enum CardboardType
{
	NONE,				//	中身なし

	NORMAL,				//	通常
	BREAKABLE,			//	割れ物注意
	RIGHTSIDEUP,		//	天地無用
	NONPACKABLE,		//	梱包不可
}
