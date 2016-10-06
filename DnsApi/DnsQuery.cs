using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using DnsApi.PAB.DnsUtils;

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

        public void LookUp(string name, RecordType recordType, bool bypassResolverCache)
        {
            var internalRecordType = ConvertToCType(recordType);
            var pResults = IntPtr.Zero;
            var status = PInvoke.DnsQuery(ref name, internalRecordType,
                bypassResolverCache
                    ? PInvoke.DnsQueryOptions.DNS_QUERY_BYPASS_CACHE
                    : PInvoke.DnsQueryOptions.DNS_QUERY_STANDARD, IntPtr.Zero, ref pResults, IntPtr.Zero);
            if (status != 0)
            {
                throw new Win32Exception(status);
            }

            PInvoke.DNS_RECORD record;
            for (var iterator = pResults; !iterator.Equals(IntPtr.Zero); iterator = record.pNext)
            {
                record = (PInvoke.DNS_RECORD) Marshal.PtrToStructure(iterator, typeof (PInvoke.DNS_RECORD));
                if (record.wType == (ushort)internalRecordType)
                {
                    switch (record.wType)
                    {
                        case (ushort)PInvoke.DnsRecordTypes.DNS_TYPE_A:
                            break;
                        case (ushort)PInvoke.DnsRecordTypes.DNS_TYPE_NS:
                            break;
                        case (ushort)PInvoke.DnsRecordTypes.DNS_TYPE_CNAME:
                            break;
                        case (ushort)PInvoke.DnsRecordTypes.DNS_TYPE_PTR:
                            break;
                        case (ushort)PInvoke.DnsRecordTypes.DNS_TYPE_MX:
                            break;
                        case (ushort)PInvoke.DnsRecordTypes.DNS_TYPE_TEXT:
                            break;
                        case (ushort)PInvoke.DnsRecordTypes.DNS_TYPE_SRV:
                            break;
                        case (ushort)PInvoke.DnsRecordTypes.DNS_TYPE_AAAA:
                            break;
                        case (ushort)PInvoke.DnsRecordTypes.DNS_TYPE_ANY:
                            break;
                    }
                }
            }
            PInvoke.DnsRecordListFree(pResults, 0);
        }

        public void LookUp(string name, RecordType type)
        {
            LookUp(name, type, false);
        }
    }
}
