using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NavisPlugins.Infos
{
    public class ModelFileInfoList:List<ModelFileInfo>
    {
        public ModelFileInfoList(DirectoryInfo dirInfo)
        {
            List<FileInfo> modelFiles = new List<FileInfo>();
            modelFiles.AddRange(dirInfo.GetFiles("*.vue"));
            modelFiles.AddRange(dirInfo.GetFiles("*.rvm"));
            modelFiles.AddRange(dirInfo.GetFiles("*.ifc"));
            for (int i = 0; i < modelFiles.Count; i++)
            {
                FileInfo file = modelFiles[i];
                ModelFileInfo vueFile = new ModelFileInfo(file);
                vueFile.Id = i + 1;
                this.Add(vueFile);
            }
            SortByName();
            //SortBySize2();
        }

        private void ResetId()
        {
            for (int i = 0; i < this.Count; i++)
            {
                ModelFileInfo vueFile = this[i];
                vueFile.Id = i + 1;
            }
        }

        public void SortByName()
        {
            this.Sort((a, b) =>
            {
                return b.VueFile.CompareTo(a.VueFile);
            });
            ResetId();
            //Directory.GetFiles()
            //Directory.GetDirectories
        }

        public void SortBySize()
        {
            this.Sort((a, b) =>
            {
                return b.VueSize.CompareTo(a.VueSize);
            });
            ResetId();
        }

        public void SortBySize2()
        {
            this.Sort((a, b) =>
            {
                return a.VueSize.CompareTo(b.VueSize);
            });
            ResetId();
        }
    }
}
