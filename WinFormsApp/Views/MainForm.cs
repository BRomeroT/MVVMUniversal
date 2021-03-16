using Core.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormsApp.Views
{
    public partial class MainForm : Form
    {
        LoginViewModel viewModel = new();
        public MainForm()
        {
            InitializeComponent();
            UserTextBox.DataBindings.Add("Text", viewModel, nameof(viewModel.User));
            PasswordTextBox.DataBindings.Add("Text", viewModel, nameof(viewModel.Password));

            LoginButton.DataBindings.Add("Enabled", viewModel, nameof(viewModel.LoginCommand.CanExecuteIsEnabled));

        }

        private void LoginButton_Click(object sender, EventArgs e) =>
            viewModel.LoginCommand.Execute();
    }
}
