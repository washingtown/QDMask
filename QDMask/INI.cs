using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
using System.IO;
using System.Linq;
using System.Text;

namespace QDMask
{
    public class IniFile
    {
        public string inipath;
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, byte[] retVal, int size, string filePath);
        /// <summary> 
        /// 构造方法 
        /// </summary> 
        /// <param name="INIPath">文件路径</param> 
        public IniFile(string INIPath)
        {
            inipath = INIPath;
        }
        /// <summary> 
        /// 写入INI文件 
        /// </summary> 
        /// <param name="Section">项目名称(如 [TypeName] )</param> 
        /// <param name="Key">键</param> 
        /// <param name="Value">值</param> 
        public void IniWriteValue(string Section, string Key, string Value)
        {
            WritePrivateProfileString(Section, Key, Value, this.inipath);
        }
        /// <summary> 
        /// 读出INI文件 
        /// </summary> 
        /// <param name="Section">项目名称(如 [TypeName] )</param> 
        /// <param name="Key">键</param> 
        public string IniReadValue(string Section, string Key)
        {
            StringBuilder temp = new StringBuilder(500);
            int i = GetPrivateProfileString(Section, Key, "", temp, 500, this.inipath);
            return temp.ToString();
        }
        /// <summary>
        /// 获取配置文件中的字段列表
        /// </summary>
        /// <returns></returns>
        public StringCollection IniGetSections()
        {
            byte[] buffer = new byte[65535];
            StringBuilder temp = new StringBuilder(500);
            int i = GetPrivateProfileString(null, null, "", buffer, buffer.GetUpperBound(0), this.inipath);
            StringCollection strings = new StringCollection();
            GetStringsFromBuffer(buffer, i, strings);
            return strings;
        }
        /// <summary>
        /// 获取指定字段的键列表
        /// </summary>
        /// <param name="section">字段</param>
        /// <returns></returns>
        public StringCollection IniGetKeys(string section)
        {
            byte[] buffer = new byte[65535];
            StringBuilder temp = new StringBuilder(500);
            int i = GetPrivateProfileString(section, null, "", buffer, buffer.GetUpperBound(0), this.inipath);
            StringCollection strings = new StringCollection();
            GetStringsFromBuffer(buffer, i, strings);
            return strings;
        }
        /// <summary>
        /// 删除指定字段
        /// </summary>
        /// <param name="Section">字段名</param>
        public void IniDeleteSection(string Section)
        {
            WritePrivateProfileString(Section, null, null, this.inipath);
        }
        /// <summary>
        /// 删除指定键
        /// </summary>
        /// <param name="Section">字段</param>
        /// <param name="Key">键</param>
        public void IniDeleteKey(string Section, string Key)
        {
            WritePrivateProfileString(Section, Key, null, this.inipath);
        }
        /// <summary>
        /// 重命名指定字段
        /// </summary>
        /// <param name="section">字段名</param>
        /// <param name="newSection">新字段名</param>
        public void IniRenameSection(string section, string newSection)
        {
            Dictionary<string, string> keyValues = new Dictionary<string, string>();
            foreach (string key in IniGetKeys(section))
            {
                keyValues.Add(key, IniReadValue(section, key));
            }
            IniDeleteSection(section);
            foreach (string key in keyValues.Keys)
            {
                IniWriteValue(newSection, key, keyValues[key]);
            }
        }
        /// <summary>
        /// 重命名指定键
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <param name="newKey"></param>
        public void IniRenameKey(string section, string key, string newKey)
        {
            string keyValue = IniReadValue(section, key);
            IniDeleteKey(section, key);
            IniWriteValue(section, newKey, keyValue);
        }
        /// <summary> 
        /// 验证文件是否存在 
        /// </summary> 
        /// <returns>布尔值</returns> 
        public bool ExistINIFile()
        {
            return File.Exists(inipath);
        }
        /// <summary>
        /// 由缓冲区获得文本
        /// </summary>
        /// <param name="Buffer">二进制缓冲区</param>
        /// <param name="bufLen">长度</param>
        /// <param name="Strings">获得的文本</param>
        private void GetStringsFromBuffer(Byte[] Buffer, int bufLen, StringCollection Strings)
        {
            Strings.Clear();
            if (bufLen != 0)
            {
                int start = 0;
                for (int i = 0; i < bufLen; i++)
                {
                    if ((Buffer[i] == 0) && ((i - start) > 0))
                    {
                        String s = Encoding.GetEncoding(0).GetString(Buffer, start, i - start);
                        Strings.Add(s);
                        start = i + 1;
                    }
                }
            }
        }
    }
}