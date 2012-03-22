using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using L2TowerDatabaseGenerator.Tables;

namespace L2TowerDatabaseGenerator
{
    class L2OffParse
    {
        public void Process(string dir)
        {
             Process_npcdata_txt(dir);
             Process_skilldata_txt(dir);
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

        private void Process_skilldata_txt(string dir)
        {
            Log.log("Processing skilldata.txt - Started");
            TxtFileParser dp = new TxtFileParser(dir + "l2offscript\\skilldata.txt", "skill");

            for (int i = 0; i < dp.getRecordsCount(); i++)
            {
                SkillInfo it = new SkillInfo();
                it.id = processInt(dp.getValue(i, "skill_id"));
                it.level = processInt(dp.getValue(i, "level"));

                if (it.getRecord())
                {
                    it.is_magic = 1==processInt(dp.getValue(i, "is_magic"));
                    if (it.is_magic) it.is_physic = false;
                    it.mp_consume = processInt(dp.getValue(i, "mp_consume1"))+processInt(dp.getValue(i, "mp_consume2"));
                    it.hp_consume = processInt(dp.getValue(i, "hp_consume"));
                    it.cast_range = processInt(dp.getValue(i, "cast_range"));
                    it.effect_range = processInt(dp.getValue(i, "effective_range"));

                    it.updateOrInsertRecord();
                }
                Log.log2(i + 1, dp.getRecordsCount());
            }
            Log.log("Processing skilldata.txt - Finished");
        }

        private void Process_npcdata_txt(string dir)
        {
            Log.log("Processing npcdata.txt - Started");
            TxtFileParser dp = new TxtFileParser(dir + "l2offscript\\npcdata.txt", "npc");

            for (int i = 0; i < dp.getRecordsCount(); i++)
            {
                NpcInfo it = new NpcInfo();
                it.id = processInt(dp.getValue(i, 1));
                if (it.getRecord())
                {
                    if (it.name.Length == 0)
                    {
                        it.name = dp.getValue(i, 2);
                    }
                    it.level = processInt(dp.getValue(i, "level"));
                    /*it.agro_range = processInt(dp.getValue(i, "agro_range"));
                      string ai = dp.getValue(i, "npc_ai");
                      if (ai==null || ai.IndexOf("{[IsAggressive]=1}")==-1)
                          it.agro_range = 0;*/
                    it.updateOrInsertRecord();
                }
                Log.log2(i + 1, dp.getRecordsCount());
            }
            Log.log("Processing npcdata.txt - Finished");
        }
    }
}
