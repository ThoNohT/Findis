/********************************************************************************
 Copyright (C) 2015 Eric Bataille <e.c.p.bataille@gmail.com>

 This program is free software; you can redistribute it and/or modify
 it under the terms of the GNU General Public License as published by
 the Free Software Foundation; either version 2 of the License, or
 (at your option) any later version.

 This program is distributed in the hope that it will be useful,
 but WITHOUT ANY WARRANTY; without even the implied warranty of
 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 GNU General Public License for more details.

 You should have received a copy of the GNU General Public License
 along with this program; if not, write to the Free Software
 Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307, USA.
********************************************************************************/


using System.Windows.Forms;

namespace Findis.Proto
{
    public partial class CurrencyForm : Form
    {
        public string CurrencyName { get; set; }

        public decimal ExchangeRate { get; set; }

        public bool Success { get; set; }

        public CurrencyForm()
        {
            InitializeComponent();
        }

        public void SetInfoText(string text)
        {
            lblInfo.Text = text;
        }

        public void SetValues(string currencyName, decimal exchangeRate)
        {
            txtCurrencyName.Text = currencyName;
            udExchangeRate.Value = exchangeRate;
        }

        private void btnCancel_Click(object sender, System.EventArgs e)
        {
            Success = false;
            Close();
        }

        private void btnOk_Click(object sender, System.EventArgs e)
        {
            CurrencyName = txtCurrencyName.Text;
            ExchangeRate = udExchangeRate.Value;

            if (ExchangeRate <= 0)
            {
                MessageBox.Show("Exchange rate has to be higher than 0.");
                udExchangeRate.Focus();
                return;
            }

            if (CurrencyName.Length < 1 || CurrencyName.Length > 10 || string.IsNullOrWhiteSpace(CurrencyName))
            {
                MessageBox.Show("Name has to be at least 1 and at most 10 characters long.");
                txtCurrencyName.Focus();
                return;
            }
            
            Success = true;
            Close();
        }
    }
}
