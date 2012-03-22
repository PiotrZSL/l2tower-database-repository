using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace L2TowerDatabaseGenerator.Tables
{
    class NpcInfo : AbstractTable
    {
        [PrimaryKey]
        public long id;
        public string name = "";
        public string title = "";
        public long level;
      //  public long agro_range;
    }
}
