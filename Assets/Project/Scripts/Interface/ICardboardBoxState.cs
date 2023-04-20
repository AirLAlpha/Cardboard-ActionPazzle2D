/**********************************************
 * 
 *  ICardboardBoxState.cs 
 *  ダンボールの状態を示すインターフェース
 * 
 *  製作者：牛丸 文仁
 *  制作日：2023/04/17
 * 
 **********************************************/
public interface ICardboardBoxState
{
	public abstract void StateUpdate();		//	各ステートにおける更新処理
}
