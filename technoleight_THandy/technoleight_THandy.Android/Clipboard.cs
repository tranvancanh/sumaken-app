using Android.Content;
using THandy.Droid;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using THandy.Interface;

[assembly: Dependency(typeof(ClipBoard_Droid))]

namespace THandy.Droid
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