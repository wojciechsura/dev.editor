﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.ViewModels.WebBrowserWindow
{
    public interface IWebBrowserWindowAccess
    {
        void Show();
        void ShowHtml(string html);
    }
}
