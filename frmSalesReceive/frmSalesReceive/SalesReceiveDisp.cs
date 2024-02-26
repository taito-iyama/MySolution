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

        private void buttonNext_Click(object sender, EventArgs e)
        {

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
            buttonNext.Click += buttonNext_Click; // クリックイベントを追加
            buttonNext.MouseEnter += PictureBox_MouseEnter;
            buttonNext.MouseLeave += PictureBox_MouseLeave;
            buttonNext.SizeMode = PictureBoxSizeMode.Zoom;
            panelFooter.Controls.Add(buttonNext);

            // 前ボタンの設定
            buttonBack = new PictureBox
            {
                Image = Image.FromFile(Path.Combine(directory, "buttonBack.png")),
                Size = new Size((int)(panelFooter.Height * 0.7), (int)(panelFooter.Height * 0.7)),
                Cursor = Cursors.Hand,

            };
            buttonBack.Location = new Point(labelFooter.Location.X - buttonBack.Width - 6, (panelFooter.Height - buttonBack.Height) / 2);
            buttonBack.Click += buttonNext_Click; // クリックイベントを追加
            buttonBack.MouseEnter += PictureBox_MouseEnter;
            buttonBack.MouseLeave += PictureBox_MouseLeave;
            buttonBack.SizeMode = PictureBoxSizeMode.Zoom;
            panelFooter.Controls.Add(buttonBack);


            // 表示グリッドの設定
            gridReceive = new DataGridView
            {
                ReadOnly = true,
                AllowUserToAddRows = false,
                Location = new Point(0, panelFooter.Height),
            };
            gridReceive.DefaultCellStyle.Font = new Font(gridReceive.Font.FontFamily, 17);
            gridReceive.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            gridReceive.AutoResizeRows(DataGridViewAutoSizeRowsMode.AllCells);
            gridReceive.Size = new Size(this.Width, this.ClientSize.Height - (panelHeader.Height + panelFooter.Height));
            gridReceive.Location = new Point(0, panelFooter.Height);
            this.Controls.Add(gridReceive);
        }

        private void Update_Disp()
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    connection.Open();

                    DataTable dt = new DataTable();
                    string query =
                        $@"SELECT " +
                        $"MC.[AliasName] , " +
                        $"MP.[AliasName] , " +
                        $"DSR.[Thickness] , " +
                        $"DSR.[Width] , " +
                        $"DSR.[Length] , " +
                        $"DSR.[MaterialSubCode1], " +
                        $"DSR.[MaterialSubCode2], " +
                        $"DSR.[Volume], " +
                        $"DSD.[PrdOutputDate], " +
                        $"MM.[MachineName], " +
                        $"MSH.[StockHouseName], " +
                        $"DSH.[Comment1], " +
                        $"DSH.[ProcessOrderNo], " +
                        $"DSP.[SameFlag] " +
                        
                        $"FROM [ono].[dbo].[DatSalesReceive] AS DSR " +
                    
                        $"INNER JOIN [ono].[dbo].[MstCustomer] AS MC ON " +
                            $"DSR.[CustCode] = MC.[CustCode] " +
                    
                        $"INNER JOIN [ono].[dbo].[MstProduct] AS MP ON " +
                            $"DSR.[ProductCode] = MP.[ProductCode] " +
                        
                        $"INNER JOIN [ono].[dbo].[DatSalesDetail] DSD ON " +
                            $"DSR.[SalesNo] = DSD.[SalesNo] AND " +
                            $"DSR.[SalesLinkNo] = DSD.[SalesLinkNo] " +
                    
                            
                        $"INNER JOIN [ono].[dbo].[DatProcessHeader] AS DPH ON " +    
                            $"DSR.[SalesNo] = DPH.[SalesNo] " +
                    
                        $"INNER JOIN [ono].[dbo].[MstMachine]AS MM ON " +
                            $"DPH.[MachineNo] = MM.[MachineNo] " +
                        
                        $"INNER JOIN [ono].[dbo].[MstStockHouse] AS MSH ON " +
                            $"MSH.[StockHouseCode] = MSH.[StockHouseCode] " +
                    
                        $"INNER JOIN [ono].[dbo].[DatSalesHeader] AS DSH ON " +
                            $"DSR.[SalesNo] = DSH.[SalesNo] " + 
                    
                        $"INNER JOIN [ono].[dbo].[DatSalesPackage] AS DSP ON " +
                            $"DSR.[SalesNo] = DSP.[SalesNo] AND " +
                            $"DSR.[SalesDetailNo] = DSP.[SalesDetailNo] AND " +
                            $"DSR.[SalesLinkNo] = DSP.[SalesLinkNo] " +    
                        
                        $"WHERE CAST(DSR.InputDate AS DATE) = '{ReceiveDate.ToString("yyyy-MM-dd")}'";
                    //WHERE CAST(InputDate AS DATE) = '2024-02-20';

                    SqlCommand command = new SqlCommand(query, connection);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        dt.Clear();
                        dt.Load(reader);
                    }
                    connection.Close();

                    Set_ReceiveTable(dt);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("SQL Serverへの接続に失敗しました。エラー: " + ex.Message);
                }
            }
            
        }

        private void Set_ReceiveTable(DataTable dt)
        {
            
            ReceiveTable.Clear();
            for (int i = 0; i < columnsList.Count; i++)
            {
                gridReceive.Columns.Add(columnsList[i].ColumnDBName, columnsList[i].ColumnName);
                gridReceive.Columns[i].Width = (int)((decimal)this.ClientSize.Width * (columnsList[i].Width / 10000));

                for (int j = 0; j < dt.Rows.Count; j++)
                {
                    if(i == 0)
                        gridReceive.Rows.Add();
                    switch (columnsList[i].ColumnName)
                    {
                        case "等級・仕立":
                            gridReceive.Rows[j].Cells[i].Value = dt.Rows[j][$"{columnsList[i].ColumnDBName}1"].ToString() + " " + dt.Rows[j][$"{columnsList[i].ColumnDBName}2"].ToString();
                            break;
                        case "出荷日":
                            gridReceive.Rows[j].Cells[i].Value = dt.Rows[j][columnsList[i].ColumnDBName].ToString().Substring(5, 5);
                            break;
                        case "摘要":    
                            gridReceive.Rows[j].Cells[i].Value = (dt.Rows[j][columnsList[i].ColumnDBName].ToString() != "") ? dt.Rows[j][columnsList[i].ColumnDBName] : "";
                            break;
                        case "同包":
                            gridReceive.Rows[j].Cells[i].Value = (dt.Rows[j][columnsList[i].ColumnDBName].ToString() == "True") ? "★" : "";
                            break;
                        default:
                                gridReceive.Rows[j].Cells[i].Value = dt.Rows[j][columnsList[i].ColumnDBName];
                            break;
                    }
                }
            }
            gridReceive.DefaultCellStyle.Font = new Font(gridReceive.Font.FontFamily, 17);
            gridReceive.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            gridReceive.AutoResizeRows(DataGridViewAutoSizeRowsMode.AllCells);
            gridReceive.Size = new Size(this.Width, this.ClientSize.Height - (panelHeader.Height + panelFooter.Height));
            gridReceive.Location = new Point(0, panelFooter.Height);
            gridReceive.ReadOnly = true;
            gridReceive.AllowUserToAddRows = false;
            gridReceive.Location = new Point(0, panelHeader.Height);
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
