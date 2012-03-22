using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace L2TowerDatabaseGenerator.Tables
{
    class RecipeInfoItem : AbstractTable
    {
        [PrimaryKey]
        public long id;
        [PrimaryKey]
        public long id_item;
        public long count;
    }
}
