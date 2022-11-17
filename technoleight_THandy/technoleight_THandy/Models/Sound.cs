﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using technoleight_THandy.Event;
using static System.Environment;

namespace technoleight_THandy.Models
{
    public class Sound
    {
        public List<Sound> SoundOkeyList
        {
            get
            {

                var soundList = new List<Sound>();

                var executingAssembly = Assembly.GetExecutingAssembly();
                string folderName = string.Format("{0}.Sound.ScanOkeySound", executingAssembly.GetName().Name);
                var soundOkeyList = executingAssembly.GetManifestResourceNames().Where(r => r.StartsWith(folderName) && r.EndsWith(".mp3")).ToList();

                for (int i = 0; i < soundOkeyList.Count(); ++i)
                {
                    Sound sound1 = new Sound
                    {
                        DisplayName = "OK" + (i + 1).ToString(),
                        Item = soundOkeyList[i].ToString()
                    };
                    soundList.Add(sound1);
                }

                return soundList;
            }
            set { }
        }

        public List<Sound> SoundErrorList
        {
            get
            {

                var soundList = new List<Sound>();

                var executingAssembly = Assembly.GetExecutingAssembly();
                string folderName = string.Format("{0}.Sound.ScanErrorSound", executingAssembly.GetName().Name);
                var soundErrorList = executingAssembly.GetManifestResourceNames().Where(r => r.StartsWith(folderName) && r.EndsWith(".mp3")).ToList();

                for (int i = 0; i < soundErrorList.Count(); ++i)
                {
                    Sound sound1 = new Sound
                    {
                        DisplayName = "Error" + (i + 1).ToString(),
                        Item = soundErrorList[i].ToString()
                    };
                    soundList.Add(sound1);
                }

                return soundList;
            }
            set { }
        }

        public string DisplayName { get; set; }

        public string Item { get; set; }
    }

}
