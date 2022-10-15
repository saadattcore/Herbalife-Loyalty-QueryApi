using Ninject;
using Ninject.Web.Common;
using System;
using System.Web;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using System.Web.Http;
using Ninject.Web.WebApi;
using HL.Loyalty.Providers.QueryAPI.ProgramProvider;
using HL.Loyalty.Providers.QueryAPI.Activities;
using HL.Loyalty.Providers.QueryAPI.Rewards;
using HL.Loyalty.Repository.QueryAPI.ActivityRepository;
using HL.Loyalty.Repository.QueryAPI.CustomerRepository;
using HL.Loyalty.Repository.QueryAPI.ProgramRepository;
using HL.Loyalty.Repository.QueryAPI.RewardRepository;
using HL.Loyalty.Providers.QueryAPI.TransactionProvider;
using HL.Loyalty.Providers.QueryAPI.LoyaltyEventProvider;


//[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(NinjectConfiguration), "Start")]
//[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(NinjectConfiguration), "Stop")]


namespace HL.Loyalty.QueryAPI
{
  

    public static class NinjectConfiguration
    {

        private static readonly Bootstrapper Bootstrapper = new Bootstrapper();

        public static void Start()
        {
           
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            Bootstrapper.Initialize(CreateKernel);
        }

        public static void Stop()
        {
            Bootstrapper.ShutDown();
        }

        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

                RegisterServices(kernel);
                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        internal static void RegisterServices(IKernel kernel)
        {

           

            //Providers

            kernel.Bind<IProgramProvider>()
           .To<ProgramProvider>()
           .InSingletonScope();

            kernel.Bind<IActivityProvider>()
           .To<ActivityProvider>()
           .InSingletonScope();

            kernel.Bind<IRewardsProvider>()
            .To<RewardsProvider>()
            .InSingletonScope();

            kernel.Bind<ITransactionProvider>()
            .To<TransactionProvider>()
            .InSingletonScope();

            kernel.Bind<ILoyaltyEventProvider>()
           .To<LoyaltyEventProvider>()
           .InSingletonScope();




            GlobalConfiguration.Configuration.DependencyResolver = new NinjectDependencyResolver(kernel);
        }
    }
}