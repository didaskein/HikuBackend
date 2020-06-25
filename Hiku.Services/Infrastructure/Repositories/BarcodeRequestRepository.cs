using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Hiku.Services.Models;
using Hiku.Services.Infrastructure.Repositories.Sql;

namespace Hiku.Services.Infrastructure.Repositories
{
    public sealed partial class BarcodeRequestRepository : RepositoryBase<BarcodeRequest, Guid>
    {
        public BarcodeRequestRepository(HikuDbContext hikuDbContext, ILogger<BarcodeRequestRepository> logger): base(hikuDbContext, logger)
        {
        }

        public override Task<BarcodeRequest> GetAsync(Guid id)
        {
            return this.HikuDbContext.BarcodeRequest
                .AsNoTracking()
                .FirstOrDefaultAsync(barcodeRequest => barcodeRequest.Id == id);
        }

        public override async Task<bool> UpdateAsync(BarcodeRequest entity)
        {
            try
            {
                this.HikuDbContext.Update<BarcodeRequest>(entity);

                var changes = await this.HikuDbContext.SaveChangesAsync().ConfigureAwait(false);

                return changes > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating entity from db");

                return false;
            }
        }
    }
}
