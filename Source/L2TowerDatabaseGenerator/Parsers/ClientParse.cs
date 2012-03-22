using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using L2TowerDatabaseGenerator.Tables;
using System.Windows.Forms;

namespace L2TowerDatabaseGenerator
{
    class ClientParse
    {
        public void Process(string dir)
        {
            ProcessNpcInfo(dir);
            ProcessNpcInfoPassive(dir);
            ProcessItemInfo(dir);
            ProcessSkillInfo(dir);
            ProcessSkillGrpInfo(dir);
            ProcessRecipeInfo(dir);
        }

        private string processString(string s)
        {
            if (s.StartsWith("a,"))
                s = s.Remove(0, 2);
            if (s.EndsWith("\\0"))
                s = s.Remove(s.Length - 2, 2);
            if (s == "none")
                return "";
            return s;
        }

        private long processInt(string s)
        {
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

        private void ProcessSkillGrpInfo(string dir)
        {
            Log.log("Processing skillgrp.txt - Started");
            DatFileParser dp = new DatFileParser(dir + "client\\skillgrp.txt");

            for (int i = 0; i < dp.getRecordsCount(); i++)
            {
                SkillInfo ni = new SkillInfo();
                ni.id = processInt(dp.getValue(i, 0));
                ni.level = processInt(dp.getValue(i, 1));
                ni.getRecord();
                ni.mp_consume = processInt(dp.getValue(i, 4));
                ni.cast_range = processInt(dp.getValue(i, 5));
                ni.is_physic = 0 == processInt(dp.getValue(i, "is_magic"));
                ni.is_magic = 1 == processInt(dp.getValue(i, "is_magic"));
                ni.is_dance_song = 3 == processInt(dp.getValue(i, "is_magic"));

                ni.is_passive = 2 == processInt(dp.getValue(i, 3));
                ni.is_active = 0 == processInt(dp.getValue(i, 3)) || 1 == processInt(dp.getValue(i, 3));
                ni.is_toogle = 3 == processInt(dp.getValue(i, 3));

                ni.icon = processString(dp.getValue(i, "icon_name"));
                ni.hp_consume = processInt(dp.getValue(i, "UNK_1"));
                ni.updateOrInsertRecord();
                Log.log2(i + 1, dp.getRecordsCount());
            }
            Log.log("Processing skillgrp.txt - Finished");
        }

        private void ProcessNpcInfoPassive(string dir)
        {
            Log.log("Processing npcgrp.txt - Started");
            DatFileParser dp = new DatFileParser(dir + "client\\npcgrp.txt");

            for (int i = 0; i < dp.getRecordsCount(); i++)
            {
                NpcInfoPassive ni = new NpcInfoPassive();
                ni.id = Convert.ToInt32(dp.getValue(i, "tag"));
                for (int ii = 0; ii < 20; ii++)
                {
                    if (!(dp.isColumn("dtab1[" + (ii * 2) + "]") && dp.isColumn("dtab1[" + ((ii * 2) + 1) + "]"))) continue;
                    try
                    {
                        ni.skillId = Convert.ToInt32(dp.getValue(i, "dtab1[" + (ii * 2) + "]"));
                    }
                    catch (Exception e)
                    {
                        ni.skillId = 0;
                    }
                    try
                    {
                        ni.skillLvl = Convert.ToInt32(dp.getValue(i, "dtab1[" + ((ii * 2) + 1) + "]"));
                    }
                    catch (Exception e)
                    {
                        ni.skillLvl = 0;
                    }
                    if (ni.skillLvl !=0 && ni.skillId!=0) ni.insertRecord();
                }
                Log.log2(i + 1, dp.getRecordsCount());
            }
            Log.log("Processing npcgrp.txt - Finished");
        }
        

        private void ProcessNpcInfo(string dir)
        {
            Log.log("Processing npcname-e.txt - Started");
            DatFileParser dp = new DatFileParser(dir + "client\\npcname-e.txt");

            for (int i = 0; i < dp.getRecordsCount(); i++)
            {
                NpcInfo ni = new NpcInfo();
                ni.id = processInt(dp.getValue(i, "id"));
                ni.name = processString(dp.getValue(i, "name")).Trim();
                ni.title = processString(dp.getValue(i, "description")).Trim();
                if (ni.name.Length != 0) ni.insertRecord();
                Log.log2(i + 1, dp.getRecordsCount());
            }
            Log.log("Processing npcname-e.txt - Finished");
        }

        private void ProcessItemInfo(string dir)
        {
            Log.log("Processing itemname-e.txt - Started");
            DatFileParser dp = new DatFileParser(dir + "client\\itemname-e.txt");

            for (int i = 0; i < dp.getRecordsCount(); i++)
            {
                ItemInfo ni = new ItemInfo();
                ni.id = processInt(dp.getValue(i, "id"));
                ni.name = processString(dp.getValue(i, "name")).Trim();
                string tmp = processString(dp.getValue(i, "add_name")).Trim();
                if (tmp.Length > 0 && ni.name.Length > 0)
                    ni.name += " - " + tmp;
             //  ni.description = processString(dp.getValue(i, "description")).Trim();
                if (ni.name.Length != 0) ni.insertRecord();
                Log.log2(i + 1, dp.getRecordsCount());
            }
            Log.log("Processing itemname-e.txt - Finished");
        }

        private void ProcessSkillInfo(string dir)
        {
            Log.log("Processing SkillName-e.txt - Started");
            DatFileParser dp = new DatFileParser(dir + "client\\SkillName-e.txt");

            for (int i = 0; i < dp.getRecordsCount(); i++)
            {
                SkillInfo ni = new SkillInfo();
                ni.id = processInt(dp.getValue(i, "id"));
                ni.level = processInt(dp.getValue(i, "level"));
                ni.name = processString(dp.getValue(i, "name")).Trim();
                string tmp = processString(dp.getValue(i, "desc_add1")).Trim();
                if (tmp.Length > 0 && ni.name.Length > 0)
                    ni.name += " (" + tmp + ")";
              //  ni.description = processString(dp.getValue(i, "description")).Trim();
                if (ni.name.Length != 0) ni.insertRecord();
                Log.log2(i + 1, dp.getRecordsCount());
            }
            Log.log("Processing SkillName-e.txt - Finished");
        }

        private void ProcessRecipeInfo(string dir)
        {
            Log.log("Processing recipe-c.txt - Started");
            DatFileParser dp = new DatFileParser(dir + "client\\recipe-c.txt");

            for (int i = 0; i < dp.getRecordsCount(); i++)
            {
                RecipeInfo ni = new RecipeInfo();
                ni.id = processInt(dp.getValue(i, "id_recipe"));
                ni.level = processInt(dp.getValue(i, "level"));
                ni.id_item = processInt(dp.getValue(i, "id_item"));
                ni.mp_cost = processInt(dp.getValue(i, "mp_cost"));
                ni.count = processInt(dp.getValue(i, "count"));
                ni.success_rate = processInt(dp.getValue(i, "success_rate"));
                ni.insertRecord();

                long tmp = processInt(dp.getValue(i, "materials_cnt"));
                for(long j=0;j<tmp;j++)
                {
                    RecipeInfoItem nj = new RecipeInfoItem();
                    nj.id = ni.id;
                    nj.id_item = processInt(dp.getValue(i, "materials_m[" + j + "]_id"));
                    nj.count = processInt(dp.getValue(i, "materials_m[" + j + "]_cnt"));
                    if (nj.id_item !=0) nj.insertRecord();
                }
                Log.log2(i + 1, dp.getRecordsCount());
            }
            Log.log("Processing recipe-c.txt - Finished");
        }
    }
}
