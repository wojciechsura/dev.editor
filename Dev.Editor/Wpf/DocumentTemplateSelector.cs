﻿using Dev.Editor.BusinessLogic.ViewModels.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Dev.Editor.Wpf
{
    public class DocumentTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (container is FrameworkElement element)
            {
                if (item is TextDocumentViewModel)
                {
                    return element.FindResource("TextDocumentTemplate") as DataTemplate;
                }
                else if (item is HexDocumentViewModel)
                {
                    return element.FindResource("HexDocumentTemplate") as DataTemplate;
                }
                else if (item is BinDocumentViewModel)
                {
                    return element.FindResource("BinDocumentTemplate") as DataTemplate;
                }
            }

            return null;
        }
    }
}
