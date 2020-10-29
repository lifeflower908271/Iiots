using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;
using NLE.Device.Led;
using Utilities;

namespace NLE.Device.Led
{

    public sealed class LedCfgSrc
    {
        public static PlayModeConverter PlayModeTo => Singleton<PlayModeConverter>.GetInstance();
        public static LevelConverter LevelTo => Singleton<LevelConverter>.GetInstance();

        #region Led播放模式
        public sealed class PlayModeModel
        {

            public PlayMode Value { get; set; }

            public override string ToString()
            {
                switch (Value)
                {
                    case PlayMode.Left:
                        return "左移";
                    case PlayMode.Up:
                        return "上移";
                    case PlayMode.Down:
                        return "下移";
                    case PlayMode.DownCover:
                        return "下覆盖";
                    case PlayMode.UpCover:
                        return "上覆盖";
                    case PlayMode.WhitenUp:
                        return "翻白显示";
                    case PlayMode.Twinkle:
                        return "闪烁显示";
                    case PlayMode.Immediately:
                        return "立即打出";
                    default:
                        return null;
                }
            }
        }

        [ValueConversion(typeof(PlayModeModel), typeof(String))]
        public sealed class PlayModeConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return ((PlayModeModel)value).ToString();
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }

        private static ObservableCollection<PlayModeModel> _obsPlayMode;

        public static ObservableCollection<PlayModeModel> ObsPlayMode
        {
            get
            {
                if (_obsPlayMode == null)
                {
                    Interlocked.CompareExchange(ref _obsPlayMode, new ObservableCollection<PlayModeModel>(), null);
                }
                return _obsPlayMode;
            }
        }

        private static void ObsPlayModeInitialize()
        {
            ObsPlayMode.Add(new PlayModeModel() { Value = PlayMode.Left });
            ObsPlayMode.Add(new PlayModeModel() { Value = PlayMode.Up });
            ObsPlayMode.Add(new PlayModeModel() { Value = PlayMode.Down });
            ObsPlayMode.Add(new PlayModeModel() { Value = PlayMode.DownCover });
            ObsPlayMode.Add(new PlayModeModel() { Value = PlayMode.UpCover });
            ObsPlayMode.Add(new PlayModeModel() { Value = PlayMode.WhitenUp });
            ObsPlayMode.Add(new PlayModeModel() { Value = PlayMode.Twinkle });
            ObsPlayMode.Add(new PlayModeModel() { Value = PlayMode.Immediately });
        }
        #endregion


        #region Led播放速度
        public sealed class SpeedModel
        {

            public Speed Value { get; set; }

            public override string ToString()
            {
                switch (Value)
                {
                    case Speed.Level0:
                        return "level-0";
                    case Speed.Level1:
                        return "level-1";
                    case Speed.Level2:
                        return "level-2";
                    case Speed.Level3:
                        return "level-3";
                    case Speed.Level4:
                        return "level-4";
                    case Speed.Level5:
                        return "level-5";
                    case Speed.Level6:
                        return "level-6";
                    case Speed.Level7:
                        return "level-7";
                    default:
                        return null;
                }
            }
        }

        [ValueConversion(typeof(SpeedModel), typeof(String))]
        public sealed class LevelConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return ((SpeedModel)value).ToString();
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }

        private static ObservableCollection<SpeedModel> _obsSpeed;

        public static ObservableCollection<SpeedModel> ObsSpeed
        {
            get
            {
                if (_obsSpeed == null)
                {
                    Interlocked.CompareExchange(ref _obsSpeed, new ObservableCollection<SpeedModel>(), null);
                }
                return _obsSpeed;
            }
        }

        private static void ObsSpeedInitialize()
        {
            ObsSpeed.Add(new SpeedModel() { Value = Speed.Level0 });
            ObsSpeed.Add(new SpeedModel() { Value = Speed.Level1 });
            ObsSpeed.Add(new SpeedModel() { Value = Speed.Level2 });
            ObsSpeed.Add(new SpeedModel() { Value = Speed.Level3 });
            ObsSpeed.Add(new SpeedModel() { Value = Speed.Level4 });
            ObsSpeed.Add(new SpeedModel() { Value = Speed.Level5 });
            ObsSpeed.Add(new SpeedModel() { Value = Speed.Level6 });
            ObsSpeed.Add(new SpeedModel() { Value = Speed.Level7 });
        }
        #endregion


        static LedCfgSrc()
        {
            ObsPlayModeInitialize();
            ObsSpeedInitialize();
        }
    }
}
