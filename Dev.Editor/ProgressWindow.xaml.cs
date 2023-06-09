﻿using Autofac;
using Dev.Editor.BusinessLogic.ViewModels.ProgressWindow;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace Dev.Editor
{
    /// <summary>
    /// Logika interakcji dla klasy ProgressWindow.xaml
    /// </summary>
    public partial class ProgressWindow : Window, IProgressWindowAccess
    {
        private ProgressWindowViewModel viewModel;

        public ProgressWindow(string operationTitle, BackgroundWorker worker, object workerParameter)
        {
            InitializeComponent();

            viewModel = Dependencies.Container.Instance.Resolve<ProgressWindowViewModel>(
                new NamedParameter("operationTitle", operationTitle),
                new NamedParameter("worker", worker),
                new NamedParameter("workerParameter", workerParameter),
                new NamedParameter("access", this));
            DataContext = viewModel;
        }

        private void HandleWindowLoaded(object sender, RoutedEventArgs e)
        {
            viewModel.NotifyLoaded();
        }
    }
}
