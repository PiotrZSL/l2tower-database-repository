using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Community.CsharpSqlite.SQLiteClient;
using System.Windows.Forms;
using System.Data;

namespace L2TowerDatabaseGenerator.Tables
{
    class PrimaryKey : Attribute { };

    abstract class AbstractTable
    {
        public bool insertRecord()
        {
            Type ja = this.GetType();
            string tmp = "INSERT INTO " + ja.Name + "(";
            System.Reflection.FieldInfo[] fieldInfo = ja.GetFields();

            bool was = false;
            foreach (System.Reflection.FieldInfo info in fieldInfo)
            {
                if (was) tmp += ",";
                tmp += info.Name;
                was = true;
            }
            tmp += ") VALUES (";
            was = false;
            foreach (System.Reflection.FieldInfo info in fieldInfo)
            {
                if (was) tmp += ",";
                tmp += "@" + info.Name;
                was = true;
            }
            tmp += ");";

            SqliteCommand sql = new SqliteCommand(tmp, SqliteManager.connection);

            foreach (System.Reflection.FieldInfo info in fieldInfo)
            {
                object temp = info.GetValue(this);
                if (temp is long)
                {
                    sql.Parameters.Add(new SqliteParameter("@"+info.Name, temp));
                }
                else
                    if (temp is string)
                    {
                        sql.Parameters.Add(new SqliteParameter("@" + info.Name, temp));
                    }
                    else
                        if (temp is double)
                        {
                            sql.Parameters.Add(new SqliteParameter("@" + info.Name, temp));
                        }
                        else
                            if (temp is bool)
                            {
                                if ((Boolean)temp)
                                    sql.Parameters.Add(new SqliteParameter("@" + info.Name, "1"));
                                else
                                    sql.Parameters.Add(new SqliteParameter("@" + info.Name, "0"));
                            }
                            else
                            {
                                sql.Parameters.Add(new SqliteParameter("@" + info.Name, temp.ToString()));
                            }
            }
            

            try
            {
                return sql.ExecuteNonQuery() != 0;
            }
            catch (Exception e)
            {
                MessageBox.Show("Error durring row insert.\n" + sql.ToString() + "\n" + e.ToString());
                return false;
            }
        }

        public bool updateOrInsertRecord()
        {
            if (updateRecord()) return true;
            return insertRecord();
        }

