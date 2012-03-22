using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using MySql.Data.MySqlClient;
using System.Windows.Forms;

namespace L2TowerDatabaseGenerator
{
    class MysqlManager
    {
        public static void StartMysql()
        {
            System.Diagnostics.ProcessStartInfo p = new System.Diagnostics.ProcessStartInfo(Path.GetDirectoryName(Application.ExecutablePath)+"/mysql/mysql_start.bat");
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.StartInfo = p;
            Log.log("Starting mysql database");
            proc.Start();
            proc.WaitForExit();
        }

        public static void StopMysql()
        {
            System.Diagnostics.ProcessStartInfo p = new System.Diagnostics.ProcessStartInfo(Path.GetDirectoryName(Application.ExecutablePath)+"/mysql/mysql_stop.bat");
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            Log.log("Stoping mysql database");
            proc.StartInfo = p;
            proc.Start();
            proc.WaitForExit();
        }

        public static void LoadDatabase(string name)
        {
            Log.log("Loading mysql database for: "+name);
            File.Copy(Path.GetDirectoryName(Application.ExecutablePath)+"\\database_installer.bat", Path.GetDirectoryName(Application.ExecutablePath)+"\\..\\data\\" + name + "\\l2jsql\\database_installer.bat", true);
            File.Copy(Path.GetDirectoryName(Application.ExecutablePath)+"\\vars.txt", Path.GetDirectoryName(Application.ExecutablePath)+"\\../data\\" + name + "\\l2jsql\\vars.txt", true);

            System.Diagnostics.ProcessStartInfo p = new System.Diagnostics.ProcessStartInfo(Path.GetDirectoryName(Application.ExecutablePath) + "\\..\\data\\" + name + "\\l2jsql\\database_installer.bat");
            p.WorkingDirectory = Path.GetDirectoryName(Application.ExecutablePath)+"/../data/" + name + "/l2jsql/";
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.StartInfo = p;
            proc.Start();
            proc.WaitForExit();
            File.Delete(Path.GetDirectoryName(Application.ExecutablePath)+"/../data/" + name + "/l2jsql/database_installer.bat");
            File.Delete(Path.GetDirectoryName(Application.ExecutablePath)+"/../data/" + name + "/l2jsql/vars.txt");
            Log.log("Loaded mysql database for: " + name);
        }

        public static MySqlConnection connection = null;

        public static bool OpenDatabase()
        {
            try
            {
                Log.log("Opening mysql database connection");
                connection = new MySqlConnection("Server=localhost;Port=3311;Database=l2jgs;Uid=root;Pwd=root;");
                connection.Open();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public static void CloseDatabase()
        {
            try
            {
                Log.log("Closing mysql database connection");
                connection.Close();
                connection = null;
            }
            catch (Exception e)
            {
            }
        }
    }
}
