﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
// ReSharper disable InconsistentNaming

namespace CLDAP.NET
{
    enum Opcode : ushort
    {
        LOGON_PRIMARY_QUERY          = 7,
        LOGON_PRIMARY_RESPONSE       = 12,
        LOGON_SAM_LOGON_REQUEST      = 18,
        LOGON_SAM_LOGON_RESPONSE     = 19,
        LOGON_SAM_PAUSE_RESPONSE     = 20,
        LOGON_SAM_USER_UNKNOWN       = 21,
        LOGON_SAM_LOGON_RESPONSE_EX  = 23,
        LOGON_SAM_PAUSE_RESPONSE_EX  = 24,
        LOGON_SAM_USER_UNKNOWN_EX    = 25
    }

    enum DS_FLAG : uint
    {
        DS_PDC_FLAG                    = 0x00000001,  // The server holds the PDC FSMO role (PdcEmulationMasterRole).
                                                      // FSMO roles are defined in [MS-ADTS] section 3.1.1.1.11. Certain
                                                      // updates can be performed only on the holder of the PDC FSMO role.
        DS_GC_FLAG                     = 0x00000004,  // The server is a GC server and will accept and process messages directed
                                                      // to it on the global catalog ports ([MS-ADTS] section 3.1.1.3.1.10).
        DS_LDAP_FLAG                   = 0x00000008,  // The server is an LDAP server.
        DS_DS_FLAG                     = 0x00000010,  // The server is a DC.
        DS_KDC_FLAG                    = 0x00000020,  // The server is running the Kerberos Key Distribution Center service.
        DS_TIMESERV_FLAG               = 0x00000040,  // The Win32 Time Service, as specified in [MS-W32T], is present on the server.
        DS_CLOSEST_FLAG                = 0x00000080,  // The server is in the same site as the client. This is a hint
                                                      // to the client that it is well-connected to the server in terms of speed.
        DS_WRITABLE_FLAG               = 0x00000100,  // Indicates that the server is not an RODC. As described in [MS-ADTS] section 3.1.1.1.9,
                                                      // all NC replicas hosted on an RODC do not accept originating updates.
        DS_GOOD_TIMESERV_FLAG          = 0x00000200,  // The server is a reliable time server.
        DS_NDNC_FLAG                   = 0x00000400,  // The NC is an application NC.
        DS_SELECT_SECRET_DOMAIN_6_FLAG = 0x00000800,  // The server is an RODC.
        DS_FULL_SECRET_DOMAIN_6_FLAG   = 0x00001000,  // The server is a writable DC, not running Windows 2000 Server or Windows Server 2003.
        DS_WS_FLAG                     = 0x00002000,  // The Active Directory Web Service, as specified in [MS-ADDM],
                                                      // is present on the server.
        DS_DS_8_FLAG                   = 0x00004000,  // The server is not running Windows 2000, Windows Server 2003,
                                                      // Windows Server 2008, or Windows Server 2008 R2.
        DS_DS_9_FLAG                   = 0x00008000,  // The server is not running Windows 2000, Windows Server2003,
                                                      // Windows Server 2008, Windows Server 2008 R2, or Windows Server 2012.
        DS_DNS_CONTROLLER_FLAG         = 0x20000000,  // The server has a DNS name.
        DS_DNS_DOMAIN_FLAG             = 0x40000000,  // The NC is a default NC.
        DS_DNS_FOREST_FLAG             = 0x80000000   // The NC is the forest root
    }

    [StructLayout(LayoutKind.Sequential)]
    struct GUID
    {
        public int a;
        public short b;
        public short c;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] d;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct NETLOGON_SAM_LOGON_RESPONSE_EX_HEADER
    {
        public Opcode Opcode;
        public ushort Sbz;
        public uint Flags; // DS_FLAG bit field
        public GUID DomainGuid;
    }

