using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.IO;
using System.Security.Cryptography;

namespace Stutton.DocumentCreator.Services.Vsts
{
    public sealed class FilesBasedTokenCache : TokenCache
    {
        public string AdalV3CacheFilePath { get; }
        public string MsalCacheFilePath { get; }

        private static readonly object _fileLock = new object();

        // Initializes the cache against a local file.
        // If the file is already present, it loads its content in the ADAL cache
        public FilesBasedTokenCache(string adalV3FilePath, string msalCacheFilePath)
        {
            AdalV3CacheFilePath = adalV3FilePath;
            AfterAccess = AfterAccessNotification;
            MsalCacheFilePath = msalCacheFilePath;
            BeforeAccess = BeforeAccessNotification;
            BeforeAccessNotification(null);
        }

        // Empties the persistent store.
        public override void Clear()
        {
            base.Clear();
            File.Delete(AdalV3CacheFilePath);
            File.Delete(MsalCacheFilePath);
        }

        // Triggered right before ADAL needs to access the cache.
        // Reload the cache from the persistent store in case it changed since the last access.
        void BeforeAccessNotification(TokenCacheNotificationArgs args)
        {
            lock (_fileLock)
            {
                DeserializeAdalV3(ReadFromFileIfExists(AdalV3CacheFilePath));
                DeserializeMsalV3(ReadFromFileIfExists(MsalCacheFilePath));
            }
        }

        // Triggered right after ADAL accessed the cache.
        void AfterAccessNotification(TokenCacheNotificationArgs args)
        {
            // if the access operation resulted in a cache update
            if (HasStateChanged)
            {
                lock (_fileLock)
                {
                    // reflect changes in the persistent store
                    WriteToFileIfNotNull(AdalV3CacheFilePath, SerializeAdalV3());
                    WriteToFileIfNotNull(MsalCacheFilePath, SerializeMsalV3());

                    // once the write operation took place, restore the HasStateChanged bit to false
                    this.HasStateChanged = false;
                }
            }
        }

        /// <summary>
        /// Read the content of a file if it exists
        /// </summary>
        /// <param name="path">File path</param>
        /// <returns>Content of the file (in bytes)</returns>
        private byte[] ReadFromFileIfExists(string path)
        {
            var protectedBytes = (!string.IsNullOrEmpty(path) && File.Exists(path)) ? File.ReadAllBytes(path) : null;
            var unprotectedBytes = (protectedBytes != null) ? ProtectedData.Unprotect(protectedBytes, null, DataProtectionScope.CurrentUser) : null;
            return unprotectedBytes;
        }

        /// <summary>
        /// Writes a blob of bytes to a file. If the blob is <c>null</c>, deletes the file
        /// </summary>
        /// <param name="path">path to the file to write</param>
        /// <param name="blob">Blob of bytes to write</param>
        private static void WriteToFileIfNotNull(string path, byte[] blob)
        {
            if (blob != null)
            {
                var protectedBytes = ProtectedData.Protect(blob, null, DataProtectionScope.CurrentUser);
                File.WriteAllBytes(path, protectedBytes);
            }
            else
            {
                File.Delete(path);
            }
        }
    }
}
