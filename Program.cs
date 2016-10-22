using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using CLDAP.NET;
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

        private static void PrintCldapPingUsage()
        {
            Console.WriteLine("TODO: Change usage...currently used to test DNS only");
            Console.WriteLine("ADMapper cldapping <name> <ipaddress> <port>");
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
            try
            {
                IList<DnsRecordBase> records;
                Console.WriteLine($"Executing query for name '{args[1]}' record type '{args[0]}':");
                switch (args[0])
                {
                    case "a":
                        records = DnsQuery.LookUp<DnsARecord>(args[1], true).Cast<DnsRecordBase>().ToList();
                        break;
                    case "aaaa":
                        records = DnsQuery.LookUp<DnsAaaaRecord>(args[1], true).Cast<DnsRecordBase>().ToList();
                        break;
                    case "srv":
                        records = DnsQuery.LookUp<DnsSrvRecord>(args[1], true).Cast<DnsRecordBase>().ToList();
                        break;
                    case "txt":
                        records = DnsQuery.LookUp<DnsTxtRecord>(args[1], true).Cast<DnsRecordBase>().ToList();
                        break;
                    case "ns":
                        records = DnsQuery.LookUp<DnsNsRecord>(args[1], true).Cast<DnsRecordBase>().ToList();
                        break;
                    case "mx":
                        records = DnsQuery.LookUp<DnsMxRecord>(args[1], true).Cast<DnsRecordBase>().ToList();
                        break;
                    case "cname":
                        records = DnsQuery.LookUp<DnsCnameRecord>(args[1], true).Cast<DnsRecordBase>().ToList();
                        break;
                    case "ptr":
                        records = DnsQuery.LookUp<DnsPtrRecord>(args[1], true).Cast<DnsRecordBase>().ToList();
                        break;
                    case "soa":
                        records = DnsQuery.LookUp<DnsSoaRecord>(args[1], true).Cast<DnsRecordBase>().ToList();
                        break;
                    case "any":
                        records = DnsQuery.LookUp<DnsRecordBase>(args[1], true);
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
            catch (Exception ex)
            {
                Console.WriteLine($"  ERROR: {ex.Message}");
            }
        }

        private static void HandleCldapPing(string[] args)
        {
            if (args.Count() < 3)
            {
                PrintCldapPingUsage();
            }
            try
            {
                Console.WriteLine(Cldap.Ping(args[0], IPAddress.Parse(args[1]), int.Parse(args[2])));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ERROR: {ex.Message}");
            }
        }

        private class DcInfo
        {
            public DcInfo(string dnsName, int port)
            {
                DnsName = dnsName;
                Port = port;
            }

            public string DnsName { get; private set; }

            public int Port { get; private set; }
        }
        private static IPAddress[] ResolveIPs(string dcDnsName)
        {
            var records = DnsQuery.LookUp<DnsARecord>(dcDnsName, true).Cast<DnsRecordBase>().ToList();
            records.AddRange(DnsQuery.LookUp<DnsAaaaRecord>(dcDnsName, true).Cast<DnsRecordBase>().ToList());
            return
                records.Select(r => r is DnsARecord ? ((DnsARecord)r).IPv4Address : ((DnsAaaaRecord)r).IPv6Address)
                    .ToArray();
        }

        private static DcInfo[] ResolveDCs(string dnsName)
        {
            var records = DnsQuery.LookUp<DnsSrvRecord>(dnsName);
            return records.Select(r => new DcInfo(r.NameTarget, r.Port)).ToArray();
        }

        private static void HandleMap(string[] args)
        {
            var dcInfos = ResolveDCs(args[0]);

            //var ipAddresses = ResolveIPs()
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
                    HandleCldapPing(args.Skip(1).ToArray());
                    break;
                case "map":
                    HandleMap(args.Skip(1).ToArray());
                    break;
                default:
                    PrintUsage();
                    break;
            }
        }
    }
}
