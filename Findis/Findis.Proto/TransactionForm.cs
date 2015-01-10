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
    public partial class TransactionForm : Form
    {
        public string Description { get; set; }

        public DateTime DateTime { get; set; }

        public bool Success { get; set; }

        public List<KeyDisplayPair<int, string>> InvitablePersons { get; set; }

        public List<KeyDisplayPair<int, string>> Participants { get; set; }

        public TransactionForm()
        {
            InitializeComponent();
        }

        public void SetInfoText(string text)
        {
            lblInfo.Text = text;
        }

        public void SetValues(string description, DateTime dateTime,
            IEnumerable<KeyDisplayPair<int, string>> invitablePersons,
            IEnumerable<KeyDisplayPair<int, string>> participants)
        {
            txtDescription.Text = description;
            dateTimePicker.Value = dateTime;

            lstPersons.Items.Clear();
            foreach (var person in invitablePersons)
                lstPersons.Items.Add(person);

            if (lstPersons.Items.Count > 0)
                lstPersons.SelectedIndex = 0;
            
            lstParticipants.Items.Clear();
            foreach (var person in participants)
                lstParticipants.Items.Add(person);

            if (lstParticipants.Items.Count > 0)
                lstParticipants.SelectedIndex = 0;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Success = false;
            Close();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            Description = txtDescription.Text;
            DateTime = dateTimePicker.Value;

            InvitablePersons = new List<KeyDisplayPair<int, string>>();
            foreach (var person in lstPersons.Items)
                InvitablePersons.Add((KeyDisplayPair<int, string>) person);

            Participants = new List<KeyDisplayPair<int, string>>();
            foreach (var person in lstParticipants.Items)
                Participants.Add((KeyDisplayPair<int, string>) person);

            if (Description.Length < 1 || Description.Length > 255 || string.IsNullOrWhiteSpace(Description))
            {
                MessageBox.Show("Description has to be at least 1 and at most 255 characters long.");
                txtDescription.Focus();
                return;
            }
            
            Success = true;
            Close();
        }

        private void btnInclude_Click(object sender, EventArgs e)
        {
            if (lstPersons.SelectedIndex == -1) return;

            var person = lstPersons.SelectedItem;

            lstPersons.Items.Remove(person);
            lstParticipants.Items.Add(person);
            lstParticipants.SelectedItem = person;
        }

        private void btnExclude_Click(object sender, EventArgs e)
        {
            if (lstParticipants.SelectedIndex == -1) return;

            var person = lstParticipants.SelectedItem;

            lstParticipants.Items.Remove(person);
            lstPersons.Items.Add(person);
            lstPersons.SelectedItem = person;
        }
    }
}
