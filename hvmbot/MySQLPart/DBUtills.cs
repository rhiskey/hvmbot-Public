using hvmbot;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace vkMCBot.Mysql
{
    class DBUtills
    {
        public static MySqlConnection GetDBConnection()
        {
            //CREATE USER 'myuser'@'%' IDENTIFIED BY 'mypass';
            //GRANT ALL ON databasename.* TO 'myuser'@'%';

            string host = Configuration.dbAuth.host; 
            int port = Configuration.dbAuth.port;
            string database = Configuration.dbAuth.database;
            string username = Configuration.dbAuth.username;
            string password = Configuration.dbAuth.password;

            return DBMySQLUtils.GetDBConnection(host, port, database, username, password);
        }
    }
}
