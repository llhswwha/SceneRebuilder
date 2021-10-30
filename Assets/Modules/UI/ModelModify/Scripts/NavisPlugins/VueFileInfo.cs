using System;
using System.Collections.Generic;
using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Forms;

namespace NavisPlugins.Infos
{
    public class ModelFileInfo
    {
        public int Id = 0;

        public string VueFile;

        public string XmlFile;

        public double VueSize;

        public int State = 0;

        public string FilePath;

        public override string ToString()
        {
            return string.Format("{0}\t{1:F0}m\t{2}", Id, VueSize, VueFile);
        }

        public FileInfo FileInf;

        public string GetModelPath()
        {
            string path = FileInf.FullName;
            string nwcPath = FileInf.FullName.Replace(FileInf.Extension, ".nwc");
            //return nwcPath;
            if (File.Exists(nwcPath))
            {
                return nwcPath;
            }
            return path;
        }

        public bool IsNWCExist()
        {
            string path = FileInf.FullName;
            string nwcPath = FileInf.FullName.Replace(FileInf.Extension, ".nwc");
            return File.Exists(nwcPath);
        }

        public ModelFileInfo(FileInfo file)
        {
            FileInf = file;

            VueFile = file.Name;
            VueSize = file.Length / (1024.0 * 1024);

            FilePath = file.FullName;

            if (IsNWCExist())
            {
                State = 1;
            }
        }

        internal void MoveToDir(string subDir)
        {
            string dirPath = FileInf.Directory.FullName;
            string subDirPath = dirPath + "\\" + subDir + "\\";
            DirectoryInfo dirInfo = new DirectoryInfo(subDirPath);
            if (dirInfo.Exists == false)
            {
                dirInfo.Create();
            }

            MoveOtherFile(subDirPath, ".xml");
            MoveOtherFile(subDirPath, ".mdb");
            MoveOtherFile(subDirPath, ".ldb");
            MoveOtherFile(subDirPath, ".mdb2");
            MoveOtherFile(subDirPath, ".nwc");

            //MessageBox.Show(subDirPath + File.Name);
            FileInf.MoveTo(subDirPath + FileInf.Name);
        }

        private void MoveOtherFile(string subDirPath,string extension)
        {
            string oldFile = FileInf.FullName.Replace(".vue", extension);
            FileInfo xmlFile = new FileInfo(oldFile);
            if (xmlFile.Exists)
            {
                xmlFile.MoveTo(subDirPath + xmlFile.Name);
            }
            else
            {
                //MessageBox.Show("不存在:" + xmlFile.FullName);
            }
        }

    }
}
