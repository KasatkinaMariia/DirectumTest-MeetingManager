using System;
using System.Collections.Generic;
using System.Threading; 
namespace MeetingManager
{
	internal class Reminder : IDisposable
	{
		private readonly SortedList<DateTime, Meeting> reminderTime = new SortedList<DateTime, Meeting>();
		private Thread reminderThread;
		private bool isClosed = false;

		public Reminder()
		{
			reminderThread = new Thread(Remind);
			reminderThread.Start();
		}
		public void Add(DateTime time, Meeting meeting)
		{
			if (time < DateTime.Now)
				throw new Exception("Время напоминания уже прошло");
			lock (reminderTime)
			{
				reminderTime.Add(time, meeting);
			}
		}
		public void Remove(DateTime time)
		{
			lock (reminderTime)
			{
				if (!reminderTime.ContainsKey(time))
					return;
				reminderTime.Remove(time);
			}
		}
		public void Remind()
		{
			var sleepTime = 2;
			while (!isClosed)
			{
				Thread.Sleep(sleepTime * 1000);
				lock (reminderTime)
				{
					if (reminderTime.Count == 0)
						continue;
					var timeDifference = (DateTime.Now - reminderTime.Keys[0]).TotalSeconds;
					if (0 < timeDifference && timeDifference < sleepTime * 5)
					{
						Console.WriteLine($"Уведомление о встрече\n {reminderTime[reminderTime.Keys[0]]}");
						reminderTime.RemoveAt(0);
					}
				}
			}
		}
		public void Dispose()
		{
			isClosed = true;
		}
	}
}