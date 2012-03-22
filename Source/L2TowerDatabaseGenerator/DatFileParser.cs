using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace L2TowerDatabaseGenerator
{
    class DatFileParser
    {
        public DatFileParser(string file)
        {
            StreamReader streamReader = new StreamReader(file);
            try
            {
                if (streamReader.EndOfStream) throw new IOException("File is empty");
                string line = streamReader.ReadLine();
                columns = line.Split('\t');
                if (columns.Length == 0) throw new IOException("No columns found");
                foreach (string w in columns)
                {
                    if (w.Length == 0) throw new IOException("Empty column name");
                }
                while (!streamReader.EndOfStream)
                {
                    line = streamReader.ReadLine();
                    string[] t = line.Split('\t');
                    if (t.Length != columns.Length) continue;
                    data.Add(t);
                }
            }
            finally
            {
                streamReader.Close();
            }
        }

        private string[] columns;
        private List<string[]> data = new List<string[]>();

        public bool isColumn(string name)
        {
            foreach (string s in columns)
            {
                if (s == name) return true;
            }
            return false;
        }

        public int getRecordsCount()
        {
            return data.Count;
        }

        public string getValue(int index, string column)
        {
            if (index >= data.Count) return null;
            int i = 0;
            foreach (string s in columns)
            {
                if (s == column) return data[index][i];
                ++i;
            }
            throw new Exception("Column not found - " + column);
        }

        public string getValue(int index, int column)
        {
            if (index >= data.Count) return null;
            if (columns.Length <= column) return null;
            return data[index][column];
        }
    }
}
