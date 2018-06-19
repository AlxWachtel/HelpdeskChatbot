using System;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace DatabaseAccess
{
    class SQL
    {
        private MySqlConnection connection;
        private String server = "127.0.0.1";
        private String database = "universität";
        private String uid = "user";
        private String password = "luser";
        public SQL()
        {
            initializeConnection();
        }

        private void initializeConnection()
        {
            string connectionString = "server=" + server + ";" + "uid=" + uid + ";" + "pwd=" + password + ";" + "database=" + database;
            connection = new MySqlConnection(connectionString);
        }

        public String[] executeCommand(String commandText) {
            try
            {
                connection.Open();
                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = commandText;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Connection = connection;
                MySqlDataReader reader = cmd.ExecuteReader();
                string[] res = generateResult(reader);
                connection.Close();
                return res;
            }
            catch (Exception ex) {
                Console.WriteLine(ex.ToString());
                return new string[] { ex.ToString() };//"Error, please check command and try again."};
            }
        }

        private string[] generateResult(MySqlDataReader reader) {

            LinkedList<string> list = new LinkedList<string>();

            if (reader.HasRows)
            {
                
                while (reader.Read())
                {
                    String matrNr = reader.GetString(0);
                    String vorname = reader.GetString(1);
                    String nachname = reader.GetString(2);
                    String adresse = reader.GetString(3);
                    String stadt = reader.GetString(4);
                    String plz = reader.GetString(5);
                    String fach = reader.GetString(6);
                    list.AddLast(matrNr + ";" + vorname + ";" + nachname + ";" + adresse + ";" + stadt + ";" + plz + ";" + fach);
                }
                string[] arr = new string[list.Count];
                list.CopyTo(arr, 0);
                return arr;
            } else
            {
                return new string[] {"nothing to return"};
            }

            
        }
    }
}
