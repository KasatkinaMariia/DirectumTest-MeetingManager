using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetingManager
{
    class Program
    {
        static Manager manager = new Manager();

        static void Main(string[] args)
        {
            while (true)
            {
                PrintMainMenuCommands();
                var option = Console.ReadLine();
                try
                {
                    switch (option)
                    {
                        case "1":
                            AddMeeting();
                            break;
                        case "2":
                            EditMeeting();
                            break;
                        case "3":
                            DeleteMeeting();
                            break;
                        case "4":
                            PrintMeetingListForTheDay();
                            break;
                        case "5":
                            ExportMeetingList();
                            break;
                        case "100":
                            Console.WriteLine("Выход из программы");
                            manager.Dispose();
                            return;
                        default:
                            Console.WriteLine("Введен неверный символ. Пожалуйста, введите номер действия, которое хотите совершить");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public static DateTime ReadDateTime()
        {
            DateTime dateTime;
            try
            {
                dateTime = DateTime.Parse(Console.ReadLine());
            }
            catch
            {
                throw new ArgumentException("Некорректный формат даты");
            }
            return dateTime;
        }

        public static void PrintMainMenuCommands()
        {
            Console.WriteLine("Выберите действие:");
            Console.WriteLine("1 - добавить новую встречу");
            Console.WriteLine("2 - редактировать данные о встрече");
            Console.WriteLine("3 - удалить встречу");
            Console.WriteLine("4 - посмотреть расписание на определенный день");
            Console.WriteLine("5 - экспортировать расписание на определенный день в текстовый файл");
            Console.WriteLine("100 - выход из программы");
        }

        public static void PrintEditCommands()
        {
            Console.WriteLine("Выберите действие: ");
            Console.WriteLine("1 - изменить название / описание встречи");
            Console.WriteLine("2 - изменить дату встречи");
            Console.WriteLine("3 - изменить время начала");
            Console.WriteLine("4 - изменить время окончания");
            Console.WriteLine("5 - изменить время напоминания");
            Console.WriteLine("0 - сохранить изменения и завершить редактирование");
            Console.WriteLine("-1 - отменить изменения");
        }

        public static void AddMeeting()
        {
            Console.Write("Введите имя собеседника или описание встречи: ");
            string name = Console.ReadLine();
            Console.Write("Введите дату встречи в формате dd.mm.yyyy: ");
            DateTime date = ReadDateTime();
            Console.Write("Введите время начала встречи в формате hh:mm: ");
            DateTime startTime = ReadDateTime();
            Console.Write("Введите время начала встречи в формате hh:mm: ");
            DateTime endTime = ReadDateTime();
            Meeting meeting = new Meeting(date, startTime, endTime, name);
            manager.AddMeeting(meeting);
            Console.WriteLine("Встреча успешно добавлена");
            Console.Write("Нужно ли напоминание? д/н: ");
            var option = Console.ReadLine();
            if (option == "д")
            {
                Console.Write("Введите время, за которое необходимо напомнить о встрече, в минутах: ");
                int reminderTimeInMinutes = int.Parse(Console.ReadLine());
                manager.RemindAboutMeetingIn(meeting, reminderTimeInMinutes);
                Console.WriteLine("Напоминание успешно добавлено");
            };            
        }

        public static void EditMeeting()
        {
            Console.Write("Введите дату и время встречи, которую хотите изменить (в формате dd.mm.yyyy hh:mm): ");
            DateTime meetingDate = ReadDateTime();
            Meeting oldMeeting = manager.Find(meetingDate);
            Console.WriteLine(oldMeeting.ToString());
            Console.Write("Вы хотите внести изменения для этой встречи? д/н: ");
            if (Console.ReadLine() != "д")
                return;
            var name = oldMeeting.Name;
            var date = oldMeeting.Date;
            var startTime = oldMeeting.StartTime;
            var endTime = oldMeeting.EndTime;
            int reminderTimeInMinutes = 0;

            while (true)
            {
                PrintEditCommands();
                var option = Console.ReadLine();
                switch (option)
                {
                    case "1":
                        Console.Write("Введите новое название / описание встречи: ");
                        name = Console.ReadLine();
                        break;
                    case "2":
                        Console.Write("Введите новую дату встречи (dd.mm.yyyy): ");
                        date = ReadDateTime(); ;
                        break;
                    case "3":
                        Console.Write("Введите новое время начала встречи (hh:mm): ");
                        startTime = ReadDateTime();
                        break;
                    case "4":
                        Console.Write("Введите новое время окончания встречи (hh:mm): ");
                        endTime = ReadDateTime();
                        break;
                    case "5":
                        Console.Write("Введите время, за которое необходимо напомнить о встрече, в минутах: ");
                        reminderTimeInMinutes = int.Parse(Console.ReadLine());
                        break;
                    case "0":
                        Meeting newMeeting = new Meeting(date, startTime, endTime, name);
                        manager.EditMeeting(oldMeeting, newMeeting);
                        if (reminderTimeInMinutes != 0)
                            manager.RemindAboutMeetingIn(newMeeting, reminderTimeInMinutes);
                        Console.WriteLine("Изменения успешно сохранены");
                        return;
                    case "-1":
                        return;
                    default:
                        Console.WriteLine("Такого действия не существует. Введите номер действия, которое хотите совершить");
                        break;
                }
            }
        }

        public static void DeleteMeeting()
        {
            Console.Write("Введите дату и время начала встречи, которую хотите удалить (dd.mm.yyyy hh:mm): ");
            DateTime date = ReadDateTime();
            Meeting meeting = manager.Find(date);
            Console.WriteLine(meeting.ToString());
            Console.Write("Вы хотите удалить эту встречу? д/н: ");
            if (Console.ReadLine() != "д")
                return;
            manager.DeleteMeeting(manager.Find(date));
            Console.WriteLine("Встреча успешно удалена");
        }

        public static void PrintMeetingListForTheDay()
        {
            Console.Write("Введите дату в формате dd.mm.yyyy: ");
            DateTime date = ReadDateTime();
            Console.WriteLine(manager.GetMeetingListForTheDay(date));
        }

        public static void ExportMeetingList()
        {
            Console.Write("Введите дату: ");
            DateTime date = ReadDateTime();
            Console.Write("Введите путь и имя файла (path\\fileName.txt) нажмите ENTER для выбора директории по умолчанию: ");
            string fileName = Console.ReadLine();
            manager.ExportMeetingList(date, fileName);
            Console.WriteLine("Расписание успешно записано");
        }
    }
}
