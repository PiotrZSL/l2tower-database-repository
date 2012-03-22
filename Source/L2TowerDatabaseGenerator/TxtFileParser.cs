using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace L2TowerDatabaseGenerator
{
    class TxtFileParser
    {
        private class TxtFileParserItem
        {
            public string key;
            public string value;
        }

        private List<TxtFileParserItem[]> items = new List< TxtFileParserItem[] >();

        public TxtFileParser(string file, string prefix)
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

            while (true)
            {
                int begin = line.IndexOf("/*");
                if (-1 == begin) break;
                int end = line.IndexOf("*/", begin);
                if (-1 == end) break;
                line = line.Remove(begin, end - begin + 2);
            }
            streamReader.Close();
            int lastIndex = 0;
            do
            {
                int begin = line.IndexOf(prefix + "_begin", lastIndex);
                if (-1 == begin) break;
                int end = line.IndexOf(prefix + "_end", begin);
                if (-1 == end) break;
                lastIndex = end;
                string l = line.Substring(begin + (prefix + "_begin").Length, end - begin - (prefix + "_begin").Length - 1).Trim();
                string[] table = l.Split('\t');

                List<TxtFileParserItem> it = new List<TxtFileParserItem>();
                foreach (string s in table)
                {
                    TxtFileParserItem tmp = new TxtFileParserItem();
                    if (s.IndexOf('=') != -1)
                    {
                        tmp.key = s.Remove(s.IndexOf('=')).Trim();
                        tmp.value = s.Remove(0, s.IndexOf('=') + 1).Trim();
                    }
                    else
                    {
                        tmp.key = "";
                        tmp.value = s.Trim();
                    }

                    if (tmp.value.Length > 0)
                        it.Add(tmp);
                }
                if (it.Count > 0)
                {
                    items.Add(it.ToArray());
                }
            } while (true);
            Log.log("Finished loading into memory - " + file);
        }

        public int getRecordsCount()
        {
            return items.Count;
        }

        public string getValue(int index, string column)
        {
            if (index >= items.Count) return null;
            foreach (TxtFileParserItem s in items[index])
            {
                if (s.key == column) return s.value;
            }
            return null;
        }

        public string getValue(int index, int column)
        {
            if (index >= items.Count) return null;
            if (items[index].Length <= column) return null;
            return items[index][column].value;
        }
    }
}
