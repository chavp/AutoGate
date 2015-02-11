using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HisTrnActivityDtlWorker.Test.Domains
{
    public class HisTrnActivity
    {
        protected HisTrnActivity() 
        {
            CreatedDate = DateTime.Now;
            CreatedBy = Environment.MachineName;
        }
        public HisTrnActivity(
            string transactionID, 
            DateTime occurredDateTime,
            RegisteredGate registeredGate, 
            GateActivity gateActivity)
            : this()
        {
            TransactionID = transactionID;
            OccurredDateTime = occurredDateTime;
            RegisteredGate = registeredGate;
            GateActivity = gateActivity;
        }

        public virtual long ID { get; protected set; }

        public virtual string TransactionID { get; protected set; }

        public virtual RegisteredGate RegisteredGate { get; protected set; }
        public virtual GateActivity GateActivity { get; protected set; }

        public virtual DateTime OccurredDateTime { get; set; }

        public virtual string PassportNO { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string Nationality { get; set; }
        public virtual string Gender { get; set; }
        public virtual string DateOfBirth { get; set; }
        public virtual int Age { get; set; }
        public virtual string TM6 { get; set; }
        public virtual string FlightNo { get; set; }

        public virtual DateTime CreatedDate { get; set; }
        public virtual string CreatedBy { get; set; }
        public virtual string Dessciption { get; set; }

    }
}
