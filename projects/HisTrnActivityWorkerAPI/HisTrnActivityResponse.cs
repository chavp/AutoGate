using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HisTrnActivityWorkerAPI
{
    public class HisTrnActivityResponse
    {
        public string TransactionID { get; set; }
        public string PassportNO { get; set; }
        public virtual int RegisteredGateSeq { get; set; }
        public virtual string GateActivityCode { get; set; }
        public DateTime OccurredDateTime { get; set; }
    }
}
