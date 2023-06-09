﻿using Autofac;
using Dev.Editor.BusinessLogic.ViewModels.SubstitutionCipher;
using Fluent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Dev.Editor
{
    /// <summary>
    /// Logika interakcji dla klasy SubstitutionCipherWindow.xaml
    /// </summary>
    public partial class SubstitutionCipherWindow : RibbonWindow, ISubstitutionCipherWindowAccess
    {
        private readonly SubstitutionCipherWindowViewModel viewModel;
        private readonly DispatcherTimer timer;

        private void HandleTimerTick(object sender, EventArgs e)
        {
            (sender as DispatcherTimer).Stop();
            viewModel.NotifyActionTimerElapsed();
        }

        public void RestartActionTimer()
        {
            timer.Stop();
            timer.Start();
        }

        public SubstitutionCipherWindow(ISubstitutionCipherHost host)
        {
            InitializeComponent();

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(300);
            timer.Tick += HandleTimerTick;

            viewModel = Dependencies.Container.Instance.Resolve<SubstitutionCipherWindowViewModel>(new NamedParameter("host", host),
                new NamedParameter("access", this));
            this.DataContext = viewModel;
        }

        private void HandleWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();

            // This window's focus and owner window's focus are independent
            // However, user expects, that focusing this window focuses
            // the application, so we have to ensure, that main window gains
            // focus at this point.
            if (Owner != null)
            {
                Owner.Focus();
            }
        }

        private void HandleTbCipherGotFocus(object sender, RoutedEventArgs e)
        {
            (sender as System.Windows.Controls.TextBox).SelectAll();
        }

        public void FocusAlphabetEntry(AlphabetEntryViewModel alphabetEntryToFocus)
        {
            var container = icAlphabet.ItemContainerGenerator.ContainerFromItem(alphabetEntryToFocus);
            if (container != null && container is ContentPresenter presenter)
            {
                DataTemplate dataTemplate = presenter.ContentTemplate;
                var textBox = (System.Windows.Controls.TextBox)dataTemplate.FindName("tbCipher", presenter);
                textBox?.Focus();
            }
        }
    }
}
