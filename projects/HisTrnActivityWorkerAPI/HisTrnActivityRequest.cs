using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HisTrnActivityWorkerAPI
{
    public class HisTrnActivityRequest
    {
        public HisTrnActivityRequest()
        {
            Start = 0;
            Limit = 500;
            QueryFromOccurredDateTime = DateTime.Now.AddMonths(-1);
            QueryToOccurredDateTime = DateTime.Now;
        }

        public int Start { get; set; }
        public int Limit { get; set; }

        public string QueryPassportNO { get; set; }
        public DateTime QueryFromOccurredDateTime { get; set; }
        public DateTime QueryToOccurredDateTime { get; set; }
    }
}
