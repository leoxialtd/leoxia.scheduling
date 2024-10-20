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
        public void Cron_should_be_parsed(string cronText, string now, string expected)
        {
            var nowTime = DateTimeOffset.ParseExact(now, "yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture);
            var cron = new CronExpression(cronText);
            var next = cron.GetNextTrigger(nowTime);
            Assert.That(next.ToString("yyyy/MM/dd HH:mm:ss"), Is.EqualTo(expected));
        }
    }
}
