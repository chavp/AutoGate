using FluentNHibernate.Mapping;
using HisTrnActivityWorker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HisTrnActivityWorker.Mappings
{
    public class HisTrnActivityMap
        : ClassMap<HisTrnActivity>
    {
        public HisTrnActivityMap()
        {
            Table("HIS_TRN_ACTIVITY");

            // Primary Key
            Id(t => t.ID).GeneratedBy.Identity();

            Map(t => t.TransactionID, "TRANSACTION_ID")
               .Length(50)
               .Not.Nullable();

            References(x => x.RegisteredGate, "SEQ_REG_GATE").Not.Nullable();
            References(x => x.GateActivity, "ACT_ID").Not.Nullable();

            Map(t => t.OccurredDateTime, "OccurredDateTime").Not.Nullable();

            Map(t => t.PassportNO, "PASSPORT_NO").Length(30);
            Map(t => t.FirstName, "FIRST_NAME").Length(150);
            Map(t => t.LastName, "LAST_NAME").Length(150);
            Map(t => t.Nationality, "NATIONALITY").Length(100);
            Map(t => t.Gender, "GENDER").Length(10);
            Map(t => t.DateOfBirth, "DATE_OF_BIRTH").Length(50);
            Map(t => t.Age, "AGE");
            Map(t => t.TM6, "TM6").Length(200);
            Map(t => t.FlightNo, "FLIGHT_NO").Length(50);
            Map(t => t.CreatedDate, "CREATED_DATE").Not.Nullable();
            Map(t => t.CreatedBy, "CREATED_BY").Not.Nullable();
        }
    }
}
