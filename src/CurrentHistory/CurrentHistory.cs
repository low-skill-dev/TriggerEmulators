﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Media;
using System.Drawing;

namespace Emulators
{
	public class CurrentHistory
	{
		public static long NowTicksOfTheDay=> DateTime.Now.Ticks-DateTime.Today.Ticks;
		public event EventHandler? HistoryChanged;
		protected Timer HistoryRecordInvoker = new Timer { Interval = 50d, AutoReset = true, Enabled = true };

		public int RecordInterval_msec
		{
			get => (int)HistoryRecordInvoker.Interval;
			set => HistoryRecordInvoker.Interval = value;
		}

		public int RecordsStored { get; set; }
		public int SecondsStored
		{
			get => RecordsStored / RecordInterval_msec;
			set => RecordsStored = value * 1000 / RecordInterval_msec;
		}

		public List<CurrentRecord> Records { get; protected set; } = new();

		protected ICurrentSource _TrackedGenerator;
		public CurrentHistory(ICurrentSource TrackedGenerator)
		{
			this._TrackedGenerator = TrackedGenerator;
			this.HistoryRecordInvoker.Elapsed += PushRecordFromTracked;
			this.SecondsStored = 30;
			PushRecordFromTracked(this,EventArgs.Empty);
		}

		protected void PushRecordFromTracked(object? sender, EventArgs e)
		{
			var record = new CurrentRecord
			{
				Voltage_Volt = this._TrackedGenerator.CurrentLevel_Volt,
				TimeCreated = NowTicksOfTheDay
			};

			this.Records.Add(record);
			Console.WriteLine("Added record, current count is " + this.Records.Count);

			while (Records.Count > RecordsStored)
				Records.RemoveAt(0);

			HistoryChanged?.Invoke(this, EventArgs.Empty);
		}

		public List<(PointF p1, PointF p2)> GetChartLines(float PointIntervalMsec = 100)
		{
			List<(PointF p1, PointF p2)> Out = new List<(PointF p1, PointF p2)>((int)(SecondsStored / PointIntervalMsec + 0.5f));

			for (int i = 0; i<this.Records.Count-1; i++)
			{
				var currRec = Records[i];
				var nextRec = Records[i+1];
				//if (currRec == nextRec) break;

				Out.Add((
					new PointF(currRec.TimeCreated, currRec.Voltage_Volt),
					new PointF(nextRec.TimeCreated, nextRec.Voltage_Volt)
				));
			}

			return Out;
		}
	}
}
