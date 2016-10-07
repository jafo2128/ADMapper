using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using Win32DnsApi.DnsRecords;

namespace Win32DnsApi
{
    public static class DnsQuery
    {
        private static PInvoke.DnsRecordTypes ResolveFromType(Type t)
        {
            if (t == typeof(DnsARecord))
                return PInvoke.DnsRecordTypes.DNS_TYPE_A;
            if (t == typeof(DnsNsRecord))
                return PInvoke.DnsRecordTypes.DNS_TYPE_NS;
            if (t == typeof(DnsCnameRecord))
                return PInvoke.DnsRecordTypes.DNS_TYPE_CNAME;
            if (t == typeof(DnsPtrRecord))
                return PInvoke.DnsRecordTypes.DNS_TYPE_PTR;
            if (t == typeof(DnsMxRecord))
                return PInvoke.DnsRecordTypes.DNS_TYPE_MX;
            if (t == typeof(DnsTxtRecord))
                return PInvoke.DnsRecordTypes.DNS_TYPE_TEXT;
            if (t == typeof(DnsSrvRecord))
                return PInvoke.DnsRecordTypes.DNS_TYPE_SRV;
            if (t == typeof(DnsAaaaRecord))
                return PInvoke.DnsRecordTypes.DNS_TYPE_AAAA;
            return PInvoke.DnsRecordTypes.DNS_TYPE_ANY;
        }

        public static IList<T> LookUp<T>(string name, bool bypassResolverCache) where T : DnsRecordBase
        {
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
            {
                throw new DnsApiException($"Platform '{Environment.OSVersion.Platform}' not supported",
                    new NotSupportedException());
            }
            var internalRecordType = ResolveFromType(typeof(T));
            var pResults = IntPtr.Zero;
            var status = PInvoke.DnsQuery(ref name, internalRecordType,
                bypassResolverCache
                    ? PInvoke.DnsQueryOptions.DNS_QUERY_BYPASS_CACHE
                    : PInvoke.DnsQueryOptions.DNS_QUERY_STANDARD, IntPtr.Zero, ref pResults, IntPtr.Zero);
            if (status == 9501) // DNS_INFO_NO_RECORDS
            {
                return new List<T>();
            }
            if (status != 0)
            {
                throw new DnsApiException($"Error resolving '{name}' from DNS with record type {internalRecordType}",
                    new Win32Exception(status));
            }

            var recordsFound = new List<DnsRecordBase>();
            try
            {
                PInvoke.DNS_RECORD record;
                for (var iterator = pResults; !iterator.Equals(IntPtr.Zero); iterator = record.pNext)
                {
                    record = (PInvoke.DNS_RECORD) Marshal.PtrToStructure(iterator, typeof (PInvoke.DNS_RECORD));
                    DnsRecordBase recordBaseFound;
                    switch (record.wType)
                    {
                        case (ushort) PInvoke.DnsRecordTypes.DNS_TYPE_A:
                            recordBaseFound =
                                new DnsARecord(IPAddressHelpers.ConvertUintToIpAddress(record.Data.A.IpAddress));
                            break;
                        case (ushort) PInvoke.DnsRecordTypes.DNS_TYPE_NS:
                            recordBaseFound = new DnsNsRecord(Marshal.PtrToStringAuto(record.Data.NS.pNameHost));
                            break;
                        case (ushort) PInvoke.DnsRecordTypes.DNS_TYPE_CNAME:
                            recordBaseFound = new DnsCnameRecord(Marshal.PtrToStringAuto(record.Data.CNAME.pNameHost));
                            break;
                        case (ushort) PInvoke.DnsRecordTypes.DNS_TYPE_PTR:
                            recordBaseFound = new DnsPtrRecord(Marshal.PtrToStringAuto(record.Data.PTR.pNameHost));
                            break;
                        case (ushort) PInvoke.DnsRecordTypes.DNS_TYPE_MX:
                            recordBaseFound = new DnsMxRecord(record.Data.MX.wPreference,
                                Marshal.PtrToStringAuto(record.Data.MX.pNameExchange));
                            break;
                        case (ushort) PInvoke.DnsRecordTypes.DNS_TYPE_TEXT:
                            var stringList = new List<string>();
                            var count = record.Data.TXT.dwStringCount;
                            for (var i = 0; i < count; i++)
                            {
                                var strPtr = IntPtr.Add(record.Data.TXT.pStringArray, i*4);
                                var str = Marshal.PtrToStringAuto(strPtr);
                                stringList.Add(str);
                            }
                            recordBaseFound = new DnsTxtRecord(stringList.ToArray());
                            break;
                        case (ushort) PInvoke.DnsRecordTypes.DNS_TYPE_SRV:
                            recordBaseFound = new DnsSrvRecord(record.Data.SRV.uPriority, record.Data.SRV.wWeight,
                                Marshal.PtrToStringAuto(record.Data.SRV.pNameTarget), record.Data.SRV.wPort);
                            break;
                        case (ushort) PInvoke.DnsRecordTypes.DNS_TYPE_AAAA:
                            recordBaseFound = new DnsAaaaRecord(IPAddressHelpers.ConvertAAAAToIpAddress(record.Data.AAAA));
                            break;
                        default:
                            continue;
                    }
                    if (internalRecordType == PInvoke.DnsRecordTypes.DNS_TYPE_ANY ||
                        record.wType == (ushort) internalRecordType)
                    {
                        recordsFound.Add(recordBaseFound);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new DnsApiException("Error parsing DNS response", ex);
            }
            finally
            {
                PInvoke.DnsRecordListFree(pResults, 0);
            }
            return recordsFound.Cast<T>().ToList();
        }

        public static IList<T> LookUp<T>(string name) where T : DnsRecordBase
        {
            return LookUp<T>(name, false);
        }
    }
}
