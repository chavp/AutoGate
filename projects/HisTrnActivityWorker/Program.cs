using CWN.AutoGate.EGateMessage;
using CWN.Lib.RMQManager;
using EasyNetQ;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using HisTrnActivityWorker.Mappings;
using HisTrnActivityWorker.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HisTrnActivityWorker
{
    using NH = NHibernate;
    using NHibernate.Linq;
    using NHibernate.Tool.hbm2ddl;
    using HisTrnActivityWorker.Models;
    using log4net;
    using HisTrnActivityWorkerAPI;
    using Castle.Windsor.Installer;
    using Castle.Windsor;
    using Topshelf;
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;

    class Program
    {
        static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();

            var container = new WindsorContainer().Install(FromAssembly.This());

            HostFactory.Run(x =>
            {
                x.Service<IHisTrnActivityWorkerService>(s =>
                {
                    s.ConstructUsing(name => container.Resolve<IHisTrnActivityWorkerService>());
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc =>
                    {
                        tc.Stop();
                        container.Release(tc);
                        container.Dispose();
                    });
                });

                x.RunAsLocalSystem();

                x.SetDescription("HisTrnActivityWorker Services.");
                x.SetDisplayName("HisTrnActivityWorker.Services");
                x.SetServiceName("HisTrnActivityWorker.Services");
            });
        }
    }

    public class BusBuilder
    {
        public static IBus CreateMessageBus()
        {
            string rabbitMQHost = Settings.Default.RabbitMQHost;
            string username = Settings.Default.RabbitMQUsername;
            string password = Settings.Default.RabbitMQPassword;
            string virtualHost = Settings.Default.RabbitMQVirtualHost;
            string connectionString =
                string.Format(
                "host={0};virtualHost={1};username={2};password={3}",
                rabbitMQHost, virtualHost, username, password);

            return RabbitHutch.CreateBus(connectionString);
        }
    }

    public class NHSessionBuilder
    {
        public static NH.ISessionFactory CreateMSSQLSessionFactory()
        {
            var config = new NH.Cfg.Configuration();
            return Fluently.Configure(config)
                .Database(MsSqlConfiguration.MsSql2012
                .ConnectionString(c => c
                .Server(Settings.Default.DBServer)
                .Username(Settings.Default.DBUsername)
                .Password(Settings.Default.DBPassword)
                .Database(Settings.Default.DBName)))
                .Mappings(m => m.FluentMappings.AddFromAssemblyOf<GateActivityMap>())
                .ExposeConfiguration(TreatConfiguration)
                .BuildSessionFactory();
        }

        static void TreatConfiguration(NH.Cfg.Configuration configuration)
        {
            var update = new SchemaUpdate(configuration);
            update.Execute(false, true);
        }
    }

    public class Installer : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<IHisTrnActivityWorkerService>().ImplementedBy<HisTrnActivityWorkerService>().LifestyleTransient(),
                Component.For<IBus>().UsingFactoryMethod(BusBuilder.CreateMessageBus).LifestyleSingleton(),
                Component.For<NH.ISessionFactory>().UsingFactoryMethod(NHSessionBuilder.CreateMSSQLSessionFactory).LifestyleSingleton()
                );
        }
    }

    public interface IHisTrnActivityWorkerService
    {
        void Start();
        void Stop();
    }

    public class HisTrnActivityWorkerService : IHisTrnActivityWorkerService
    {
        static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IBus bus;
        private readonly NH.ISessionFactory sessionFactory;

        public HisTrnActivityWorkerService(IBus bus, NH.ISessionFactory sessionFactory)
        {
            this.bus = bus;
            this.sessionFactory = sessionFactory;
        }

        public void Start()
        {
            Logger.Info("HisTrnActivityWorker - Start");

            this.bus.Subscribe<RMQMessage>("HisTrnActivityWorker",
                        msg =>
                        {
                            try
                            {
                                var rmQMessage = msg;
                                //Console.WriteLine(msg.PassengerInformation.PassportNo);
                                using (var session = this.sessionFactory.OpenSession())
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

                                        Logger.Info("Save: new GateActivity " + code);

                                    }

                                    int seq = int.Parse(rmQMessage.GateInformation.GateID);
                                    var regGate = (from g in session.Query<RegisteredGate>()
                                                   where g.Seq == int.Parse(rmQMessage.GateInformation.GateID)
                                                   select g).FirstOrDefault();

                                    if (regGate == null)
                                    {
                                        Logger.Info("Error not found RegisteredGate Seq = " + seq);
                                    }
                                    else if (rmQMessage.PassengerInformation == null)
                                    {
                                        Logger.Info("Error PassengerInformation is Null");
                                    }
                                    else if (string.IsNullOrEmpty(rmQMessage.PassengerInformation.PassengerTranID))
                                    {
                                        Logger.Info("Error PassengerInformation.PassengerTranID is Null Or Empty");
                                    }
                                    else
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

                                        Logger.Info("Save: new HisTrnActivity completed ");
                                    }

                                    tran.Commit();
                                }
                            }
                            catch (Exception ex)
                            {
                                Logger.Error(ex);
                            }
                        });

            // bind HisTrnActivity services
            try
            {
                this.bus.Respond<HisTrnActivityRequest, List<HisTrnActivityResponse>>(request =>
                {
                    string query = string.Format(
                        "HisTrnActivityRequest Query - PassportNO: {0}, From - To OccurredDateTime: {1} - {2}, Start: {3}, Limit: {4}",
                        request.QueryPassportNO,
                        request.QueryFromOccurredDateTime,
                        request.QueryToOccurredDateTime,
                        request.Start, request.Limit);

                    Logger.Info(query);
                    var results = new List<HisTrnActivityResponse>();
                    using (var session = this.sessionFactory.OpenSession())
                    {
                        var qHisTrnActivity = (from h in session.Query<HisTrnActivity>()
                                               where request.QueryFromOccurredDateTime.Date <= h.OccurredDateTime.Date
                                               && h.OccurredDateTime.Date <= request.QueryToOccurredDateTime.Date
                                               select h);

                        if (!string.IsNullOrEmpty(request.QueryPassportNO))
                        {
                            qHisTrnActivity = qHisTrnActivity.Where(h => h.PassportNO.Contains(request.QueryPassportNO));
                        }

                        qHisTrnActivity = qHisTrnActivity.Skip(request.Start).Take(request.Limit);

                        foreach (var item in qHisTrnActivity)
                        {
                            results.Add(new HisTrnActivityResponse
                            {
                                PassportNO = item.PassportNO,
                                TransactionID = item.TransactionID,
                                GateActivityCode = item.GateActivity.Code,
                                RegisteredGateSeq = item.RegisteredGate.Seq,
                                OccurredDateTime = item.OccurredDateTime,
                            });
                        }

                    }
                    return results;
                });
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        public void Stop()
        {
            // shutdown code
            if (this.bus != null)
                this.bus.Dispose();

            if (this.sessionFactory != null)
                this.sessionFactory.Dispose();

            Logger.Info("HisTrnActivityWorker - Stop");
        }
    }
}
