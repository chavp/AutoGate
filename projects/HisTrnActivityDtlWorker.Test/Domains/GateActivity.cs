using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HisTrnActivityDtlWorker.Test.Domains
{
    public class GateActivity
    {
        protected GateActivity() 
        {
            CreatedDate = DateTime.Now;
            CreatedBy = Environment.MachineName;
        }
        public GateActivity(string code, string name) 
            : this()
        {
            Code = code;
            Name = name;
        }

        public virtual int ID { get; protected set; }
        public virtual string Code { get; protected set; }
        public virtual string Name { get; protected set; }
        public virtual string Detail { get; set; }
        public virtual DateTime CreatedDate { get; set; }
        public virtual string CreatedBy { get; set; }
        public virtual DateTime? UpdateDate { get; set; }
        public virtual string UpdateBy { get; set; }
    }
}
