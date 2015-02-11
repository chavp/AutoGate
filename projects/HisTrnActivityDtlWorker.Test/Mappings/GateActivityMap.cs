using FluentNHibernate.Mapping;
using HisTrnActivityDtlWorker.Test.Domains;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HisTrnActivityDtlWorker.Test.Mappings
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
                .Length(20)
                .Not.Nullable();
            Map(t => t.Name, "ACT_NAME")
               .Length(50)
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
