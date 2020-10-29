using System;

namespace NLE.Device.Led
{
	/// <summary>
	/// 播放模式
	/// </summary>
	public enum PlayMode
	{
		/// <summary>
		/// 1 -- 左移
		/// </summary>
		Left = 1,
		/// <summary>
		///  2 -- 上移
		/// </summary>
		Up,
		/// <summary>
		///    3 -- 下移
		/// </summary>
		Down,
		/// <summary>
		/// 4 -- 下覆盖
		/// </summary>
		DownCover,
		/// <summary>
		///  5 -- 上覆盖
		/// </summary>
		UpCover,
		/// <summary>
		/// 6 -- 翻白显示(反色)
		/// </summary>
		WhitenUp,
		/// <summary>
		///  7 -- 闪烁显示
		/// </summary>
		Twinkle,
		/// <summary>
		///  8 -- 立即打出
		/// </summary>
		Immediately
	}
}
