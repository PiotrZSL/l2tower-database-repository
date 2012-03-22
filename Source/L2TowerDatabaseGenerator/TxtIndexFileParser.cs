using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace L2TowerDatabaseGenerator
{
    class TxtIndexFileParser
    {
        private Dictionary<string, string> items = new Dictionary<string, string>();

        public TxtIndexFileParser(string file, string prefix)
        {
            StreamReader streamReader = new StreamReader(file);
            StringBuilder sb = new StringBuilder();
            string line = "";
            {
                string wp = streamReader.ReadToEnd();
                string[] tmp2 = Regex.Split(wp, @"\r?\n|\r");
                foreach (string t in tmp2)
                {
                    string tmp = t;
                    if (tmp.IndexOf("//") != -1)
                        tmp = tmp.Remove(tmp.IndexOf("//"));
                    sb.Append(" ");
                    sb.Append(tmp);
                }
            }
            line = sb.ToString();
            while(true)
            {
                int begin = line.IndexOf("/*");
                if (-1 == begin) break;
                int end = line.IndexOf("*/", begin);
                if (-1 == end) break;
                line = line.Remove(begin, end-begin+2);
            }
            streamReader.Close();
            string[] lines = line.Split('\n');
            foreach (string s in lines)
            {
                string[] tmp = s.Split('=');
                if (tmp.Length != 2) continue;
                tmp[0] = tmp[0].Trim();
                tmp[1] = tmp[1].Trim();
                if (tmp[0].Length > 0)
                {
                    if (!items.ContainsKey(tmp[0]))
                    items.Add(tmp[0],tmp[1]);
                }
            }
        }

        public string getValue(string key)
        {
          if (! items.ContainsKey(key)) return null;
          return items[key];
        }
    }
}
