﻿/*
    Myrtille: A native HTML4/5 Remote Desktop Protocol client.

    Copyright(c) 2014-2021 Cedric Coste
    Copyright(c) 2018 Paul Oliver (Olive Innovations)

    Licensed under the Apache License, Version 2.0 (the "License");
    you may not use this file except in compliance with the License.
    You may obtain a copy of the License at

        http://www.apache.org/licenses/LICENSE-2.0

    Unless required by applicable law or agreed to in writing, software
    distributed under the License is distributed on an "AS IS" BASIS,
    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
    See the License for the specific language governing permissions and
    limitations under the License.
*/

using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Web;

namespace Myrtille.Helpers
{
    /*
    CAUTION! client ip check is not very reliable to identify a machine or user, as a protection; it just mitigates the risk of a spoofing attack
    depending on the (client and server) network configuration (NAT, proxies, reverse proxies, etc.), the ip gathered by the server may not be the real client ip
    in an enterprise, for instance, people using the same proxy will have the same public ip
    people with a mobile device (smartphone/tablet/laptop, etc.), moving from an internet access point to another, will change ip regularly
    also, http request headers can be altered and thus shouldn't be trusted
    */

    // https://stackoverflow.com/questions/6832724/is-it-reliable-to-use-the-ip-address-to-identify-a-user-on-your-website

    public static class ClientIPHelper
    {
        // based on http://www.grantburton.com/2008/11/30/fix-for-incorrect-ip-addresses-in-wordpress-comments/
        public static string ClientIPFromRequest(this HttpRequestBase request, bool skipPrivate, string[] ignoreAddresses)
        {
            foreach (var item in s_HeaderItems)
            {
                var ipString = (item.ServerVariable ? request.ServerVariables[item.Key] : request.Headers[item.Key]);

                if (String.IsNullOrEmpty(ipString))
                    continue;

                if (item.Split)
                {
                    foreach (var ip in ipString.Split(','))
                        if (ValidIP(ip, skipPrivate) && !ignoreAddresses.Contains(ip))
                            return ip;
                }
                else
                {
                    if (ValidIP(ipString, skipPrivate) && !ignoreAddresses.Contains(ipString))
                        return ipString;
                }
            }

            return request.UserHostAddress;
        }

        private static bool ValidIP(string ip, bool skipPrivate)
        {
            IPAddress ipAddr;

            ip = ip == null ? String.Empty : ip.Trim();

            if (0 == ip.Length
                || false == IPAddress.TryParse(ip, out ipAddr)
                || (ipAddr.AddressFamily != AddressFamily.InterNetwork
                    && ipAddr.AddressFamily != AddressFamily.InterNetworkV6))
                return false;

            if (skipPrivate && ipAddr.AddressFamily == AddressFamily.InterNetwork)
            {
                var addr = IpRange.AddrToUInt64(ipAddr);
                foreach (var range in s_PrivateRanges)
                {
                    if (range.Encompasses(addr))
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Provides a simple class that understands how to parse and
        /// compare IP addresses (IPV4) ranges.
        /// </summary>
        private sealed class IpRange
        {
            private readonly UInt64 _start;
            private readonly UInt64 _end;

            public IpRange(string startStr, string endStr)
            {
                _start = ParseToUInt64(startStr);
                _end = ParseToUInt64(endStr);
            }

            public static UInt64 AddrToUInt64(IPAddress ip)
            {
                var ipBytes = ip.GetAddressBytes();
                UInt64 value = 0;

                foreach (var abyte in ipBytes)
                {
                    value <<= 8;    // shift
                    value += abyte;
                }

                return value;
            }

            public static UInt64 ParseToUInt64(string ipStr)
            {
                var ip = IPAddress.Parse(ipStr);
                return AddrToUInt64(ip);
            }

            public bool Encompasses(UInt64 addrValue)
            {
                return _start <= addrValue && addrValue <= _end;
            }

            public bool Encompasses(IPAddress addr)
            {
                var value = AddrToUInt64(addr);
                return Encompasses(value);
            }
        };

        private static readonly IpRange[] s_PrivateRanges =
            new IpRange[] {
            new IpRange("0.0.0.0","2.255.255.255"),
            new IpRange("10.0.0.0","10.255.255.255"),
            new IpRange("127.0.0.0","127.255.255.255"),
            new IpRange("169.254.0.0","169.254.255.255"),
            new IpRange("172.16.0.0","172.31.255.255"),
            new IpRange("192.0.2.0","192.0.2.255"),
            new IpRange("192.168.0.0","192.168.255.255"),
            new IpRange("255.255.255.0","255.255.255.255")
            };

        /// <summary>
        /// Describes a header item (key) and if it is expected to be 
        /// a comma-delimited string
        /// </summary>
        private sealed class HeaderItem
        {
            public readonly string Key;
            public readonly bool Split;
            public readonly bool ServerVariable;

            public HeaderItem(string key, bool split, bool serverVariable)
            {
                Key = key;
                Split = split;
                ServerVariable = serverVariable;
            }
        }

        // order is in trust/use order top to bottom
        private static readonly HeaderItem[] s_HeaderItems =
            new HeaderItem[] {
            new HeaderItem("X-Client-IP",false,true),
            new HeaderItem("HTTP_CLIENT_IP",false,false),
            new HeaderItem("X-Forwarded-For",true,true),
            new HeaderItem("HTTP_X_FORWARDED_FOR",true,false),
            new HeaderItem("HTTP_X_FORWARDED",false,false),
            new HeaderItem("HTTP_X_CLUSTER_CLIENT_IP",false,false),
            new HeaderItem("HTTP_FORWARDED_FOR",false,false),
            new HeaderItem("HTTP_FORWARDED",false,false),
            new HeaderItem("HTTP_VIA",false,false),
            new HeaderItem("REMOTE_ADDR",false,false)
            };
    }
}