using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Core.Metadata.Edm;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VNPTPM.Model.Core
{
    public class EFDALContainer : IDALContainer, IDisposable
    {
        private static Dictionary<string, MetadataWorkspace> metadataWorkspaceMemoizer = new Dictionary<string, MetadataWorkspace>();
        public static Dictionary<string, Stream> MetaData = new Dictionary<string, Stream>();
        private IRepository _Repository = null;
        private IUnitOfWork _UnitOfWork = null;
        private EFDbContext _DbContext = null;

        public EFDALContainer()
        {
            if (_DbContext == null)
            {
                _DbContext = new EFDbContext("VNPTPMEntities");
            }
        }

        public EFDALContainer(DatabaseInfo DbInfo)
        {
            _DbContext = new EFDbContext("VNPTPMEntities");

        }

        public EFDbContext DBContext
        {
            get { return _DbContext; }
        }

        public IRepository Repository
        {
            get
            {
                if (_Repository == null)
                {
                    _Repository = CreateRepository(UnitOfWork);
                }
                return _Repository;
            }
        }

        public IUnitOfWork UnitOfWork
        {
            get
            {
                if (_UnitOfWork == null)
                {
                    _UnitOfWork = CreateUnitOfWork();
                }

                return _UnitOfWork;
            }
        }

        private IRepository CreateRepository(IUnitOfWork unitOfWork)
        {
            return new EFRepository(unitOfWork);
        }

        private IUnitOfWork CreateUnitOfWork()
        {
            return new EFUnitOfWork(_DbContext);
        }

        public void Close()
        {
            if (_DbContext.Database.Connection.State == System.Data.ConnectionState.Open)
                _DbContext.Database.Connection.Close();

            _DbContext.Dispose();
        }

        public void Dispose()
        {
            //Close();
            GC.SuppressFinalize(this);
        }

        private string ExtractString(string source, string start, string end)
        {
            // You should check for errors in real-world code, omitted for brevity
            int startIndex = source.IndexOf(start) + start.Length;
            if (startIndex > (start.Length - 1))
            {
                int endIndex = source.IndexOf(end, startIndex);
                return source.Substring(startIndex, endIndex - startIndex);
            }
            else
                return "";
        }
        public DatabaseInfo CreateDefaultDatabase()
        {
            string DbName = ConfigurationManager.AppSettings["DbSystemName"];

            DatabaseInfo _DBInfo = new DatabaseInfo();
            ConnectionStringSettings conn = ConfigurationManager.ConnectionStrings[DbName];
            if (conn != null)
            {
                Crypto crypto = new Crypto();
                string CryptoString = conn.ConnectionString;//crypto.Decrypt(conn.ConnectionString);
                string server = ExtractString(CryptoString, "data source=", ";");
                string dbName = ExtractString(CryptoString, "initial catalog=", ";");
                string login = ExtractString(CryptoString, "user id=", ";");
                string password = ExtractString(CryptoString, "password=", ";");
                string providerName = ExtractString(CryptoString, "provider=", ";");
                string trustedConnection = ExtractString(CryptoString, "Trusted_Connection=", ";");

                _DBInfo.Name = conn.Name;
                _DBInfo.DBName = dbName;
                _DBInfo.ProviderName = providerName;
                _DBInfo.DataSource = server;

                if (login != "")
                {
                    _DBInfo.LoginName = login;
                    _DBInfo.Password = password;
                }
                else if (trustedConnection != "")
                    _DBInfo.LoginName = trustedConnection;
            }

            _DBInfo.Schema = "dbo";
            return _DBInfo;
        }
    }
}
