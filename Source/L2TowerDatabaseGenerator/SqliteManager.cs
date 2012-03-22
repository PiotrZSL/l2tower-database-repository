using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Community.CsharpSqlite.SQLiteClient;
using L2TowerDatabaseGenerator.Tables;

namespace L2TowerDatabaseGenerator
{
    class SqliteManager
    {
        public static SqliteConnection connection;
        private static System.Data.Common.DbTransaction trans;

        public static bool OpenDatabase(string dbFileName)
        {
            try
            {
                Log.log("Opening sqlite database connection: "+dbFileName);
                string cs = string.Format("Version=3,uri=file:{0}", dbFileName);
                connection = new SqliteConnection(cs);
                connection.Open();
                trans =  connection.BeginTransaction();
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
                Log.log("Closing sqlight database connection");
                trans.Commit();
                connection.Close();
                connection = null;
            }
            catch (Exception e)
            {
            }
        }

        public static void CreateTables()
        {
            Log.log("Creating sqllite tables");
            AbstractTable.createTable(typeof(NpcInfo));
            AbstractTable.createTable(typeof(NpcInfoPassive));
            AbstractTable.createTable(typeof(ItemInfo));
            AbstractTable.createTable(typeof(SkillInfo));
            AbstractTable.createTable(typeof(RecipeInfo));
            AbstractTable.createTable(typeof(RecipeInfoItem));
        }
    }
}
