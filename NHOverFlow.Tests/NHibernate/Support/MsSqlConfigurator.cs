using System.Data.SqlClient;
using Microsoft.Win32;

namespace MyOverFlow.Tests.NHibernate.Support
{
    public enum SetupAction
    {
        CreateDbAndSchema = 0,
        CreateSchema,
        UpdateSchema,
        None
    }

    public class MsSqlConfigurator
    {
        private readonly SqlConnectionStringBuilder _builder;
        public string DatabaseName { get; set; }
        public string ConnectionString { get; set; }
        public bool Express { get; set; }

        public MsSqlConfigurator(string connectionString, bool express)
        {
            _builder = new SqlConnectionStringBuilder(connectionString);
            DatabaseName = _builder.InitialCatalog;
            ConnectionString = connectionString;
            Express = express;
        }

        public void CreateMedia()
        {
            string sqlServerDataDirectory = GetSqlServerDataDirectory();
            string createDatabaseScript = "IF (SELECT DB_ID('" + DatabaseName + "')) IS NULL  "
                                          + " CREATE DATABASE " + DatabaseName
                                          + " ON PRIMARY "
                                          + " (NAME = '" + DatabaseName + "_Data', "
                                          + @" FILENAME = '" + sqlServerDataDirectory + DatabaseName + ".mdf', "
                                          + " SIZE = 5MB,"
                                          + " FILEGROWTH =" + 10 + ") "
                                          + " LOG ON (NAME ='" + DatabaseName + "_Log', "
                                          + @" FILENAME = '" + sqlServerDataDirectory + DatabaseName + ".ldf', "
                                          + " SIZE = 1MB, "
                                          + " FILEGROWTH =" + 5 + ")";

            ExecuteDbScript(createDatabaseScript, ConnectionStringFor("master"));
        }

        public void DestroyMedia()
        {
            var script = string.Concat("IF (SELECT DB_ID('", DatabaseName, "')) IS NOT NULL ", "DROP DATABASE ", DatabaseName);
            ExecuteDbScript(script, ConnectionStringFor("master"));
        }

        protected virtual string ConnectionStringFor(string databaseName)
        {
            var builder = new SqlConnectionStringBuilder(ConnectionString) { InitialCatalog = databaseName };
            //string result = null;
            //if(Express)
            //    result = string.Format(@"Server=(local)\SqlExpress;initial catalog={0};Integrated Security=SSPI", databaseName);
            //else
            //    result = string.Format("Server=(local);initial catalog={0};Integrated Security=SSPI", databaseName);

            return builder.ConnectionString;
        }

        private void ExecuteDbScript(string sqlScript, string connectionString)
        {
            using (var conn = new SqlConnection(connectionString))
            using (var command = new SqlCommand(sqlScript, conn))
            {
                conn.Open();
                command.ExecuteNonQuery();
            }
        }

        private string GetSqlServerDataDirectory()
        {
            string result;
            if (Express)
            {
                const string sqlServerRegKey = @"SOFTWARE\Microsoft\Microsoft SQL Server\";
                var sqlServerInstanceName =
                    (string)Registry.LocalMachine.OpenSubKey(sqlServerRegKey + @"Instance Names\SQL")
                                    .GetValue("SQLEXPRESS");
                var sqlServerInstanceSetupRegKey = sqlServerRegKey + sqlServerInstanceName + @"\Setup";
                result = Registry.LocalMachine
                                 .OpenSubKey(sqlServerInstanceSetupRegKey)
                                 .GetValue("SQLDataRoot") + @"\Data\";
            }
            else
            {
                //HKEY_LOCAL_MACHINE\\
                const string sqlServerRegKey = @"SOFTWARE\Microsoft\Microsoft SQL Server\";
                var sqlServerInstanceName = (string)Registry.LocalMachine.OpenSubKey(sqlServerRegKey + @"Instance Names\SQL")
                                                            .GetValue("MSSQLSERVER");
                var sqlServerInstanceSetupRegKey = sqlServerRegKey + sqlServerInstanceName + @"\Setup";
                result = Registry.LocalMachine.OpenSubKey(sqlServerInstanceSetupRegKey)
                                 .GetValue("SQLDataRoot") + @"\Data\";

            }

            return result;
        }
    }
}