using FluentNHibernate.Mapping;
using HisTrnActivityWorker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HisTrnActivityWorker.Mappings
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
