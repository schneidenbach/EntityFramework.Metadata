using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using EntityFramework.Metadata.Test.CodeFirst.Domain;
using EntityFramework.Metadata.Test.CodeFirst.Domain.ComplexTypes;

namespace EntityFramework.Metadata.Test.CodeFirst
{
    public class TestContext : DbContext
    {
        public TestContext() : base("TestContext")
        {
            
        }

        public TestContext(string connectionStringName) : base(connectionStringName)
        {
            
        }

        private const string ContractDiscriminator = "__typeid";


        public DbSet<TestUser> Users { get; set; }
 
        public DbSet<Page> Pages { get; set; } 

        public DbSet<PageTranslations> PageTranslations { get; set; }

        public DbSet<WorkerTPT> WorkerTpts { get; set; }
        public DbSet<ManagerTPT> ManagerTpts { get; set; }
        
        public DbSet<EmployeeTPH> EmployeeTphs { get; set; } 
        public DbSet<ManagerTPH> ManagerTphs { get; set; }
        public DbSet<AWorkerTPH> AWorkerTphs { get; set; }
        
        public DbSet<ContractBase> ContractBases { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<ContractFixed> FixedContracts { get; set; }
        public DbSet<ContractStock> StockContracts { get; set; }
        public DbSet<ContractKomb1> K1Contracts { get; set; }
        public DbSet<ContractKomb2> K2Contracts { get; set; }

        public DbSet<Foo> Foos { get; set; }
        public DbSet<EntityWithMappedPk> EntityWithMappedPks { get; set; }

        public DbSet<House> Houses { get; set; }

        public DbSet<Company> Companies { get; set; }

        protected override void OnModelCreating(DbModelBuilder mb)
        {
            mb.ComplexType<Contact>();
            mb.ComplexType<Address>();

            mb.Entity<Foo>().ToTable("FOO", "dbx");

            mb.Entity<TestUser>().ToTable("Users");
            mb.Entity<TestUser>().Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            mb.Entity<TestUser>().Property(x => x.FirstName).HasColumnName("Name");
            mb.Entity<TestUser>().Ignore(x => x.FullName);

            mb.Entity<Page>().HasKey(x => x.PageId);
            mb.Entity<Page>().Property(x => x.Title).HasMaxLength(255).IsRequired();
            mb.Entity<Page>().HasOptional(x => x.Parent).WithMany().HasForeignKey(x => x.ParentId);
            mb.Entity<Page>().HasRequired(x => x.CreatedBy).WithMany().HasForeignKey(x => x.CreatedById);
            mb.Entity<Page>().HasOptional(x => x.ModifiedBy).WithMany().HasForeignKey(x => x.ModifiedById);

            mb.Entity<PageTranslations>().HasKey(x => new {x.PageId, x.Language});
            mb.Entity<PageTranslations>().Property(x => x.Title).HasMaxLength(255);

            mb.Entity<EmployeeTPH>().Property(x => x.Title).HasColumnName("JobTitle");
            mb.Entity<EmployeeTPH>().Ignore(x => x.NameWithTitle);
            mb.Entity<EmployeeTPH>()
                .Map(x => x.ToTable("Employees"))
                .Map<AWorkerTPH>(m => m.Requires("__employeeType").HasValue(1))
                .Map<ManagerTPH>(m => m.Requires("__employeeType").HasValue(2));

            mb.Entity<AWorkerTPH>().HasRequired(x => x.Boss).WithMany(x => x.Henchmen).HasForeignKey(x => x.BossId);
            
            mb.Entity<WorkerTPT>().HasRequired(x => x.Boss).WithMany(x => x.Henchmen);
            mb.Entity<WorkerTPT>().HasOptional(x => x.Referee).WithMany().HasForeignKey(x => x.RefereeId);


            mb.Entity<ContractBase>().HasRequired(x => x.MeteringPoint).WithMany().HasForeignKey(x => x.MeteringPointId).WillCascadeOnDelete(false);
            mb.Entity<ContractBase>().Property(x => x.PackageName).HasMaxLength(50);
            mb.Entity<ContractBase>().Property(x => x.ContractNr).HasMaxLength(50);
            mb.Entity<ContractBase>().Property(x => x.AvpContractNr).HasMaxLength(50);

            mb.Entity<ContractBase>()
                .Map(x => x.ToTable("Contracts"))
                .Map<Contract>(x => x.Requires(ContractDiscriminator).HasValue(0))
                .Map<ContractFixed>(x => x.Requires(ContractDiscriminator).HasValue(1))
                .Map<ContractStock>(x => x.Requires(ContractDiscriminator).HasValue(2))
                .Map<ContractKomb1>(x => x.Requires(ContractDiscriminator).HasValue(3))
                .Map<ContractKomb2>(x => x.Requires(ContractDiscriminator).HasValue(4));

            mb.Entity<ContractStock>().Property(x => x.Margin).HasPrecision(18, 6);

            mb.Entity<ContractKomb1>().Property(x => x.Base).HasPrecision(18, 4);
            mb.Entity<ContractKomb1>().Property(x => x.StockMargin).HasPrecision(18, 6).HasColumnName("Margin1");
            mb.Entity<ContractKomb1>().Property(x => x.FixPricesJson).HasColumnName("PricesJson1");

            mb.Entity<ContractKomb2>().Property(x => x.Part1Margin).HasPrecision(18, 6);
            mb.Entity<ContractKomb2>().Property(x => x.Part2Margin).HasPrecision(18, 6);


            mb.Entity<EntityWithMappedPk>().HasKey(k => k.BancoId);

            mb.Entity<EntityWithMappedPk>().Property(p => p.BancoId).HasColumnName("BancoID").IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            mb.Entity<EntityWithMappedPk>().Property(p => p.Nombre).HasColumnName("Nombre").IsRequired();

            mb.Entity<EntityWithMappedPk>().ToTable("EntityWithMapping");

            base.OnModelCreating(mb);
        }
    }
}