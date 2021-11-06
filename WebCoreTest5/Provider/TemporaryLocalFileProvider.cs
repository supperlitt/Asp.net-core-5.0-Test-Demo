using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Physical;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WebCoreTest5.Provider
{
    public class TemporaryLocalFileProvider : IFileProvider
    {
        private readonly DirectoryInfo _fileRoot;
        private readonly DirectoryInfo _root;

        public TemporaryLocalFileProvider(DirectoryInfo tmpRoot, DirectoryInfo fileRoot)
        {
            _fileRoot = fileRoot;
            _root = tmpRoot;
        }
        public IFileInfo GetFileInfo(string tmpFileId)
        {
            tmpFileId = tmpFileId.TrimStart('/', '\\');
            var path = Path.Combine(_root.FullName, tmpFileId);
            if (!File.Exists(path))
            {
                return new NotFoundFileInfo(tmpFileId);
            }

            var text = File.ReadAllText(path);
            var descriptor = System.Text.Json.JsonSerializer.Deserialize<TemporaryLocalFileDescriptor>(text);
            if (descriptor.Expiry < DateTime.Now)
            {
                File.Delete(path);
                return new NotFoundFileInfo(tmpFileId);
            }

            // 通过 id转文件
            string fileName = descriptor.FileId;
            return new PhysicalFileInfo(new FileInfo(Path.Combine(_fileRoot.FullName, fileName)));
        }

        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            throw new System.NotImplementedException();
        }

        public IChangeToken Watch(string filter)
        {
            throw new System.NotImplementedException();
        }
    }

    public class TemporaryLocalFileDescriptor
    {
        public string FileId { get; set; }
        public bool IsDownload { get; set; }
        public DateTimeOffset Expiry { get; set; }
    }
}
