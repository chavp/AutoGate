using FluentNHibernate.Mapping;
using HisTrnActivityDtlWorker.Test.Domains;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HisTrnActivityDtlWorker.Test.Mappings
{
    public class RegisteredGateMap
        : ClassMap<RegisteredGate>
    {
        public RegisteredGateMap()
        {
            Table("MST_REGISTERED_GATE");

            // Primary Key
            Id(t => t.Seq, "SEQ");

        }
    }
}
