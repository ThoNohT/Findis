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
using System.Linq;
using System.Windows.Forms;
using Findis.Business;

namespace Findis.Proto
{
    public partial class TotalsForm : Form
    {
        public TotalsForm(int eventId)
        {
            InitializeComponent();

            var reportManager = new ReportManager();

            var transactionOverviews = reportManager.GetTransactionOverviewsForEvent(eventId);
            var participantOverviews = reportManager.GetParticipantOverviewsForEvent(eventId);
            var @event = new EventManager().GetEvent(eventId);

            Text = string.Format("Totals for {0}", @event.Name);

            txtInfo.Clear();
            var totalAmount = transactionOverviews.Select(t => t.TotalAmount).DefaultIfEmpty(0).Sum();
            AddLine("Total amount: {0:F2}", totalAmount);

            AddLine("");
            foreach (var participant in participantOverviews)
            {
                AddLine("{0}: {1:F2} in {2} transactions. Totals: {3:F2}, average: {4:F2}.",
                    participant.PersonName, participant.TotalContributed, participant.ParticipationCount,
                    participant.TotalInParticipations, participant.AverageInParticipations);
            }

            AddLine("");
            var balances = new Dictionary<string, decimal>();
            foreach (var participant in participantOverviews)
            {
                var balance = participant.TotalContributed - participant.AverageInParticipations;
                balances.Add(participant.PersonName, balance);
                AddLine("{0}: {1:F2} out of {2:F2} = {3:F2} {4}.",
                    participant.PersonName, participant.TotalContributed, participant.AverageInParticipations,
                    Math.Abs(balance), balance > 0 ? "debit" : "credit");
            }

            AddLine("");
            var credits = balances.Where(x => x.Value < 0).Select(
                x => new KeyDisplayPair<string, decimal>(x.Key, Math.Abs(x.Value)))
                .OrderByDescending(x => x.Value).ToList();
            var debits = balances.Where(x => x.Value > 0).OrderByDescending(x => x.Value).ToList();
            // == 0 doesn't have to do anything.
            
            var totalCredits = credits.Select(x => x.Value).DefaultIfEmpty(0).Sum();
            var totalDebits = debits.Select(x => x.Value).DefaultIfEmpty(0).Sum();
            if (string.Format("{0:F2}", totalCredits) != string.Format("{0:F2}", totalDebits))
                AddLine("Credits ({0:F2}) different than debits ({1:F2})", totalCredits, totalDebits);
            
            while (credits.Any() && debits.Any())
            {
                var transaction = new Tuple<string, string, decimal>(credits[0].Key, debits[0].Key,
                    Math.Min(credits[0].Value, debits[0].Value));

                var result = PerformTransaction(transaction, credits, debits);
                credits = result.Item1;
                debits = result.Item2;
            }
        }

        public override sealed string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }

        private Tuple<List<KeyDisplayPair<string, decimal>>, List<KeyValuePair<string, decimal>>> PerformTransaction(
            Tuple<string, string, decimal> transaction, List<KeyDisplayPair<string, decimal>> credits,
            List<KeyValuePair<string, decimal>> debits)
        {
            AddLine("{0} pays {1} {2:F2}.", transaction.Item1, transaction.Item2, transaction.Item3);

            credits[0] = new KeyDisplayPair<string, decimal>(credits[0].Key, credits[0].Value - transaction.Item3);
            debits[0] = new KeyValuePair<string, decimal>(debits[0].Key, debits[0].Value - transaction.Item3);

            credits = credits.Where(x => x.Value > 0).OrderByDescending(x => x.Value).ToList();
            debits = debits.Where(x => x.Value > 0).OrderByDescending(x => x.Value).ToList();
            return new Tuple<List<KeyDisplayPair<string, decimal>>, List<KeyValuePair<string, decimal>>>(credits, debits);
        }

        private void AddLine(string text, params object[] args)
        {
            AddLine(string.Format(text, args));
        }

        private void AddLine(string text)
        {
            txtInfo.AppendText(text + Environment.NewLine);
        }
    }
}
