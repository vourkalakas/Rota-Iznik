﻿using System;
using System.Data.Entity;
using System.Transactions;

namespace Data.Initializers
{
    public class DropCreateCeDatabaseIfModelChanges<TContext> : SqlCeInitializer<TContext> where TContext : DbContext
    {
        #region Strategy implementation
        
        public override void InitializeDatabase(TContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var replacedContext = ReplaceSqlCeConnection(context);

            bool databaseExists;
            using (new TransactionScope(TransactionScopeOption.Suppress))
            {
                databaseExists = replacedContext.Database.Exists();
            }

            if (databaseExists)
            {
                if (context.Database.CompatibleWithModel(throwIfNoMetadata: true))
                {
                    return;
                }

                replacedContext.Database.Delete();
            }

            // Database didn't exist or we deleted it, so we now create it again.
            context.Database.Create();

            Seed(context);
            context.SaveChanges();
        }

        #endregion

        #region Seeding methods
        protected virtual void Seed(TContext context)
        {
        }

        #endregion
    }

}