        public bool getRecord()
        {
            Type ja = this.GetType();
            System.Reflection.FieldInfo[] fieldInfo = ja.GetFields();
            string where = "";
            string fields = "";
            foreach (System.Reflection.FieldInfo info in fieldInfo)
            {
                if (info.IsDefined(typeof(PrimaryKey), true))
                {
                    if (where.Length != 0) where += " AND ";
                    where += "("+info.Name+" = @"+info.Name+")";
                }
                else
                {
                    if (fields.Length != 0) fields += ", ";
                    fields += info.Name;
                }
            }
            try
            {
                SqliteCommand sql = new SqliteCommand("SELECT " + fields + " FROM " + ja.Name + " WHERE " + where, SqliteManager.connection);
                foreach (System.Reflection.FieldInfo info in fieldInfo)
                {
                    if (!info.IsDefined(typeof(PrimaryKey), true)) continue;
                    object temp = info.GetValue(this);
                    if (temp is long)
                    {
                        sql.Parameters.Add(new SqliteParameter("@"+info.Name, temp));
                    }
                    else
                        if (temp is string)
                        {
                            sql.Parameters.Add(new SqliteParameter("@" + info.Name, temp));
                        }
                        else
                            if (temp is double)
                            {
                                sql.Parameters.Add(new SqliteParameter("@" + info.Name, temp));
                            }
                            else
                                if (temp is bool)
                                {
                                    if ((Boolean)temp)
                                        sql.Parameters.Add(new SqliteParameter("@" + info.Name, "1"));
                                    else
                                        sql.Parameters.Add(new SqliteParameter("@" + info.Name, "0"));
                                }
                                else
                                {
                                    sql.Parameters.Add(new SqliteParameter("@" + info.Name, temp.ToString()));
                                }
                }

                IDataReader rs = sql.ExecuteReader();
                if (null == rs) return false;
                try
                {
                    if (!rs.Read()) return false;
                    int i = 0;
                    foreach (System.Reflection.FieldInfo info in fieldInfo)
                    {
                        if (!info.IsDefined(typeof(PrimaryKey), true))
                        {
                            Type temp = info.FieldType;
                            if (temp == typeof(long))
                            {
                                info.SetValue(this, rs.GetInt64(i));
                            }
                            else
                                if (temp == typeof(string))
                                {
                                    info.SetValue(this, rs.GetString(i));
                                }
                                else
                                    if (temp == typeof(double))
                                    {
                                        info.SetValue(this, rs.GetDouble(i));
                                    }
                                    else
                                        if (temp == typeof(bool))
                                        {
                                            info.SetValue(this, rs.GetInt32(i) == 1);
                                        }
                                        else
                                        {
                                            info.SetValue(this, rs.GetString(i));
                                        }
                            i++;
                        }
                    }
                    return true;
                } finally
                {
                    rs.Close();
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool updateRecord()
        {
             Type ja = this.GetType();
            string tmp = "UPDATE " + ja.Name + " SET ";
            System.Reflection.FieldInfo[] fieldInfo = ja.GetFields();

            bool was = false;
            foreach (System.Reflection.FieldInfo info in fieldInfo)
            {
                if (info.IsDefined(typeof(PrimaryKey), true)) continue;
                if (was) tmp += ",";
                tmp += info.Name+"=@"+info.Name;
                was = true;
            }
            tmp+=" WHERE ";
            string where = "";
            foreach (System.Reflection.FieldInfo info in fieldInfo)
            {
                if (info.IsDefined(typeof(PrimaryKey), true))
                {
                    if (where.Length != 0) where += " AND ";
                    where += "("+info.Name+" = @"+info.Name+")";
                }
            }
            tmp += where;

            SqliteCommand sql = new SqliteCommand(tmp, SqliteManager.connection);
            foreach (System.Reflection.FieldInfo info in fieldInfo)
            {
                object temp = info.GetValue(this);
                if (temp is long)
                {
                    sql.Parameters.Add(new SqliteParameter("@"+info.Name, temp));
                }
                else
                    if (temp is string)
                    {
                        sql.Parameters.Add(new SqliteParameter("@" + info.Name, temp));
                    }
                    else
                        if (temp is double)
                        {
                            sql.Parameters.Add(new SqliteParameter("@" + info.Name, temp));
                        }
                        else
                            if (temp is bool)
                            {
                                if ((Boolean)temp)
                                    sql.Parameters.Add(new SqliteParameter("@" + info.Name, "1"));
                                else
                                    sql.Parameters.Add(new SqliteParameter("@" + info.Name, "0"));
                            }
                            else
                            {
                                sql.Parameters.Add(new SqliteParameter("@" + info.Name, temp.ToString()));
                            }
            }

            try
            {
                return sql.ExecuteNonQuery() != 0;
            }
            catch (Exception e)
            {
                MessageBox.Show("Error durring row update.\n" + sql.ToString() + "\n"+tmp+"\n" + e.ToString());
                return false;
            }
        }

        public static void createTable(Type ja)
        {
            try
            {
                SqliteCommand sql = new SqliteCommand("DROP TABLE " + ja.Name, SqliteManager.connection);
                sql.ExecuteNonQuery();
            }
            catch (Exception e)
            {
            }
            string tmp = "CREATE TABLE " + ja.Name + "(";
            System.Reflection.FieldInfo[] fieldInfo = ja.GetFields();

            bool was = false;
            string primary = "";
            foreach (System.Reflection.FieldInfo info in fieldInfo)
            {
                Type temp = info.FieldType;
                if (was) tmp += ", ";
                tmp += info.Name + " ";
                was = true;
                if (temp == typeof(long))
                {
                    tmp += " INT";
                }
                else
                    if (temp == typeof(string))
                    {
                        tmp += " TEXT";
                    }
                    else
                        if (temp == typeof(double))
                        {
                            tmp += " REAL";
                        }
                        else
                            if (temp == typeof(bool))
                            {
                                tmp += " INT";
                            }
                            else
                            {
                                tmp += " TEXT";
                            }
                if (info.IsDefined(typeof(PrimaryKey), true))
                {
                    if (primary.Length != 0) primary += ", ";
                    primary += info.Name;
                }
            }
            if (primary.Length > 0)
            {
                tmp += ", PRIMARY KEY(" + primary + ") ";
            }
            tmp += ");";
            SqliteCommand sql2 = new SqliteCommand(tmp, SqliteManager.connection);
            sql2.ExecuteNonQuery();
        }
    }
}
