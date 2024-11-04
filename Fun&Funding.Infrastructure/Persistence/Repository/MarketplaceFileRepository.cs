using Fun_Funding.Application.Interfaces.IRepository;
using Fun_Funding.Domain.Entity;
using Fun_Funding.Infrastructure.Persistence.Database;

namespace Fun_Funding.Infrastructure.Persistence.Repository
{
    public class MarketplaceFileRepository : BaseRepository<MarketplaceFile>, IMarketplaceFileRepository
    {
        private readonly MyDbContext _context;
        public MarketplaceFileRepository(MyDbContext context) : base(context)
        {
            _context = context;
        }

        //public async Task HardDeleteMarketplaceFilesAsync(IEnumerable<MarketplaceFile> files)
        //{
        //    if (files == null || files.Count() == 0)
        //        return;

        //    // Extract the IDs of the files to be deleted
        //    var ids = files.Select(f => f.Id).ToList();

        //    // Create the raw SQL command
        //    var sqlCommand = $"DELETE FROM MarketplaceFiles WHERE Id IN ({string.Join(",", ids.Select(id => $"'{id}'"))})";

        //    // Execute the raw SQL command asynchronously
        //    await _context.Database.ExecuteSqlRawAsync(sqlCommand);
        //}
    }
}
