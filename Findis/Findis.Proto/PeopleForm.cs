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
using System.Linq;
using System.Windows.Forms;
using Findis.Business;

namespace Findis.Proto
{
    public partial class PeopleForm : Form
    {
        public PeopleForm()
        {
            InitializeComponent();
        }

        private void PeopleForm_Load(object sender, EventArgs e)
        {
            LoadPeople();
        }

        private void LoadPeople()
        {
            lstPeople.Items.Clear();

            var people = new PersonManager().GetAllPersons();
            foreach (var person in people)
            {
                lstPeople.Items.Add(new KeyDisplayPair<int, string>(person.Id, person.Name));
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text)
                || lstPeople.Items.Cast<KeyDisplayPair<int, String>>().Any(p => p.Value == txtName.Text))
            return;

            new PersonManager().CreatePerson(txtName.Text);

            LoadPeople();
        }

        private void lstPeople_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstPeople.SelectedIndex == -1) return;

            txtName.Text = ((KeyDisplayPair<int, string>) lstPeople.SelectedItem).Value;
        }

        private void txtName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char) Keys.Return) return;
            if (lstPeople.SelectedIndex == -1) return;

            var selected = (KeyDisplayPair<int, string>)lstPeople.SelectedItem;
            var selectedIndex = lstPeople.SelectedIndex;

            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                txtName.Text = selected.Value;
                return;
            }

            new PersonManager().EditPerson(selected.Key, txtName.Text);

            LoadPeople();
            lstPeople.SelectedIndex = selectedIndex;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (lstPeople.SelectedIndex == -1) return;
            var selected = (KeyDisplayPair<int, string>)lstPeople.SelectedItem;

            new PersonManager().DeletePerson(selected.Key);
        
            LoadPeople();
            txtName.Text = string.Empty;
        }
    }
}
