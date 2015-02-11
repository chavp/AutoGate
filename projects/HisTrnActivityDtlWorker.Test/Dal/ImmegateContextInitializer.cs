using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HisTrnActivityDtlWorker.Test.Dal
{
    public class ImmegateContextInitializer
        : CreateDatabaseIfNotExists<ImmegateContext>
    {
        protected override void Seed(ImmegateContext context)
        {
            base.Seed(context);
        }
    }
}
