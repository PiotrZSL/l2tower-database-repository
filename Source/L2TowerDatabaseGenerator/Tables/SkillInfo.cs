using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace L2TowerDatabaseGenerator.Tables
{
    class SkillInfo : AbstractTable
    {
        [PrimaryKey]
        public long id;
        [PrimaryKey]
        public long level;
        public string name = "";
        public long mp_consume;
        public long hp_consume;
        public long cast_range;
        public long effect_range;

        public bool is_magic;
        public bool is_physic;
        public bool is_dance_song;

        public bool is_passive;
        public bool is_active;
        public bool is_toogle;

        public string icon = "";

        public long targetType = 0;
    }
}
