using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.Services.Paths
{
    public interface IPathService
    {
        string ConfigPath { get; }
        string StoredFilesPath { get; }
        string BinDefinitionsPath { get; }
        string AppExecutablePath { get; }
    }
}
