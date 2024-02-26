using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace frmSalesReceive
{
    internal class Get_StockHouse
    {
        public DataTable Get_StockHousetable(string connectionString)
        {
            var table = new DataTable();
            var StockHouseCodes = new List<string>();

            using (var connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    // 倉庫コードと倉庫名を取得するクエリ
                    string query = 
                        $@"SELECT DISTINCT "+
                        "DSR.[StockHouseCode], " +
                        "MSH.[StockHouseName] "+
                        "FROM [ono].[dbo].[DatSalesReceive] AS DSR " +
                        "INNER JOIN [ono].[dbo].[MstStockHouse] AS MSH " +
                        "ON MSH.StockHouseCode = DSR.StockHouseCode " +
                        "WHERE "+
                        "MSH.StockHouseCode "+
                        "BETWEEN "+
                        "100 AND 105;";
                    SqlCommand command = new SqlCommand(query, connection);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        StockHouseCodes.Clear();
                        table.Load(reader);
                    }
                    _ = command;
                    connection.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("SQL Serverへの接続に失敗しました。エラー: " + ex.Message);
                }
            }
            
            return table;
        }

        public string Get_StockHouseCode(string stockHouseItem)
        {
            string stockHouseCode = null;
            List<string> nameArray = new List<string>();
            int result;

            for (int i = 0; i < stockHouseItem.Length; i++)
                if(int.TryParse(stockHouseItem.Substring(i, 1),out result))
                    stockHouseCode += result.ToString();

            return stockHouseCode;
        }
    }
}
