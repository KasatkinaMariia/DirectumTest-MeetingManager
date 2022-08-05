using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetingManager
{
    public struct Meeting : IComparable
    {
        public DateTime Date { get; }
        public DateTime StartTime { get; }
        public DateTime EndTime { get; }
        public string Name { get; }

        public Meeting(DateTime date, DateTime startTime, DateTime endTime, string name)
        {
            if (startTime > endTime)
                throw new ArgumentException("Время начала позже времени окончания");

            if (date.Add(startTime.TimeOfDay) < DateTime.Now)
                throw new ArgumentException("Дата встречи уже прошла!");

            Date = date;
            StartTime = startTime;
            EndTime = endTime;
            Name = name;          
        }

        public override string ToString()
        {
            string meetingInfo;
            meetingInfo = "Название: " + Name
                        + "\nДата: " + Date.ToLongDateString()
                        + "\nВремя начала: " + StartTime.ToShortTimeString()
                        + "\nВремя окончания: " + EndTime.ToShortTimeString();
                                    
            return meetingInfo;
        }

        public int CompareTo(object obj)
        {
            var meeting = (Meeting)obj;
            if (Date < meeting.Date)
                return -1;
            if (Date > meeting.Date)
                return 1;

            if (StartTime < meeting.StartTime && EndTime < meeting.StartTime)
                return -1;
            if (StartTime > meeting.EndTime && EndTime > meeting.EndTime)
                return 1;
            return 0;
        }
    }
}
