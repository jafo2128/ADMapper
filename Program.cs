using System;
using System.Collections.Generic;
using System.Linq;
using Win32DnsApi;
using Win32DnsApi.DnsRecords;

namespace ADMapper
{
    internal class Program
    {
        private static void PrintDnsUsage()
        {
            Console.WriteLine("TODO: Change usage...currently used to test DNS only");
            Console.WriteLine("ADMapper dns <rectype> <name>");
            Console.WriteLine("  <rectype>  a, aaaa, srv, txt, ns, mx, cname, ptr, any");
            Environment.Exit(1);
        }

        private static void PrintUsage()
        {
            Console.WriteLine("TODO: Change usage...curently used to test");
            Console.WriteLine("ADMapper <command> <args>");
            Console.WriteLine("  <command>  dns, cldapping, map");
            Environment.Exit(1);
        }

        private static void HandleDns(string[] args)
        {
            if (args.Count() < 2)
            {
                PrintDnsUsage();
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
                    PrintDnsUsage();
                    return;
            }
            if (!records.Any())
                Console.WriteLine("  no records");
            foreach (var record in records)
            {
                Console.WriteLine($"  {record}");
            }
        }

        private static void HandleCldapping(string[] args)
        {
            
        }

        private static void HandleMap(string[] args)
        {
            
        }

        private static void Main(string[] args)
        {
            if (!args.Any())
            {
                PrintUsage();
            }
            switch (args[0])
            {
                case "dns":
                    HandleDns(args.Skip(1).ToArray());
                    break;
                case "cldapping":
                    HandleCldapping(args.Skip(1).ToArray());
                    break;
                case "map":
                    HandleMap(args.Skip(1).ToArray());
                    break;
                default:
                    PrintUsage();
                    break; nj
            }
        }
    }
}
