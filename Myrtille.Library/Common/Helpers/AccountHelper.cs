/*
    Myrtille: A native HTML4/5 Remote Desktop Protocol client.

    Copyright(c) 2014-2021 Cedric Coste

    Licensed under the Apache License, Version 2.0 (the "License");
    you may not use this file except in compliance with the License.
    You may obtain a copy of the License at

        http://www.apache.org/licenses/LICENSE-2.0

    Unless required by applicable law or agreed to in writing, software
    distributed under the License is distributed on an "AS IS" BASIS,
    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
    See the License for the specific language governing permissions and
    limitations under the License.
*/

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices.ActiveDirectory;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace Myrtille.Helpers
{
    public static class AccountHelper
    {
        #region Windows Groups

        /// <summary>
        /// retrieve the local machine administrators group name
        /// </summary>
        /// <returns>group name</returns>
        public static string GetAdministratorsGroupName()
        {
            var groupName = "Administrators";

            try
            {
                var sid = new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null);
                var account = sid.Translate(typeof(NTAccount)) as NTAccount;
                groupName = account.ToString().Substring(account.ToString().LastIndexOf("\\") + 1, account.ToString().Length - account.ToString().LastIndexOf("\\") - 1);
            }
            catch (Exception exc)
            {
                Trace.TraceWarning("Failed to retrieve administrators group ({0})", exc);
            }

            return groupName;
        }

        /// <summary>
        /// retrieve the local machine everyone group name
        /// </summary>
        /// <returns>group name</returns>
        public static string GetEveryoneGroupName()
        {
            var groupName = "Everyone";

            try
            {
                var sid = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
                var account = sid.Translate(typeof(NTAccount)) as NTAccount;
                groupName = account.ToString();
            }
            catch (Exception exc)
            {
                Trace.TraceWarning("Failed to retrieve everyone group ({0})", exc);
            }

            return groupName;
        }

        /// <summary>
        /// retrieve the local machine users group name
        /// </summary>
        /// <returns>group name</returns>
        public static string GetUsersGroupName()
        {
            var groupName = "Users";

            try
            {
                var sid = new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null);
                var account = sid.Translate(typeof(NTAccount)) as NTAccount;
                groupName = account.ToString();
            }
            catch (Exception exc)
            {
                Trace.TraceWarning("Failed to retrieve users group ({0})", exc);
            }

            return groupName;
        }

        /// <summary>
        /// retrieve the local machine remote desktop users group name
        /// </summary>
        /// <returns>group name</returns>
        public static string GetRemoteDesktopUsersGroupName()
        {
            var groupName = "Remote Desktop Users";

            try
            {
                var sid = new SecurityIdentifier(WellKnownSidType.BuiltinRemoteDesktopUsersSid, null);
                var account = sid.Translate(typeof(NTAccount)) as NTAccount;
                groupName = account.ToString();
            }
            catch (Exception exc)
            {
                Trace.TraceWarning("Failed to retrieve remote desktop users group ({0})", exc);
            }

            return groupName;
        }

        /// <summary>
        /// retrieve the local machine network service account name
        /// </summary>
        /// <returns>account name</returns>
        public static string GetNetworkServiceAccountName()
        {
            var accountName = "NT AUTHORITY\\NETWORK SERVICE";

            try
            {
                var sid = new SecurityIdentifier(WellKnownSidType.NetworkServiceSid, null);
                var account = sid.Translate(typeof(NTAccount)) as NTAccount;
                accountName = account.Value;
            }
            catch (Exception exc)
            {
                Trace.TraceWarning("Failed to retrieve network service account ({0})", exc);
            }

            return accountName;
        }

        #endregion

        #region Windows Users

        #region Windows Logon

        [DllImport("advapi32.dll", SetLastError = true, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool LogonUser(
            [MarshalAs(UnmanagedType.LPStr)] string pszUserName,
            [MarshalAs(UnmanagedType.LPStr)] string pszDomain,
            [MarshalAs(UnmanagedType.LPStr)] string pszPassword,
            int dwLogonType,
            int dwLogonProvider,
            ref IntPtr phToken);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool CloseHandle(
            IntPtr handle);

        // from winbase.h
        public enum LogonType
        {
            /// <summary>
            /// This logon type is intended for users who will be interactively using the computer, such as a user being logged on  
            /// by a terminal server, remote shell, or similar process.
            /// This logon type has the additional expense of caching logon information for disconnected operations;
            /// therefore, it is inappropriate for some client/server applications,
            /// such as a mail server.
            /// </summary>
            LOGON32_LOGON_INTERACTIVE = 2,

            /// <summary>
            /// This logon type is intended for high performance servers to authenticate plaintext passwords.
            /// The LogonUser function does not cache credentials for this logon type.
            /// </summary>
            LOGON32_LOGON_NETWORK = 3,

            /// <summary>
            /// This logon type is intended for batch servers, where processes may be executing on behalf of a user without
            /// their direct intervention. This type is also for higher performance servers that process many plaintext
            /// authentication attempts at a time, such as mail or Web servers.
            /// The LogonUser function does not cache credentials for this logon type.
            /// </summary>
            LOGON32_LOGON_BATCH = 4,

            /// <summary>
            /// Indicates a service-type logon. The account provided must have the service privilege enabled.
            /// </summary>
            LOGON32_LOGON_SERVICE = 5,

            /// <summary>
            /// This logon type is for GINA DLLs that log on users who will be interactively using the computer.
            /// This logon type can generate a unique audit record that shows when the workstation was unlocked.
            /// </summary>
            LOGON32_LOGON_UNLOCK = 7,

            /// <summary>
            /// This logon type preserves the name and password in the authentication package, which allows the server to make
            /// connections to other network servers while impersonating the client. A server can accept plaintext credentials
            /// from a client, call LogonUser, verify that the user can access the system across the network, and still
            /// communicate with other servers.
            /// NOTE: Windows NT:  This value is not supported.
            /// </summary>
            LOGON32_LOGON_NETWORK_CLEARTEXT = 8,

            /// <summary>
            /// This logon type allows the caller to clone its current token and specify new credentials for outbound connections.
            /// The new logon session has the same local identifier but uses different credentials for other network connections.
            /// NOTE: This logon type is supported only by the LOGON32_PROVIDER_WINNT50 logon provider.
            /// NOTE: Windows NT:  This value is not supported.
            /// </summary>
            LOGON32_LOGON_NEW_CREDENTIALS = 9,
        }

        public enum LogonProvider
        {
            /// <summary>
            /// Use the standard logon provider for the system.
            /// The default security provider is negotiate, unless you pass NULL for the domain name and the user name
            /// is not in UPN format. In this case, the default provider is NTLM.
            /// NOTE: Windows 2000/NT:   The default security provider is NTLM.
            /// </summary>
            LOGON32_PROVIDER_DEFAULT = 0,
            LOGON32_PROVIDER_WINNT35 = 1,
            LOGON32_PROVIDER_WINNT40 = 2,
            LOGON32_PROVIDER_WINNT50 = 3
        }

        #endregion

        #region Windows Network API

        [DllImport("netapi32.dll", SetLastError = true)]
        public static extern NET_API_STATUS NetUserGetInfo(
            [MarshalAs(UnmanagedType.LPWStr)] string servername,
            [MarshalAs(UnmanagedType.LPWStr)] string username,
            int level,
            out IntPtr bufptr);

        [StructLayout(LayoutKind.Sequential)]
        public struct USER_INFO_4
        {
            [MarshalAs(UnmanagedType.LPWStr)]
            public string usri4_name;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string usri4_password;
            public uint usri4_password_age;
            public uint usri4_priv;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string usri4_home_dir;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string usri4_comment;
            public uint usri4_flags;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string usri4_script_path;
            public uint usri4_auth_flags;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string usri4_full_name;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string usri4_usr_comment;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string usri4_parms;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string usri4_workstations;
            public uint usri4_last_logon;
            public uint usri4_last_logoff;
            public uint usri4_acct_expires;
            public uint usri4_max_storage;
            public uint usri4_units_per_week;
            public IntPtr usri4_logon_hours;
            public uint usri4_bad_pw_count;
            public uint usri4_num_logons;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string usri4_logon_server;
            public uint usri4_country_code;
            public uint usri4_code_page;
            public IntPtr usri4_user_sid;
            public uint usri4_primary_group_id;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string usri4_profile;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string usri4_home_dir_drive;
            public uint usri4_password_expired;
        }

        public enum NET_API_STATUS : uint
        {
            NERR_Success = 0,
            NERR_InvalidComputer = 2351,
            NERR_NotPrimary = 2226,
            NERR_SpeGroupOp = 2234,
            NERR_LastAdmin = 2452,
            NERR_BadPassword = 2203,
            NERR_PasswordTooShort = 2245,
            NERR_UserNotFound = 2221,
            ERROR_ACCESS_DENIED = 5,
            ERROR_NOT_ENOUGH_MEMORY = 8,
            ERROR_INVALID_PARAMETER = 87,
            ERROR_INVALID_NAME = 123,
            ERROR_INVALID_LEVEL = 124,
            ERROR_MORE_DATA = 234,
            ERROR_SESSION_CREDENTIAL_CONFLICT = 1219
        }

        #endregion

        #region Windows Users Profiles

        [StructLayout(LayoutKind.Sequential)]
        public struct ProfileInfo
        {
            ///
            /// Specifies the size of the structure, in bytes.
            ///
            public int dwSize;

            ///
            /// This member can be one of the following flags: 
            /// PI_NOUI or PI_APPLYPOLICY
            ///
            public int dwFlags;

            ///
            /// Pointer to the name of the user.
            /// This member is used as the base name of the directory 
            /// in which to store a new profile.
            ///
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpUserName;

            ///
            /// Pointer to the roaming user profile path.
            /// If the user does not have a roaming profile, this member can be NULL.
            ///
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpProfilePath;

            ///
            /// Pointer to the default user profile path. This member can be NULL.
            ///
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpDefaultPath;

            ///
            /// Pointer to the name of the validating domain controller, in NetBIOS format.
            /// If this member is NULL, the Windows NT 4.0-style policy will not be applied.
            ///
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpServerName;

            ///
            /// Pointer to the path of the Windows NT 4.0-style policy file. 
            /// This member can be NULL.
            ///
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpPolicyPath;

            ///
            /// Handle to the HKEY_CURRENT_USER registry key.
            ///
            public IntPtr hProfile;
        }

        [DllImport("userenv.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool LoadUserProfile(
            IntPtr hToken,
            ref ProfileInfo lpProfileInfo);

        [DllImport("userenv.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool UnloadUserProfile(
            IntPtr hToken,
            IntPtr lpProfileInfo);

        // from userenv.h
        private enum ProfileInfoFlags : int
        {
            /// <summary>
            /// Prevents the display of profile error messages.
            /// </summary>
            PI_NOUI = 1,

            /// <summary>
            /// Apply NT4 style policy.
            /// </summary>
            PI_APPLYPOLICY = 2
        }

        #endregion

        #region Windows Known Folders

        [DllImport("shell32.dll", SetLastError = true)]
        public static extern int SHGetKnownFolderPath(
            [MarshalAs(UnmanagedType.LPStruct)]Guid rfid,
            uint dwFlags,
            IntPtr hToken,
            out IntPtr ppszPath);

        private static Guid KNOWNFOLDER_GUID_DOCUMENTS = new Guid("{FDD39AD0-238F-46AF-ADB4-6C85480369C7}");

        // from shlobj.h
        private enum KnownFolderFlags : uint
        {
            SimpleIDList = 0x00000100,
            NotParentRelative = 0x00000200,
            DefaultPath = 0x00000400,
            Init = 0x00000800,
            NoAlias = 0x00001000,
            DontUnexpand = 0x00002000,
            DontVerify = 0x00004000,
            Create = 0x00008000,
            NoAppcontainerRedirection = 0x00010000,
            AliasOnly = 0x80000000
        }

        #endregion

        /// <summary>
        /// retrieve a domain fom an UPN
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="userPassword"></param>
        /// <returns>domain</returns>
        public static string GetDomain(
            string userName,
            string userPassword)
        {
            // https://docs.microsoft.com/en-us/windows/win32/secauthn/user-name-formats

            // DOMAIN\username (domain account) or COMPUTER\username (local account)
            int pos = userName.IndexOf("\\");
            if (pos != -1)
            {
                var name = userName.Substring(0, pos);
                try
                {
                    // check if the name is a domain
                    var context = new DirectoryContext(DirectoryContextType.Domain, name, userName.Substring(pos + 1), userPassword);
                    var domain = Domain.GetDomain(context);
                    // the name is a domain, and the user credentials are valid
                    return name;
                }
                catch (Exception exc)
                {
                    // the name isn't a domain or can't be resolved (check DNS configuration) or contacted, or the user credentials are invalid
                    return string.Empty;
                }
            }
            // username@domain.com
            else
            {
                pos = userName.IndexOf("@");
                if (pos != -1)
                {
                    return userName.Substring(pos + 1);
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// retrieve a username fom an UPN
        /// </summary>
        /// <param name="userName"></param>
        /// <returns>user name</returns>
        public static string GetUserName(
            string userName)
        {
            // https://docs.microsoft.com/en-us/windows/win32/secauthn/user-name-formats

            // DOMAIN\username (domain account) or COMPUTER\username (local account)
            int pos = userName.IndexOf("\\");
            if (pos != -1)
            {
                return userName.Substring(pos + 1);
            }
            // username@domain.com
            else
            {
                pos = userName.IndexOf("@");
                if (pos != -1)
                {
                    return userName.Substring(0, pos);
                }
                else
                {
                    return userName;
                }
            }
        }

        /// <summary>
        /// retrieve an user documents folder; also validates the user credentials to prevent unauthorized access to this folder
        /// </summary>
        /// <param name="userDomain"></param>
        /// <param name="userName"></param>
        /// <param name="userPassword"></param>
        /// <returns>documents directory</returns>
        public static string GetUserDocumentsFolder(
            string userDomain,
            string userName,
            string userPassword)
        {
            var token = IntPtr.Zero;

            try
            {
                // logon the user, domain (if defined) or local otherwise
                // myrtille must be running on a machine which is part of the domain for it to work
                if (LogonUser(userName, string.IsNullOrEmpty(userDomain) ? Environment.MachineName : userDomain, userPassword, string.IsNullOrEmpty(userDomain) ? (int)LogonType.LOGON32_LOGON_INTERACTIVE : (int)LogonType.LOGON32_LOGON_NETWORK, (int)LogonProvider.LOGON32_PROVIDER_DEFAULT, ref token))
                {
                    string serverName = null;
                    if (!string.IsNullOrEmpty(userDomain))
                    {
                        var context = new DirectoryContext(DirectoryContextType.Domain, userDomain, userName, userPassword);
                        var controller = Domain.GetDomain(context).FindDomainController();
                        serverName = controller.Name;
                    }

                    IntPtr bufPtr;
                    if (NetUserGetInfo(serverName, userName, 4, out bufPtr) == NET_API_STATUS.NERR_Success)
                    {
                        var userInfo = new USER_INFO_4();
                        userInfo = (USER_INFO_4)Marshal.PtrToStructure(bufPtr, typeof(USER_INFO_4));

                        var profileInfo = new ProfileInfo
                        {
                            dwSize = Marshal.SizeOf(typeof(ProfileInfo)),
                            dwFlags = (int)ProfileInfoFlags.PI_NOUI,
                            lpServerName = string.IsNullOrEmpty(userDomain) ? Environment.MachineName : serverName.Split(new[] { "." }, StringSplitOptions.None)[0],
                            lpUserName = string.IsNullOrEmpty(userDomain) ? userName : string.Format(@"{0}\{1}", userDomain, userName),
                            lpProfilePath = userInfo.usri4_profile
                        };

                        // load the user profile (roaming if a domain is defined, local otherwise), in order to have it mounted into the registry hive (HKEY_CURRENT_USER)
                        // the user must have logged on at least once for windows to create its profile (this is forcibly done as myrtille requires an active remote session for the user to enable file transfer)
                        if (LoadUserProfile(token, ref profileInfo))
                        {
                            if (profileInfo.hProfile != IntPtr.Zero)
                            {
                                try
                                {
                                    // retrieve the user documents folder path, possibly redirected by a GPO to a network share (read/write accessible to domain users)
                                    // ensure the user doesn't have exclusive rights on it (otherwise myrtille won't be able to access it)
                                    IntPtr outPath;
                                    var result = SHGetKnownFolderPath(KNOWNFOLDER_GUID_DOCUMENTS, (uint)KnownFolderFlags.DontVerify, token, out outPath);
                                    if (result == 0)
                                    {
                                        return Marshal.PtrToStringUni(outPath);
                                    }
                                }
                                finally
                                {
                                    UnloadUserProfile(token, profileInfo.hProfile);
                                }
                            }
                        }
                    }
                }
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            catch (Exception exc)
            {
                Trace.TraceError("Failed to retrieve user {0} documents folder ({1})", userName, exc);
                throw;
            }
            finally
            {
                if (token != IntPtr.Zero)
                {
                    CloseHandle(token);
                }
            }
        }

        /// <summary>
        /// retrieve an user home folder; also validates the user credentials to prevent unauthorized access to this folder
        /// </summary>
        /// <param name="userDomain"></param>
        /// <param name="userName"></param>
        /// <param name="userPassword"></param>
        /// <remarks>doesn't works in a reliable way (HomeDirectory is often null)</remarks>
        /// <returns>home directory</returns>
        public static string GetUserHomeFolder(
            string userDomain,
            string userName,
            string userPassword)
        {
            try
            {
                using (var context = new PrincipalContext(ContextType.Domain, userDomain, userName, userPassword))
                {
                    var user = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, userName);
                    return user.HomeDirectory;
                }
            }
            catch (Exception exc)
            {
                Trace.TraceError("Failed to retrieve user {0} home folder ({1})", userName, exc);
                throw;
            }
        }

        /// <summary>
        /// create a local user
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="description"></param>
        /// <param name="userPassword"></param>
        /// <param name="userCannotChangePassword"></param>
        /// <param name="passwordNeverExpires"></param>
        public static void CreateLocalUser(
            string userName,
            string description,
            string userPassword,
            bool userCannotChangePassword,
            bool passwordNeverExpires)
        {
            try
            {
                using (var context = new PrincipalContext(ContextType.Machine))
                {
                    var user = UserPrincipal.FindByIdentity(context, userName);
                    if (user == null)
                    {
                        user = new UserPrincipal(context);
                        user.Name = userName;
                        user.DisplayName = userName;
                        user.Description = description;
                        user.SetPassword(userPassword);
                        user.UserCannotChangePassword = userCannotChangePassword;
                        user.PasswordNeverExpires = passwordNeverExpires;
                        user.Save();
                    }
                }
            }
            catch (Exception exc)
            {
                Trace.TraceError("Failed to create local user {0} ({1})", userName, exc);
                throw;
            }
        }

        /// <summary>
        /// add a local user to a group
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="groupName"></param>
        public static void AddLocalUserToGroup(
            string userName,
            string groupName)
        {
            try
            {
                using (var context = new PrincipalContext(ContextType.Machine))
                {
                    var user = UserPrincipal.FindByIdentity(context, userName);
                    if (user != null)
                    {
                        var group = GroupPrincipal.FindByIdentity(context, groupName);
                        if (group != null && !group.Members.Contains(user))
                        {
                            group.Members.Add(user);
                            group.Save();
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                Trace.TraceError("Failed to add local user {0} to group {1} ({2})", userName, groupName, exc);
                throw;
            }
        }

        /// <summary>
        /// delete a local user
        /// </summary>
        /// <param name="userName"></param>
        public static void DeleteLocalUser(
            string userName)
        {
            try
            {
                using (var context = new PrincipalContext(ContextType.Machine))
                {
                    var user = UserPrincipal.FindByIdentity(context, userName);
                    if (user != null)
                    {
                        user.Delete();
                    }
                }
            }
            catch (Exception exc)
            {
                Trace.TraceError("Failed to delete local user {0} ({1})", userName, exc);
                throw;
            }
        }

        #endregion
    }
}