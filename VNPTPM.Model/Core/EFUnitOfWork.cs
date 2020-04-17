using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VNPTPM.Model.Core
{
    public class EFUnitOfWork : IUnitOfWork
    {
        private DbTransaction _Transaction = null;
        //private DbTransaction _SysTransaction = null;
        private EFDbContext _DbContext = null;
        //private EFDbContext _SysDbContext = null;

        public EFUnitOfWork(EFDbContext dbContext)
        {
            _DbContext = dbContext;

            //if (_DbContext.IsSysConnect == false)
            //_SysDbContext = EFDbContextManager.GetEFDbContextForKey(EFDALContainer.DbSystemName);
        }

        public bool IsInTransaction
        {
            get { return _Transaction != null; }
        }

        #region SaveChanges

        public void SaveChanges()
        {
            SaveChanges(System.Data.Entity.Core.Objects.SaveOptions.AcceptAllChangesAfterSave);
        }

        public void SaveChanges(System.Data.Entity.Core.Objects.SaveOptions saveOptions)
        {
            SaveChanges(_DbContext, saveOptions, _Transaction);

            //if (_DbContext.IsSysConnect == false && _SysDbContext != null)
            //SaveChanges(_SysDbContext, saveOptions, _SysTransaction);
        }

        private void SaveChanges(EFDbContext dbContext, System.Data.Entity.Core.Objects.SaveOptions saveOptions, DbTransaction transaction)
        {
            if (transaction != null)
            {
                throw new ApplicationException("A transaction is running. Call BeginTransaction instead.");
            }

            if (dbContext.ChangeTracker.Entries().Any(x => x.State != System.Data.Entity.EntityState.Detached && x.State != System.Data.Entity.EntityState.Unchanged))
            {
                try
                {
                    if (BeforeSave != null)
                    {
                        List<object> para = new List<object>
                        {
                            this.UserID,
                            dbContext
                        };

                        BeforeSave(para);
                    }
                    DBTracker tracker = new DBTracker(dbContext);

                    System.Data.Entity.Core.Objects.ObjectContext objContext = dbContext.ObjectContext;
                    objContext.SaveChanges(saveOptions);

                    tracker.SaveChanged(this);
                }
                catch (Exception exp)
                {
                    if (SaveException != null)
                    {
                        SaveException(exp);
                    }
                    else
                    {
                        throw exp;
                    }
                }
            }
        }

        #endregion

        #region Begin transaction

        public void BeginTransaction()
        {
            BeginTransaction(IsolationLevel.ReadCommitted);
        }

        //vuong edit 20/2/2012
        public void BeginTransaction(IsolationLevel level)
        {
            BeginTransaction(level, _DbContext.ObjectContext, ref _Transaction);

            //if (_DbContext.IsSysConnect == false && _SysDbContext != null)
            //    BeginTransaction(level, _SysDbContext.ObjectContext, ref _SysTransaction);
        }

        private void BeginTransaction(IsolationLevel level, System.Data.Entity.Core.Objects.ObjectContext objContext, ref DbTransaction transaction)
        {
            if (transaction != null)
            {
                throw new ApplicationException("Cannot begin a new transaction while an existing transaction is still running. " +
                                                "Please commit or rollback the existing transaction before starting a new one.");
            }

            if (objContext.Connection.State != ConnectionState.Open)
                objContext.Connection.Open();

            transaction = objContext.Connection.BeginTransaction(level);
        }

        #endregion

        #region RollBack transaction

        public void RollBackTransaction()
        {
            RollBackTransaction(_Transaction, _DbContext);

            //if (_DbContext.IsSysConnect == false && _SysDbContext != null)
            //{
            //    if (_SysDbContext.ChangeTracker.Entries().Any(x => x.State != EntityState.Detached && x.State != EntityState.Unchanged))
            //        RollBackTransaction(_SysTransaction, _SysDbContext);
            //    else
            //        ReleaseCurrentTransaction(_SysTransaction);
            //}
        }

        private void RollBackTransaction(DbTransaction transaction, EFDbContext dbContext)
        {
            if (transaction == null)
            {
                throw new ApplicationException("Cannot roll back a transaction while there is no transaction running.");
            }

            try
            {
                transaction.Rollback();
            }
            catch (Exception exp)
            {
                if (SaveException != null)
                {
                    SaveException(exp);
                }
                else
                {
                    throw exp;
                }
            }
            finally
            {
                ReleaseCurrentTransaction(transaction);
            }
        }

        #endregion

        #region Commit transaction

        /// <summary>
        /// Commit without SaveOption
        /// </summary>
        /// <returns></returns>
        public bool CommitTransaction()
        {
            bool result = CommitTransaction(System.Data.Entity.Core.Objects.SaveOptions.AcceptAllChangesAfterSave);
            return result;
        }

        /// <summary>
        /// Commit with SaveOption
        /// </summary>
        /// <returns></returns>
        public bool CommitTransaction(System.Data.Entity.Core.Objects.SaveOptions saveOptions)
        {
            bool result = false;

            result = CommitTransaction(saveOptions, _Transaction, _DbContext);

            //if (_DbContext.IsSysConnect == false && _SysDbContext != null)
            //{
            //    if (_SysDbContext.ChangeTracker.Entries().Any(x => x.State != EntityState.Detached && x.State != EntityState.Unchanged))
            //        result = CommitTransaction(saveOptions, _SysTransaction, _SysDbContext);
            //    else
            //        ReleaseCurrentTransaction(_SysTransaction);
            //}

            return result;
        }

        private bool CommitTransaction(System.Data.Entity.Core.Objects.SaveOptions saveOptions, DbTransaction transaction, EFDbContext dbContext)
        {
            bool result = false;

            if (transaction == null)
            {
                throw new ApplicationException("Cannot commit a transaction while there is no transaction running.");
            }

            try
            {
                if (dbContext.ChangeTracker.Entries().Any(x => x.State != System.Data.Entity.EntityState.Detached && x.State != System.Data.Entity.EntityState.Unchanged))
                {
                    try
                    {
                        if (BeforeSave != null)
                        {
                            List<object> para = new List<object>();
                            para.Add(this.UserID);
                            para.Add(dbContext);

                            BeforeSave(para);
                        }

                        System.Data.Entity.Core.Objects.ObjectContext objContext = dbContext.ObjectContext;
                        objContext.SaveChanges(saveOptions);
                    }
                    catch (Exception exp)
                    {
                        if (SaveException != null)
                        {
                            SaveException(exp);
                        }
                        else
                        {
                            throw exp;
                        }
                    }
                }

                transaction.Commit();
                result = true;
            }
            catch (Exception exp)
            {
                throw exp;
            }
            finally
            {
                ReleaseCurrentTransaction(transaction);
            }

            return result;
        }

        /// <summary>
        /// Commit without SaveOption
        /// </summary>
        /// <returns></returns>
        public bool CommitTransaction(bool b)
        {
            bool result = CommitTransaction(b, System.Data.Entity.Core.Objects.SaveOptions.None);
            return result;
        }

        /// <summary>
        /// Commit with SaveOption
        /// </summary>
        /// <returns></returns>
        public bool CommitTransaction(bool b, System.Data.Entity.Core.Objects.SaveOptions saveOptions)
        {
            bool result = false;

            result = CommitTransaction(b, saveOptions, _Transaction, _DbContext);

            //if (_DbContext.IsSysConnect == false && _SysDbContext != null)
            //{
            //    if (_SysDbContext.ChangeTracker.Entries().Any(x => x.State != EntityState.Detached && x.State != EntityState.Unchanged))
            //        result = CommitTransaction(b, saveOptions, _SysTransaction, _SysDbContext);
            //    else
            //        ReleaseCurrentTransaction(_SysTransaction);
            //}

            return result;
        }

        private bool CommitTransaction(bool b, System.Data.Entity.Core.Objects.SaveOptions saveOptions, DbTransaction transaction, EFDbContext dbContext)
        {
            bool result = false;

            if (transaction == null)
            {
                throw new ApplicationException("Cannot commit a transaction while there is no transaction running.");
            }
            if (!b)
                transaction.Rollback();
            else
            {
                try
                {
                    System.Data.Entity.Core.Objects.ObjectContext objContext = ((IObjectContextAdapter)dbContext).ObjectContext;
                    objContext.SaveChanges(saveOptions);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    result = true;
                }
                catch (Exception ex)
                {
                    Console.Write(ex.Message);
                }
                finally
                {
                    ReleaseCurrentTransaction(transaction);
                }
            }

            ReleaseCurrentTransaction(transaction);

            return result;
        }

        #endregion

        //private void TransactionException(Exception exp, EFDbContext dbContext, DbTransaction transaction)
        //{
        //    if (exp is OptimisticConcurrencyException)
        //    {
        //        List<object> entries = new List<object>();

        //        ObjectContext objContext = dbContext.ObjectContext();

        //        IEnumerable<ObjectStateEntry> stateEntries = objContext.ObjectStateManager.GetObjectStateEntries(EntityState.Deleted | EntityState.Modified);
        //        foreach (ObjectStateEntry stateEntry in stateEntries)
        //        {
        //            object obj;
        //            if (objContext.TryGetObjectByKey(stateEntry.EntityKey, out obj))
        //                entries.Add(obj);
        //        }

        //        if (entries.Count > 0)
        //        {
        //            try
        //            {
        //                objContext.Refresh(RefreshMode.ClientWins, entries);
        //                objContext.SaveChanges();
        //                transaction.Commit();
        //            }
        //            catch
        //            {
        //                RollBackTransaction();
        //            }
        //        }
        //        else
        //        {
        //            RollBackTransaction();
        //        }
        //    }
        //    else
        //    {
        //        RollBackTransaction();
        //    }

        //    ReleaseCurrentTransaction(transaction);
        //}

        public void ReleaseCurrentTransaction(DbTransaction transaction)
        {
            if (transaction != null)
            {
                transaction.Dispose();
                transaction = null;
            }

            if (_Transaction != null)
            {
                _Transaction.Dispose();
                _Transaction = null;
            }
        }

        public object Orm
        {
            get { return _DbContext; }
        }

        #region Implementation of IDisposable
        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        private void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            if (_disposed)
                return;

            _disposed = true;
        }
        private bool _disposed;

        public OnBeforeSave BeforeSave
        {
            get;
            set;
        }
        #endregion

        public string UserID
        {
            get;
            set;
        }

        public OnSaveException SaveException
        {
            get;
            set;
        }


        public string BUID
        {
            get;
            set;
        }

    }
}
