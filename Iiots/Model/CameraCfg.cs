using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Utilities
{
    /// <summary>
    /// 摄像头配置模型
    /// </summary>
    [XmlRoot("CameraConfig")]
    public class CameraCfg
    {
        public String IpAddress { get; set; } = String.Empty;
        public String UserName { get; set; } = String.Empty;
        public String Password { get; set; } = String.Empty;
        public String RTSPParameter { get; set; } = String.Empty;

        /// <summary>
        /// RTSP地址
        /// </summary>
        public string RTSPURL
        {
            get { return $@"rtsp://{UserName}:{Password}@{IpAddress}/{RTSPParameter}"; }
        }


    }
}

