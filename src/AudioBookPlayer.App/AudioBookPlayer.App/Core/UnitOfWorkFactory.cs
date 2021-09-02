using AudioBookPlayer.App.Domain.Data;
using AudioBookPlayer.App.Domain.Services;
using AudioBookPlayer.App.Persistence.SqLite;

namespace AudioBookPlayer.App.Core
{
    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        private readonly ApplicationDbContext context;
        private readonly ICoverService coverService;

        public UnitOfWorkFactory(ApplicationDbContext context, ICoverService coverService)
        {
            this.context = context;
            this.coverService = coverService;
        }

        public IUnitOfWork CreateUnitOfWork(bool useTransaction)
        {
            return new UnitOfWork(context, coverService, useTransaction);
        }
    }
}