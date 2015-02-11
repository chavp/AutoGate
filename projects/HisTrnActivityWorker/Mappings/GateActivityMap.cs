using FluentNHibernate.Mapping;
using HisTrnActivityWorker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HisTrnActivityWorker.Mappings
{
    public class GateActivityMap
        : ClassMap<GateActivity>
    {
        public GateActivityMap()
        {
            Table("MST_ACTIVITY");

            // Primary Key
            Id(t => t.ID, "ACT_ID").GeneratedBy.Identity();

            Map(t => t.Code, "ACT_CODE")
                .Length(100)
                .Not.Nullable();
            Map(t => t.Name, "ACT_NAME")
               .Length(100)
               .Not.Nullable();
            Map(t => t.Detail, "ACT_DETAIL")
               .Length(150);
            Map(t => t.CreatedDate, "INS_DATETIME");
            Map(t => t.CreatedBy, "INS_BY");
            Map(t => t.UpdateDate, "UPD_DATETIME");
            Map(t => t.UpdateBy, "UPD_BY");
        }
    }
}
