using System;
using System.Threading.Tasks;

namespace technoleight_THandy.Interface
{
	public interface IClipBoard
	{
		//ペースト用メソッド
		Task<string> GetTextFromClipBoardAsync();
	}
}