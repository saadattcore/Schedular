using System;

namespace Emaratech.Services.Scheduler.Entities
{
    public static class JobScheduleExtensions
    {
        public const string SecondsFrequencyTypeId = "00000000000000000000000000000001";
        public const string MinutesFrequencyTypeId = "00000000000000000000000000000002";
        public const string HoursFrequencyTypeId = "00000000000000000000000000000003";
        public const string DaysFrequencyTypeId = "00000000000000000000000000000004";
        public const string WeeksFrequencyTypeId = "00000000000000000000000000000005";
        public const string MonthsFrequencyTypeId = "00000000000000000000000000000006";
        public const string YearsFrequencyTypeId = "00000000000000000000000000000007";

        public static DateTime? GetNextExecutionDate(this JobSchedule schedule, Job job)
        {
            if (job?.ExecutedDate != null)
            {
                DateTime nextDate = schedule.StartDate.GetValueOrDefault();
                switch (schedule.ScheduleTypeId)
                {
                    case SecondsFrequencyTypeId:
                    case MinutesFrequencyTypeId:
                    case HoursFrequencyTypeId:
                    case DaysFrequencyTypeId:
                        nextDate = new DateTime(job.ExecutedDate.Value.Year, job.ExecutedDate.Value.Month, job.ExecutedDate.Value.Day,nextDate.Hour,nextDate.Minute,nextDate.Second).AddDays(-1);
                        break;
                    case MonthsFrequencyTypeId:
                        nextDate = new DateTime(job.ExecutedDate.Value.Year, job.ExecutedDate.Value.Month, job.ExecutedDate.Value.Day, nextDate.Hour, nextDate.Minute, nextDate.Second).AddMonths(-2);
                        break;

                }

                while (nextDate <= job.ExecutedDate.Value)
                {

                    switch (schedule.ScheduleTypeId)
                    {
                        case SecondsFrequencyTypeId:
                            nextDate = nextDate.AddSeconds(schedule.ScheduleFrequency);
                            break;
                        case MinutesFrequencyTypeId:
                            nextDate = nextDate.AddMinutes(schedule.ScheduleFrequency);
                            break;
                        case HoursFrequencyTypeId:
                            nextDate = nextDate.AddHours(schedule.ScheduleFrequency);
                            break;
                        case DaysFrequencyTypeId:
                            nextDate = nextDate.AddDays(schedule.ScheduleFrequency);
                            break;
                        case WeeksFrequencyTypeId:
                            nextDate = nextDate.AddDays(schedule.ScheduleFrequency*7);
                            break;
                        case MonthsFrequencyTypeId:
                            nextDate = nextDate.AddMonths(schedule.ScheduleFrequency);
                            break;
                        case YearsFrequencyTypeId:
                            nextDate = nextDate.AddYears(schedule.ScheduleFrequency);
                            break;
                    }
                }
                return nextDate;

            }
            return null;
        }
    }
}
