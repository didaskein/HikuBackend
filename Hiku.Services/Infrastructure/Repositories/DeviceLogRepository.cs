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
    public sealed partial class DeviceLogRepository : RepositoryBase<DeviceLog, Guid>
    {
        public DeviceLogRepository(HikuDbContext hikuDbContext, ILogger<DeviceLogRepository> logger): base(hikuDbContext, logger)
        {
        }

        public override Task<DeviceLog> GetAsync(Guid id)
        {
            return this.HikuDbContext.DeviceLog
                .AsNoTracking()
                .FirstOrDefaultAsync(DeviceLog => DeviceLog.Id == id);
        }

        public override async Task<bool> UpdateAsync(DeviceLog entity)
        {
            try
            {
                this.HikuDbContext.Update<DeviceLog>(entity);

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
