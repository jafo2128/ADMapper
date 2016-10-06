using System;
using System.Collections.Generic;
using System.Linq;
using DnsApi;
using DnsApi.DnsRecords;

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
            Console.WriteLine("Executing query for name '{args[1]}' record type '{args[0]}':");
            switch (args[0])
            {
                case "a":
                    records = (IList<DnsRecordBase>) DnsQuery.LookUp<DnsARecord>(args[1]);
                    break;
                case "aaaa":
                    records = (IList<DnsRecordBase>)DnsQuery.LookUp<DnsARecord>(args[1]);
                    break;
                case "srv":
                    records = (IList<DnsRecordBase>)DnsQuery.LookUp<DnsARecord>(args[1]);
                    break;
                case "txt":
                    records = (IList<DnsRecordBase>)DnsQuery.LookUp<DnsARecord>(args[1]);
                    break;
                case "ns":
                    records = (IList<DnsRecordBase>)DnsQuery.LookUp<DnsARecord>(args[1]);
                    break;
                case "mx":
                    records = (IList<DnsRecordBase>)DnsQuery.LookUp<DnsARecord>(args[1]);
                    break;
                case "cname":
                    records = (IList<DnsRecordBase>)DnsQuery.LookUp<DnsARecord>(args[1]);
                    break;
                case "ptr":
                    records = (IList<DnsRecordBase>)DnsQuery.LookUp<DnsARecord>(args[1]);
                    break;
                case "any":
                    records = DnsQuery.LookUp<DnsRecordBase>(args[1]);
                    break;
                default:
                    PrintUsage();
                    return;
            }
            foreach (var record in records)
            {
                Console.WriteLine($"  {record}");
            }
        }
    }
}
