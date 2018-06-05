using System;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data.Common;

namespace SearchDbApi.Data.Sql
{
    public class SqlReader : IDisposable
    {
        private readonly SqlConnection connection;
        private bool connectIsOpen;

        public SqlReader(string connectionString)
        {
            connection = new SqlConnection(connectionString);
            connectIsOpen = false;
        }

        public async Task<IList<IList<object>>> ExecuteAsync(string query)
        {
            if (!connectIsOpen) {
                await connection.OpenAsync();
                connectIsOpen = true;
            }

            SqlCommand command = new SqlCommand(query, connection);
            SqlDataReader reader = await command.ExecuteReaderAsync();

            var resultList = new List<IList<object>>();
            if (reader.HasRows) {
                while (await reader.ReadAsync())
                {
                    var row = new List<object>();
                    for (int i = 0; i < reader.FieldCount; ++i) {
                        row.Add(reader.GetValue(i));
                    }

                    resultList.Add(row);
                }
            }

            reader.Close();
            return resultList;
        }

        public void Dispose()
        {
            if (connectIsOpen) {
                connection.Close();
                connectIsOpen = false;
            }
        }
    }
}
