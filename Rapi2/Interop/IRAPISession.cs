using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Rapi2.Interop
{
    [Guid("76a78b7d-8e54-4c06-ac38-459e6a1ab5e3"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IRAPISession
    {
        void CeRapiInit();

        void CeRapiUninit();

        [PreserveSig]
        int CeGetLastError();

        [PreserveSig]
        int CeRapiGetError();

        void CeRapiFreeBuffer(
            [In] IntPtr Buffer);

        [PreserveSig]
        IntPtr CeFindFirstFile(
            [In, MarshalAs(UnmanagedType.LPWStr)] string FileName,
            [In, Out, MarshalAs(UnmanagedType.Struct)] ref CE_FIND_DATA FindData);

        [PreserveSig]
        int CeFindNextFile(
            [In] IntPtr FoundFile,
            [In, Out, MarshalAs(UnmanagedType.Struct)] ref CE_FIND_DATA FindData);

        [PreserveSig]
        int CeFindClose(
            [In] IntPtr FoundFile);

        [PreserveSig]
        uint CeGetFileAttributes(
            [In, MarshalAs(UnmanagedType.LPWStr)] string FileName);

        [PreserveSig]
        int CeSetFileAttributes(
            [In, MarshalAs(UnmanagedType.LPWStr)] string FileName,
            [In] uint FileAttrib);

        [PreserveSig]
        IntPtr CeCreateFile(
            [In, MarshalAs(UnmanagedType.LPWStr)] string lpFileName,
            [In] uint dwDesiredAccess,
            [In] uint dwShareMode,
            [In] IntPtr lpSecurityAttributes,
            [In] uint dwCreationDistribution,
            [In] uint dwFlagsAndAttributes,
            [In] IntPtr hTemplateFile);

        [PreserveSig]
        int CeReadFile(
            [In] IntPtr hFile,
            [In, Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] byte[] lpBuffer,
            [In] uint nNumberOfBytesToRead,
            [In, Out] ref int lpNumberOfBytesRead,
            [In] IntPtr lpOverlapped);

        [PreserveSig]
        int CeWriteFile(
            [In] IntPtr hFile,
            [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] byte[] lpBuffer,
            [In] int nNumberOfBytesToWrite,
            [In, Out] ref int lpNumberOfBytesWritten,
            [In] IntPtr lpOverlapped);

        [PreserveSig]
        int CeCloseHandle(
            [In] IntPtr hObject);

        [PreserveSig]
        int CeFindAllFiles(
            [In, MarshalAs(UnmanagedType.LPWStr)] string Path,
            [In] int Flags,
            [In, Out] ref int pFoundCount,
            [Out] out IntPtr ppFindDataArray);

        [PreserveSig]
        IntPtr CeFindFirstDatabase(
            [In] uint dwDbaseType);

        [PreserveSig]
        uint CeFindNextDatabase(
            [In] IntPtr hEnum);

        [PreserveSig]
        int CeOidGetInfo([In] uint oid, [In, Out, MarshalAs(UnmanagedType.Struct)] ref CEOIDINFO poidInfo);

        [PreserveSig]
        uint CeCreateDatabase(
            [In, MarshalAs(UnmanagedType.LPWStr)] string lpszName,
            [In] uint dwDbaseType,
            [In] ushort cNumSortOrder,
            [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] SortOrderDescriptor[] rgSortSpecs);

        [PreserveSig]
        IntPtr CeOpenDatabase(
            [In, Out] ref uint poid,
            [In, MarshalAs(UnmanagedType.LPWStr)] string lpszName,
            [In] uint propid,
            [In] int dwFlags,
            [In] IntPtr hwndNotify);

        [PreserveSig]
        int CeDeleteDatabase(
            [In] uint oidDbase);

        [PreserveSig]
        uint CeReadRecordProps(
            [In] IntPtr hDbase,
            [In] uint dwFlags,
            [In, Out] ref ushort lpcPropID,
            //[In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] uint[] rgPropID,
            [In] uint[] rgPropID,
            [In, Out] ref IntPtr lplpBuffer,
            [In, Out] ref int lpcbBuffer);

        [PreserveSig]
        uint CeWriteRecordProps(
            [In] IntPtr hDbase,
            [In] uint oidRecord,
            [In] ushort cPropID,
            [In, MarshalAs(UnmanagedType.LPArray)] RemoteDatabase.PropertyValue[] rgPropVal);

        [PreserveSig]
        int CeDeleteRecord(
            [In] IntPtr hDatabase,
            [In] uint oidRecord);

        [PreserveSig]
        uint CeSeekDatabase(
            [In] IntPtr hDatabase,
            [In] uint dwSeekType,
            [In] uint dwValue,
            [In, Out] ref int lpdwIndex);

        [PreserveSig]
        int CeSetDatabaseInfo(
            [In] uint oidDbase,
            ref IntPtr /*CEDBASEINFO*/ pNewInfo);

        [PreserveSig]
        uint CeSetFilePointer(
            [In] IntPtr hFile,
            [In] int lDistanceToMove,
            [In, Out] ref int lpDistanceToMoveHigh,
            [In] uint dwMoveMethod);

        [PreserveSig]
        int CeSetEndOfFile(
            [In] IntPtr hFile);

        [PreserveSig]
        int CeCreateDirectory(
            [In, MarshalAs(UnmanagedType.LPWStr)] string lpPathName,
            [In] IntPtr lpSecurityAttributes);

        [PreserveSig]
        int CeRemoveDirectory(
            [In, MarshalAs(UnmanagedType.LPWStr)] string lpPathName);

        [PreserveSig]
        int CeCreateProcess(
            [In, MarshalAs(UnmanagedType.LPWStr)] string lpszImageName,
            [In, MarshalAs(UnmanagedType.LPWStr)] string lpszCmdLine,
            [In] int lpsaProcess,
            [In] int lpsaThread,
            [In] int fInheritHandles,
            [In] int fdwCreate,
            [In] int lpvEnvironment,
            [In] int lpszCurDir,
            [In] int lpsiStartInfo,
            [Out] out PROCESS_INFORMATION lppiProcInfo);

        [PreserveSig]
        int CeMoveFile(
            [In, MarshalAs(UnmanagedType.LPWStr)] string lpExistingFileName,
            [In, MarshalAs(UnmanagedType.LPWStr)] string lpNewFileName);

        [PreserveSig]
        int CeCopyFile(
            [In, MarshalAs(UnmanagedType.LPWStr)] string lpExistingFileName,
            [In, MarshalAs(UnmanagedType.LPWStr)] string lpNewFileName,
            [In] int bFailIfExists);

        [PreserveSig]
        int CeDeleteFile(
            [In, MarshalAs(UnmanagedType.LPWStr)] string FileName);

        [PreserveSig]
        uint CeGetFileSize(
            [In] IntPtr hFile,
            [In, Out] ref uint lpFileSizeHigh);

        [PreserveSig]
        int CeRegOpenKeyEx(
            [In] uint hKey,
            [In, MarshalAs(UnmanagedType.LPWStr)] string lpszSubKey,
            [In] uint dwReserved,
            [In] uint samDesired,
            [In, Out] ref uint phkResult);

        [PreserveSig]
        int CeRegEnumKeyEx(
            [In] uint hKey,
            [In] uint dwIndex,
            [In, Out] StringBuilder lpName,
            [In, Out] ref uint lpcbName,
            [In] int lpReserved,
            [In, Out] StringBuilder lpClass,
            [In, Out] ref uint lpcbClass,
            [In] IntPtr lpftLastWriteTime);

        [PreserveSig]
        int CeRegCreateKeyEx(
            [In] uint hKey,
            [In, MarshalAs(UnmanagedType.LPWStr)] string lpszSubKey,
            [In] int dwReserved,
            [In, MarshalAs(UnmanagedType.LPWStr)] string lpszClass,
            [In] uint fdwOptions,
            [In] uint samDesired,
            [In] IntPtr lpSecurityAttributes,
            [In, Out] ref uint phkResult,
            [In, Out] ref uint lpdwDisposition);

        [PreserveSig]
        int CeRegCloseKey(
            [In] uint hKey);

        [PreserveSig]
        int CeRegDeleteKey(
            [In] uint hKey,
            [In, MarshalAs(UnmanagedType.LPWStr)] string lpszSubKey);

        [PreserveSig]
        int CeRegEnumValue(
            [In] uint hKey,
            [In] uint dwIndex,
            [In, Out] StringBuilder lpszValueName,
            [In, Out] ref uint lpcbValueName,
            [In, Out] int lpReserved,
            [Out] out uint lpType,
            [In, Out] IntPtr lpData,
            [In, Out] ref uint lpcbData);

        [PreserveSig]
        int CeRegDeleteValue(
            [In] uint hKey,
            [In, MarshalAs(UnmanagedType.LPWStr)] string lpszValueName);

        [PreserveSig]
        int CeRegQueryInfoKey(
            [In] uint hKey,
            [Out, MarshalAs(UnmanagedType.LPWStr)] string lpClass,
            [In, Out] ref int lpcbClass,
            [In] IntPtr lpReserved,
            [Out] out int lpcSubKeys,
            [Out] out int lpcbMaxSubKeyLen,
            [Out] out int lpcbMaxClassLen,
            [Out] out int lpcValues,
            [Out] out int lpcbMaxValueNameLen,
            [Out] out int lpcbMaxValueLen,
            [In] IntPtr lpcbSecurityDescriptor,
            [In] IntPtr lpftLastWriteTime);

        [PreserveSig]
        int CeRegQueryValueEx(
            [In] uint hKey,
            [In, MarshalAs(UnmanagedType.LPWStr)] string lpValueName,
            [In] IntPtr lpReserved,
            [Out] out int lpType,
            [In, Out] IntPtr lpData,
            [In, Out] ref int lpcbData);

        [PreserveSig]
        int CeRegSetValueEx(
            [In] uint hKey,
            [In, MarshalAs(UnmanagedType.LPWStr)] string lpValueName,
            [In] int Reserved,
            [In] int dwType,
            [In] IntPtr lpData,
            [In] int cbData);

        [PreserveSig]
        int CeGetStoreInformation(
            ref StoreInfo lpsi);

        [PreserveSig]
        int CeGetSystemMetrics(
            [In] int nIndex);

        [PreserveSig]
        int CeGetDesktopDeviceCaps(
            [In] int nIndex);

        [PreserveSig]
        int CeFindAllDatabases(
            [In] int DbaseType,
            [In] ushort Flags,
            [In, Out] ref ushort cFindData,
            [Out] out IntPtr ppFindData);

        void CeGetSystemInfo(ref SystemInformation sysInfo);

        [PreserveSig]
        int CeSHCreateShortcut(
            [In, MarshalAs(UnmanagedType.LPWStr)] string lpszShortcut,
            [In, MarshalAs(UnmanagedType.LPWStr)] string lpszTarget);

        [PreserveSig]
        int CeSHGetShortcutTarget(
            [In, MarshalAs(UnmanagedType.LPWStr)] string lpszShortcut,
            [In, Out] StringBuilder lpszTarget,
            [In] int cbMax);

        [PreserveSig]
        int CeCheckPassword(
            [In, MarshalAs(UnmanagedType.LPWStr)] string lpszPassword);

        [PreserveSig]
        int CeGetFileTime(
            [In] IntPtr hFile,
            [In, Out] ref System.Runtime.InteropServices.ComTypes.FILETIME lpCreationTime,
            [In, Out] ref System.Runtime.InteropServices.ComTypes.FILETIME lpLastAccessTime,
            [In, Out] ref System.Runtime.InteropServices.ComTypes.FILETIME lpLastWriteTime);

        [PreserveSig]
        int CeSetFileTime(
            [In] IntPtr hFile,
            [In] CFILETIME lpCreationTime,
            [In] CFILETIME lpLastAccessTime,
            [In] CFILETIME lpLastWriteTime);

        [PreserveSig]
        int CeGetVersionEx(
            ref CEOSVERSIONINFO lpVersionInformation);

        [PreserveSig]
        IntPtr CeGetWindow(
            [In] IntPtr hWnd,
            [In] uint uCmd);

        [PreserveSig]
        int CeGetWindowLong(
            [In] IntPtr hWnd,
            [In] int nIndex);

        [PreserveSig]
        int CeGetWindowText(
            [In] IntPtr hWnd,
            [In, MarshalAs(UnmanagedType.LPWStr)] string lpString,
            [In] int nMaxCount);

        [PreserveSig]
        int CeGetClassName(
            [In] IntPtr hWnd,
            [In, MarshalAs(UnmanagedType.LPWStr)] string lpClassName,
            [In] int nMaxCount);

        void CeGlobalMemoryStatus(
            ref MemoryStatus lpmst);

        [PreserveSig]
        int CeGetSystemPowerStatusEx(
            ref PowerStatus pstatus,
            [In] int fUpdate);

        [PreserveSig]
        uint CeGetTempPath(
            [In] int nBufferLength,
            [In, Out] StringBuilder lpBuffer);

        [PreserveSig]
        uint CeGetSpecialFolderPath(
            [In] int nFolder,
            [In] int nBufferLength,
            [In, Out] StringBuilder lpBuffer);

        void CeRapiInvoke(
            [In, MarshalAs(UnmanagedType.LPWStr)] string pDllPath,
            [In, MarshalAs(UnmanagedType.LPWStr)] string pFunctionName,
            [In] int cbInput,
            [In] IntPtr pInput,
            [Out] out int pcbOutput,
            [Out] out IntPtr ppOutput,
            [In, Out, MarshalAs(UnmanagedType.IUnknown)] ref IntPtr ppIRAPIStream,
            [In] int dwReserved);

        [PreserveSig]
        IntPtr CeFindFirstDatabaseEx(
            ref Guid pguid,
            [In] int dwDbaseType);

        [PreserveSig]
        uint CeFindNextDatabaseEx(
            [In] IntPtr hEnum,
            ref Guid pguid);

        [PreserveSig]
        uint CeCreateDatabaseEx(
            ref Guid pceguid,
            ref IntPtr /*CEDBASEINFO*/ lpCEDBInfo);

        [PreserveSig]
        int CeSetDatabaseInfoEx(
            ref Guid pceguid,
            [In] uint oidDbase,
            ref IntPtr /*CEDBASEINFO*/ pNewInfo);

        [PreserveSig]
        IntPtr CeOpenDatabaseEx(
            [In] ref Guid pceguid,
            ref uint poid,
            [In, MarshalAs(UnmanagedType.LPWStr)] string lpszName,
            int propid,
            [In] int dwFlags,
            [In] IntPtr pReq);

        [PreserveSig]
        int CeDeleteDatabaseEx(
            [In] ref Guid pceguid,
            [In] uint oidDbase);

        [PreserveSig]
        uint CeReadRecordPropsEx(
            [In] IntPtr hDbase,
            [In] uint dwFlags,
            [In, Out] ref ushort lpcPropID,
            [In] ref uint[] rgPropID,
            [In, Out] ref IntPtr lplpBuffer,
            [In, Out] ref int lpcbBuffer,
            [In] IntPtr hHeap);

        [PreserveSig]
        int CeMountDBVol(
            ref Guid pceguid,
            [In, MarshalAs(UnmanagedType.LPWStr)] string lpszDBVol,
            [In] int dwFlags);

        [PreserveSig]
        int CeUnmountDBVol(
            ref Guid pceguid);

        [PreserveSig]
        int CeFlushDBVol(
            ref Guid pceguid);

        [PreserveSig]
        int CeEnumDBVolumes(
            ref Guid pceguid,
            [In, MarshalAs(UnmanagedType.LPWStr)] string lpBuf,
            [In] int dwNumChars);

        [PreserveSig]
        int CeOidGetInfoEx(
            ref Guid pceguid,
            [In] uint oid,
            ref IntPtr /*CEOIDINFO*/ oidInfo);

        void CeSyncStart(
            [In, MarshalAs(UnmanagedType.LPWStr)] string szCommand);

        void CeSyncStop();

        [PreserveSig]
        int CeQueryInstructionSet(
            [In] int dwInstructionSet,
            [In, Out] ref int lpdwCurrentInstructionSet);

        [PreserveSig]
        int CeGetDiskFreeSpaceEx(
            [In, MarshalAs(UnmanagedType.LPWStr)] string lpDirectoryName,
            ref ulong lpFreeBytesAvailableToCaller,
            ref ulong lpTotalNumberOfBytes,
            ref ulong lpTotalNumberOfFreeBytes);

    };
}