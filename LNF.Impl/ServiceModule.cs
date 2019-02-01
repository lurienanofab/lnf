﻿using LNF.Impl.DependencyInjection.Web;
using LNF.Repository;
using System;
using System.Web;

namespace LNF.Impl
{
    public class ServiceModule : IHttpModule
    {
        private IDisposable _uow;

        public void Init(HttpApplication context)
        {
            ServiceProvider.Current = IOC.Resolver.GetInstance<ServiceProvider>();

            context.BeginRequest += Context_BeginRequest;
            context.EndRequest += Context_EndRequest;
        }

        private void Context_BeginRequest(object sender, System.EventArgs e)
        {
            _uow = DA.StartUnitOfWork();
        }

        private void Context_EndRequest(object sender, System.EventArgs e)
        {
            _uow.Dispose();
        }

        public void Dispose()
        {
            // nothing to do here...
        }
    }
}
