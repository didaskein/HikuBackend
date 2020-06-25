using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Hiku.Services.Models;
using System.Threading.Tasks;
using System.Data;

namespace Hiku.Services.Infrastructure.Repositories.Sql
{
    public partial class HikuDbContext : DbContext
    {
        public int? CommandTimeout
        {
            get
            {
                return Database.GetCommandTimeout();
            }
            set
            {
                Database.SetCommandTimeout(value);
            }
        }

        public async Task<System.Data.Common.DbConnection> GetOpenedDbConnectionAsync()
        {
            var connection = Database.GetDbConnection();

            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync().ConfigureAwait(false);
            }

            return (connection);
        }
    }
}

