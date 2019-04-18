using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using Microsoft.Win32;

using PowerSystem.IEEEComtrade;

namespace PowerSystem
{
    public static class Config
    {
        static Dictionary<string, Color> Cores=new Dictionary<string,Color>();
        static string ANAPath = string.Empty;
        static string iDefaultZone = string.Empty;
        public static Brush On
        {
            get
            {
                return new SolidBrush(GetColorByPhase("On"));
            }
        }
        public static Brush Off
        {
            get
            {
                return new SolidBrush(GetColorByPhase("Off"));
            }
        }
        public static Color GetRegistryColor(this TComtrade.TBaseChannel.TPh Ph)
        {
            return GetColorByPhase(Ph);
        }
        public static string AnaFile
        {
            get
            {
                if (ANAPath == string.Empty)
                {
                    ANAPath = OpenAnaFile();
                }
                return ANAPath;
            }
            set
            {
                ANAPath = value;
            }
        }
        private static string OpenAnaFile()
        {
            string sPath = "";
            RegistryKey Key = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Default).OpenSubKey(@"Software\Oscillograph", true);
            if (Key == null)
            {
                Key = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Default).CreateSubKey(@"Software\Oscillograph");
            }
            sPath = (string)Key.GetValue("Ana File Path", @"Exemplos\BR1206D.ANA");
            if (!File.Exists(sPath))
            {
                sPath = pOpenAnaFile();
                if (sPath != string.Empty)
                {
                    Key.SetValue("Ana File Path", sPath);
                }
            }
            Key.SetValue("Ana File Path", sPath);
            Key.Close();
            return sPath;
        }
        public static string pOpenAnaFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "ANAFAS Files (*.ana)|*.ana";
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                switch (openFileDialog.SafeFileName.ToLower().Substring(openFileDialog.SafeFileName.Length - 3))
                {
                    case "ana":
                    case "ANA":
                        if (openFileDialog.CheckFileExists)
                        {
                            return openFileDialog.FileName;
                        }
                        break;
                }
            }
            return string.Empty;
        }
        public static Color GetColorByPhase(string ph)
        {
            Color Cor;
            if (!Cores.TryGetValue(ph, out Cor))
            {
                Cor = GetColorFromRegistry(ph);
                Cores.Add(ph, Cor);
            }
            return Cor;
        }
        public static void SetColorByPhase(string ph, Color value)
        {
            if (Cores.ContainsKey(ph))
            {
                Cores[ph] = value;
            }
            else
            {
                Cores.Add(ph, value);
            }
        }
        public static void Commit()
        {
            foreach (string C in Cores.Keys)
            {
                SetColorToRegistry(C, Cores[C]);
            }
            SetRegister("DefaultZone", iDefaultZone);
            SetRegister("Ana File Path", ANAPath);
            Reload();
        }
        internal static void Reload()
        {
            Cores.Clear();
            iDefaultZone = string.Empty;
        }
        private static Color GetColorFromRegistry(string ph)
        {
            object C = null;
            Color iColor;
            RegistryKey Key = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Default).OpenSubKey(@"Software\Oscillograph\Colors", true);
            if (Key == null)
            {
                Key = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Default).CreateSubKey(@"Software\Oscillograph\Colors");
            }
            C = Key.GetValue(ph);
            if (C == null)
            {
                switch (ph)
                {
                    case "A": iColor = System.Drawing.Color.Blue; break;
                    case "B": iColor = System.Drawing.Color.Black; break;
                    case "C": iColor = System.Drawing.Color.Red; break;
                    case "N": iColor = System.Drawing.Color.Green; break;
                    case "S0": iColor = System.Drawing.Color.Green; break;
                    case "S1": iColor = System.Drawing.Color.Blue; break;
                    case "S2": iColor = System.Drawing.Color.Black; break;
                    case "On": iColor = System.Drawing.Color.Red; break;
                    case "Off": iColor = System.Drawing.Color.Green; break;
                    default: iColor = System.Drawing.Color.Black; break;
                }
                Key.SetValue(ph, iColor.ToArgb());
            }
            else
            {
                iColor = Color.FromArgb((int)C);
            }
            Key.Close();
            return iColor;
        }
        private static void SetColorToRegistry(string ph, Color value)
        {
            RegistryKey Key = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Default).OpenSubKey(@"Software\Oscillograph\Colors", true);
            if (Key == null)
            {
                Key = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Default).CreateSubKey(@"Software\Oscillograph\Colors");
            }
            if ((int)Key.GetValue(ph, 0) != value.ToArgb())
            {
                Key.SetValue(ph, value.ToArgb());
            }
        }
        public static string DefaultZone
        {
            get
            {
                if (string.Empty == iDefaultZone)
                {
                    iDefaultZone = GetRegister("DefaultZone", string.Empty);
                }
                return iDefaultZone;
            }
            set
            {
                iDefaultZone = value;
            }
        }
        private static string GetRegister(string Propertie,string Default)
        {
            string sReturn = "";
            RegistryKey Key = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Default).OpenSubKey(@"Software\Oscillograph", true);
            if (Key == null)
            {
                Key = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Default).CreateSubKey(@"Software\Oscillograph");
            }
            sReturn = (string)Key.GetValue(Propertie, Default);
            Key.Close();
            return sReturn;
        }
        private static void SetRegister(string Propertie, string value)
        {
            RegistryKey Key = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Default).OpenSubKey(@"Software\Oscillograph", true);
            Key.SetValue(Propertie, value);
        }
    }
}
