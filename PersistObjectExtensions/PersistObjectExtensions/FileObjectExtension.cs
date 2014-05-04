using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.PersistObjectExtension
{
    public static class FileObjectExtension
    {

        public static void FilePersist(this object __obj, String filepath)
        {
            WriteBinary(filepath, __obj);
        }

        public static void LoadFromFile(this Object __obj, String filepath)
        {
            Type _to = __obj.GetType();
            Object retv = ReadBinary<Object>(filepath);

            if (__obj is System.Collections.IList)
            {
                System.Collections.IList OList = (System.Collections.IList)__obj;
                System.Collections.IList lList = (System.Collections.IList)retv;

                OList.Clear();
                foreach (var item in lList)
                {
                    OList.Add(item);
                }

                return;
            }

            var Pinfs = _to.GetProperties();
            foreach (var item in Pinfs)
            {
                var vsource = item.GetValue(retv);
                item.SetValue(__obj, vsource);
            }
        }

        private static void WriteBinary<T>(string __fPath, T __objW)
        {
            using (Stream stream = File.Open(__fPath, FileMode.Create))
            {
                var bin = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                bin.Serialize(stream, __objW);
            }
        }


        private static T ReadBinary<T>(string __fPath)
        {
            using (Stream stream = File.Open(__fPath, FileMode.Open))
            {
                var bin = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                return (T)bin.Deserialize(stream);
            }
        }
    }
}
