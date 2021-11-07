using Microsoft.EntityFrameworkCore;

namespace Transaction.Domain.Transactions
{
    public partial class TransactionDbContext : DbContext
    {
        public TransactionDbContext(DbContextOptions<TransactionDbContext> options)
             : base(options)
        {
        }

        public virtual DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            Transaction.Configure(modelBuilder);
        }
    }
}
