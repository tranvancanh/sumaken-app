using Android.Content;
using technoleight_THandy.Droid;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using technoleight_THandy.Interface;

[assembly: Dependency(typeof(ClipBoard_Droid))]

namespace technoleight_THandy.Droid
{

	public class ClipBoard_Droid : IClipBoard
	{
        //ペースト用メソッド
        public async Task<string> GetTextFromClipBoardAsync()
		{
			//クリップボードからテキストを取得
			return await Clipboard.GetTextAsync();
		}
	}
}