    // RFC 1035 compressed strings go here
    // See [MS-ADTS] 6.3.7
    //  DnsForestName
    //  DnsDomainName
    //  DnsHostName
    //  NetbiosDomainName
    //  ComputerName
    //  UserName
    //  DcSiteName
    //  ClientSiteName
    //  NextClosestSiteName (Included only if NETLOGON_NT_VERSION_WITH_CLOSEST_SITE is used)
    [StructLayout(LayoutKind.Sequential)]
    internal struct NETLOGON_SAM_LOGON_RESPONSE_EX_FOOTER
    {
        public uint NtVersion;
        public ushort LmNtToken;
        public ushort Lm20Token;
    }

    internal static class NetlogonResponseDecoder
    {
        private static T DecodeStruct<T>(byte[] buf)
        {
            var objPtr = IntPtr.Zero;
            try
            {
                var objSz = Marshal.SizeOf(typeof(T));
                objPtr = Marshal.AllocHGlobal(objSz);
                Marshal.Copy(buf, 0, objPtr, objSz);
                var structure = (T) Marshal.PtrToStructure(objPtr, typeof(T));
                return structure;
            }
            catch
            {
                return default(T);
            }
            finally
            {
                Marshal.FreeHGlobal(objPtr);
            }
        }

        

        private static string[] DecodeCompressedStrings(byte[] buf)
        {
            var strings = new List<string>();
            var startIndex = Marshal.SizeOf(typeof(NETLOGON_SAM_LOGON_RESPONSE_EX_HEADER));
            var endIndex = buf.Length - Marshal.SizeOf(typeof(NETLOGON_SAM_LOGON_RESPONSE_EX_FOOTER));
            var labels = new SortedList<int, string>();
            string str = null;
            for (; startIndex < endIndex; startIndex++)
            {
                if (buf[startIndex] == 0x00)
                {
                    strings.Add(str);
                    str = null;
                    labels.Add(startIndex, null);
                }
                else if ((buf[startIndex] & 0xc0) == 0xc0) // 1100 0000, Bit-8 and Bit-7 set means pointer
                {
                    if (startIndex + 1 >= endIndex)
                    {
                        throw new Exception("TODO: Bad encoding on string ptr...");
                    }
                    var ptr = (int)buf[startIndex + 1];
                    var labelIndex = labels.IndexOfKey(ptr);
                    if (labelIndex == -1)
                    {
                        throw new Exception("TODO: Bad encoded value on string ptr...");
                    }
                    var ptrStr =
                        labels.Skip(labelIndex)
                            .TakeWhile(label => label.Value != null)
                            .Aggregate<KeyValuePair<int, string>, string>(null,
                                (current, label) => current == null ? label.Value : current + "." + label.Value);
                    str = str == null ? ptrStr : str + "." + ptrStr;
                    strings.Add(str);
                    str = null;
                    startIndex++; // Just skip ptr marker, the loop condition will do the rest
                }
                else
                {
                    var sz = (int)buf[startIndex];
                    if (startIndex + 1 + sz >= endIndex)
                    {
                        throw new Exception("TODO: Bad encoding on string label...");
                    }
                    var label = Encoding.UTF8.GetString(buf.Skip(startIndex + 1).Take(sz).ToArray());
                    labels.Add(startIndex, label);
                    str = str == null ? label : str + "." + label;
                    startIndex += sz; // just skip label length, the loop condition will do the rest
                }
            }
            return strings.ToArray();
        }

        public static ServerInformation Decode(byte[] buf)
        {
            var header = DecodeStruct<NETLOGON_SAM_LOGON_RESPONSE_EX_HEADER>(buf);
            var strings = DecodeCompressedStrings(buf);
            var footer =
                DecodeStruct<NETLOGON_SAM_LOGON_RESPONSE_EX_FOOTER>(
                    buf.Skip(buf.Length - Marshal.SizeOf(typeof(NETLOGON_SAM_LOGON_RESPONSE_EX_FOOTER))).ToArray());
            var serverInformation =
                new ServerInformation(
                    new Guid(header.DomainGuid.a, header.DomainGuid.b, header.DomainGuid.c, header.DomainGuid.d),
                    header.Flags, strings);
            if ((footer.NtVersion & 0x04) == 0 || footer.LmNtToken != 0xffff || footer.Lm20Token != 0xffff)
            {
                throw new Exception("TODO: Parsing failed...");
            }
            return serverInformation;
        }
    }
}
