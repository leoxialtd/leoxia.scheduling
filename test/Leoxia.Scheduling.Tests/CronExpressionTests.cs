using System.Globalization;
using Leoxia.Scheduling.Domain;
using Leoxia.Scheduling.Domain.Cron;
using NUnit.Framework;

namespace Leoxia.Scheduling.Tests
{
    [TestFixture]
    public class CronExpressionTests
    {
        [Theory]
        [TestCase("* * * * * 1", "2024/10/21 00:00:00", "2024/10/21 00:00:01")] // Every second of Monday
        [TestCase("* * * * * 1", "2024/10/22 00:00:00", "2024/10/28 00:00:01")] // Every second of Monday
        [TestCase("* * * * * 7", "2024/10/22 00:00:00", "2024/10/27 00:00:01")] // Every second of Sunday
        [TestCase("0 0 0 1 1 *", "2024/10/22 00:00:00", "2025/01/01 00:00:00")] // Only 1st January at 00:00:00
        [TestCase("0 0 1-3 1 1 *", "2024/10/22 00:00:00", "2025/01/01 01:00:00")] // Only 1st January at 1 - 3 am
        [TestCase("0 0 4-9 1 1 *", "2024/10/22 00:00:00", "2025/01/01 04:00:00")] // Only 1st January at 1 - 3 am
        [TestCase("0 0 1,2,3 1 1 *", "2024/10/22 00:00:00", "2025/01/01 01:00:00")] // Only 1st January at 1 2 and 3 am
        [TestCase("0 0 1,2,3 1 1 *", "2025/01/01 01:01:30", "2025/01/01 02:00:00")] // Only 1st January at 1 2 and 3 am
        [TestCase("*/15 * * * * *", "2025/01/01 01:01:30", "2025/01/01 01:01:45")] // Every 15 seconds
        [TestCase("*/15 * * * * *", "2025/01/01 01:01:45", "2025/01/01 01:02:00")] // Every 15 seconds
        [TestCase("0 * * * * *", "2024/10/18 12:30:30", "2024/10/18 12:31:00")]
        [TestCase("0 0 * * * *", "2024/10/18 12:30:00", "2024/10/18 13:00:00")]
        [TestCase("0 0 12 * * *", "2024/10/18 11:59:59", "2024/10/18 12:00:00")]
        [TestCase("0 0 12 * * 1-5", "2024/10/18 12:00:00", "2024/10/21 12:00:00")] // Next Monday (weekday)
        [TestCase("*/2 0-10/2 12 * * *", "2024/10/18 12:00:01", "2024/10/18 12:02:00")] // Every 2 seconds, every 2 hours between 0-10
        [TestCase("0 0 12 * 5 *", "2024/10/18 11:00:00", "2024/05/01 12:00:00")] // May 1st at 12 PM
        [TestCase("0 30 10 15 * *", "2024/10/14 12:00:00", "2024/10/15 10:30:00")] // 15th of the month
        [TestCase("0 0 12 1 * *", "2024/10/18 12:00:00", "2024/11/01 12:00:00")] // Noon on the 1st of every month
        [TestCase("0 0 0 29 2 *", "2023/02/28 23:59:59", "2024/02/29 00:00:00")] // Leap year check (Feb 29th)
        [TestCase("0 0 12 * * 0", "2024/10/18 12:00:00", "2024/10/20 12:00:00")] // Every Sunday at noon
        [TestCase("0 0 12 * * 7", "2024/10/18 12:00:00", "2024/10/20 12:00:00")] // Same as above, Sunday is treated as both 0 and 7
        [TestCase("0 30 10 31 12 *", "2024/12/01 00:00:00", "2024/12/31 10:30:00")] // December 31st at 10:30 AM
        [TestCase("0 0 0 * 6 *", "2024/05/31 23:59:59", "2024/06/01 00:00:00")] // Midnight on June 1st
        [TestCase("0 0 9 15 * *", "2024/03/15 08:59:59", "2024/03/15 09:00:00")] // 9 AM on the 15th of every month
        [TestCase("*/10 * * * * *", "2024/10/18 12:00:05", "2024/10/18 12:00:10")] // Every 10 seconds
        [TestCase("*/10 0 * * * *", "2024/10/18 12:00:55", "2024/10/18 13:00:00")] // Every 10 seconds at the top of the hour
        [TestCase("0 0 10 1 * 1", "2024/10/18 12:00:00", "2024/11/01 10:00:00")] // 10 AM on the 1st of the month, only on Mondays
        [TestCase("*/2 */10 * * * *", "2024/10/18 10:00:01", "2024/10/18 10:00:02")] // Every 2 seconds, every 10th minute
        [TestCase("0 30 23 * * *", "2024/10/18 22:00:00", "2024/10/18 23:30:00")] // 11:30 PM every day
        [TestCase("*/15 0-10 * * * *", "2024/10/18 09:59:45", "2024/10/18 10:00:00")] // Every 15 seconds within the first 10 minutes of each hour
        [TestCase("0 0 0 * * 1", "2024/10/18 00:00:00", "2024/10/21 00:00:00")] // Midnight on Mondays
        [TestCase("0 0 12 29 2 *", "2023/03/01 00:00:00", "2024/02/29 12:00:00")] // Leap year, February 29th at noon
        [TestCase("*/5 * * * * *", "2024/10/18 12:30:58", "2024/10/18 12:31:00")] // Every 5 seconds, close to the minute boundary
        [TestCase("0 0 0 * 3 5", "2024/03/01 00:00:00", "2024/03/01 00:00:00")] // Midnight on Fridays in March
        [TestCase("0 0 0 * 2 6", "2024/02/29 00:00:00", "2024/02/29 00:00:00")] // Midnight on leap-year February Saturdays
        public void Cron_should_be_parsed(string cronText, string now, string expected)
        {
            var nowTime = DateTimeOffset.ParseExact(now, "yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture);
            var cron = new CronExpression(cronText);
            var next = cron.GetNextTrigger(nowTime);
            Assert.That(next.ToString("yyyy/MM/dd HH:mm:ss"), Is.EqualTo(expected));
        }
    }
}
