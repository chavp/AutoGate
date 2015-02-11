using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWN.AutoGate.AMC.Lib.Controls.HisTrnActivity.ViewModels
{
    using HisTrnActivityWorkerAPI;
    using Microsoft.Practices.Prism.Mvvm;

    public class HisTrnActivityResponseModel : BindableBase
    {
        public HisTrnActivityResponseModel()
        {
            hisTrnActivityResponse = new HisTrnActivityResponse();
        }

        public HisTrnActivityResponseModel(HisTrnActivityResponse hisTrnActivityResponse )
        {
            this.hisTrnActivityResponse = hisTrnActivityResponse;
        }

        private HisTrnActivityResponse hisTrnActivityResponse;
        
        public string PassportNO
        {
            get { return this.hisTrnActivityResponse.PassportNO; }
        }

        public string TransactionID
        {
            get { return this.hisTrnActivityResponse.TransactionID; }
        }

        public DateTime OccurredDateTime
        {
            get { return this.hisTrnActivityResponse.OccurredDateTime; }
        }

        public string GateActivityCode
        {
            get { return this.hisTrnActivityResponse.GateActivityCode; }
        }

        public int RegisteredGateSeq
        {
            get { return this.hisTrnActivityResponse.RegisteredGateSeq; }
        }
    }
}
