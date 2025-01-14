using System;
using System.IO;
using System.Collections.Generic;

namespace Screenary
{
	public class PcapWriter : Pcap
	{
		BinaryWriter writer;
				
		public PcapWriter(Stream stream) : base(stream)
		{
			writer = new BinaryWriter(stream);
		}
		
		public PcapWriter(string filename) : this(File.OpenWrite(filename))
		{
			pcap_header.magic_number = 0xA1B2C3D4;
			pcap_header.version_major = 2;
			pcap_header.version_minor = 4;
			pcap_header.thiszone = 0;
			pcap_header.sigfigs = 0;
			pcap_header.snaplen = 0xFFFFFFFF;
			pcap_header.network = 0;
			
			WriteHeader(pcap_header);
		}
		
		private void WriteHeader(pcap_header_t pcap_header)
		{
			writer.Write(pcap_header.magic_number);
			writer.Write(pcap_header.version_major);
			writer.Write(pcap_header.version_minor);
			writer.Write(pcap_header.thiszone);
			writer.Write(pcap_header.sigfigs);
			writer.Write(pcap_header.snaplen);
			writer.Write(pcap_header.network);
		}
			
		private void WriteRecord(pcap_record_t pcap_record)
		{
			writer.Write(pcap_record.ts_sec);
			writer.Write(pcap_record.ts_usec);
			writer.Write(pcap_record.incl_len);
			writer.Write(pcap_record.orig_len);
			writer.Write(pcap_record.buffer);
		}
		
		public void AddRecord(PcapRecord pcapRecord)
		{
			TimeSpan timeSpan = pcapRecord.Time;
			
			UInt32 ticks = (UInt32)timeSpan.Ticks;
			UInt32 ticksPerSecond = (UInt32)TimeSpan.TicksPerSecond;
			UInt32 ticksPerMilisecond = (UInt32)TimeSpan.TicksPerMillisecond;

			pcap_record.ts_sec = ticks / ticksPerSecond;	
			pcap_record.ts_usec = (ticks % ticksPerSecond) * (1000 / ticksPerMilisecond);
			pcap_record.incl_len = (UInt32)pcapRecord.Length;
			pcap_record.orig_len = (UInt32)pcapRecord.Length;
			pcap_record.buffer = pcapRecord.Buffer;
				
			WriteRecord(pcap_record);
		}
		
		public void Flush()
		{
			writer.Flush();
		}
		
		public void Close()
		{
			writer.Close();
		}
	}

}

