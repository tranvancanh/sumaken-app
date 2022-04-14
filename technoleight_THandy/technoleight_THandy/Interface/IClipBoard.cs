using System;
using System.Threading.Tasks;

namespace THandy.Interface
{
	public interface IClipBoard
	{
		//ペースト用メソッド
		Task<string> GetTextFromClipBoardAsync();
	}
}