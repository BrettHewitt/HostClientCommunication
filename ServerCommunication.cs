/*dataDyne Laboratories - Host Client Communication - A class that allows program to program communication
Copyright(C) 2019 Brett Michael Hewitt

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.If not, see<http://www.gnu.org/licenses/>.*/

using System;
using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;

namespace HostClientCommunication
{
    public class ServerCommunication
    {
        private readonly Stream _Stream;
        private readonly UnicodeEncoding _StreamEncoding;

        public ServerCommunication(Stream stream)
        {
            _Stream = stream;
            _StreamEncoding = new UnicodeEncoding();
        }

        public string ReadMessage()
        {
            int length = _Stream.ReadByte()*256;
            length += _Stream.ReadByte();
            byte[] buffer = new byte[length];
            _Stream.Read(buffer, 0, length);

            return _StreamEncoding.GetString(buffer);
        }

        public JObject ReadMessageAsJObject()
        {
            int length = _Stream.ReadByte()*256;
            length += _Stream.ReadByte();
            byte[] buffer = new byte[length];
            _Stream.Read(buffer, 0, length);

            string msg = _StreamEncoding.GetString(buffer);

            return JObject.Parse(msg);
        }

        public int SendMessage(string outString)
        {
            byte[] buffer = _StreamEncoding.GetBytes(outString);
            int length = buffer.Length;
            if (length > UInt16.MaxValue)
            {
                length = (int) UInt16.MaxValue;
            }
            _Stream.WriteByte((byte) (length / 256));
            _Stream.WriteByte((byte) (length & 255));
            _Stream.Write(buffer, 0, length);
            _Stream.Flush();

            return buffer.Length + 2;
        }
    }
}
