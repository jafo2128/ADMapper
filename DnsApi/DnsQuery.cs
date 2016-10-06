using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using DnsApi.DnsRecords;

namespace DnsApi
{
    public class DnsQuery
    {
        public enum RecordType
        {
            ARecord = 0x1,
            NsRecord = 0x2,
            CnameRecord = 0x5,
            PtrRecord = 0xc,
            MxRecord = 0xf,
            TxtRecord = 0x10,
            SrvRecord = 0x21,
            AaaaRecord = 0x1c,
            AnyRecord = 0xff
        }

        private static PInvoke.DnsRecordTypes ConvertToCType(RecordType t)
        {
            return (PInvoke.DnsRecordTypes) t;
        }

        private static PInvoke.DnsRecordTypes ResolveFromType(Type t)
        {
            // TODO:
            return PInvoke.DnsRecordTypes.DNS_TYPE_A;
        }

        public IList<IDnsRecord> LookUp<T>(string name, bool bypassResolverCache) where T : IDnsRecord
        {

            var internalRecordType = ResolveFromType(typeof(T));
            var pResults = IntPtr.Zero;
            var status = PInvoke.DnsQuery(ref name, internalRecordType,
                bypassResolverCache
                    ? PInvoke.DnsQueryOptions.DNS_QUERY_BYPASS_CACHE
                    : PInvoke.DnsQueryOptions.DNS_QUERY_STANDARD, IntPtr.Zero, ref pResults, IntPtr.Zero);
            if (status != 0)
            {
                throw new Win32Exception(status);
            }

            var recordsFound = new List<IDnsRecord>();
            try
            {
                PInvoke.DNS_RECORD record;
                for (var iterator = pResults; !iterator.Equals(IntPtr.Zero); iterator = record.pNext)
                {
                    record = (PInvoke.DNS_RECORD) Marshal.PtrToStructure(iterator, typeof (PInvoke.DNS_RECORD));
                    IDnsRecord recordFound;
                    switch (record.wType)
                    {
                        case (ushort) PInvoke.DnsRecordTypes.DNS_TYPE_A:
                            recordFound =
                                new DnsARecord(IPAddressHelpers.ConvertUintToIpAddress(record.Data.A.IpAddress));
                            break;
                        case (ushort) PInvoke.DnsRecordTypes.DNS_TYPE_NS:
                            recordFound = new DnsNsRecord(Marshal.PtrToStringAuto(record.Data.NS.pNameHost));
                            break;
                        case (ushort) PInvoke.DnsRecordTypes.DNS_TYPE_CNAME:
                            recordFound = new DnsCnameRecord(Marshal.PtrToStringAuto(record.Data.CNAME.pNameHost));
                            break;
                        case (ushort) PInvoke.DnsRecordTypes.DNS_TYPE_PTR:
                            recordFound = new DnsPtrRecord(Marshal.PtrToStringAuto(record.Data.PTR.pNameHost));
                            break;
                        case (ushort) PInvoke.DnsRecordTypes.DNS_TYPE_MX:
                            recordFound = new DnsMxRecord(record.Data.MX.wPreference,
                                Marshal.PtrToStringAuto(record.Data.MX.pNameExchange));
                            break;
                        case (ushort) PInvoke.DnsRecordTypes.DNS_TYPE_TEXT:
                            var stringList = new List<string>();
                            var count = record.Data.TXT.dwStringCount;
                            for (var i = 0; i < count; i++)
                            {
                                stringList.Add(Marshal.PtrToStringAuto(record.Data.TXT.pStringArray + i));
                            }
                            recordFound = new DnsTxtRecord(stringList.ToArray());
                            break;
                        case (ushort) PInvoke.DnsRecordTypes.DNS_TYPE_SRV:
                            recordFound = new DnsSrvRecord(record.Data.SRV.uPriority, record.Data.SRV.wWeight,
                                Marshal.PtrToStringAuto(record.Data.SRV.pNameTarget), record.Data.SRV.wPort);
                            break;
                        case (ushort) PInvoke.DnsRecordTypes.DNS_TYPE_AAAA:
                            recordFound = new DnsAaaaRecord(IPAddressHelpers.ConvertAAAAToIpAddress(record.Data.AAAA));
                            break;
                        default:
                            continue;
                    }
                    if (internalRecordType == PInvoke.DnsRecordTypes.DNS_TYPE_ANY ||
                        record.wType == (ushort) internalRecordType)
                    {
                        recordsFound.Add(recordFound);
                    }
                }
            }
            catch (Exception)
            {
                // TODO:
                throw;
            }
            finally
            {
                PInvoke.DnsRecordListFree(pResults, 0);
            }
            return recordsFound;
        }

        public IList<IDnsRecord> LookUp<T>(string name) where T : IDnsRecord
        {
            return LookUp<T>(name, false);
        }
    }
}
