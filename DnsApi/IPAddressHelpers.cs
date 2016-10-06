using System.Net;
// ReSharper disable InconsistentNaming

namespace DnsApi
{
    internal static class IPAddressHelpers
    {
        /// <summary>
        /// Converts an unsigned int to an ip address object
        /// See http://msdn.microsoft.com/en-us/library/windows/desktop/cc982163(v=vs.85).aspx
        /// </summary>
        /// <param name="ipAddress">The unsigned int to convert to an ip address object</param>
        /// <returns>The converted ip address</returns>
        public static IPAddress ConvertUintToIpAddress(uint ipAddress)
        {
            // x86 is in little endian
            // Network byte order (what the IPAddress object requires) is big endian
            // Ex - 0x7F000001 is 127.0.0.1
            var addressBytes = new byte[4];
            addressBytes[0] = (byte)((ipAddress & 0xFF000000u) >> 24);
            addressBytes[1] = (byte)((ipAddress & 0x00FF0000u) >> 16);
            addressBytes[2] = (byte)((ipAddress & 0x0000FF00u) >> 8);
            addressBytes[3] = (byte)(ipAddress & 0x000000FFu);
            return new IPAddress(addressBytes);
        }

        /// <summary>
        /// Converts the data from the AAAA record into an ip address object
        /// See http://msdn.microsoft.com/en-us/library/windows/desktop/ms682140(v=vs.85).aspx
        /// </summary>
        /// <param name="data">The AAAA record to convert</param>
        /// <returns>The converted ip address</returns>
        public static IPAddress ConvertAAAAToIpAddress(PInvoke.DNS_AAAA_DATA data)
        {
            var addressBytes = new byte[16];
            addressBytes[0] = (byte)(data.Ip6Address0 & 0x000000FF);
            addressBytes[1] = (byte)((data.Ip6Address0 & 0x0000FF00) >> 8);
            addressBytes[2] = (byte)((data.Ip6Address0 & 0x00FF0000) >> 16);
            addressBytes[3] = (byte)((data.Ip6Address0 & 0xFF000000) >> 24);
            addressBytes[4] = (byte)(data.Ip6Address1 & 0x000000FF);
            addressBytes[5] = (byte)((data.Ip6Address1 & 0x0000FF00) >> 8);
            addressBytes[6] = (byte)((data.Ip6Address1 & 0x00FF0000) >> 16);
            addressBytes[7] = (byte)((data.Ip6Address1 & 0xFF000000) >> 24);
            addressBytes[8] = (byte)(data.Ip6Address2 & 0x000000FF);
            addressBytes[9] = (byte)((data.Ip6Address2 & 0x0000FF00) >> 8);
            addressBytes[10] = (byte)((data.Ip6Address2 & 0x00FF0000) >> 16);
            addressBytes[11] = (byte)((data.Ip6Address2 & 0xFF000000) >> 24);
            addressBytes[12] = (byte)(data.Ip6Address3 & 0x000000FF);
            addressBytes[13] = (byte)((data.Ip6Address3 & 0x0000FF00) >> 8);
            addressBytes[14] = (byte)((data.Ip6Address3 & 0x00FF0000) >> 16);
            addressBytes[15] = (byte)((data.Ip6Address3 & 0xFF000000) >> 24);

            return new IPAddress(addressBytes);
        }
    }
}
