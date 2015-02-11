using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using CWN.AutoGate.EGateMessage;
using HisTrnActivityDtlWorker.Test.Domains;
using System.Data.Entity.Infrastructure;
using NHibernate;
using NHibernate.Linq;
using NHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using HisTrnActivityDtlWorker.Test.Mappings;
using FluentNHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using EasyNetQ;
using HisTrnActivityWorkerAPI;
using System.Collections.Generic;

namespace HisTrnActivityDtlWorker.Test
{
    [TestClass]
    public class ConnectionTest
    {
        ISessionFactory _sessionFactory = null;

        [TestInitialize]
        public void Setup()
        {
            _sessionFactory = CreateMSSQLSessionFactory();
        }

        [TestMethod]
        public void TestConnection()
        {
            using (var session = _sessionFactory.OpenSession())
            {
                var his = (from a in session.Query<HisTrnActivity>()
                          select a).ToList();


            }
        }

        [TestMethod]
        public void TestAddHisTrnActivity()
        {
            var rmQMessage = new RMQMessage()
                {
                    TimeStamp = DateTime.Now,
                };
            rmQMessage.GateActivityState = ENUM_GATE_ACTIVITY_STATE.BACK_GATE_OPEN;
            rmQMessage.GateInformation = new GateInfo
            {
                GateID = "1"
            };

            rmQMessage.PassengerInformation = new PassengerInfo
            {
                PassengerTranID = Guid.NewGuid().ToString(),
                Age = 99,
                DateOfBirth = "31/12/9999",
                Description = "Test-Description",
                PassportNo = "Test-PassportNo",
                FirstName = "Test-FirstName",
                LastName = "Test-LastName",
                Nationality = "Test-Nationality",
                Gender = "M",
                TM6 = "Test-TM6",
                FlightNo = "Test-FlightNo"
            };

            using (var session = _sessionFactory.OpenSession())
            using (var tran = session.BeginTransaction())
            {
                var code = rmQMessage.GateActivityState.ToString();
                var gateActivity = (from a in session.Query<GateActivity>()
                                    where a.Code == rmQMessage.GateActivityState.ToString()
                                    select a).FirstOrDefault();
                
                if (gateActivity == null)
                {
                    gateActivity = new GateActivity(code, code);

                    session.Save(gateActivity);
                    
                }

                var regGate = (from g in session.Query<RegisteredGate>()
                               where g.Seq == int.Parse(rmQMessage.GateInformation.GateID)
                               select g).First();

                for (int i = 0; i < 10; i++)
                {
                    var newHisTrnAct = new HisTrnActivity
                    (rmQMessage.PassengerInformation.PassengerTranID, rmQMessage.TimeStamp, regGate, gateActivity)
                    {
                        PassportNO = rmQMessage.PassengerInformation.PassportNo,
                        FirstName = rmQMessage.PassengerInformation.FirstName,
                        LastName = rmQMessage.PassengerInformation.LastName,
                        Nationality = rmQMessage.PassengerInformation.Nationality,
                        Gender = rmQMessage.PassengerInformation.Gender,
                        DateOfBirth = rmQMessage.PassengerInformation.DateOfBirth,
                        Age = rmQMessage.PassengerInformation.Age,
                        TM6 = rmQMessage.PassengerInformation.TM6,
                        FlightNo = rmQMessage.PassengerInformation.FlightNo,
                    };

                    session.Save(newHisTrnAct);
                }
                
                tran.Commit();
            }

        }

        [TestMethod]
        public void TestPublishRMQMessage()
        {
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
                var rmQMessage = new RMQMessage()
                {
                    TimeStamp = DateTime.Now,
                };
                rmQMessage.GateActivityState = ENUM_GATE_ACTIVITY_STATE.FINGER_PRINT_SCAN_SUCCESS;
                rmQMessage.GateInformation = new GateInfo
                {
                    GateID = "2"
                };

                rmQMessage.PassengerInformation = new PassengerInfo
                {
                    PassengerTranID = Guid.NewGuid().ToString(),
                    Age = 99,
                    DateOfBirth = "31/12/9999",
                    Description = "Test-Description",
                    PassportNo = "Test-PassportNo",
                    FirstName = "Test-FirstName",
                    LastName = "Test-LastName",
                    Nationality = "Test-Nationality",
                    Gender = "M",
                    TM6 = "Test-TM6",
                    FlightNo = "Test-FlightNo"
                };

                bus.Publish<RMQMessage>(rmQMessage);
            }
        }

        [TestMethod]
        public void TestRequestHisTrnActivityWorker()
        {
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
                    QueryFromOccurredDateTime = DateTime.Now.AddMonths(-1),
                    QueryToOccurredDateTime = DateTime.Now,
                };
                var response = bus.Request<HisTrnActivityRequest, List<HisTrnActivityResponse>>(myRequest);
                Console.WriteLine(response.Count());
            }
        }

        private ISessionFactory CreateMSSQLSessionFactory()
        {
            var config = new Configuration();
            //config.BeforeBindMapping += (sender, e) =>
            //{
            //    // change to foreach if more than one classmapping per file
            //    var c = e.Mapping.RootClasses[0];
            //    c.Items =
            //        // sort everything with a column (simple property, reference, ...)
            //        c.Items.OfType<IColumnsMapping>().OrderBy(cm => cm.Columns.First().name)
            //        // concat everything that has no column (collection, formula, ...)
            //        .Concat(c.Items.Where(o => !(o is IColumnsMapping))).ToArray();
            //};

            return Fluently.Configure(config)
                .Database(MsSqlConfiguration.MsSql2012
                .ConnectionString(c => c
                .Server("172.16.24.195")
                .Username("sa")
                .Password("P@ssw0rd")
                .Database("IMMEGATE")))
                .Mappings(m => m.FluentMappings.AddFromAssemblyOf<GateActivityMap>())
                .ExposeConfiguration(TreatConfiguration)
                .BuildSessionFactory();
        }
        //private void BuildMSSQLSchema(Configuration config)
        //{
        //    new SchemaExport(config).Create(false, true);
        //}
        protected virtual void TreatConfiguration(Configuration configuration)
        {
            var update = new SchemaUpdate(configuration);
            update.Execute(false, true);
        }
    }

    //public static class RMQMessageExtension
    //{
    //    public static HisTrnActivity ToHisTrnActivity(this RMQMessage rmq)
    //    {
    //        return new HisTrnActivity(
    //            rmq.PassengerInformation.PassengerTranID, )
    //        {

    //        };
    //    }
    //}
}
