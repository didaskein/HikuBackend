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
    public sealed partial class BatteryRepository : RepositoryBase<Battery, Guid>
    {
        public BatteryRepository(HikuDbContext hikuDbContext, ILogger<BatteryRepository> logger): base(hikuDbContext, logger)
        {
        }

        public override Task<Battery> GetAsync(Guid id)
        {
            return this.HikuDbContext.Battery
                .AsNoTracking()
                .FirstOrDefaultAsync(battery => battery.Id == id);
        }

        public override async Task<bool> UpdateAsync(Battery entity)
        {
            try
            {
                this.HikuDbContext.Update<Battery>(entity);

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
