using BankReport.Services.Database;
using BankReport.ViewModels;
using BankReport.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace BankReport
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();
        
        }

        private async void OnMenuItemClicked(object sender, EventArgs e)
        {
            Current.FlyoutIsPresented = true;
            //await Shell.Current.GoToAsync("//LoginPage");
        }

    }
}
