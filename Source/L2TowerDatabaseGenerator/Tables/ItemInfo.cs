using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace L2TowerDatabaseGenerator.Tables
{
    class ItemInfo : AbstractTable
    {
        [PrimaryKey]
        public long id;
        public string name;
    }
}
