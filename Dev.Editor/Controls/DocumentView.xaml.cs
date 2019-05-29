﻿using Dev.Editor.BusinessLogic.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Dev.Editor.Controls
{
    /// <summary>
    /// Logika interakcji dla klasy DocumentView.xaml
    /// </summary>
    public partial class DocumentView : UserControl
    {
        public void Load(Stream documentStream)
        {
            teEditor.Load(documentStream);

            if (documentStream is IDisposable disposableStream)
                disposableStream.Dispose();
        }

        public DocumentView()
        {
            InitializeComponent();
        }
    }
}
