using System.Web.Http;
using Owin;
using Ninject.Web.Common;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using Ninject;
using Ninject.Web.WebApi;
using System.Web;
using HL.Loyalty.Providers.QueryAPI.ProgramProvider;
using HL.Loyalty.Providers.QueryAPI.Activities;
using HL.Loyalty.Providers.QueryAPI.Rewards;
using System;
using HL.Loyalty.Providers.QueryAPI.CustomerProvider;
using HL.Loyalty.Repository.QueryAPI.ActivityRepository;
using HL.Loyalty.Repository.QueryAPI.CustomerRepository;
using HL.Loyalty.Repository.QueryAPI.ProgramRepository;
using HL.Loyalty.Repository.QueryAPI.RewardRepository;
using System.Data.Common;
using System.Data.SqlClient;
using System.Fabric;
using System.Data;
using Loyalty.QueryAPI.Authentication;
using HL.Loyalty.Common;
using System.Runtime.InteropServices;
using HL.Loyalty.Common.QueryAPI;
using HL.Loyalty.Repository.QueryAPI.TransactionRepository;
using HL.Loyalty.Providers.QueryAPI.TransactionProvider;
using HL.Loyalty.Repository.QueryAPI.LoyaltyEventRepository;
using HL.Loyalty.Providers.QueryAPI.LoyaltyEventProvider;

namespace Loyalty.QueryAPI
{
    public static class Startup
    {
        // This code configures Web API. The Startup class is specified as a type
        // parameter in the WebApp.Start method.
        public static void ConfigureApp(IAppBuilder appBuilder)
        {
            // Configure Web API for self-host. 
            HttpConfiguration config = new HttpConfiguration();

            //config.Routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);

            config.Routes.MapHttpRoute(
               name: "DefaultApi",
               routeTemplate: "api/{controller}/{action}/{id}",
               defaults: new { id = RouteParameter.Optional, action = "Get" }
           );



            Bootstrapper Bootstrapper = new Bootstrapper();
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            Bootstrapper.Initialize(CreateKernel);
            config.DependencyResolver = new NinjectDependencyResolver(Bootstrapper.Kernel);
            //config.Filters.Add(new GenericTokenAuthenticationFilter());

            appBuilder.Use<HealthCheckMiddleware>();
            appBuilder.UseWebApi(config);

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
            var strConnection = GetSetting("LoyaltyDBConnectionString");
            var urlCatalogServices = GetSetting("UrlCatalogService");

            if (!string.IsNullOrWhiteSpace(strConnection) && !string.IsNullOrWhiteSpace(urlCatalogServices))
            {
                kernel.Bind<IConfigurationSettings>().To<ConfigurationSettings>()
                                                    .WithConstructorArgument("connectionString", strConnection)
                                                    .WithConstructorArgument("urlCatalogServices", urlCatalogServices);

                //Repositories
                kernel.Bind<IActivityRepository>()
               .To<ActivityRepository>()
               .InSingletonScope();

                kernel.Bind<ICustomerRepository>()
                .To<CustomerRepository>()
                .InSingletonScope();

                kernel.Bind<IProgramRepository>()
                .To<ProgramRepository>()
                .InSingletonScope();

                kernel.Bind<IRewardRepository>()
                .To<RewardRepository>()
                .InSingletonScope();

                kernel.Bind<ITransactionRepository>()
                .To<TransactionRepository>()
                .InSingletonScope();

                kernel.Bind<ILoyaltyEventRepository>()
                .To<LoyaltyEventRepository>()
                .InSingletonScope();


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

                kernel.Bind<ICustomerProvider>()
                .To<CustomerProvider>()
                .InSingletonScope();

                kernel.Bind<ITransactionProvider>()
               .To<TransactionProvider>()
               .InSingletonScope();

                kernel.Bind<ILoyaltyEventProvider>()
               .To<LoyaltyEventProvider>()
               .InSingletonScope();
            }


            GlobalConfiguration.Configuration.DependencyResolver = new NinjectDependencyResolver(kernel);
        }


          public static string GetSetting(string key)
        {
            try
            {       
                IntPtr valuePtr = IntPtr.Zero;
                var activationContext = FabricRuntime.GetActivationContext();
                var configurationPackage = activationContext.GetConfigurationPackageObject("Config");
                var environmentConfig = configurationPackage.Settings.Sections["EnvironmentConfig"];
                if (Convert.ToBoolean(environmentConfig.Parameters["UseEncryption"].Value)  && environmentConfig.Parameters[key].IsEncrypted)
                {
                    var configValue = environmentConfig.Parameters[key].DecryptValue();
                    valuePtr = Marshal.SecureStringToGlobalAllocUnicode(configValue);
                    return Marshal.PtrToStringUni(valuePtr);
                }
                else                    
                {
                    if (environmentConfig.Parameters.Contains(key))
                        return environmentConfig.Parameters[key].Value;
                    else
                        return null;
                }

            }
            catch(Exception ex)
            {
                return null;
            }
        }
    }
}
