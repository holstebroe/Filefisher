using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;

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

                var cnn = new SQLiteConnection("Data Source="+databaseFileName);

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

            SQLiteConnection cnn = new SQLiteConnection("Data Source=C:CheckoutWorldDominator.s3db");

            cnn.Open();

            SQLiteCommand mycommand = new SQLiteCommand(cnn);

            mycommand.CommandText = sql;

            int rowsUpdated = mycommand.ExecuteNonQuery();

            cnn.Close();

            return rowsUpdated;

        }

        public string ExecuteScalar(string sql)
        {

            SQLiteConnection cnn = new SQLiteConnection("Data Source=C:CheckoutWorldDominator.s3db");

            cnn.Open();

            SQLiteCommand mycommand = new SQLiteCommand(cnn);

            mycommand.CommandText = sql;

            object value = mycommand.ExecuteScalar();

            cnn.Close();

            if (value != null)
            {

                return value.ToString();

            }

            return "";

        }
    }
}
