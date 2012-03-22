using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace L2TowerDatabaseGenerator.Tables
{
    class RecipeInfo : AbstractTable
    {
        [PrimaryKey]
        public long id;
        public long id_item;
        public long count;

        public long level;
        public long mp_cost;
        public long success_rate;
    }
}
