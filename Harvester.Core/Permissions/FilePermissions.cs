using System.Security.AccessControl;

namespace ZondervanLibrary.Harvester.Core.Permissions
{
    public static class FilePermissions
    {
        /// <summary>
        /// Create Directory Permissions that allow universal access
        /// </summary>
        /// <returns>Directory Permissions that allow universal access</returns>
        public static DirectorySecurity CreateDirectoryPermissions()
        {
            DirectorySecurity security = new DirectorySecurity();
            FileSystemRights directoryFlags = FileSystemRights.ReadData | FileSystemRights.WriteData | FileSystemRights.AppendData | FileSystemRights.ReadExtendedAttributes | FileSystemRights.WriteExtendedAttributes | FileSystemRights.ExecuteFile | FileSystemRights.DeleteSubdirectoriesAndFiles | FileSystemRights.ReadAttributes | FileSystemRights.WriteAttributes | FileSystemRights.Delete | FileSystemRights.ReadPermissions | FileSystemRights.ChangePermissions | FileSystemRights.TakeOwnership | FileSystemRights.Synchronize | FileSystemRights.FullControl;

            FileSystemAccessRule accRule = new FileSystemAccessRule("SYSTEM", directoryFlags, InheritanceFlags.None, PropagationFlags.InheritOnly, AccessControlType.Allow);
            security.ResetAccessRule(accRule);

            return security;
        }

        /// <summary>
        /// Create File Permissions that allow universal access
        /// </summary>
        /// <returns>File Permissions that allow universal access</returns>
        public static FileSecurity CreateFilePermissions()
        {
            FileSecurity security = new FileSecurity();
            FileSystemRights fileFlags = FileSystemRights.ReadData | FileSystemRights.WriteData | FileSystemRights.AppendData | FileSystemRights.ReadExtendedAttributes | FileSystemRights.WriteExtendedAttributes | FileSystemRights.ExecuteFile | FileSystemRights.DeleteSubdirectoriesAndFiles | FileSystemRights.ReadAttributes | FileSystemRights.WriteAttributes | FileSystemRights.Delete | FileSystemRights.ReadPermissions | FileSystemRights.ChangePermissions | FileSystemRights.TakeOwnership | FileSystemRights.Synchronize | FileSystemRights.FullControl;

            FileSystemAccessRule accRule = new FileSystemAccessRule("SYSTEM", fileFlags, InheritanceFlags.None, PropagationFlags.InheritOnly, AccessControlType.Allow);
            security.ResetAccessRule(accRule);

            return security;
        }
    }
}
