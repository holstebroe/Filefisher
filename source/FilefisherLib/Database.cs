using System;
using System.Data;
using System.Data.SQLite;
using System.IO;

namespace FilefisherLib
{
    public class Database
    {
        private readonly string databaseFileName;

        public Database(string databaseFileName)
        {
            this.databaseFileName = databaseFileName;
        }

        public void Initialize()
        {
            if (!File.Exists(databaseFileName))
                CreateNewDatabase(databaseFileName);
        }

        public void CreateNewDatabase(string fileName)
        {
            File.Copy("DatabaseTemplate.s3db", fileName);
        }

        public DataTable GetDataTable(string sql)
        {
            var dt = new DataTable();

            try
            {
                var cnn = new SQLiteConnection("Data Source=" + databaseFileName);

                cnn.Open();

                var mycommand = new SQLiteCommand(cnn) {CommandText = sql};

                var reader = mycommand.ExecuteReader();

                dt.Load(reader);

                reader.Close();

                cnn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }

            return dt;
        }

        public int ExecuteNonQuery(string sql)
        {
            var cnn = new SQLiteConnection("Data Source=C:CheckoutWorldDominator.s3db");

            cnn.Open();

            var mycommand = new SQLiteCommand(cnn);

            mycommand.CommandText = sql;

            var rowsUpdated = mycommand.ExecuteNonQuery();

            cnn.Close();

            return rowsUpdated;
        }

        public string ExecuteScalar(string sql)
        {
            var cnn = new SQLiteConnection("Data Source=C:CheckoutWorldDominator.s3db");

            cnn.Open();

            var mycommand = new SQLiteCommand(cnn);

            mycommand.CommandText = sql;

            var value = mycommand.ExecuteScalar();

            cnn.Close();

            if (value != null) return value.ToString();

            return "";
        }
    }
}