using Hiku.Services.Infrastructure.Repositories.Sql;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hiku.Services.Infrastructure.Repositories
{
    public abstract class RepositoryBase<TEntity> : IDisposable
        where TEntity : class
    {
        private HikuDbContext _hikuDbContext;
        protected readonly ILogger _logger;

        protected HikuDbContext HikuDbContext
        {
            get
            {
                return _hikuDbContext;
            }
        }

        public RepositoryBase(HikuDbContext securityDbContext, ILogger<RepositoryBase<TEntity>> logger)
        {
            _logger = logger;
            _hikuDbContext = securityDbContext;
        }

        public virtual Task<List<TEntity>> GetAllAsync()
        {
            return this.HikuDbContext.Set<TEntity>()
                .AsNoTracking()
                .ToListAsync();
        }

        public virtual async Task<bool> CreateAsync(TEntity entity)
        {
            try
            {
                this.HikuDbContext.Add<TEntity>(entity);

                var changes = await this.HikuDbContext.SaveChangesAsync().ConfigureAwait(false);

                return changes > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while adding entity from db");

                return false;
            }
        }

        public virtual async Task<bool> RemoveAsync(TEntity entity)
        {
            try
            {
                this.HikuDbContext.Remove<TEntity>(entity);

                var changes = await this.HikuDbContext.SaveChangesAsync().ConfigureAwait(false);

                return changes > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while removing entity from db");

                return false;
            }
        }

        internal virtual IQueryable<TEntity> GetQueryable()
        {
            return this.HikuDbContext.Set<TEntity>().AsQueryable();
        }

        #region IDisposable Implementation

        // Dispose() calls Dispose(true)
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // NOTE: Leave out the finalizer altogether if this class doesn't 
        // own unmanaged resources itself, but leave the other methods
        // exactly as they are. 
        ~RepositoryBase()
        {
            // Finalizer calls Dispose(false)
            Dispose(false);
        }

        // The bulk of the clean-up code is implemented in Dispose(bool)
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
            }
        }

        #endregion
    }

    public abstract class RepositoryBase<TEntity, TPrimaryKey> : RepositoryBase<TEntity>
        where TEntity : class
    {
        public RepositoryBase(HikuDbContext dbContext, ILogger<RepositoryBase<TEntity>> logger)
            : base(dbContext, logger)
        {
        }

        public abstract Task<TEntity> GetAsync(TPrimaryKey id);

        public abstract  Task<bool> UpdateAsync(/*TPrimaryKey id, */ TEntity entity);
    }
}
