using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Transaction.Api.UnitTest.Helpers;
using Transaction.Domain.Transactions;
using Transaction.Domain.UnitOfWorks;
using Transaction.Infrastructure.UnitOfWorks;

namespace Transaction.Domain.Test
{
    public class ServiceFactory
    {
        private static bool setupData = false;
        private readonly string _databaseName;
        private IServiceScope _serviceScope;
        public IServiceCollection ServiceCollection;

        public ServiceFactory(bool setupData = true, string databaseName = "TransactionDB")
        {
            this.ServiceCollection = new ServiceCollection();
            this.ConfigureServices();
            this._databaseName = databaseName;
            if (setupData)
            {
                this.SetupData();
            }
        }


        public static IEnumerable<Transactions.Transaction> InitialData
            => EmbeddedJsonResourceReaderHelper.Get<IEnumerable<Transactions.Transaction>>
            ($"{typeof(ServiceFactory).Namespace}.InitialData.json");

        private IServiceScope ServiceScope
        {
            get
            {
                if (this._serviceScope is null)
                {
                    this._serviceScope = this.ServiceCollection.BuildServiceProvider().CreateScope();
                }
                return this._serviceScope;
            }
        }

        public void ConfigureServices()
        {
            this.ServiceCollection.AddLogging();

            // Add Db context.
            this.ServiceCollection.AddDbContext<TransactionDbContext>(
            options =>
                options.UseInMemoryDatabase(this._databaseName)
                    .UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll));

            this.ServiceCollection.AddScoped<IUnitOfWork, UnitOfWork>();
            this.ServiceCollection.AddScoped<ITransactionService, TransactionService>();
            this.ServiceCollection.AddScoped<ITransactionImportService, TransactionImportService>();
        }

        public T GetRequiredService<T>()
        {
            return this.ServiceScope.ServiceProvider.GetRequiredService<T>();
        }

        private void SetupData()
        {
            if (!setupData)
            {
                IUnitOfWork unitOfWork = this.GetRequiredService<IUnitOfWork>();
                foreach (Transactions.Transaction transaction in InitialData)
                {
                    unitOfWork.TransactionRepository.Insert(transaction);
                }
                _ = unitOfWork.SaveAsync().Result;

                setupData = true;
            }
        }
    }
}
