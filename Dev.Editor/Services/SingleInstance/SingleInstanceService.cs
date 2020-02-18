using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dev.Editor.Services.SingleInstance
{
    public class SingleInstanceService
    {
        private const string MUTEX_NAME = "Dev_Editor";
        private const string MEMORY_MAPPED_FILE_NAME = "Dev_Editor_Window_Handle";

        private Mutex mutex;
        private bool isMainInstance;
        private MemoryMappedFile hwndFile;

        public SingleInstanceService()
        {
            mutex = new Mutex(true, MUTEX_NAME, out bool created);
            isMainInstance = created;

            hwndFile = MemoryMappedFile.CreateOrOpen(MEMORY_MAPPED_FILE_NAME, sizeof(long));
        }

        public void StoreMainWindowHandle(IntPtr handle)
        {
            if (!isMainInstance)
                throw new InvalidOperationException("This cannot be executed in secondary instance!");

            using (var stream = hwndFile.CreateViewStream(0, sizeof(long), MemoryMappedFileAccess.Write))
            {
                var writer = new BinaryWriter(stream);
                writer.Write(handle.ToInt64());
            }
        }

        public IntPtr ReadMainwWindowHandle()
        {
            if (isMainInstance)
                throw new InvalidOperationException("This cannot be executed in main instance!");

            using (var stream = hwndFile.CreateViewStream(0, sizeof(long), MemoryMappedFileAccess.Read))
            {
                var reader = new BinaryReader(stream);
                var handleLong = reader.ReadInt64();

                return new IntPtr(handleLong);
            }
        }

        public bool IsMainInstance => isMainInstance;
    }
}
