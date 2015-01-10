namespace Findis.Proto
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.mnuManage = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuManagePersons = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuNewEvent = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuDeleteEvent = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuCalculateTotals = new System.Windows.Forms.ToolStripMenuItem();
            this.cmbEvent = new System.Windows.Forms.ToolStripComboBox();
            this.split1 = new System.Windows.Forms.SplitContainer();
            this.grpContribution = new System.Windows.Forms.GroupBox();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnEditContribution = new System.Windows.Forms.Button();
            this.grpTransaction = new System.Windows.Forms.GroupBox();
            this.btnEditTransaction = new System.Windows.Forms.Button();
            this.btnDeleteTransaction = new System.Windows.Forms.Button();
            this.btnAddTransaction = new System.Windows.Forms.Button();
            this.btnEditCurrency = new System.Windows.Forms.Button();
            this.btnSetBaseCurrency = new System.Windows.Forms.Button();
            this.btnDeleteCurrency = new System.Windows.Forms.Button();
            this.lstEventCurrencies = new System.Windows.Forms.ListBox();
            this.btnAddCurrency = new System.Windows.Forms.Button();
            this.btnDeletePerson = new System.Windows.Forms.Button();
            this.btnAddPerson = new System.Windows.Forms.Button();
            this.cmbAddParticipant = new System.Windows.Forms.ComboBox();
            this.lstEventParticipants = new System.Windows.Forms.ListBox();
            this.txtEventName = new System.Windows.Forms.TextBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.eventGrid = new System.Windows.Forms.DataGridView();
            this.transactionGrid = new System.Windows.Forms.DataGridView();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.split1)).BeginInit();
            this.split1.Panel1.SuspendLayout();
            this.split1.Panel2.SuspendLayout();
            this.split1.SuspendLayout();
            this.grpContribution.SuspendLayout();
            this.grpTransaction.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.eventGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.transactionGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuManage,
            this.cmbEvent});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1258, 27);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // mnuManage
            // 
            this.mnuManage.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuManagePersons,
            this.toolStripSeparator1,
            this.mnuNewEvent,
            this.mnuDeleteEvent,
            this.toolStripSeparator2,
            this.mnuCalculateTotals});
            this.mnuManage.Name = "mnuManage";
            this.mnuManage.Size = new System.Drawing.Size(62, 23);
            this.mnuManage.Text = "Manage";
            // 
            // mnuManagePersons
            // 
            this.mnuManagePersons.Name = "mnuManagePersons";
            this.mnuManagePersons.Size = new System.Drawing.Size(158, 22);
            this.mnuManagePersons.Text = "People";
            this.mnuManagePersons.Click += new System.EventHandler(this.mnuManagePersons_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(155, 6);
            // 
            // mnuNewEvent
            // 
            this.mnuNewEvent.Name = "mnuNewEvent";
            this.mnuNewEvent.Size = new System.Drawing.Size(158, 22);
            this.mnuNewEvent.Text = "New Event";
            this.mnuNewEvent.Click += new System.EventHandler(this.mnuNewEvent_Click);
            // 
            // mnuDeleteEvent
            // 
            this.mnuDeleteEvent.Name = "mnuDeleteEvent";
            this.mnuDeleteEvent.Size = new System.Drawing.Size(158, 22);
            this.mnuDeleteEvent.Text = "Delete Event";
            this.mnuDeleteEvent.Click += new System.EventHandler(this.mnuDeleteEvent_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(155, 6);
            // 
            // mnuCalculateTotals
            // 
            this.mnuCalculateTotals.Enabled = false;
            this.mnuCalculateTotals.Name = "mnuCalculateTotals";
            this.mnuCalculateTotals.Size = new System.Drawing.Size(158, 22);
            this.mnuCalculateTotals.Text = "Calculate Totals";
            this.mnuCalculateTotals.Click += new System.EventHandler(this.mnuCalculateTotals_Click);
            // 
            // cmbEvent
            // 
            this.cmbEvent.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEvent.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbEvent.Name = "cmbEvent";
            this.cmbEvent.Size = new System.Drawing.Size(121, 23);
            this.cmbEvent.SelectedIndexChanged += new System.EventHandler(this.cmbEvent_SelectedIndexChanged);
            // 
            // split1
            // 
            this.split1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.split1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.split1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.split1.Location = new System.Drawing.Point(0, 27);
            this.split1.Name = "split1";
            // 
            // split1.Panel1
            // 
            this.split1.Panel1.Controls.Add(this.grpContribution);
            this.split1.Panel1.Controls.Add(this.grpTransaction);
            this.split1.Panel1.Controls.Add(this.btnEditCurrency);
            this.split1.Panel1.Controls.Add(this.btnSetBaseCurrency);
            this.split1.Panel1.Controls.Add(this.btnDeleteCurrency);
            this.split1.Panel1.Controls.Add(this.lstEventCurrencies);
            this.split1.Panel1.Controls.Add(this.btnAddCurrency);
            this.split1.Panel1.Controls.Add(this.btnDeletePerson);
            this.split1.Panel1.Controls.Add(this.btnAddPerson);
            this.split1.Panel1.Controls.Add(this.cmbAddParticipant);
            this.split1.Panel1.Controls.Add(this.lstEventParticipants);
            this.split1.Panel1.Controls.Add(this.txtEventName);
            // 
            // split1.Panel2
            // 
            this.split1.Panel2.Controls.Add(this.splitContainer1);
            this.split1.Size = new System.Drawing.Size(1258, 719);
            this.split1.SplitterDistance = 261;
            this.split1.TabIndex = 2;
            // 
            // grpContribution
            // 
            this.grpContribution.Controls.Add(this.btnDelete);
            this.grpContribution.Controls.Add(this.btnEditContribution);
            this.grpContribution.Location = new System.Drawing.Point(3, 455);
            this.grpContribution.Name = "grpContribution";
            this.grpContribution.Size = new System.Drawing.Size(253, 55);
            this.grpContribution.TabIndex = 12;
            this.grpContribution.TabStop = false;
            this.grpContribution.Text = "Contribution";
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(170, 19);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 23);
            this.btnDelete.TabIndex = 3;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnEditContribution
            // 
            this.btnEditContribution.Location = new System.Drawing.Point(89, 19);
            this.btnEditContribution.Name = "btnEditContribution";
            this.btnEditContribution.Size = new System.Drawing.Size(75, 23);
            this.btnEditContribution.TabIndex = 2;
            this.btnEditContribution.Text = "Edit";
            this.btnEditContribution.UseVisualStyleBackColor = true;
            this.btnEditContribution.Click += new System.EventHandler(this.btnEditContribution_Click);
            // 
            // grpTransaction
            // 
            this.grpTransaction.Controls.Add(this.btnEditTransaction);
            this.grpTransaction.Controls.Add(this.btnDeleteTransaction);
            this.grpTransaction.Controls.Add(this.btnAddTransaction);
            this.grpTransaction.Location = new System.Drawing.Point(3, 397);
            this.grpTransaction.Name = "grpTransaction";
            this.grpTransaction.Size = new System.Drawing.Size(253, 52);
            this.grpTransaction.TabIndex = 11;
            this.grpTransaction.TabStop = false;
            this.grpTransaction.Text = "Transaction";
            // 
            // btnEditTransaction
            // 
            this.btnEditTransaction.Location = new System.Drawing.Point(89, 20);
            this.btnEditTransaction.Name = "btnEditTransaction";
            this.btnEditTransaction.Size = new System.Drawing.Size(75, 23);
            this.btnEditTransaction.TabIndex = 2;
            this.btnEditTransaction.Text = "Edit";
            this.btnEditTransaction.UseVisualStyleBackColor = true;
            this.btnEditTransaction.Click += new System.EventHandler(this.btnEditTransaction_Click);
            // 
            // btnDeleteTransaction
            // 
            this.btnDeleteTransaction.Location = new System.Drawing.Point(170, 20);
            this.btnDeleteTransaction.Name = "btnDeleteTransaction";
            this.btnDeleteTransaction.Size = new System.Drawing.Size(75, 23);
            this.btnDeleteTransaction.TabIndex = 1;
            this.btnDeleteTransaction.Text = "Delete";
            this.btnDeleteTransaction.UseVisualStyleBackColor = true;
            this.btnDeleteTransaction.Click += new System.EventHandler(this.btnDeleteTransaction_Click);
            // 
            // btnAddTransaction
            // 
            this.btnAddTransaction.Location = new System.Drawing.Point(8, 20);
            this.btnAddTransaction.Name = "btnAddTransaction";
            this.btnAddTransaction.Size = new System.Drawing.Size(75, 23);
            this.btnAddTransaction.TabIndex = 0;
            this.btnAddTransaction.Text = "Add";
            this.btnAddTransaction.UseVisualStyleBackColor = true;
            this.btnAddTransaction.Click += new System.EventHandler(this.btnAddTransaction_Click);
            // 
            // btnEditCurrency
            // 
            this.btnEditCurrency.Location = new System.Drawing.Point(92, 339);
            this.btnEditCurrency.Name = "btnEditCurrency";
            this.btnEditCurrency.Size = new System.Drawing.Size(75, 23);
            this.btnEditCurrency.TabIndex = 10;
            this.btnEditCurrency.Text = "Edit";
            this.btnEditCurrency.UseVisualStyleBackColor = true;
            this.btnEditCurrency.Click += new System.EventHandler(this.btnEditCurrency_Click);
            // 
            // btnSetBaseCurrency
            // 
            this.btnSetBaseCurrency.Location = new System.Drawing.Point(11, 368);
            this.btnSetBaseCurrency.Name = "btnSetBaseCurrency";
            this.btnSetBaseCurrency.Size = new System.Drawing.Size(75, 23);
            this.btnSetBaseCurrency.TabIndex = 9;
            this.btnSetBaseCurrency.Text = "Set Base";
            this.btnSetBaseCurrency.UseVisualStyleBackColor = true;
            this.btnSetBaseCurrency.Click += new System.EventHandler(this.btnSetBaseCurrency_Click);
            // 
            // btnDeleteCurrency
            // 
            this.btnDeleteCurrency.Location = new System.Drawing.Point(173, 339);
            this.btnDeleteCurrency.Name = "btnDeleteCurrency";
            this.btnDeleteCurrency.Size = new System.Drawing.Size(75, 23);
            this.btnDeleteCurrency.TabIndex = 8;
            this.btnDeleteCurrency.Text = "Delete";
            this.btnDeleteCurrency.UseVisualStyleBackColor = true;
            this.btnDeleteCurrency.Click += new System.EventHandler(this.btnDeleteCurrency_Click);
            // 
            // lstEventCurrencies
            // 
            this.lstEventCurrencies.FormattingEnabled = true;
            this.lstEventCurrencies.Location = new System.Drawing.Point(11, 212);
            this.lstEventCurrencies.Name = "lstEventCurrencies";
            this.lstEventCurrencies.Size = new System.Drawing.Size(245, 121);
            this.lstEventCurrencies.TabIndex = 7;
            // 
            // btnAddCurrency
            // 
            this.btnAddCurrency.Location = new System.Drawing.Point(11, 339);
            this.btnAddCurrency.Name = "btnAddCurrency";
            this.btnAddCurrency.Size = new System.Drawing.Size(75, 23);
            this.btnAddCurrency.TabIndex = 6;
            this.btnAddCurrency.Text = "Add";
            this.btnAddCurrency.UseVisualStyleBackColor = true;
            this.btnAddCurrency.Click += new System.EventHandler(this.btnAddCurrency_Click);
            // 
            // btnDeletePerson
            // 
            this.btnDeletePerson.Location = new System.Drawing.Point(92, 183);
            this.btnDeletePerson.Name = "btnDeletePerson";
            this.btnDeletePerson.Size = new System.Drawing.Size(75, 23);
            this.btnDeletePerson.TabIndex = 4;
            this.btnDeletePerson.Text = "Delete";
            this.btnDeletePerson.UseVisualStyleBackColor = true;
            this.btnDeletePerson.Click += new System.EventHandler(this.btnDeletePerson_Click);
            // 
            // btnAddPerson
            // 
            this.btnAddPerson.Location = new System.Drawing.Point(11, 183);
            this.btnAddPerson.Name = "btnAddPerson";
            this.btnAddPerson.Size = new System.Drawing.Size(75, 23);
            this.btnAddPerson.TabIndex = 3;
            this.btnAddPerson.Text = "Add";
            this.btnAddPerson.UseVisualStyleBackColor = true;
            this.btnAddPerson.Click += new System.EventHandler(this.btnAddPerson_Click);
            // 
            // cmbAddParticipant
            // 
            this.cmbAddParticipant.FormattingEnabled = true;
            this.cmbAddParticipant.Location = new System.Drawing.Point(11, 156);
            this.cmbAddParticipant.Name = "cmbAddParticipant";
            this.cmbAddParticipant.Size = new System.Drawing.Size(245, 21);
            this.cmbAddParticipant.TabIndex = 2;
            // 
            // lstEventParticipants
            // 
            this.lstEventParticipants.FormattingEnabled = true;
            this.lstEventParticipants.Location = new System.Drawing.Point(11, 29);
            this.lstEventParticipants.Name = "lstEventParticipants";
            this.lstEventParticipants.Size = new System.Drawing.Size(245, 121);
            this.lstEventParticipants.TabIndex = 1;
            // 
            // txtEventName
            // 
            this.txtEventName.Location = new System.Drawing.Point(11, 3);
            this.txtEventName.Name = "txtEventName";
            this.txtEventName.Size = new System.Drawing.Size(245, 20);
            this.txtEventName.TabIndex = 0;
            this.txtEventName.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtEventName_KeyPress);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.eventGrid);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.transactionGrid);
            this.splitContainer1.Size = new System.Drawing.Size(991, 717);
            this.splitContainer1.SplitterDistance = 358;
            this.splitContainer1.TabIndex = 2;
            // 
            // eventGrid
            // 
            this.eventGrid.AllowUserToAddRows = false;
            this.eventGrid.AllowUserToDeleteRows = false;
            this.eventGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.eventGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.eventGrid.Location = new System.Drawing.Point(0, 0);
            this.eventGrid.Name = "eventGrid";
            this.eventGrid.ReadOnly = true;
            this.eventGrid.RowHeadersVisible = false;
            this.eventGrid.Size = new System.Drawing.Size(991, 358);
            this.eventGrid.TabIndex = 0;
            this.eventGrid.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.eventGrid_RowEnter);
            // 
            // transactionGrid
            // 
            this.transactionGrid.AllowUserToAddRows = false;
            this.transactionGrid.AllowUserToDeleteRows = false;
            this.transactionGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.transactionGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.transactionGrid.Location = new System.Drawing.Point(0, 0);
            this.transactionGrid.Name = "transactionGrid";
            this.transactionGrid.ReadOnly = true;
            this.transactionGrid.RowHeadersVisible = false;
            this.transactionGrid.Size = new System.Drawing.Size(991, 355);
            this.transactionGrid.TabIndex = 1;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1258, 746);
            this.Controls.Add(this.split1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "Findis - Finance Distributor";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.SizeChanged += new System.EventHandler(this.MainForm_SizeChanged);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.split1.Panel1.ResumeLayout(false);
            this.split1.Panel1.PerformLayout();
            this.split1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.split1)).EndInit();
            this.split1.ResumeLayout(false);
            this.grpContribution.ResumeLayout(false);
            this.grpTransaction.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.eventGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.transactionGrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem mnuManage;
        private System.Windows.Forms.ToolStripMenuItem mnuManagePersons;
        private System.Windows.Forms.ToolStripComboBox cmbEvent;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem mnuNewEvent;
        private System.Windows.Forms.ToolStripMenuItem mnuDeleteEvent;
        private System.Windows.Forms.SplitContainer split1;
        private System.Windows.Forms.TextBox txtEventName;
        private System.Windows.Forms.ListBox lstEventParticipants;
        private System.Windows.Forms.Button btnDeletePerson;
        private System.Windows.Forms.Button btnAddPerson;
        private System.Windows.Forms.ComboBox cmbAddParticipant;
        private System.Windows.Forms.DataGridView eventGrid;
        private System.Windows.Forms.DataGridView transactionGrid;
        private System.Windows.Forms.Button btnDeleteCurrency;
        private System.Windows.Forms.ListBox lstEventCurrencies;
        private System.Windows.Forms.Button btnAddCurrency;
        private System.Windows.Forms.Button btnSetBaseCurrency;
        private System.Windows.Forms.Button btnEditCurrency;
        private System.Windows.Forms.GroupBox grpContribution;
        private System.Windows.Forms.GroupBox grpTransaction;
        private System.Windows.Forms.Button btnDeleteTransaction;
        private System.Windows.Forms.Button btnAddTransaction;
        private System.Windows.Forms.Button btnEditContribution;
        private System.Windows.Forms.Button btnEditTransaction;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem mnuCalculateTotals;
    }
}

