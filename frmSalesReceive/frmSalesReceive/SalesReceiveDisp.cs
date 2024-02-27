using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace frmSalesReceive
{
    public partial class SalesReceiveDisp : Form
    {
        DateTime ReceiveDate;
        string StockHouse;
        string directory = Path.GetDirectoryName(Application.ExecutablePath);
        string ConnectionString;
        int pageHeadIndex = 0;
        int pageFootIndex = 14;
        DataTable ReceiveTable = new DataTable();

        List<_Columns> columnsList = new List<_Columns>()    
        {
            new _Columns(1184, "得意先名", "AliasName"),
            new _Columns(2774, "製品名", "AliasName1"),
            new _Columns(408, "厚み", "Thickness"),
            new _Columns(461, "幅", "Width"),
            new _Columns(513, "長さ", "Length"),
            new _Columns(513, "等級・仕立", "MaterialSubCode"),
            new _Columns(309, "本数", "Volume"),
            new _Columns(395, "出荷日", "PrdOutputDate"),
            new _Columns(395, "機台", "MachineName"),
            new _Columns(704, "着倉庫", "StockHouseName"),
            new _Columns(1888, "摘要", "Comment1"),
            new _Columns(263, "同包", "SameFlag")
        };

        DataGridView gridReceive;
        Panel panelHeader;
        Panel panelFooter;
        Label labelHeader;
        Label labelFooter;
        PictureBox buttonNext;
        PictureBox buttonBack;
        Button buttonChange;

        public SalesReceiveDisp(DateTime receivedate, string stockhouse, string connectionString)
        {
            InitializeComponent();
            ReceiveDate = receivedate;
            StockHouse = stockhouse;
            ConnectionString = connectionString;
        }
        private void SalesReceiveDisp_Load(object sender, EventArgs e)
        {
            Set_Controls();
            Update_Disp();
        }

        private void button_Click(object sender, EventArgs e)
        {
            if(sender == buttonNext)
            {
                pageHeadIndex += 15;
                gridReceive.Rows.Clear();
                gridReceive.Columns.Clear();
                Set_ReceiveTable(ReceiveTable, ref pageHeadIndex, ref pageFootIndex);
            }
            else if(sender == buttonBack)
            {
                pageHeadIndex -= 15;
                gridReceive.Rows.Clear();
                gridReceive.Columns.Clear();
                Set_ReceiveTable(ReceiveTable, ref pageHeadIndex, ref pageFootIndex);
            }
            else
            {
                this.Close();
            }
        }

        /// <summary>
        /// マウスカーソルが画像にあった時に画像変更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PictureBox_MouseEnter(object sender, EventArgs e)
        {
            if (sender == buttonNext)
                buttonNext.Image = Image.FromFile(Path.Combine(directory, "buttonNext_selected.png"));
            else
                buttonBack.Image = Image.FromFile(Path.Combine(directory, "buttonBack_selected.png"));
        }

        /// <summary>
        /// マウスカーソルが画像から離れた時に画像変更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PictureBox_MouseLeave(object sender, EventArgs e)
        {
            if (sender == buttonNext)
                buttonNext.Image = Image.FromFile(Path.Combine(directory, "buttonNext.png"));
            else
                buttonBack.Image = Image.FromFile(Path.Combine(directory, "buttonBack.png"));
        }

        /// <summary>
        /// コントロールの設置とプロパティ設定
        /// </summary>
        private void Set_Controls()
        {
            
            // フォームは最大化で初期表示
            this.WindowState = FormWindowState.Maximized;

            // ヘッダーパネルの設定
            panelHeader = new Panel
            {
                Dock = DockStyle.Top,
                BackColor = Color.Gray,
                Height = 50
            };
            this.Controls.Add(panelHeader);

            // ヘッダーラベルの設定
            labelHeader = new Label
            {
                Text = $"[{ReceiveDate.ToString("yyyy/MM/dd(ddd)")}]入荷予定一覧表（{StockHouse}）",
                BackColor = Color.Gray,
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter
            };
            panelHeader.Controls.Add(labelHeader);
            labelHeader.Font = new Font(labelHeader.Font.FontFamily, panelHeader.Height * 0.4f);
            labelHeader.Location = new Point((panelHeader.Width - labelHeader.Width) / 2, (panelHeader.Height - labelHeader.Height) / 2);

            // フッターパネルの設定
            panelFooter = new Panel
            {
                Dock = DockStyle.Bottom,
                BackColor = Color.Gray,
                Height = 40
            };
            this.Controls.Add(panelFooter);

            // フッターラベルの設定
            labelFooter = new Label
            {
                Text = "1/1",
                BackColor = Color.Gray,
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter,

            };
            labelFooter.Location = new Point((panelFooter.Width - labelFooter.Width) / 2, (panelFooter.Height - labelFooter.Height) / 2);
            labelFooter.Font = new Font(labelFooter.Font.FontFamily, panelFooter.Height * 0.4f);
            panelFooter.Controls.Add(labelFooter);

            // 次ボタンの設定
            buttonNext = new PictureBox
            {
                Image = Image.FromFile(Path.Combine(directory, "buttonNext.png")),
                Size = new Size((int)(panelFooter.Height * 0.7), (int)(panelFooter.Height * 0.7)),
                Cursor = Cursors.Hand,
                
            };           
            buttonNext.Location = new Point(labelFooter.Location.X + labelFooter.Width, (panelFooter.Height - buttonNext.Height) / 2);
            buttonNext.Click += button_Click; // クリックイベントを追加
            buttonNext.MouseEnter += PictureBox_MouseEnter;
            buttonNext.MouseLeave += PictureBox_MouseLeave;
            buttonNext.SizeMode = PictureBoxSizeMode.Zoom;
            panelFooter.Controls.Add(buttonNext);

            // 前ボタンの設定
            buttonBack = new PictureBox
            {
                Image = Image.FromFile(Path.Combine(directory, "buttonBack.png")),
                Size = new Size((int)(panelFooter.Height * 0.7), (int)(panelFooter.Height * 0.7)),
            };
            buttonBack.Location = new Point(labelFooter.Location.X - buttonBack.Width - 6, (panelFooter.Height - buttonBack.Height) / 2);
            buttonBack.Click += button_Click; // クリックイベントを追加
            buttonBack.MouseEnter += PictureBox_MouseEnter;
            buttonBack.MouseLeave += PictureBox_MouseLeave;
            buttonBack.SizeMode = PictureBoxSizeMode.Zoom;
            panelFooter.Controls.Add(buttonBack);

            // 変更ボタン
            buttonChange = new Button
            {
                Text = "変更",
                Size = new Size((int)(panelHeader.Height * 1.2), (int)(panelHeader.Height * 0.7)),
                BackColor = Color.LightCyan,
                
            };
            buttonChange.Location = new Point((int)(labelHeader.Location.X + labelHeader.Width), (int)(panelHeader.Height - buttonChange.Height) / 2);
            buttonChange.Font = new Font(buttonChange.Font.FontFamily, buttonChange.Height * 0.4f, FontStyle.Bold);
            buttonChange.Click += button_Click;
            panelHeader.Controls.Add(buttonChange);

            // 表示グリッドの設定
            gridReceive = new DataGridView
            {
                ReadOnly = true,
                AllowUserToAddRows = false,
                Location = new Point(0, panelFooter.Height),
            };
            this.Controls.Add(gridReceive);
        }

        private void Update_Disp()
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    connection.Open();
                    Get_StockHouse get_ = new Get_StockHouse();
                    DataTable dt = new DataTable();
                    string query =
                        @"SELECT DISTINCT " + "\n" +
                        "MC.[AliasName] , " + "\n" +
                        "MP.[AliasName] , " + "\n" +
                        "DSR.[Thickness] , " + "\n" +
                        "DSR.[Width] , " + "\n" +
                        "DSR.[Length] , " + "\n" +
                        "DSR.[MaterialSubCode1], " + "\n" +
                        "DSR.[MaterialSubCode2], " + "\n" +
                        "DSR.[Volume], " + "\n" +
                        "DSD.[PrdOutputDate], " + "\n" +
                        "MM.[MachineName], " +
                        "MSH.[StockHouseName], " + "\n" +
                        "DSH.[Comment1], " + "\n" +
                        "DSH.[ProcessOrderNo], " + "\n" +
                        "DSP.[SameFlag] " + "\n" +

                        "FROM [ono].[dbo].[DatSalesReceive] AS DSR " + "\n" +
                        "INNER JOIN [ono].[dbo].[MstCustomer] AS MC ON " + "\n" +
                            "DSR.[CustCode] = MC.[CustCode] " + "\n" +
                        "INNER JOIN [ono].[dbo].[MstProduct] AS MP ON " + "\n" +
                            "DSR.[ProductCode] = MP.[ProductCode] " + "\n" +
                        "INNER JOIN [ono].[dbo].[DatSalesDetail] DSD ON " + "\n" +
                            "DSR.[SalesNo] = DSD.[SalesNo] AND " + "\n" +
                            "DSR.[SalesLinkNo] = DSD.[SalesLinkNo] " + "\n" +
                        "INNER JOIN [ono].[dbo].[DatProcessHeader] AS DPH ON " + "\n" +
                            "DSR.[SalesNo] = DPH.[SalesNo] " + "\n" +
                        "INNER JOIN [ono].[dbo].[MstMachine]AS MM ON " + "\n" +
                            "DPH.[MachineNo] = MM.[MachineNo] " + "\n" +
                        "INNER JOIN [ono].[dbo].[MstStockHouse] AS MSH ON " + "\n" +
                            "DSR.[BaseStockHouseCode] = MSH.[StockHouseCode] " + "\n" +
                        "INNER JOIN [ono].[dbo].[DatSalesHeader] AS DSH ON " + "\n" +
                            "DSR.[SalesNo] = DSH.[SalesNo] " + "\n" +
                        "INNER JOIN [ono].[dbo].[DatSalesPackage] AS DSP ON " + "\n" +
                            "DSR.[SalesNo] = DSP.[SalesNo] AND " + "\n" +
                            "DSR.[SalesDetailNo] = DSP.[SalesDetailNo] AND " + "\n" +
                            "DSR.[SalesLinkNo] = DSP.[SalesLinkNo] " + "\n" +
                        "WHERE "+ "\n" +
                        $"CAST(DSR.InputDate AS DATE) = '{ReceiveDate.ToString("yyyy-MM-dd")}' AND "+ "\n" +
                        $"DSR.[StockHouseCode] = '{get_.Get_StockHouseCode(StockHouse)}' AND " + "\n" +
                        $"DSR.[BaseStockHouseCode] = '{get_.Get_StockHouseCode(StockHouse)}' AND " + "\n" +
                        "DSR.[InputOrderNo] NOT LIKE '%-%' AND " + "\n" +
                        "DSR.[CategoryCode] = '92';";

                    SqlCommand command = new SqlCommand(query, connection);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        ReceiveTable.Clear();
                        ReceiveTable.Load(reader);
                    }
                    connection.Close();
                    gridReceive.Rows.Clear();
                    gridReceive.Columns.Clear();
                    pageFootIndex = ReceiveTable.Rows.Count;
                    Set_ReceiveTable(ReceiveTable, ref pageHeadIndex, ref pageFootIndex);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("SQL Serverへの接続に失敗しました。エラー: " + ex.Message);
                }
            }
        }

        private void Set_ReceiveTable(DataTable dt, ref int start, ref int end)
        {
            var RowsCount = end - start > 15 ? 15 : end - start;

            for (int i = 0; i < columnsList.Count; i++)
            {
                gridReceive.Columns.Add(columnsList[i].ColumnDBName, columnsList[i].ColumnName);
                gridReceive.Columns[i].Width = (int)((decimal)this.ClientSize.Width * (columnsList[i].Width / 10000));

                for (int j = start; j < start + RowsCount; j++)
                {
                    if (i == 0)
                        gridReceive.Rows.Add();
                    switch (columnsList[i].ColumnName)
                    {
                        case "等級・仕立":
                            gridReceive.Rows[j - start].Cells[i].Value = dt.Rows[j][$"{columnsList[i].ColumnDBName}1"].ToString() + " " + dt.Rows[j][$"{columnsList[i].ColumnDBName}2"].ToString();
                            break;
                        case "出荷日":
                            gridReceive.Rows[j - start].Cells[i].Value = dt.Rows[j][columnsList[i].ColumnDBName].ToString() != "" ? dt.Rows[j][columnsList[i].ColumnDBName].ToString().Substring(5, 5) : "";
                            break;
                        case "摘要":
                            gridReceive.Rows[j - start].Cells[i].Value = (dt.Rows[j][columnsList[i].ColumnDBName].ToString() != "") ? dt.Rows[j][columnsList[i].ColumnDBName] : "";
                            break;
                        case "同包":
                            gridReceive.Rows[j - start].Cells[i].Value = (dt.Rows[j][columnsList[i].ColumnDBName].ToString() == "True") ? "★" : "";
                            break;
                        default:
                            gridReceive.Rows[j - start].Cells[i].Value = dt.Rows[j][columnsList[i].ColumnDBName];
                            break;
                    }
                }
            }
            labelFooter.Text = $"{start + 1}-{(start + RowsCount).ToString()}件/{dt.Rows.Count}件中";
            gridReceive.DefaultCellStyle.Font = new Font(gridReceive.Font.FontFamily, 17);
            gridReceive.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            gridReceive.Size = new Size(this.Width, this.ClientSize.Height - (panelHeader.Height + panelFooter.Height));
            gridReceive.Location = new Point(0, panelHeader.Height);
            gridReceive.ReadOnly = true;
            gridReceive.AllowUserToAddRows = false;
            foreach (DataGridViewRow row in gridReceive.Rows)
                row.Height = (gridReceive.Height - gridReceive.ColumnHeadersHeight) / 15;
            buttonNext.Enabled = (end > 15 && end > start + 15)  ? true : false;
            buttonNext.Visible = buttonNext.Enabled ? true : false; 
            buttonBack.Enabled = start == 0 ? false : true;
            buttonBack.Visible = buttonBack.Enabled ? true : false;
            buttonNext.Location = new Point(labelFooter.Location.X + labelFooter.Width, buttonNext.Location.Y);
            buttonBack.Location = new Point(labelFooter.Location.X - buttonBack.Width, buttonBack.Location.Y);
        }

        /// <summary>
        /// _Columns構造体
        /// </summary>
        struct _Columns
        {
            public decimal Width;
            public string ColumnName;
            public string ColumnDBName;
            public _Columns(decimal _width, string _columnName, string _columnDBName)
            {
                Width = _width;
                ColumnName = _columnName;
                ColumnDBName = _columnDBName;
            }
        }
    }
}
