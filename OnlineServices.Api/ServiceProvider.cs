﻿using LNF.Models;
using LNF.Models.Billing;
using LNF.Models.Data;
using LNF.Models.Mail;
using LNF.Models.PhysicalAccess;
using LNF.Models.Scheduler;
using LNF.Models.Worker;
using System.Configuration;

namespace OnlineServices.Api
{
    public class ServiceProvider : IProvider
    {
        public ServiceProvider(IDataService data, IBillingServices billing, IMailService mail, IPhysicalAccessService physicalAccess, ISchedulerService scheduler, IWorkerService worker)
        {
            Data = data;
            Billing = billing;
            Mail = mail;
            PhysicalAccess = physicalAccess;
            Scheduler = scheduler;
            Worker = worker;
        }

        public IDataService Data { get; }

        public IBillingServices Billing { get; }

        public IMailService Mail { get; }

        public IPhysicalAccessService PhysicalAccess { get; }

        public ISchedulerService Scheduler { get; }

        public IWorkerService Worker { get; }

        public bool IsProduction()
        {
            var setting = ConfigurationManager.AppSettings["IsProduction"];
            if (string.IsNullOrEmpty(setting)) return false;
            if (bool.TryParse(setting, out bool result)) return result;
            return false;
        }
    }
}