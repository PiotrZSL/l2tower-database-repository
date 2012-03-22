using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Data;
using L2TowerDatabaseGenerator.Tables;
using System.Windows.Forms;
using System.IO;

namespace L2TowerDatabaseGenerator
{
    class L2jParse
    {
        public void Process(string dir)
        {
            Process_npc_table();
            Process_skill_scripts(dir);
        }

        private long processInt(string s)
        {
            if (null == s) return 0;
            try
            {
                return Convert.ToInt64(s);
            }
            catch (Exception e)
            {
                MessageBox.Show("Failed to convert to int - " + s);
                return 0;
            }
        }

        private void Process_skill_scripts(string dir)
        {
            Log.log("Processing skill scripts - Started");
            string[] files = Directory.GetFiles(dir + "l2jscript\\stats\\skills", "*.xml", SearchOption.AllDirectories);
            List<SkillTableItem> list = new List<SkillTableItem>();
                int i = 0;
                foreach (string fi in files)
                {
                    list.AddRange(SkillTableItem.parseXml(fi));
                    Log.log2(i++, files.Length);
                }

            Log.log("Processing skill scripts - Adding");
            i = 0;
            foreach (SkillTableItem s in list)
            {
                SkillInfo si = new SkillInfo();
                si.id = s.getSkillId();
                si.level = s.getSkillLvl();
                if (si.getRecord())
                {
                    if (si.name.Length == 0)
                        si.name = s.getName();
                    if (processInt(s.getValue("mpConsume")) > si.mp_consume)
                        si.mp_consume = processInt(s.getValue("mpConsume"));
                    if (processInt(s.getValue("hpConsume")) > si.hp_consume)
                        si.hp_consume = processInt(s.getValue("hpConsume"));
                    if ((processInt(s.getValue("castRange")) > 0) && (processInt(s.getValue("castRange")) < si.cast_range))
                        si.cast_range = processInt(s.getValue("castRange"));
                    si.effect_range = processInt(s.getValue("effectRange"));
                    if (!si.is_passive && s.getValue("operateType") != null && s.getValue("operateType").Contains("PASSIVE"))
                    {
                        si.is_passive = true;
                        si.is_active = false;
                        si.is_toogle = false;
                    }
                    if (!si.is_active && s.getValue("operateType") != null && s.getValue("operateType").Contains("ACTIVE"))
                    {
                        si.is_passive = false;
                        si.is_active = true;
                        si.is_toogle = false;
                    }
                    if (!si.is_toogle && s.getValue("operateType") != null && s.getValue("operateType").Contains("TOGGLE"))
                    {
                        si.is_passive = false;
                        si.is_active = false;
                        si.is_toogle = true;
                    }
                    
/*
0 -  
  
TARGET_AREA
TARGET_AREA_CORPSE_MOB
TARGET_AREA_SUMMON
TARGET_AREA_UNDEAD
TARGET_AURA
TARGET_BEHIND_AURA
TARGET_CLAN
TARGET_CLAN_MEMBER
TARGET_CORPSE
TARGET_CORPSE_CLAN
TARGET_CORPSE_MOB
TARGET_CORPSE_PET
TARGET_CORPSE_PLAYER
TARGET_ENEMY_SUMMON
TARGET_FLAGPOLE
TARGET_FRONT_AREA
TARGET_FRONT_AURA
TARGET_GROUND
TARGET_HOLY
TARGET_NONE
TARGET_ONE
TARGET_OWNER_PET
TARGET_PARTY
TARGET_PARTY_CLAN
TARGET_PARTY_MEMBER
TARGET_PARTY_NOTME
TARGET_PARTY_OTHER
TARGET_PET
TARGET_SELF
TARGET_SUMMON
TARGET_UNDEAD
TARGET_UNLOCKABLE*/

                    si.updateOrInsertRecord();
                }
                Log.log2(i++, list.Count);
            }

            Log.log("Processing skill scripts - Finished");
        }

        private void Process_npc_table()
        {
            Log.log("Processing npcdata.txt - Started");
            MySqlCommand sql = MysqlManager.connection.CreateCommand();
            sql.CommandText = "SELECT count(*) FROM npc";
            long count = processInt(sql.ExecuteScalar().ToString());
            sql.CommandText = "SELECT * FROM npc";
            MySqlDataReader rs = sql.ExecuteReader();
            try{
                int i = 0;
                while(rs.Read())
                {
                    NpcInfo it = new NpcInfo();
                    it.id = rs.GetInt32("idTemplate");
                    if (it.getRecord())
                    {
                        if (it.name.Length == 0)
                        {
                            it.name = rs.GetString("name");
                        }
                        if (it.title.Length == 0)
                        {
                            it.title = rs.GetString("title");
                        }
                        it.level = rs.GetInt32("level");
                        it.updateOrInsertRecord();
                    }
                    Log.log2(i++, (int)count);
                }
            } finally{
                rs.Close();
            }
            Log.log("Processing npcdata.txt - Finished");
        }
    }
}
