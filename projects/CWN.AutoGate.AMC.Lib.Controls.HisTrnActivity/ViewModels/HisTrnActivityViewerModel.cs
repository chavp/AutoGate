using EasyNetQ;
using HisTrnActivityWorkerAPI;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace CWN.AutoGate.AMC.Lib.Controls.HisTrnActivity.ViewModels
{
    public class HisTrnActivityViewerModel : BindableBase
    {
        public HisTrnActivityViewerModel()
        {
            this.SearchCommand = new DelegateCommand(this.OnSearch);
            this.QuestionnaireViewModels = new List<HisTrnActivityResponseModel>();

            this.FromOccuredDate = DateTime.Now.AddMonths(-1);
            this.ToOccuredDate = DateTime.Now;

            //this.QuestionnaireViewModels.Add(new HisTrnActivityResponseModel());
        }

        public ICommand SearchCommand { get; private set; }

        public List<HisTrnActivityResponseModel> QuestionnaireViewModels { get; set; }

        private void OnSearch()
        {
            //MessageBox.Show(this.FromOccuredDate.ToString() + ", " + this.ToOccuredDate.ToString());
            string rabbitMQHost = "172.16.24.194";
            string username = "AOTAPPUAT";
            string password = "mflv[1234";
            string virtualHost = "/";
            string connectionString =
                string.Format(
                "host={0};virtualHost={1};username={2};password={3}",
                rabbitMQHost, virtualHost, username, password);

            using (var bus = RabbitHutch.CreateBus(connectionString))
            {
                var myRequest = new HisTrnActivityRequest()
                {
                    QueryFromOccurredDateTime = this.FromOccuredDate,
                    QueryToOccurredDateTime = this.ToOccuredDate,
                };
                var response = bus.Request<HisTrnActivityRequest, List<HisTrnActivityResponse>>(myRequest);

                QuestionnaireViewModels.Clear();
                var update = new List<HisTrnActivityResponseModel>();
                foreach (var item in response)
                {
                    update.Add(new HisTrnActivityResponseModel(item));
                }

                QuestionnaireViewModels = update.OrderByDescending(d => d.OccurredDateTime).ToList();
                
                OnPropertyChanged("QuestionnaireViewModels");
            }
        }

        public DateTime FromOccuredDate { get; set; }
        public DateTime ToOccuredDate { get; set; }
    }
}
