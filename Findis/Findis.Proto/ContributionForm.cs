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


using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Findis.Proto
{
    public partial class ContributionForm : Form
    {
        public class Currency
        {
            public Currency(Business.Dto.Currency currency)
            {
                Id = currency.Id;
                Name = currency.Name;
                ExchangeRate = currency.ExchangeRate;
                IsBase = currency.IsBaseCurrency;
            }

            public int Id { get; set; }

            public string Name { get; set; }

            public decimal ExchangeRate { get; set; }

            public bool IsBase { get; set; }

            public override string ToString()
            {
                return Name;
            }
        }

        public decimal Amount { get; set; }

        public int SelectedCurrency { get; set; }

        public bool Success { get; set; }

        private Currency baseCurrency;

        public ContributionForm()
        {
            InitializeComponent();
        }

        public void SetInfoText(string text)
        {
            lblInfo.Text = text;
        }

        public void SetValues(decimal amount, IEnumerable<Currency> currencies, int selectedCurrency)
        {
            cmbCurrency.Items.Clear();
            cmbCurrency.SelectedIndexChanged -= cmbCurrency_SelectedIndexChanged;
            foreach (var currency in currencies)
            {
                cmbCurrency.Items.Add(currency);
                if (selectedCurrency == currency.Id)
                    cmbCurrency.SelectedItem = currency;

                if (currency.IsBase) baseCurrency = currency;
            }
            cmbCurrency.SelectedIndexChanged += cmbCurrency_SelectedIndexChanged;
            SelectedCurrency = selectedCurrency;
            udAmount.Value = amount;
        }

        private void cmbCurrency_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedCurrency = (Currency) cmbCurrency.SelectedItem;
            SelectedCurrency = selectedCurrency.Id;

            lblBaseAmount.Text = string.Format("Amount in {0}: {1}", baseCurrency.Name,
                udAmount.Value * selectedCurrency.ExchangeRate);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Success = false;
            Close();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            Amount = udAmount.Value;
            Success = true;
            Close();
        }

        private void udAmount_ValueChanged(object sender, EventArgs e)
        {
            var selectedCurrency = (Currency) cmbCurrency.SelectedItem;
            lblBaseAmount.Text = string.Format("Amount in {0}: {1}", baseCurrency.Name,
                udAmount.Value * selectedCurrency.ExchangeRate);
        }
    }
}
