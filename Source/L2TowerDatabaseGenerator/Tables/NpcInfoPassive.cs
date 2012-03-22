using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace L2TowerDatabaseGenerator.Tables
{
    class NpcInfoPassive : AbstractTable
    {
        [PrimaryKey]
        public long id;
        [PrimaryKey]
        public long skillId;
        public long skillLvl;
    }
}
