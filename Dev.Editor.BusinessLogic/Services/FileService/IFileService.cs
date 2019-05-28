using Dev.Editor.BusinessLogic.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.Services.FileService
{
    public interface IFileService
    {
        DocumentViewModel NewDocument();
    }
}
