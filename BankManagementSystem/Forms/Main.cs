﻿using BankManagementSystem.Database;
using BankManagementSystem.Entity;
using BankManagementSystem.Forms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace BankManagementSystem
{
    public partial class Main : Form
    {
        private static string registrationImagePathFemale = @"G:\Coding\C#\BankManagementSystem\Images\female.png";
        private static string registrationImagePathMale = @"G:\Coding\C#\BankManagementSystem\Images\male.png";
        private static string registrationImagePath = null;
        private string currentUser;
        private Employee currentEmployee;
        private Client searchedClient;
        private Account searchedAccount;
        private Manager manager;

        public Main(int id, string m = null)
        {
            InitializeComponent();
            HideAllPanel();
            DisableComponents();
            HideDepositGroupBox();
            DashboardPanel.BringToFront();
            AccountPictureBox.Location = new Point(9, AccountButton.Location.Y + 10);
            ManagerPanelPictureBox.Location = new Point(9, ManagerButton.Location.Y + 10);
            ClockTimer.Start();
            if (m == null)
            {
                currentUser = FetchData.GetEmployeeName(id);
                currentEmployee = FetchData.GetEmployee(id);
                NameLabel.Text = currentUser;
                ManagerButton.Enabled = false;
                ManagerButton.BackColor = Color.Gray;
            }
            else
            {
                RegisterButton.Enabled = false;
                RegisterButton.BackColor = Color.Gray;
                AccountButton.Enabled = false;
                AccountButton.BackColor = Color.Gray;
                this.manager = FetchData.GetManager(id);
                NameLabel.Text = manager.Name;
            }
        }
        private void DashboardButton_Click(object sender, EventArgs e)
        {
            HideAllPanel();
            ResetAllButton();
            ResetChildButton();
            ResetPanel(RegisterPanel);
            ResetCreateAccountDetails();
            ResetResultGroupBox();
            HideDepositGroupBox();
            EnableManagerPanelButtons(null);
            DashboardPanel.BringToFront();
            DashboardButton.BackColor = Color.FromArgb(43, 63, 97);
            CurrentLabel.Text = "Dashboard";
        }

        #region Register

        private void RegisterButton_Click(object sender, EventArgs e)
        {
            HideAllPanel();
            ResetAllButton();
            ResetChildButton();
            HideAccountDetails();
            RegisterPanel.Show();
            RegisterButton.BackColor = Color.FromArgb(43, 63, 97);
            CurrentLabel.Text = "Register";
        }
        private void CreateButton_Click(object sender, EventArgs e)
        {
            if (CheckEmptyCreateAccountFields())
            {
                ValidateErrorLabel.Text = "No fields can remain empty!";
                return;
            }
            else if (!VerifyName(FirstnameTextbox.Text + LastnameTextbox.Text))
            {
                ValidateErrorLabel.Text = "Name cannot contain number!";
                return;
            }
            else if (!VerifyEmail(EmailTextbox.Text))
            {
                ValidateErrorLabel.Text = "Invalid email format!";
                return;
            }
            else
            {
                ValidateErrorLabel.Text = "";
                string accType = AccountTypeComboBox.Text;
                Client c = new Client();
                Account account = new Account();

                c.Firstname = FirstnameTextbox.Text;
                c.Lastname = LastnameTextbox.Text;
                c.Nationality = NationalityTextbox.Text;
                c.NID = NIDTextbox.Text;
                c.Address = AddressTextbox.Text;
                c.PhoneNumber = PhoneNumberTextbox.Text;
                c.Email = EmailTextbox.Text;
                c.DOB = DOBDateTimePicker.Text;
                c.Occupation = OccupationTextbox.Text;
                if (MaleRadioButton.Checked)
                {
                    c.Gender = MaleRadioButton.Text;
                }
                else
                {
                    c.Gender = FemaleRadioButton.Text;
                }
                account.AccountType = AccountTypeComboBox.Text;
                if (registrationImagePath == null)
                {
                    if (c.Gender.Equals("Male"))
                    {
                        c.ImageDir = registrationImagePathMale;
                    }
                    else
                    {
                        c.ImageDir = registrationImagePathFemale;
                    }
                }
                else
                {
                    c.ImageDir = registrationImagePath;
                }

                if (Registration.RegisterAccount(c, account, currentEmployee.ID))
                {
                    MessageBox.Show("Registration Complete!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    new PrintAccountDetails(c).ShowDialog();
                    ResetCreateAccountDetails();
                }
                else
                {
                    MessageBox.Show("Error in registering", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void SearchButton_Click(object sender, EventArgs e)
        {
            string s = SearchTextbox.Text;
            if (s == "")
            {
                MessageBox.Show("Input Valid Account Number!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                try
                {
                    int id = Convert.ToInt32(s);
                    Client c = FetchData.GetClientByAccountID(id);
                    Account account = FetchData.GetAccount(id);
                    searchedClient = c;
                    searchedAccount = account;
                    if (c == null)
                    {
                        MessageBox.Show("No client found!", "Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        HideAccountDetails();
                    }
                    else
                    {
                        ShowAccountDetails(c, account);
                        if (account.AccountStatus.Equals("Closed"))
                        {
                            CloseAccountButton.Enabled = false;
                        }
                        else
                        {
                            CloseAccountButton.Enabled = true;
                        }
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Input Valid Account Number!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void CloseAccountButton_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Do you wish to close this account?", "Warning!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                searchedAccount.AccountStatus = "Closed";
                if (ModifyData.UpdateAccountStatus(searchedAccount, currentEmployee, "Closed"))
                {
                    MessageBox.Show("Account Closed!", "Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Error updating!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                HideAccountDetails();
            }
            else
            {
                return;
            }
        }
        private void BrowseImageButton_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Jpg Image(*.jpg)|*.jpg|Png Image(*.png)|*.png";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    registrationImagePath = openFileDialog.FileName;
                    ImageRegister.ImageLocation = registrationImagePath;
                }

            }
            catch (Exception)
            {
                MessageBox.Show("Error in opening image!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        private bool CheckEmptyCreateAccountFields()
        {
            foreach (Control control in CreateAccountPanel.Controls)
            {
                if (control is TextBox)
                {
                    if (((TextBox)control).Text == "")
                    {
                        return true;
                    }
                }
            }
            if (!(MaleRadioButton.Checked || FemaleRadioButton.Checked))
            {
                return true;
            }
            if (AccountTypeComboBox.Text == "")
            {
                return true;
            }
            return false;
        }
        private void FindButton_Click(object sender, EventArgs e)
        {
            ResetResultGroupBox();
            string s = RecoverAccountTextBox.Text;
            if (s == "")
            {
                return;
            }
            else
            {
                try
                {
                    int nid = Convert.ToInt32(s);
                    List<Account> accounts = FetchData.GetAccountsByNID(nid);
                    Client client = FetchData.GetClientByNID(nid);
                    if (accounts == null)
                    {
                        MessageBox.Show("No client found!", "Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    else if (accounts.Count > 0)
                    {
                        AccountOwnerPictureBox.ImageLocation = client.ImageDir;
                        int x = 0;
                        foreach (Account account in accounts)
                        {
                            Label l1 = new Label();
                            l1.AutoSize = true;
                            l1.Text = "Account ID: " + account.AccountID;
                            l1.Location = new Point(104, 69 + x * 25);
                            Label l2 = new Label();
                            l2.AutoSize = true;
                            l2.Text = "Account Type: " + account.AccountType;
                            l2.Location = new Point(380, 69 + x * 25);
                            AccountsResultGroupBox.Controls.Add(l1);
                            AccountsResultGroupBox.Controls.Add(l2);
                            x++;
                        }
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Insert Valid ID!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        #endregion Register

        #region Account

        private void AccountButton_Click(object sender, EventArgs e)
        {
            ResetAllButton();
            HideAllPanel();
            ResetChildButton();
            ResetCreateAccountDetails();
            AccountPanel.Show();
            AccountButton.BackColor = Color.FromArgb(43, 63, 97);
            CurrentLabel.Text = "Account";
        }

        #region Deposit

        private void FindButton_Deposit_Click(object sender, EventArgs e)
        {
            if (FindButton_Deposit.Text == "Find Again")
            {
                HideDepositGroupBox();
                return;
            }
            string s = SearchAccountTextbox.Text;
            if (s == "")
            {
                return;
            }
            else
            {
                try
                {
                    int id = Convert.ToInt32(s);
                    Client c = FetchData.GetClientByAccountID(id);
                    Account account = FetchData.GetAccount(id);
                    if (c == null || account == null)
                    {
                        MessageBox.Show("Invalid Account ID!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    DepositGroupBox.Show();
                    AccountOwnerPictureBox_Deposit.ImageLocation = c.ImageDir;
                    FindButton_Deposit.Text = "Find Again";
                    SearchAccountTextbox.Enabled = false;
                    AccountOwnerLabel.Text = "Account Owner: " + c.Firstname + " " + c.Lastname;
                    BalanceLabel.Text = "Balance: " + account.Balance;
                    AccountTypeLabel_Deposit.Text = "Account Type: " + account.AccountType;
                }
                catch
                {
                    MessageBox.Show("Enter Valid Account Number!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
        }
        private void DepositButton_Deposit_Click(object sender, EventArgs e)
        {

            string s = DepositTextbox.Text;
            if (s == "")
            {
                return;
            }
            else
            {
                try
                {
                    double amount = Convert.ToDouble(s);
                    int id = Convert.ToInt32(SearchAccountTextbox.Text);
                    if (ModifyData.UpdateBalance(id, amount))
                    {
                        if (!ModifyData.UpdateTransactionHistory(currentEmployee.ID, id, "Deposit", Convert.ToInt32(amount)))
                        {
                            MessageBox.Show("Error updating transaction!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        MessageBox.Show("Amount successfully added!", "Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        HideDepositGroupBox();
                        AccountOwnerPictureBox_Deposit.Image = null;
                        return;
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Amount must be numeric!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void HideDepositGroupBox()
        {
            DepositGroupBox.Hide();
            FindButton_Deposit.Text = "Find";
            SearchAccountTextbox.Enabled = true;
            DepositTextbox.Text = "";
            SearchAccountTextbox.Text = "";
            AccountOwnerPictureBox_Deposit.Image = null;
        }

        #endregion

        #region Withdraw

        private void Findbutton_Withdraw_Click(object sender, EventArgs e)
        {
            if (Findbutton_Withdraw.Text == "Find Again")
            {
                HideWithdrawGroupBox();
                return;
            }
            string s = SearchAccounttextBox_Withdraw.Text;
            if (s == "")
            {
                return;
            }
            else
            {
                try
                {
                    int id = Convert.ToInt32(s);
                    Client c = FetchData.GetClientByAccountID(id);
                    Account account = FetchData.GetAccount(id);
                    if (c == null || account == null)
                    {
                        MessageBox.Show("Invalid Account ID!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    groupBox_Withdraw.Show();
                    AccountOwnerpictureBox_Withdraw.ImageLocation = c.ImageDir;
                    Findbutton_Withdraw.Text = "Find Again";
                    SearchAccounttextBox_Withdraw.Enabled = false;
                    AccountOwnerlabel_Withdraw.Text = "Account Owner: " + c.Firstname + " " + c.Lastname;
                    Balancelabel_Withdraw.Text = "Balance: " + account.Balance;
                    AccountTypelabel_Withdraw.Text = "Account Type: " + account.AccountType;
                }
                catch
                {
                    MessageBox.Show("Enter Valid Account Number!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
        }
        private void Withdrawbutton_Withdraw_Click(object sender, EventArgs e)
        {
            string s = WithdrawtextBox.Text;
            if (s == "")
            {
                return;
            }
            else
            {
                try
                {
                    double amount = Convert.ToDouble(s);
                    int id = Convert.ToInt32(SearchAccounttextBox_Withdraw.Text);
                    if (ModifyData.UpdateBalance(id, (-amount)))
                    {
                        if (!ModifyData.UpdateTransactionHistory(currentEmployee.ID, id, "Withdraw", Convert.ToInt32(amount)))
                        {
                            MessageBox.Show("Error updating transaction!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        MessageBox.Show("Amount successfully withdrawn!", "Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        HideWithdrawGroupBox();
                        AccountOwnerpictureBox_Withdraw.Image = null;
                        return;
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Amount must be numeric!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void HideWithdrawGroupBox()
        {
            groupBox_Withdraw.Hide();
            Findbutton_Withdraw.Text = "Find";
            SearchAccounttextBox_Withdraw.Enabled = true;
            WithdrawtextBox.Text = "";
            SearchAccounttextBox_Withdraw.Text = "";
            AccountOwnerpictureBox_Withdraw.Image = null;
        }

        #endregion Withdraw

        #region Reset

        private void ResetAllButton()
        {
            foreach (Control button in MenuPanel.Controls)
            {
                if (button is Button)
                {
                    if (button.Enabled)
                        button.BackColor = Color.FromArgb(31, 30, 68);
                }
            }
        }
        private void ResetChildButton(Panel panel)
        {
            foreach (Control button in panel.Controls)
            {
                if (button is Button)
                    button.BackColor = Color.FromArgb(31, 30, 68);
            }
        }
        private void ResetChildButton()
        {
            foreach (Control button in RegisterPanel.Controls)
            {
                if (button is Button)
                    button.BackColor = Color.FromArgb(31, 30, 68);
            }
            foreach (Control button in AccountPanel.Controls)
            {
                if (button is Button)
                    button.BackColor = Color.FromArgb(31, 30, 68);
            }
        }
        private void ResetPanel(Panel panel)
        {
            if (panel.Equals(RegisterPanel))
            {
                foreach (Control control in CreateAccountPanel.Controls)
                {
                    if (control is TextBox)
                    {
                        control.Text = "";
                    }
                    else if (control is ComboBox)
                    {
                        ((ComboBox)control).SelectedItem = null;
                    }
                    else if (control is RadioButton)
                    {
                        if (((RadioButton)control).Checked)
                        {
                            ((RadioButton)control).Checked = false;
                        }
                    }
                }
                foreach (Control control in CloseAccountPanel.Controls)
                {
                    if (control is TextBox)
                    {
                        control.Text = "";
                    }
                    else if (control is ComboBox)
                    {
                        ((ComboBox)control).SelectedItem = null;
                    }
                    else if (control is RadioButton)
                    {
                        if (((RadioButton)control).Checked)
                        {
                            ((RadioButton)control).Checked = false;
                        }
                    }
                }
            }
            else if (panel.Equals(AccountPanel))
            {

            }
        }
        private void ResetCreateAccountDetails()
        {
            foreach (Control control in CreateAccountPanel.Controls)
            {
                if (control is TextBox)
                {
                    ((TextBox)control).Text = "";
                }
            }
            AccountTypeComboBox.Text = "";
            DOBDateTimePicker.ResetText();
            MaleRadioButton.Checked = false;
            FemaleRadioButton.Checked = false;
            ImageRegister.Image = null;
        }

        #endregion Reset

        #region Manager

        private void ManagerButton_Click(object sender, EventArgs e)
        {
            ManagerPanelDefaultPanel.BringToFront();
            DashboardButton.BackColor = Color.FromArgb(31, 30, 68);
            ManagerPanel.BringToFront();
            ManagerButton.BackColor = Color.FromArgb(43, 63, 97);
            CurrentLabel.Text = "Manager";
            EnableManagerPanelButtons(null);
            ClearManagerTextFields();
        }
        private void ManagerPanelButtonsHandler(object sender, EventArgs e)
        {
            if (sender.Equals(EmployeeDetailsButton))
            {
                EmployeeDetailsPanel.BringToFront();
                EmployeeDetailsButton.Enabled = false;
                EmployeeDetailsButton.BackColor = Color.Gray;
                EnableManagerPanelButtons(sender);
            }
            else if (sender.Equals(EditDetailsButton))
            {
                EditDetailsPanel.BringToFront();
                EditDetailsButton.Enabled = false;
                EditDetailsButton.BackColor = Color.Gray;
                EnableManagerPanelButtons(sender);
            }
            else if (sender.Equals(ManageButton))
            {
                ManagePanel.BringToFront();
                ManageButton.Enabled = false;
                ManageButton.BackColor = Color.Gray;
                EnableManagerPanelButtons(sender);
            }
            else if (sender.Equals(TransactionsButton))
            {
                TransactionsPanel.BringToFront();
                TransactionsButton.Enabled = false;
                TransactionsButton.BackColor = Color.Gray;
                EnableManagerPanelButtons(sender);
            }
        }
        private void EnableManagerPanelButtons(object sender)
        {
            foreach (Button button in ManagerButtonsPanel.Controls)
            {
                if (sender != null)
                {
                    if (sender.Equals(button))
                    {
                        continue;
                    }
                }

                button.Enabled = true;
                button.BackColor = Color.FromArgb(26, 25, 62);
            }
        }
        private void FindAllButton_Click(object sender, EventArgs e)
        {
            DetailsPanel_EmployeeDetails.Controls.Clear();
            List<Employee> employees = FetchData.GetAllEmployees();
            foreach (Employee employee in employees)
            {
                GroupBox groupBox = new GroupBox();
                groupBox.Text = "Employee";
                groupBox.ForeColor = Color.White;
                groupBox.Location = new Point(9, 9);
                groupBox.Dock = DockStyle.Top;
                groupBox.Size = new Size(715, 143);
                //groupBox.Padding = new Padding(0,0,0,10);

                Label l1 = new Label();
                l1.Text = "Name: " + employee.Name;
                l1.Location = new Point(39, 41);
                l1.AutoSize = true;
                Label l2 = new Label();
                l2.Text = "ID: " + employee.ID;
                l2.Location = new Point(60, 90);
                l2.AutoSize = true;
                Label l3 = new Label();
                l3.Text = "NID: " + employee.NID;
                l3.Location = new Point(220, 90);
                l3.AutoSize = true;
                Label l4 = new Label();
                l4.Text = "Gender: " + employee.Gender;
                l4.Location = new Point(434, 40);
                l4.AutoSize = true;
                Label l5 = new Label();
                l5.Text = "Date of Birth: " + employee.DOB;
                l5.Location = new Point(399, 90);
                l5.AutoSize = true;
                Label l6 = new Label();
                l6.Text = "Address: " + employee.Address;
                l6.Location = new Point(430, 64);
                l6.AutoSize = true;
                Label l7 = new Label();
                l7.Text = "Email: " + employee.Email;
                l7.Location = new Point(39, 67);
                l7.AutoSize = true;

                groupBox.Controls.Add(l1);
                groupBox.Controls.Add(l2);
                groupBox.Controls.Add(l3);
                groupBox.Controls.Add(l4);
                groupBox.Controls.Add(l5);
                groupBox.Controls.Add(l6);
                groupBox.Controls.Add(l7);

                DetailsPanel_EmployeeDetails.Controls.Add(groupBox);
            }
        }
        private void FindButton_EmployeeDetails_Click(object sender, EventArgs e)
        {
            string s = EmployeeIDTextbox.Text;
            if (s == "")
            {
                DetailsPanel_EmployeeDetails.Controls.Clear();
                return;
            }
            else
            {
                try
                {
                    DetailsPanel_EmployeeDetails.Controls.Clear();
                    int id = Convert.ToInt32(s);
                    Employee employee = FetchData.GetEmployee(id);
                    if (employee == null)
                    {
                        MessageBox.Show("Invalid employee id!", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        DetailsPanel_EmployeeDetails.Controls.Clear();
                        return;
                    }
                    else
                    {
                        GroupBox groupBox = new GroupBox();
                        groupBox.Text = "Employee";
                        groupBox.ForeColor = Color.White;
                        groupBox.Location = new Point(9, 9);
                        groupBox.Dock = DockStyle.Top;
                        groupBox.Size = new Size(715, 143);

                        Label l1 = new Label();
                        l1.Text = "Name: " + employee.Name;
                        l1.Location = new Point(39, 41);
                        l1.AutoSize = true;
                        Label l2 = new Label();
                        l2.Text = "ID: " + employee.ID;
                        l2.Location = new Point(60, 90);
                        l2.AutoSize = true;
                        Label l3 = new Label();
                        l3.Text = "NID: " + employee.NID;
                        l3.Location = new Point(220, 90);
                        l3.AutoSize = true;
                        Label l4 = new Label();
                        l4.Text = "Gender: " + employee.Gender;
                        l4.Location = new Point(434, 40);
                        l4.AutoSize = true;
                        Label l5 = new Label();
                        l5.Text = "Date of Birth: " + employee.DOB;
                        l5.Location = new Point(399, 90);
                        l5.AutoSize = true;
                        Label l6 = new Label();
                        l6.Text = "Address: " + employee.Address;
                        l6.Location = new Point(430, 64);
                        l6.AutoSize = true;
                        Label l7 = new Label();
                        l7.Text = "Email: " + employee.Email;
                        l7.Location = new Point(39, 67);
                        l7.AutoSize = true;

                        groupBox.Controls.Add(l1);
                        groupBox.Controls.Add(l2);
                        groupBox.Controls.Add(l3);
                        groupBox.Controls.Add(l4);
                        groupBox.Controls.Add(l5);
                        groupBox.Controls.Add(l6);
                        groupBox.Controls.Add(l7);

                        DetailsPanel_EmployeeDetails.Controls.Add(groupBox);
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Invalid Input!", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    DetailsPanel_EmployeeDetails.Controls.Clear();
                    return;
                }
            }
        }
        private void FindButtion_Transactions_Click(object sender, EventArgs e)
        {
            string s = EmployeeIDTextBox_Transactions.Text;
            if (s == "")
            {
                TransactionHistoryPanel.Controls.Clear();
                return;
            }
            else
            {
                try
                {
                    TransactionHistoryPanel.Controls.Clear();
                    int id = Convert.ToInt32(s);
                    Employee employee = FetchData.GetEmployee(id);
                    List<Transactions> transactions = FetchData.GetTransactionHistory(id);
                    if (transactions == null)
                    {
                        MessageBox.Show("Invalid employee id!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        TransactionHistoryPanel.Controls.Clear();
                        return;
                    }
                    foreach (Transactions transaction in transactions)
                    {
                        GroupBox groupBox = new GroupBox();
                        groupBox.Text = transaction.TransactionType;
                        groupBox.ForeColor = Color.White;
                        //groupBox.Location = new Point(9, 9);
                        groupBox.Dock = DockStyle.Top;
                        groupBox.Size = new Size(720, 112);
                        //groupBox.Padding = new Padding(0,0,0,10);

                        Label l1 = new Label();
                        l1.Text = "Employee ID: " + transaction.EmployeeID;
                        l1.Location = new Point(70, 38);
                        l1.AutoSize = true;
                        Label l2 = new Label();
                        l2.Text = "Account ID: " + transaction.AccountID;
                        l2.Location = new Point(79, 66);
                        l2.AutoSize = true;
                        Label l3 = new Label();
                        l3.Text = "Transaction ID: " + transaction.ID;
                        l3.Location = new Point(405, 66);
                        l3.AutoSize = true;
                        Label l4 = new Label();
                        l4.Text = "Employee Name: " + employee.Name;
                        l4.Location = new Point(397, 44);
                        l4.AutoSize = true;

                        groupBox.Controls.Add(l1);
                        groupBox.Controls.Add(l2);
                        groupBox.Controls.Add(l3);
                        groupBox.Controls.Add(l4);

                        TransactionHistoryPanel.Controls.Add(groupBox);
                    }

                }
                catch (Exception)
                {
                    MessageBox.Show("Invalid Input!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    TransactionHistoryPanel.Controls.Clear();
                    return;
                }
            }
        }
        private void ClearManagerTextFields()
        {
            EmployeeIDTextBox_Transactions.Text = "";
            EmployeeIDTextbox.Text = "";
        }

        #endregion Manager

        #endregion Account
        private void SubmenuButtonsHandler(object sender, EventArgs e)
        {
            if (sender is Button)
            {
                if (sender.Equals(CreateAccButton))
                {
                    ResetChildButton(RegisterPanel);
                    CreateAccountPanel.BringToFront();
                    CreateAccButton.BackColor = Color.FromArgb(22, 34, 54);

                    InitialDepositTextbox.Text = "500";
                    InitialDepositTextbox.Enabled = false;
                }
                else if (sender.Equals(CloseAccButton))
                {
                    ResetChildButton(RegisterPanel);
                    CloseAccountPanel.BringToFront();
                    CloseAccButton.BackColor = Color.FromArgb(22, 34, 54);
                }
                else if (sender.Equals(RecoverAccButton))
                {
                    ResetChildButton(RegisterPanel);
                    RecoverAccountPanel.BringToFront();
                    RecoverAccButton.BackColor = Color.FromArgb(22, 34, 54);
                }
                else if (sender.Equals(DepositButton))
                {
                    ResetChildButton(AccountPanel);
                    DepositPanel.BringToFront();
                    DepositButton.BackColor = Color.FromArgb(22, 34, 54);
                }
                else if (sender.Equals(WithdrawButton))
                {
                    ResetChildButton(AccountPanel);
                    WithdrawPanel.BringToFront();
                    WithdrawButton.BackColor = Color.FromArgb(22, 34, 54);
                }
                else if (sender.Equals(TransferButton))
                {
                    ResetChildButton(AccountPanel);
                    TransferPanel.BringToFront();
                    TransferButton.BackColor = Color.FromArgb(22, 34, 54);
                }
                else if (sender.Equals(DetailsButton))
                {
                    ResetChildButton(AccountPanel);
                    ResetAccountDetails();
                    DetailsPanel.BringToFront();
                    DetailsButton.BackColor = Color.FromArgb(22, 34, 54);
                }
            }
        }
        private void HideAllPanel()
        {
            RegisterPanel.Hide();
            AccountPanel.Hide();
        }
        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
        private void DisableComponents()
        {
            ValidateErrorLabel.Text = "";
        }
        private void AccountButton_LocationChanged(object sender, EventArgs e)
        {
            AccountPictureBox.Location = new Point(9, AccountButton.Location.Y + 10);
        }
        private void LoanButton_LocationChanged(object sender, EventArgs e)
        {
            ManagerPanelPictureBox.Location = new Point(9, ManagerButton.Location.Y + 10);
        }
        private void ClockTimer_Tick(object sender, EventArgs e)
        {
            TimeLabel.Text = DateTime.Now.ToString("hh:mm");
            SecondLabel.Text = DateTime.Now.ToString("ss");
        }
        private void TextBoxEnterColorChange(object sender, EventArgs e)
        {
            if (sender.Equals(FirstnameTextbox))
            {
                FirstnameTextbox.BackColor = Color.FromArgb(22, 67, 99);
                return;
            }
            else if (sender.Equals(LastnameTextbox))
            {
                LastnameTextbox.BackColor = Color.FromArgb(22, 67, 99);
                return;
            }
            else if (sender.Equals(EmailTextbox))
            {
                EmailTextbox.BackColor = Color.FromArgb(22, 67, 99);
                return;
            }
            else if (sender.Equals(PhoneNumberTextbox))
            {
                PhoneNumberTextbox.BackColor = Color.FromArgb(22, 67, 99);
                return;
            }
            else if (sender.Equals(NIDTextbox))
            {
                NIDTextbox.BackColor = Color.FromArgb(22, 67, 99);
                return;
            }
            else if (sender.Equals(AddressTextbox))
            {
                AddressTextbox.BackColor = Color.FromArgb(22, 67, 99);
                return;
            }
            else if (sender.Equals(NationalityTextbox))
            {
                NationalityTextbox.BackColor = Color.FromArgb(22, 67, 99);
                return;
            }
            else if (sender.Equals(OccupationTextbox))
            {
                OccupationTextbox.BackColor = Color.FromArgb(22, 67, 99);
                return;
            }
            else
            {
                return;
            }
        }
        private void TextBoxLeaveColorChange(object sender, EventArgs e)
        {
            if (sender.Equals(FirstnameTextbox))
            {
                FirstnameTextbox.BackColor = Color.FromArgb(34, 33, 74);
                return;
            }
            else if (sender.Equals(LastnameTextbox))
            {
                LastnameTextbox.BackColor = Color.FromArgb(34, 33, 74);
                return;
            }
            else if (sender.Equals(EmailTextbox))
            {
                EmailTextbox.BackColor = Color.FromArgb(34, 33, 74);
                return;
            }
            else if (sender.Equals(PhoneNumberTextbox))
            {
                PhoneNumberTextbox.BackColor = Color.FromArgb(34, 33, 74);
                return;
            }
            else if (sender.Equals(NIDTextbox))
            {
                NIDTextbox.BackColor = Color.FromArgb(34, 33, 74);
                return;
            }
            else if (sender.Equals(AddressTextbox))
            {
                AddressTextbox.BackColor = Color.FromArgb(34, 33, 74);
                return;
            }
            else if (sender.Equals(NationalityTextbox))
            {
                NationalityTextbox.BackColor = Color.FromArgb(34, 33, 74);
                return;
            }
            else if (sender.Equals(OccupationTextbox))
            {
                OccupationTextbox.BackColor = Color.FromArgb(34, 33, 74);
                return;
            }
            else
            {
                return;
            }
        }
        private void SignOutButton_Click(object sender, EventArgs e)
        {
            DialogResult confirmation = MessageBox.Show("Are you sure to log out?", "Warning!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirmation == DialogResult.Yes)
            {
                this.Dispose();
                new Login().Show();
            }
            else
            {
                return;
            }
        }
        private void HideAccountDetails()
        {
            AccountDetailsGroupBox.Hide();
        }
        private void ShowAccountDetails(Client client, Account account)
        {
            AccountDeatilsAccountIDLabel.Text = "AccountID: " + account.AccountID.ToString();
            AccountDeatilsFirstNameLabel.Text = "Firstname: " + client.Firstname;
            AccountDeatilsLastNameLabel.Text = "Lastname: " + client.Lastname;
            AccountDeatilsGenderLabel.Text = "Gender: " + client.Gender;
            AccountDeatilsNationalityLabel.Text = "Nationality: " + client.Nationality;
            AccountDeatilsNIDLabel.Text = "NID: " + client.NID;
            AccountDeatilsOccupationLabel.Text = "Occupation: " + client.Occupation;
            AccountDeatilsEmailLabel.Text = "Email: " + client.Email;
            AccountDeatilsDOBLabel.Text = "Date of Birth: " + client.DOB;
            AccountDetailsPhoneNumberLabel.Text = "Phone Numebr: " + client.PhoneNumber;
            AccountDeatilsAddressLabel.Text = "Address: " + client.Address;
            AccountDeatilsAccountTypeLabel.Text = "Account Type: " + account.AccountType;
            AccountDeatilsAccountStatusLabel.Text = "Account Status: " + account.AccountStatus;
            ClientPictureBox.ImageLocation = client.ImageDir;

            AccountDetailsGroupBox.Show();
        }
        private void ResetResultGroupBox()
        {
            foreach (Control control in AccountsResultGroupBox.Controls)
            {
                control.Text = null;
            }
            AccountOwnerPictureBox.Image = null;
        }
        private void ResetAccountDetails()
        {
            foreach (Control label in AccountDetailsGroupBox_Details.Controls)
            {
                if (label is Label)
                {
                    label.Text = "";
                }
            }
            AccountOwnerPictureBox_Details.Image = null;
        }
        private void SearchAccountTextbox_Details_TextChanged(object sender, EventArgs e)
        {
            string s = SearchAccountTextbox_Details.Text;
            if (s == "")
            {
                return;
            }
            else
            {
                try
                {
                    int id = Convert.ToInt32(s);
                    Client client = FetchData.GetClientByAccountID(id);
                    Account account = FetchData.GetAccount(id);
                    if (client == null || account == null)
                    {
                        ResetAccountDetails();
                        return;
                    }
                    else
                    {
                        AccountIDLabel_Details.Text = "AccountID: " + account.AccountID.ToString();
                        FirstNameLabel_Details.Text = "Firstname: " + client.Firstname;
                        LastNameLabel_Details.Text = "Lastname: " + client.Lastname;
                        GenderLabel_Details.Text = "Gender: " + client.Gender;
                        NationalityLabel_Details.Text = "Nationality: " + client.Nationality;
                        NIDLabel_Details.Text = "NID: " + client.NID;
                        OccupationLabel_Details.Text = "Occupation: " + client.Occupation;
                        EmailLabel_Details.Text = "Email: " + client.Email;
                        DOBLabel_Details.Text = "Date of Birth: " + client.DOB;
                        PhoneNumberLabel_Details.Text = "Phone Numebr: " + client.PhoneNumber;
                        AddressLabel_Details.Text = "Address: " + client.Address;
                        AccountTypeLabel_Details.Text = "Account Type: " + account.AccountType;
                        AccountStatusLabel_Details.Text = "Account Status: " + account.AccountStatus;
                        AccountOwnerPictureBox_Details.ImageLocation = client.ImageDir;
                        BalanceLabel_Details.Text = "Balance: " + account.Balance;
                        DueLabelDetails.Text = "Due: " + account.Due;
                        CreateDateLabel_Details.Text = "Create Date: " + account.CreateDate;
                    }
                }
                catch (Exception)
                {
                    ResetAccountDetails();
                    return;
                }
            }
        }
        private bool VerifyEmail(string email)
        {
            //int indx = email.IndexOf('@');
            string[] slice = email.Split('@');
            if (slice.Length != 2)
            {
                return false;
            }
            else
            {
                if (slice[1].Equals("gmail.com"))
                {
                    if (isNumber(((slice[0])[0]).ToString()))
                    {
                        //Console.WriteLine("asdas");
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        private bool VerifyName(string name)
        {
            foreach (char c in name)
            {
                try
                {
                    Convert.ToInt32(c.ToString());
                    return false;
                }
                catch
                {

                }
            }
            return true;
        }
        private bool isNumber(string ch)
        {
            try
            {
                int x = Convert.ToInt32(ch);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #region Transfer

        private void HidetransferGroupBox()
        {
            transferGroupBox_Transfer.Hide();
            findButton_Transfer.Text = "Find";
            accNumberSearchTextBox_Transfer.Enabled = true;
            enterAmountTextBox_Transfer.Text = "";
            accNumberSearchTextBox_Transfer.Text = "";
            AccountOwnerpictureBox_Transfer.Image = null;
        }

        private void ResetTransfer()
        {
            recAccNumberTextBox_Transfer.Text = "";
            accOwnerLabel_Transfer.Text = "Account Owner: ";
            accTypeLabel_Transfer.Text = "Account Type";
            recAccFindButton_Transfer.Text = "Find";
            recAccNumberTextBox_Transfer.Enabled = true;
        }
        private void findButton_Transfer_Click(object sender, EventArgs e)
        {
            if (findButton_Transfer.Text == "Find Again")
            {
                HidetransferGroupBox();
                ResetTransfer();
                return;
            }
            string s = accNumberSearchTextBox_Transfer.Text;
            if (s == "")
            {
                return;
            }
            else
            {
                try
                {
                    int id = Convert.ToInt32(s);
                    Client c = FetchData.GetClientByAccountID(id);
                    Account account = FetchData.GetAccount(id);
                    if (c == null || account == null)
                    {
                        MessageBox.Show("Invalid Account ID!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    transferGroupBox_Transfer.Show();
                    AccountOwnerpictureBox_Transfer.ImageLocation = c.ImageDir;
                    findButton_Transfer.Text = "Find Again";
                    accNumberSearchTextBox_Transfer.Enabled = false;
                    senderAccOwnerLabel_Transfer.Text = "Account Owner: " + c.Firstname + " " + c.Lastname;
                    senderBalanceLabel_Transfer.Text = "Balance: " + account.Balance;
                }
                catch
                {
                    MessageBox.Show("Enter Valid Account Number!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
        }
        #endregion

        private void recAccFindButton_Transfer_Click(object sender, EventArgs e)
        {
            if (recAccFindButton_Transfer.Text == "Find Again")
            {
                ResetTransfer();
            }
            string s = recAccNumberTextBox_Transfer.Text;
            if (s == "")
            {
                return;
            }
            else
            {
                try
                {
                    int id = Convert.ToInt32(s);
                    Client c = FetchData.GetClientByAccountID(id);
                    Account account = FetchData.GetAccount(id);
                    int senderId = Convert.ToInt32(accNumberSearchTextBox_Transfer.Text);
                    if (c == null || account == null || account.AccountID == senderId)
                    {
                        MessageBox.Show("Invalid Account ID!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        recAccNumberTextBox_Transfer.Text = "";
                        return;
                    }
                    transferGroupBox_Transfer.Show();
                    recAccFindButton_Transfer.Text = "Find Again";
                    recAccNumberTextBox_Transfer.Enabled = false;
                    accOwnerLabel_Transfer.Text = "Account Owner: " + c.Firstname + " " + c.Lastname;
                    accTypeLabel_Transfer.Text = "Account Type: " + account.AccountType;
                }
                catch
                {
                    MessageBox.Show("Enter Valid Account Number!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
        }

        private void transferButton_Transfer_Click(object sender, EventArgs e)
        {
            string s = enterAmountTextBox_Transfer.Text;
            if (s == "")
            {
                return;
            }
            else
            {
                try
                {
                    double amount = Convert.ToDouble(s);
                    int recId = Convert.ToInt32(recAccNumberTextBox_Transfer.Text);
                    int senderId = Convert.ToInt32(accNumberSearchTextBox_Transfer.Text);
                    if ((ModifyData.UpdateBalance(recId, amount)) && (ModifyData.UpdateBalance(senderId, (-amount))))
                    {
                        if (!ModifyData.UpdateTransactionHistory(currentEmployee.ID, recId, "Transfer", Convert.ToInt32(amount)))
                        {
                            MessageBox.Show("Error updating transaction!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        MessageBox.Show("Amount successfully added!", "Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        HidetransferGroupBox();
                        accOwnerLabel_Transfer.Text = "Account Owner: ";
                        accTypeLabel_Transfer.Text = "Account Type";
                        AccountOwnerpictureBox_Transfer.Image = null;
                        ResetTransfer();
                        return;
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Amount must be numeric!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void TransferPanel_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
