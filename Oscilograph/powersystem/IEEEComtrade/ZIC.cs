using System;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using PowerSystem;

namespace PowerSystem.IEEEComtrade
{
    public class Zic
    {
        public Stream dat;
        public Stream cfg;
        public Stream inf;
        public Stream hdr;
        ZipFile Zip;
        public Zic(string File)
        {
            if ((new FileInfo(File)).Exists)
            {
                Zip = new ZipFile(File);
                foreach (ZipEntry FZip in Zip)
                {
                    switch (FZip.Name.Substring(FZip.Name.Length - 3, 3).ToLower())
                    {
                        case "inf":
                            inf = Zip.GetInputStream(FZip);
                            break;
                        case "cfg":
                            cfg = Zip.GetInputStream(FZip);
                            break;
                        case "dat":
                            dat = Zip.GetInputStream(FZip);
                            break;
                        case "hdr":
                            hdr = Zip.GetInputStream(FZip);
                            break;
                    }
                }
            }
            else
            {
                throw (new FileNotFoundException());
            }
        }

        public Zic(Stream cfg, Stream dat)
        {
            this.dat = dat;
            this.cfg = cfg;
        }

        internal void close()
        {
            Zip.Close();
        }
        private class StaticDataSource : IStaticDataSource
        {
            private Stream stream;
            public StaticDataSource(Stream stream)
            {
                this.stream = stream;
            }
            public Stream GetSource()
            {
                return stream;
            }
        }
        internal void SaveAs(Stream File, string Name)
        {
            Zip = new ZipFile(File);
            Zip.BeginUpdate();
            if (dat != null) Zip.Add(new StaticDataSource(dat), Name + ".dat");
            if (hdr != null) Zip.Add(new StaticDataSource(hdr), Name + ".hdr");
            if (cfg != null) Zip.Add(new StaticDataSource(cfg), Name + ".cfg");
            Zip.CommitUpdate();
            Zip.Close();
            File.Close();
        }
    }
}