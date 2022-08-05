using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetingManager
{
    public class Manager : IDisposable
    {
        private readonly Dictionary<DateTime, List<Meeting>> meetingList = new Dictionary<DateTime, List<Meeting>>();
        private Reminder reminder;
        private Dictionary<Meeting, DateTime> reminderList = new Dictionary<Meeting, DateTime>();

        public Manager()
        {
            reminder = new Reminder();
        }

        public bool IsTimeAvailableFor(Meeting meeting)
        {
            if (!meetingList.ContainsKey(meeting.Date.Date) || meetingList[meeting.Date.Date].Count == 0)
                return true;
            return meetingList[meeting.Date.Date].All(x => x.CompareTo(meeting) != 0);
        }

        public void AddMeeting(Meeting meeting)
        {
            if (!IsTimeAvailableFor(meeting))
                throw new Exception("Время уже занято другой встречей");
            if (!meetingList.ContainsKey(meeting.Date.Date) || meetingList[meeting.Date.Date].Count == 0)
                meetingList.Add(meeting.Date.Date, new List<Meeting>());
            meetingList[meeting.Date.Date].Add(meeting);
        }

        public void EditMeeting(Meeting oldMeeting, Meeting newMeeting)
        {
            DeleteMeeting(oldMeeting);
            try
            {
                AddMeeting(newMeeting);
            }
            catch (Exception ex)
            {
                AddMeeting(oldMeeting);
                throw ex;
            }
        }

        public Meeting Find(DateTime date)
        {
            if (!meetingList.ContainsKey(date.Date) || meetingList[date.Date].Count == 0)
                throw new Exception("На указанную дату встреч не найдено");

            foreach (var meeting in meetingList[date.Date])
            {
                if (meeting.StartTime.TimeOfDay == date.TimeOfDay)
                    return meeting;
            }
            throw new Exception("В указанное время встреч не найдено");
        }

        public void DeleteMeeting(DateTime dateTime)
        {
            meetingList[dateTime.Date].Remove(Find(dateTime));
        }

        public void DeleteMeeting(Meeting meeting)
        {
            reminder.Remove(reminderList[meeting]);
            reminderList.Remove(meeting);
            meetingList[meeting.Date].Remove(meeting);
        }

        public string GetMeetingListForTheDay(DateTime date)
        {
            StringBuilder listForTheDay = new StringBuilder();
            if (!meetingList.ContainsKey(date.Date) || meetingList[date.Date].Count == 0)
                return "В этот день нет встреч";

            foreach (var meeting in meetingList[date.Date])
            {
                listForTheDay.AppendLine(meeting.ToString());
                listForTheDay.AppendLine();
            }           
            return listForTheDay.ToString();
        }

        public void ExportMeetingList(DateTime date, string fileName)
        {
            if (fileName == "")
                fileName = "Расписание встреч на " + date.ToShortDateString() + ".txt";

            File.WriteAllText(fileName, GetMeetingListForTheDay(date));
        }

        public void RemindAboutMeetingIn (Meeting meeting, int remindTimeInMinutes)
        {
            var reminderTime = meeting.Date.Date.Add(meeting.StartTime.TimeOfDay) - TimeSpan.FromMinutes(remindTimeInMinutes);
            SetReminderTime(meeting, reminderTime);
        }

        public void SetReminderTime(Meeting meeting, DateTime reminderTime)
        {
            if (reminderList.ContainsKey(meeting))
            {
                reminder.Remove(reminderList[meeting]);
                reminderList[meeting] = reminderTime;
            }
            else
            {
                reminderList.Add(meeting, reminderTime);
            }
            reminder.Add(reminderTime, meeting);
        }
        public void RemoveReminder(Meeting meeting)
        {
            reminder.Remove(reminderList[meeting]);
            if (reminderList.ContainsKey(meeting))
                reminderList.Remove(meeting);
        }

        public void Dispose()
        {
            reminder.Dispose();
        }
    }
}
