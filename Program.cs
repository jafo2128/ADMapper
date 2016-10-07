using System;
using System.Collections.Generic;
using System.Linq;
using Win32DnsApi;
using Win32DnsApi.DnsRecords;

namespace ADMapper
{
    class Program
    {
        static private void PrintUsage()
        {
            Console.WriteLine("TODO: Change usage...currently used to test DNS only");
            Console.WriteLine("ADMapper <rectype> <name>");
            Console.WriteLine("  <rectype>  a, aaaa, srv, txt, ns, mx, cname, ptr, any");
            Environment.Exit(1);
        }

        static void Main(string[] args)
        {
            if (args.Count() < 2)
            {
                PrintUsage();
            }
            IList<DnsRecordBase> records;
            Console.WriteLine($"Executing query for name '{args[1]}' record type '{args[0]}':");
            switch (args[0])
            {
                case "a":
                    records = DnsQuery.LookUp<DnsARecord>(args[1]).Cast<DnsRecordBase>().ToList();
                    break;
                case "aaaa":
                    records = DnsQuery.LookUp<DnsAaaaRecord>(args[1]).Cast<DnsRecordBase>().ToList();
                    break;
                case "srv":
                    records = DnsQuery.LookUp<DnsSrvRecord>(args[1]).Cast<DnsRecordBase>().ToList();
                    break;
                case "txt":
                    records = DnsQuery.LookUp<DnsTxtRecord>(args[1]).Cast<DnsRecordBase>().ToList();
                    break;
                case "ns":
                    records = DnsQuery.LookUp<DnsNsRecord>(args[1]).Cast<DnsRecordBase>().ToList();
                    break;
                case "mx":
                    records = DnsQuery.LookUp<DnsMxRecord>(args[1]).Cast<DnsRecordBase>().ToList();
                    break;
                case "cname":
                    records = DnsQuery.LookUp<DnsCnameRecord>(args[1]).Cast<DnsRecordBase>().ToList();
                    break;
                case "ptr":
                    records = DnsQuery.LookUp<DnsPtrRecord>(args[1]).Cast<DnsRecordBase>().ToList();
                    break;
                case "any":
                    records = DnsQuery.LookUp<DnsRecordBase>(args[1]);
                    break;
                default:
                    PrintUsage();
                    return;
            }
            if (!records.Any())
                Console.WriteLine("  no records");
            foreach (var record in records)
            {
                Console.WriteLine($"  {record}");
            }
        }
    }
}
