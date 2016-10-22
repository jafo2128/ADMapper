using System;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace CLDAP.NET
{
    public static class Cldap
    {
        private static byte[] GetCldapPingRequest(string dnsName)
        {
            var buf = new List<byte>();
            buf.AddRange(new byte[] { 0x30, (byte)(0x3d + dnsName.Length) }); // CLDAPMessage (tag, size)
            buf.AddRange(BerConverter.Encode("i", 1)); // Message ID
            buf.AddRange(new byte[] { 0x63, (byte)(0x38 + dnsName.Length) }); // protocolOp (Application 3: SearchRequest) (tag, size)
            buf.AddRange(new byte[] { 0x04, 0x00 }); // SearchBase (null LDAP string)
            buf.AddRange(new byte[] { 0x0a, 0x01, 0x00 }); // Scope (0: baseObject)
            buf.AddRange(new byte[] { 0x0a, 0x01, 0x00 }); // DerefAliases (0: neverDerefAliases)
            buf.AddRange(BerConverter.Encode("i", 0)); // sizeLimit (0)
            buf.AddRange(BerConverter.Encode("i", 0)); // timeLimit (1)
            buf.AddRange(BerConverter.Encode("b", false)); // typesOnly (false)
            buf.AddRange(new byte[] { 0xa0, (byte)(0x19 + dnsName.Length) }); // filter (0: AND Filter) (tag, size)
            buf.AddRange(new byte[] { 0xa3, (byte)(0x08 + dnsName.Length) }); // filter (3: EQUALITY Filter) (tag, size)
            buf.AddRange(new byte[] { 0x04, 0x04, 0x48, 0x6f, 0x73, 0x74 }); // attributeDesc (Octet String: 'Host')
            buf.AddRange(new byte[] { 0x04, (byte)dnsName.Length }); // assertionValue (Octet String)
            buf.AddRange(Encoding.UTF8.GetBytes(dnsName)); // encoded as utf-8
            buf.AddRange(new byte[] { 0xa3, 0x0d }); // filter (3: EQUALITY Filter) (tag, size)
            buf.AddRange(new byte[] { 0x04, 0x05, 0x4e, 0x74, 0x56, 0x65, 0x72 }); // attributeDesc (Octet String: 'NtVer')
            buf.AddRange(new byte[] { 0x04, 0x04, 0x06, 0x00, 0x00, 0x00 }); // attributeDesc (Octet String: DWORD=6 encoded backwards)
            buf.AddRange(new byte[] { 0x30, 0x0a }); // attributes (tag, size)
            buf.AddRange(new byte[] { 0x04, 0x08, 0x4e, 0x65, 0x74, 0x6c, 0x6f, 0x67, 0x6f, 0x6e }); // attributeSelector (Octet String: 'Netlogon')
            return buf.ToArray();
        }

        public static string Ping(string dnsName, IPAddress ipAddress, int port)
        {
            var cldapPing = GetCldapPingRequest(dnsName);

            using (var udpClient = new UdpClient())
            {
                udpClient.Connect(ipAddress, port);
                udpClient.Send(cldapPing, cldapPing.Length);
                var remoteIpEndPoint = new IPEndPoint(ipAddress, 0);
                var buf = udpClient.Receive(ref remoteIpEndPoint);
                return BitConverter.ToString(buf).ToLower().Replace('-', ' ');
            }
        }
    }
}
