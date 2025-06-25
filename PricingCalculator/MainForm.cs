using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace PricingCalculator
{
    public partial class MainForm : Form
    {
        private PricingEngine pricingEngine = new PricingEngine();
        private HistoryManager historyManager = new HistoryManager();

        private TextBox txtProductName;
        private TextBox txtCost;
        private TextBox txtPackaging;
        private TextBox txtDelivery;
        private TextBox txtOverhead;
        private TextBox txtMargin;
        private TextBox txtCompetitorPrice;
        private TextBox txtStock;

        private Label lblResult;
        private DataGridView dgvHistory;
        private ToolTip toolTip = new ToolTip();

        public MainForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Расчёт цены на товар";
            this.Width = 800;
            this.Height = 600;

            int startX = 20;
            int startY = 20;

            SetupTooltips();

            // === Поля ввода с подсказками ? ===
            AddInputWithInfo("Название товара:", ref txtProductName, startX, ref startY, "Наименование товара");
            AddInputWithInfo("Себестоимость:", ref txtCost, startX, ref startY, "Затраты на производство или закупку");
            AddInputWithInfo("Упаковка:", ref txtPackaging, startX, ref startY, "Стоимость упаковки");
            AddInputWithInfo("Доставка:", ref txtDelivery, startX, ref startY, "Стоимость доставки до клиента");
            AddInputWithInfo("Косвенные (%):", ref txtOverhead, startX, ref startY, "Процент косвенных расходов (аренда, зарплаты и т.д.)");
            AddInputWithInfo("Желаемая маржа (%):", ref txtMargin, startX, ref startY, "Целевая маржа на товар");
            AddInputWithInfo("Цена конкурента:", ref txtCompetitorPrice, startX, ref startY, "Цена аналогичного товара у конкурента");
            AddInputWithInfo("Остаток на складе:", ref txtStock, startX, ref startY, "Количество единиц товара на складе");

            // === Кнопки ===
            startY += 40;
            Button btnCalculate = new Button()
            {
                Text = "Рассчитать цену",
                Location = new System.Drawing.Point(startX, startY),
                Width = 150
            };
            btnCalculate.Click += BtnCalculate_Click;

            Button btnClear = new Button()
            {
                Text = "Очистить",
                Location = new System.Drawing.Point(startX + 160, startY),
                Width = 100
            };
            btnClear.Click += BtnClear_Click;

            // === Результат ===
            startY += 40;
            lblResult = new Label()
            {
                Location = new System.Drawing.Point(startX, startY),
                Width = 600,
                Height = 60,
                ForeColor = System.Drawing.Color.Blue
            };
            Controls.Add(lblResult);

            // === История расчётов ===
            startY += 100;
            Label lblHistory = new Label() { Text = "История расчётов", Location = new System.Drawing.Point(startX, startY) };
            Controls.Add(lblHistory);

            startY += 20;
            dgvHistory = new DataGridView()
            {
                Location = new System.Drawing.Point(startX, startY),
                Width = 740,
                Height = 200,
                AutoGenerateColumns = true
            };

            UpdateHistoryGrid();

            // === Добавление элементов ===
            Controls.Add(btnCalculate);
            Controls.Add(btnClear);
            Controls.Add(dgvHistory);
        }

        private void SetupTooltips()
        {
            toolTip.ToolTipIcon = ToolTipIcon.Info;
            toolTip.IsBalloon = true;
            toolTip.UseFading = true;
            toolTip.AutoPopDelay = 5000;
        }

        private void AddInputWithInfo(string labelText, ref TextBox textBox, int x, ref int y, string info)
        {
            Label label = new Label()
            {
                Text = labelText,
                Location = new System.Drawing.Point(x, y)
            };
            Controls.Add(label);

            textBox = new TextBox()
            {
                Location = new System.Drawing.Point(x + 130, y),
                Width = 150
            };
            Controls.Add(textBox);

            Label infoLabel = new Label()
            {
                Text = "?",
                ForeColor = System.Drawing.Color.Blue,
                Cursor = Cursors.Hand,
                Location = new System.Drawing.Point(x + 290, y)
            };
            toolTip.SetToolTip(infoLabel, info);
            Controls.Add(infoLabel);

            y += 40;
        }

        private void UpdateHistoryGrid()
        {
            dgvHistory.DataSource = null;

            var bindingList = new BindingSource();
            bindingList.DataSource = historyManager.GetHistory();

            dgvHistory.DataSource = bindingList;

            // Переименовываем столбцы на русский
            foreach (DataGridViewColumn column in dgvHistory.Columns)
            {
                switch (column.Name)
                {
                    case "Name":
                        column.HeaderText = "Название";
                        break;
                    case "Cost":
                        column.HeaderText = "Себестоимость";
                        break;
                    case "Packaging":
                        column.HeaderText = "Упаковка";
                        break;
                    case "Delivery":
                        column.HeaderText = "Доставка";
                        break;
                    case "OverheadPercentage":
                        column.HeaderText = "Косвенные (%)";
                        break;
                    case "Margin":
                        column.HeaderText = "Маржа (%)";
                        break;
                    case "CompetitorPrice":
                        column.HeaderText = "Цена конкурента";
                        break;
                    case "FinalPrice":
                        column.HeaderText = "Финальная цена";
                        break;
                    case "Stock":
                        column.HeaderText = "Остаток на складе";
                        break;
                    case "TotalValue":
                        column.HeaderText = "Стоимость остатков";
                        break;
                }
            }
        }

        private void BtnCalculate_Click(object sender, EventArgs e)
        {
            Product product = new Product();
            bool success = TryGetInputs(product);
            if (!success)
            {
                MessageBox.Show("Пожалуйста, введите корректные данные.");
                return;
            }

            double finalPrice = pricingEngine.CalculateFinalPrice(product);
            double competitorBasedPrice = pricingEngine.AdjustForMarket(product, finalPrice);

            // Расчёт общей стоимости
            if (!int.TryParse(txtStock.Text, out int stock)) stock = 0;
            double totalValue = finalPrice * stock;

            string resultText = $"Рассчитанная цена: {finalPrice:F2} ₽\n" +
                                $"Рекомендованная цена: {competitorBasedPrice:F2} ₽\n" +
                                $"Маржа: {product.Margin:F2}%\n" +
                                $"Общая стоимость остатков: {totalValue:F2} ₽";

            lblResult.Text = resultText;

            // Добавляем в историю
            historyManager.AddToHistory(new HistoryEntry
            {
                Name = product.Name,
                Cost = product.Cost,
                Packaging = product.Packaging,
                Delivery = product.Delivery,
                OverheadPercentage = product.OverheadPercentage,
                Margin = product.Margin,
                CompetitorPrice = product.CompetitorPrice,
                FinalPrice = competitorBasedPrice,
                Stock = stock,
                TotalValue = totalValue
            });
            UpdateHistoryGrid();
        }

        private bool TryGetInputs(Product product)
        {
            product.Name = txtProductName.Text.Trim();
            if (string.IsNullOrEmpty(product.Name)) return false;

            if (!double.TryParse(txtCost.Text, out double cost)) return false;
            if (!double.TryParse(txtPackaging.Text, out double packaging)) return false;
            if (!double.TryParse(txtDelivery.Text, out double delivery)) return false;
            if (!double.TryParse(txtOverhead.Text, out double overheadPercentage)) return false;
            if (!double.TryParse(txtMargin.Text, out double margin)) return false;
            if (!double.TryParse(txtCompetitorPrice.Text, out double competitorPrice)) return false;

            product.Cost = cost;
            product.Packaging = packaging;
            product.Delivery = delivery;
            product.OverheadPercentage = overheadPercentage;
            product.Margin = margin;
            product.CompetitorPrice = competitorPrice;

            return true;
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            txtProductName.Clear();
            txtCost.Clear();
            txtPackaging.Clear();
            txtDelivery.Clear();
            txtOverhead.Clear();
            txtMargin.Clear();
            txtCompetitorPrice.Clear();
            txtStock.Clear();
            lblResult.Text = string.Empty;
        }
    }
}