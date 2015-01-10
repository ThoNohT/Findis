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
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Findis.Business;
using Findis.Common;

namespace Findis.Proto
{
    public partial class MainForm : Form
    {
        private List<TransactionSummary> transactionSummaries = new List<TransactionSummary>();
        private List<TransactionDetail> transactionDetails = new List<TransactionDetail>();

        public MainForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            split1.Enabled = false;
            mnuCalculateTotals.Enabled = false;
            UpdateEventList();
            AlignEventGrid();
            AlignTransactionGrid();
        }

        private void UpdateEventList()
        {
            cmbEvent.Items.Clear();
            
            // Load the events.
            var events = new EventManager().GetAllEvents();
            foreach (var @event in events)
                cmbEvent.Items.Add(KeyDisplayPair.From(@event.Id, @event.Name));
        }

        

        private void mnuNewEvent_Click(object sender, EventArgs e)
        {
            var newName = Microsoft.VisualBasic.Interaction.InputBox("Please enter a name for the event.", "New event");

            if (string.IsNullOrWhiteSpace(newName))
                return;

            var @event = new EventManager().CreateEvent(newName, new List<int>());

            UpdateEventList();
            LoadEvent(@event.Id);
        }

        private void cmbEvent_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbEvent.SelectedIndex > -1)
            {
                var selected = (KeyDisplayPair<int, string>)cmbEvent.SelectedItem;
                LoadEvent(selected.Key);
            }
            else
            {
                LoadEvent(-1);
            }
        }

        private void LoadEvent(int id)
        {
            if (id == -1)
            {
                mnuCalculateTotals.Enabled = false;
                split1.Enabled = false;
                cmbEvent.SelectedIndex = -1;
                return;
            }

            var eventRowIndex = eventGrid.CurrentCell != null ? (int?)eventGrid.CurrentCell.RowIndex : null;
            var transactionRowIndex = transactionGrid.CurrentCell != null
                ? (int?) transactionGrid.CurrentCell.RowIndex : null;

            var @event = new EventManager().GetEvent(id);
            cmbEvent.SelectedIndex = cmbEvent.Items.IndexOf(KeyDisplayPair.From(@event.Id, @event.Name));
            split1.Enabled = true;
            mnuCalculateTotals.Enabled = true;


            txtEventName.Text = @event.Name;
            lstEventParticipants.Items.Clear();
            foreach (var person in @event.Participants)
                lstEventParticipants.Items.Add(KeyDisplayPair.From(person.Id, person.Name));

            LoadEventOverview(id);
            LoadEventTransactions(id);

            if (eventRowIndex != null && eventGrid.Rows.Count > eventRowIndex)
            {
                eventGrid.CurrentCell = eventGrid.Rows[(int)eventRowIndex].Cells[1];
                eventGrid_RowEnter(null, new DataGridViewCellEventArgs(0, (int)eventRowIndex));
            }

            if (transactionRowIndex != null && transactionGrid.Rows.Count > transactionRowIndex)
            {
                transactionGrid.CurrentCell = transactionGrid.Rows[(int)transactionRowIndex].Cells[2];
            }

        }

        private void LoadEventTransactions(int id)
        {
            if (id == -1)
                return;

            var i = 0;
            var transactions = new TransactionManager().GetTransactionsForEvent(id).Reverse();
            transactionSummaries = transactions.Select(x =>
                new TransactionSummary {
                    Id = x.Id,
                    Counter = ++i,
                    Description = x.Description,
                    DateTime = x.DateTime.ToString(CultureInfo.InvariantCulture),
                    Contributors = string.Join(", ", x.Contributors.Select(y => y.Name)),
                    ExtraParticipants = string.Join(",", x.ExtraParticipants.Select(e => e.Name)),
                    ExcludedParticipants = string.Join(",", x.ExcludedParticipants.Select(e => e.Name)),
                    TotalVolume = x.TotalVolume.ToString("F2")
                }).ToList();

            AlignEventGrid();
        }

        private void AlignEventGrid()
        {
            eventGrid.DataSource = transactionSummaries;

            var width = eventGrid.Width;

            eventGrid.Columns[0].Visible = false; // Id
            eventGrid.Columns[1].Width = 50; // Counter
            eventGrid.Columns[2].Width = 125; // DateTime
            eventGrid.Columns[3].Width = Math.Max(150, width - 725); // Description
            eventGrid.Columns[4].Width = 150; // Contributors
            eventGrid.Columns[5].Width = 150; // Extra Participants
            eventGrid.Columns[5].Width = 150; // Excluded Participants
            eventGrid.Columns[6].Width = 100; // TotalVolume
        }

        private void AlignTransactionGrid()
        {
            transactionGrid.DataSource = transactionDetails;

            var width = Math.Max(100, transactionGrid.Width / 4);

            transactionGrid.Columns[0].Visible = false; // PersonId
            transactionGrid.Columns[1].Visible = false; // ContributionId
            transactionGrid.Columns[2].Width = width; // Name
            transactionGrid.Columns[3].Width = width; // Currency

            transactionGrid.Columns[4].Width = width; // Amount
            transactionGrid.Columns[5].Width = width; // BaseAmount
        }

        private void LoadEventOverview(int id)
        {
            if (id == -1) return;

            var eventManager = new EventManager();
            var @event = eventManager.GetEvent(id);
            
            var allPeople = new PersonManager().GetAllPersons();
            var assignedPeople = @event.Participants;
            var missingPeople = allPeople.Where(x => !assignedPeople.Select(y => y.Id).Contains(x.Id));

            lstEventParticipants.Items.Clear();
            foreach (var person in assignedPeople)
                lstEventParticipants.Items.Add(KeyDisplayPair.From(person.Id, person.Name));

            cmbAddParticipant.Text = string.Empty;
            cmbAddParticipant.Items.Clear();
            foreach (var person in missingPeople)
                cmbAddParticipant.Items.Add(KeyDisplayPair.From(person.Id, person.Name));

            var currencies = eventManager.GetCurrenciesForEvent(id).Select(x => new CurrencyDisplay(x));
            lstEventCurrencies.Items.Clear();
            foreach (var currency in currencies)
                lstEventCurrencies.Items.Add(currency);
        }

        private void mnuDeleteEvent_Click(object sender, EventArgs e)
        {
            if (cmbEvent.SelectedIndex <= -1) return;

            var selected = (KeyDisplayPair<int, string>)cmbEvent.SelectedItem;
            if (!Protect(() => new EventManager().DeleteEvent(selected.Key)))
                return;
        
            LoadEvent(-1);
            UpdateEventList();
        }

        private void txtEventName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Return) return;
            if (cmbEvent.SelectedIndex == -1) return;

            var selected = (KeyDisplayPair<int, string>)cmbEvent.SelectedItem;
            var selectedIndex = cmbEvent.SelectedIndex;

            if (string.IsNullOrWhiteSpace(txtEventName.Text))
            {
                txtEventName.Text = selected.Value;
                return;
            }
            var eventManager = new EventManager();
            var @event = eventManager.GetEvent(selected.Key);
            if (!Protect(() => eventManager.EditEvent(selected.Key, txtEventName.Text,
                    @event.Participants.Select(x => x.Id).ToList())))
                return;
            
            UpdateEventList();
            cmbEvent.SelectedIndex = selectedIndex;
        }

        private void mnuManagePersons_Click(object sender, EventArgs e)
        {
            var peopleForm = new PeopleForm();
            peopleForm.ShowDialog(this);
        }

        private void btnDeletePerson_Click(object sender, EventArgs e)
        {
            if (lstEventParticipants.SelectedIndex == -1) return;

            var selectedPersonId = (KeyDisplayPair<int, string>)lstEventParticipants.SelectedItem;
            var selectedEventId = (KeyDisplayPair<int, string>)cmbEvent.SelectedItem;

            var eventManager = new EventManager();
            var selectedEvent = eventManager.GetEvent(selectedEventId.Key);

            if (!Protect(() => eventManager.EditEvent(selectedEvent.Id, selectedEvent.Name,
                selectedEvent.Participants.Where(x => x.Id != selectedPersonId.Key).Select(x => x.Id).ToList())))
                return;

            LoadEventOverview(selectedEventId.Key);
        }

        private void btnAddPerson_Click(object sender, EventArgs e)
        {
            if (cmbAddParticipant.SelectedIndex == -1) return;

            var selected = (KeyDisplayPair<int, string>)cmbAddParticipant.SelectedItem;
            var selectedEventId = (KeyDisplayPair<int, string>)cmbEvent.SelectedItem;

            var eventManager = new EventManager();

            var selectedEvent = eventManager.GetEvent(selectedEventId.Key);
            if (!Protect(() => eventManager.EditEvent(selectedEvent.Id, selectedEvent.Name,
                selectedEvent.Participants.Select(x => x.Id).Union(selected.Key.Enumerate()).ToList())))
                return;

            LoadEventOverview(selectedEventId.Key);
        }

        private void eventGrid_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            var transactionId = (int)eventGrid.Rows[e.RowIndex].Cells[0].Value;

            var transaction = new TransactionManager().GetParticipationsForTransaction(transactionId);

            transactionDetails = transaction.Select(participation =>
                new TransactionDetail {
                    PersonId = participation.Person.Id,
                    Name = participation.Person.Name,
                    ContributionId = participation.ContributionId,
                    Currency = participation.Currency.ValueOrElse(x => x.Name, ""),
                    Amount = participation.Amount.ValueOrElse(x => string.Format("{0:F2}", x), ""),
                    BaseAmount = participation.Amount.ValueOrElse(
                        x => string.Format("{0:F2}", x * participation.Currency.Value.ExchangeRate), ""),
                }).ToList();

            AlignTransactionGrid();
        }

        private void btnSetBaseCurrency_Click(object sender, EventArgs e)
        {
            if (lstEventCurrencies.SelectedIndex == -1) return;

            var selectedEventId = (KeyDisplayPair<int, string>)cmbEvent.SelectedItem;
            var selectedCurrencyId = (CurrencyDisplay)lstEventCurrencies.SelectedItem;

            if (!Protect(() => new EventManager().SetBaseCurrency(selectedCurrencyId.Id)))
                return;

            LoadEventOverview(selectedEventId.Key);
        }

        private void btnAddCurrency_Click(object sender, EventArgs e)
        {
            var currencyForm = new CurrencyForm();
            currencyForm.SetInfoText("Please enter the details for a new currency.");
            currencyForm.ShowDialog();

            if (!currencyForm.Success) return;

            var selectedEventId = (KeyDisplayPair<int, string>)cmbEvent.SelectedItem;
            if (!Protect(() => new EventManager().CreateCurrency(selectedEventId.Key, currencyForm.CurrencyName,
                currencyForm.ExchangeRate)))
                return;

            LoadEventOverview(selectedEventId.Key);
        }

        private void btnEditCurrency_Click(object sender, EventArgs e)
        {
            if (lstEventCurrencies.SelectedIndex == -1) return;
            var selectedCurrencyId = (CurrencyDisplay)lstEventCurrencies.SelectedItem;

            var currencyForm = new CurrencyForm();
            currencyForm.SetInfoText(
                string.Format("Please enter the new details for the currency {0}.", selectedCurrencyId.Name));
            currencyForm.SetValues(selectedCurrencyId.Name, selectedCurrencyId.ExchangeRate);
            currencyForm.ShowDialog();

            if (!currencyForm.Success) return;

            var selectedEventId = (KeyDisplayPair<int, string>)cmbEvent.SelectedItem;
            if (!Protect(() => new EventManager().EditCurrency(selectedCurrencyId.Id, currencyForm.CurrencyName,
                currencyForm.ExchangeRate)))
                return;

            LoadEventOverview(selectedEventId.Key);
        }

        private void btnDeleteCurrency_Click(object sender, EventArgs e)
        {
            if (lstEventCurrencies.SelectedIndex == -1) return;

            var selectedEventId = (KeyDisplayPair<int, string>)cmbEvent.SelectedItem;
            var selectedCurrencyId = (CurrencyDisplay)lstEventCurrencies.SelectedItem;

            if (!Protect(() => new EventManager().DeleteCurrency(selectedCurrencyId.Id)))
                return;

            LoadEventOverview(selectedEventId.Key);
        }

        private void btnAddTransaction_Click(object sender, EventArgs e)
        {
            var selectedEventId = (KeyDisplayPair<int, string>)cmbEvent.SelectedItem;

            var eventManager = new EventManager();
            var currencies = eventManager.GetCurrenciesForEvent(selectedEventId.Key);
            
            var hasCurrency = currencies.Any();

            if (!hasCurrency)
            {
                MessageBox.Show("Please define a currency for the event before creating a transaction.",
                    "Add Transaction");
                return;
            }

            var @event = eventManager.GetEvent(selectedEventId.Key);
            var persons = new PersonManager().GetAllPersons();
            
            var invitablePersons =
                persons.Except(@event.Participants).Select(x => KeyDisplayPair.From(x.Id, x.Name)).ToList();

            var eventParticipants = @event.Participants.Select(x => KeyDisplayPair.From(x.Id, x.Name)).ToList();

            var transactionForm = new TransactionForm();
            transactionForm.SetInfoText("Please enter the details for the new transaction.");
            transactionForm.SetValues("", DateTime.Now, invitablePersons.Except(eventParticipants).ToList(),
                eventParticipants);
            transactionForm.ShowDialog();

            if (!transactionForm.Success) return;

            if (!Protect(() =>
                new TransactionManager().CreateTransaction(@event.Id, transactionForm.Description,
                    transactionForm.DateTime,
                    transactionForm.Participants.Except(eventParticipants).Select(x => x.Key).ToList(),
                    eventParticipants.Except(transactionForm.Participants).Select(x => x.Key).ToList())))
                return;

            LoadEvent(selectedEventId.Key);
        }

        private void btnEditTransaction_Click(object sender, EventArgs e)
        {
            var selectedEventId = (KeyDisplayPair<int, string>)cmbEvent.SelectedItem;
            if (eventGrid.CurrentCell == null) return;
            var rowIndex = eventGrid.CurrentCell.RowIndex;
            var transactionId = (int)eventGrid.Rows[rowIndex].Cells[0].Value;

            var transactionManager = new TransactionManager();
            var transaction = transactionManager.GetTransaction(transactionId);
            var @event = new EventManager().GetEvent(selectedEventId.Key);
            
            var allPersons =
                new PersonManager().GetAllPersons().Select(x => KeyDisplayPair.From(x.Id, x.Name)).ToList();
            var eventPersons = @event.Participants.Select(x => KeyDisplayPair.From(x.Id, x.Name)).ToList();
            var invitablePersons = allPersons.Except(eventPersons).ToList();
            var extraParticipants =
                transaction.ExtraParticipants.Select(x => KeyDisplayPair.From(x.Id, x.Name)).ToList();
            var excludedParticipants =
                transaction.ExcludedParticipants.Select(x => KeyDisplayPair.From(x.Id, x.Name)).ToList();

            var transactionForm = new TransactionForm();
            transactionForm.SetInfoText("Please enter the new details for the transaction.");
            transactionForm.SetValues(transaction.Description, transaction.DateTime,
                invitablePersons.Except(extraParticipants).Union(excludedParticipants).ToList(),
                eventPersons.Except(excludedParticipants).Union(extraParticipants).ToList());
            transactionForm.ShowDialog();

            if (!transactionForm.Success) return;

            transaction.Description = transactionForm.Description;
            transaction.DateTime = transactionForm.DateTime;

            var extra = transactionForm.Participants.Except(eventPersons).Select(x => x.Key).ToList();
            var excluded = eventPersons.Except(transactionForm.Participants).Select(x => x.Key).ToList();

            if (!Protect(() =>
                transactionManager.EditTransaction(transaction.Id, transactionForm.Description,
                    transactionForm.DateTime, extra, excluded)))
                return;
            
            LoadEvent(selectedEventId.Key);
        }

        private void btnDeleteTransaction_Click(object sender, EventArgs e)
        {
            var selectedEventId = (KeyDisplayPair<int, string>)cmbEvent.SelectedItem;
            if (eventGrid.CurrentCell == null) return;
            var rowIndex = eventGrid.CurrentCell.RowIndex;
            var transactionId = (int)eventGrid.Rows[rowIndex].Cells[0].Value;

            var transactionManager = new TransactionManager();
            var transaction = transactionManager.GetTransaction(transactionId);
            if (transaction.Contributors.Any()
                && MessageBox.Show(string.Format("Are you sure you want to delete the transaction {0}? " +
                                                 "This will also remove its contributions.",
                    transaction.Description), "Delete transaction", MessageBoxButtons.YesNo) != DialogResult.Yes)
                return;

            if (!Protect(() => transactionManager.DeleteTransaction(transactionId)))
                return;

            LoadEvent(selectedEventId.Key);
        }

        private void btnEditContribution_Click(object sender, EventArgs e)
        {
            var selectedEventId = (KeyDisplayPair<int, string>)cmbEvent.SelectedItem;
            if (transactionGrid.CurrentCell == null) return;
            var transactionRowIndex = transactionGrid.CurrentCell.RowIndex;
            var detail = (TransactionDetail)transactionGrid.Rows[transactionRowIndex].DataBoundItem;

            var eventRowIndex = eventGrid.CurrentCell.RowIndex;
            var transactionId = (int)eventGrid.Rows[eventRowIndex].Cells[0].Value;

            var transactionManager = new TransactionManager();

            var participations = transactionManager.GetParticipationsForTransaction(transactionId);
            var currencies = new EventManager().GetCurrenciesForEvent(selectedEventId.Key)
                .Select(x => new ContributionForm.Currency(x)).ToList();

            var contributionForm = new ContributionForm();
                if (detail.ContributionId.IsNone)
                {
                    contributionForm.SetValues(0, currencies, currencies.Single(c => c.IsBase).Id);
                }
                else
                {
                    var contribution =
                        participations.Single(x => x.ContributionId.IsSome && x.ContributionId == detail.ContributionId);
                    contributionForm.SetValues(contribution.Amount.Value, currencies, contribution.Currency.Value.Id);
                }

            var transaction = transactionManager.GetTransaction(transactionId);
                contributionForm.SetInfoText(string.Format(
                    "Please set the details for the contribution of {0} to {1}.",
                    detail.Name, transaction.Description));
                contributionForm.ShowDialog();

            if (!contributionForm.Success) return;

            if (!Protect(() =>
                transactionManager.SetContribution(transactionId, detail.PersonId, contributionForm.Amount,
                    contributionForm.SelectedCurrency)))
                return;
            
            LoadEvent(selectedEventId.Key);
        }

        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            AlignEventGrid();
            AlignTransactionGrid();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var selectedEventId = (KeyDisplayPair<int, string>)cmbEvent.SelectedItem;
            if (transactionGrid.CurrentCell == null) return;
            var transactionRowIndex = transactionGrid.CurrentCell.RowIndex;
            var detail = (TransactionDetail)transactionGrid.Rows[transactionRowIndex].DataBoundItem;

            var eventRowIndex = eventGrid.CurrentCell.RowIndex;
            var transactionId = (int)eventGrid.Rows[eventRowIndex].Cells[0].Value;

            if (!Protect(() => new TransactionManager().DeleteContribution(transactionId, detail.PersonId)))
                return;

            LoadEvent(selectedEventId.Key);
        }

        private void mnuCalculateTotals_Click(object sender, EventArgs e)
        {
            var selectedEventId = (KeyDisplayPair<int, string>)cmbEvent.SelectedItem;

            var totalsForm = new TotalsForm(selectedEventId.Key);
            totalsForm.Show();
        }

        private static bool Protect(Action action)
        {
            try
            {
                action.Invoke();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error.");
                return false;
            }
        }
    }
}
