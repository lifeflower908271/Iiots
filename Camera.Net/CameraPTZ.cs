using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Camera.Net
{
	public class CameraPTZ
	{
		public string UserName { get; set; }

		public string Password { get; set; }

		public string Host { get; set; }

		public int Port { get; set; }

		public string Args { get; set; }

		public string CmdLeft
		{
			get
			{
				return this.GetPTZCmd(PTZ.Left);
			}
			set
			{
				this.SetPTZCmd(PTZ.Left, value);
			}
		}

		public string CmdRight
		{
			get
			{
				return this.GetPTZCmd(PTZ.Right);
			}
			set
			{
				this.SetPTZCmd(PTZ.Right, value);
			}
		}

		public string CmdUp
		{
			get
			{
				return this.GetPTZCmd(PTZ.Up);
			}
			set
			{
				this.SetPTZCmd(PTZ.Up, value);
			}
		}

		public string CmdDown
		{
			get
			{
				return this.GetPTZCmd(PTZ.Down);
			}
			set
			{
				this.SetPTZCmd(PTZ.Down, value);
			}
		}

		public string CmdStop
		{
			get
			{
				return this.GetPTZCmd(PTZ.Stop);
			}
			set
			{
				this.SetPTZCmd(PTZ.Stop, value);
			}
		}

		private string GetPTZCmd(PTZ control)
		{
			if (this.dicCmd.ContainsKey(control))
			{
				return this.dicCmd[control];
			}
			return control.ToString().ToLower();
		}

		private void SetPTZCmd(PTZ control, string value)
		{
			this.dicCmd[control] = value;
		}

		private string UrlCombine(params string[] paths)
		{
			return paths.Aggregate((string current, string path) => string.Format("{0}/{1}", current.Trim(new char[]
			{
				'/'
			}), path.Trim(new char[]
			{
				'/'
			})));
		}

		private void SendCommand(PTZ control)
		{
			string strUrl = string.Empty;
			if (this.Host.ToLower().StartsWith("http://"))
			{
				strUrl = this.Host;
			}
			else
			{
				strUrl = "http://[Host]";
				if (this.Port != 80)
				{
					strUrl = "http://[Host]:[Port]";
				}
			}
			if (!string.IsNullOrEmpty(this.Args))
			{
				strUrl = this.UrlCombine(new string[]
				{
					strUrl,
					this.Args
				});
			}
			string cmd = this.GetPTZCmd(control);
			strUrl = strUrl.Replace("[Host]", this.Host);
			strUrl = strUrl.Replace("[Port]", this.Port.ToString());
			strUrl = strUrl.Replace("[UserName]", this.UserName);
			strUrl = strUrl.Replace("[Password]", this.Password);
			strUrl = strUrl.Replace("[PTZ]", cmd);
			try
			{
				HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strUrl);
				request.KeepAlive = false;
				request.Timeout = 5000;
				request.Method = "GET";
				if (!string.IsNullOrEmpty(this.UserName) || !string.IsNullOrEmpty(this.Password))
				{
					request.Credentials = new NetworkCredential(this.UserName, this.Password);
				}
				HttpWebResponse response = request.GetResponse() as HttpWebResponse;
				string empty = string.Empty;
				using (StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
				{
					sr.ReadToEnd();
				}
				response.Close();
			}
			catch (Exception)
			{
			}
		}

		private void TurnCommand(PTZ ptz)
		{
			Task.Run(delegate()
			{
				this.SendCommand(ptz);
				Thread.Sleep(100);
			}).ContinueWith(delegate(Task o)
			{
				this.SendCommand(PTZ.Stop);
			});
		}

		public void TurnLeft()
		{
			this.TurnCommand(PTZ.Left);
		}

		public void TurnRight()
		{
			this.TurnCommand(PTZ.Right);
		}

		public void TurnUp()
		{
			this.TurnCommand(PTZ.Up);
		}

		public void TurnDown()
		{
			this.TurnCommand(PTZ.Down);
		}

		private Dictionary<PTZ, string> dicCmd = new Dictionary<PTZ, string>();
	}
}
