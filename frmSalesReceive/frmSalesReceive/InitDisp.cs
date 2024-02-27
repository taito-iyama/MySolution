using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace frmSalesReceive
{
    public partial class InitDisp : Form
    {
        DateTime SelectedDate;
        string InputStockHouse;
        const string connectionstring =
            @"Data Source=192.168.19.10;" +
            "Integrated Security=False;" +
            "User ID=sa;" +
            "Password=Onoplus2022*;";

        Label LabelReceiveDate;
        Label LabelStockHouse;
        DateTimePicker ReceiveDate;
        ComboBox StockHouse;
        Button SalesReceive;

        public InitDisp(DateTime dateTime, string stockHouse)
        {
            InitializeComponent();
            SelectedDate = dateTime;
            InputStockHouse = stockHouse;
        }

        private void InitDisp_Load(object sender, EventArgs e)
        {
            Add_Controls();
        }

        private void Button_Click(object sender, EventArgs e)
        {
            this.Enabled = false;
            SelectedDate = ReceiveDate.Value;
            var salesReceiveDisp = new SalesReceiveDisp(SelectedDate, InputStockHouse, connectionstring);
            salesReceiveDisp.FormClosed += (s, args) => { this.Enabled = true; };
            salesReceiveDisp.ShowDialog();
        }

        private void StockHouse_TextChanged(object sender, EventArgs e)
        {
            InputStockHouse = StockHouse.SelectedItem.ToString();
        }

        private void Add_Controls()
        {
            // フォーム呼び出し時の設定
            this.Size = new Size(300, 125);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // 入荷予定ラベルの設定
            LabelReceiveDate = new Label
            {
                Text = "入荷予定日：",
                TextAlign = ContentAlignment.MiddleRight,
                Width = 80,
                Location = new Point(10, 5)
            };
            this.Controls.Add(LabelReceiveDate);

            // 入荷倉庫ラベルの設定
            LabelStockHouse = new Label
            {
                Text = "入荷倉庫：",
                TextAlign = ContentAlignment.MiddleRight,
                Width = 80,
                Location = new Point(10, 30),
            };
            this.Controls.Add(LabelStockHouse);


            // 入荷予定日の入力欄
            ReceiveDate = new DateTimePicker
            {
                Width = 170,
                Location = new Point(92, 5),
                Value = SelectedDate,
                MinDate = DateTime.Today.AddYears(-5),
                MaxDate = DateTime.Today.AddYears(1)
        };
            this.Controls.Add(ReceiveDate);

            // 入荷倉庫の入力欄
            Get_StockHouse _stockHouse = new Get_StockHouse();
            // 倉庫コード（100～110）、倉庫名を取得
            var StockHouseList = _stockHouse.Get_StockHousetable(connectionstring);
            StockHouse = new ComboBox
            {
                Width = 170,
                Location = new Point(92, 30),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            // 取得した倉庫を入荷倉庫のコンボボックスに追加
            for (int i = 0; i < StockHouseList.Rows.Count; i++)
                StockHouse.Items.Add($"{StockHouseList.Rows[i]["StockHouseCode"]}：{StockHouseList.Rows[i]["StockHouseName"]}");
            StockHouse.SelectedIndexChanged += StockHouse_TextChanged;
            StockHouse.SelectedItem = InputStockHouse;
            this.Controls.Add(StockHouse);


            // 決定ボタンの設定
            SalesReceive = new Button
            {
                Text = "決定",
                Size = new Size(100, 20),
                Location = new Point(160, 55),
            };
            SalesReceive.Click += Button_Click;
            this.Controls.Add(SalesReceive);
        }
    }
}
