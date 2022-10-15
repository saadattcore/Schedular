using AutoMapper;
using Emaratech.DatabaseLayer;
using Emaratech.Services.Scheduler.Contracts.DataAccess;
using Emaratech.Services.Scheduler.Contracts.DataAccess.Models;
using Emaratech.Services.TestCommons.Mocks;
using Emaratech.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Emaratech.Services.Scheduler.DataAccess.Tests
{
    [TestClass]
    public class SchedulerDALTests
    {
        private static IMapper mapper;

        private IUnitOfWork unitOfWork;
        private FakeTransaction lastTransaction;

        static SchedulerDALTests()
        {
            var mapperConfig = new MapperConfiguration(_ => { });
            mapperConfig.Configure();
            mapper = mapperConfig.CreateMapper();
        }

        [TestInitialize]
        public void Init()
        {
            var fake = new FakeUnitOfWork(TransactionProvider);

            fake.AddRepository(new GenericRepositoryMock<Entities.Job, string>(x => x.Id));
            fake.AddRepository(new GenericRepositoryMock<Entities.JobInstance, string>(x => x.Id));
            fake.AddRepository(new GenericRepositoryMock<Entities.JobSchedule, string>(x => x.Id));
            fake.AddRepository(new GenericRepositoryMock<Entities.JobSource, string>(x => x.Id));
            fake.AddRepository(new GenericRepositoryMock<Entities.SystemJob, string>(x => x.Id));
            unitOfWork = fake;
        }

        [TestMethod]
        public void AddJob_Success()
        {
            var job = new Job
            {
                IsActive = true,
                Systems = new string[1] { "system" },
                Name = "Job",
                JobSource = new JobSource
                {
                    Process = "Process.exe",
                    Content = new byte[0]
                }
            };

            var id = GetService().AddJob(job);

            Assert.IsNotNull(id);
            AssertTransactionSucceded();
        }

        [TestMethod]
        public void AddJobSchedule_Success()
        {
            var job = InsertJob();

            var schedule = new JobSchedule
            {
                JobId = job.JobId,
                ScheduleTypeId = "00000000000000000000000000000001",
                ScheduleFrequency = 1
            };
            GetService().AddJobSchedule(schedule);

            AssertTransactionSucceded();

            var jobSchedule = unitOfWork.Repository<Entities.JobSchedule>().Get(j => j.JobId == job.JobId).FirstOrDefault();
            Assert.IsNotNull(jobSchedule);
            Assert.AreEqual(schedule.ScheduleTypeId, jobSchedule.ScheduleTypeId);
            Assert.AreEqual(schedule.ScheduleFrequency, jobSchedule.ScheduleFrequency);
        }

        [TestMethod]
        public void FetchNextJobWithLockUnlock_Success()
        {
            var job = InsertJob();
            var schedule = InsertJobSchedule(job.JobId, 5);
            
            var nextJob = GetService().FetchNext(null, null);
            var lockSuccess = GetService().LockJob(job.JobId);
            Assert.IsNotNull(nextJob);
            Assert.AreEqual(job.JobId, nextJob.JobId);
            Assert.IsTrue(lockSuccess);

            // If we fetch next before the next schedule or before the lock time limit
            // Lock result shoud be false
            GetService().FetchNext(null, null);
            lockSuccess = GetService().LockJob(job.JobId);
            Assert.IsFalse(lockSuccess);

            // Job should be locked
            var jobEntity = unitOfWork.Repository<Entities.Job>().Get(j => j.JobId == job.JobId && j.IsActive).FirstOrDefault();
            Assert.IsTrue(jobEntity.IsLocked);

            // Unlock job
            GetService().UnlockJob(job.JobId);

            jobEntity = unitOfWork.Repository<Entities.Job>().Get(j => j.JobId == job.JobId && j.IsActive).FirstOrDefault();
            Assert.IsFalse(jobEntity.IsLocked);

            // Wait for next schedule
            Thread.Sleep(1000 * schedule.ScheduleFrequency);

            // Get next again
            nextJob = GetService().FetchNext(null, null);
            Assert.IsNotNull(nextJob);
            Assert.AreEqual(job.JobId, nextJob.JobId);

            // Job can be locked again
            lockSuccess = GetService().LockJob(job.JobId);
            Assert.IsTrue(lockSuccess);
        }

        private Entities.Job InsertJob()
        {
            var jobId = Utils.GenerateGuidKey();
            var job = new Entities.Job
            {
                Id = jobId,
                JobId = jobId,
                CreatedDate = DateTime.Now,
                CreatedBy = "-1",
                IsEnabled = true,
                IsActive = true,
                MaxLockSeconds = 10,
                Name = "Test",
                Version = 1
            };
            unitOfWork.Repository<Entities.Job>().Insert(job);
            return job;
        }

        private Entities.JobSchedule InsertJobSchedule(string jobId, int secondsFrequency)
        {
            var schedule = new Entities.JobSchedule
            {
                Id = Utils.GenerateGuidKey(),
                JobId = jobId,
                CreatedDate = DateTime.Now,
                CreatedBy = "-1",
                Version = 1,
                ScheduleTypeId = "00000000000000000000000000000001",
                ScheduleFrequency = secondsFrequency
            };
            unitOfWork.Repository<Entities.JobSchedule>().Insert(schedule);
            return schedule;
        }

        private FakeTransaction TransactionProvider()
        {
            lastTransaction = new FakeTransaction();

            return lastTransaction;
        }

        private void AssertTransactionSucceded()
        {
            if (lastTransaction == null)
                throw new InvalidOperationException("Transaction was not used");

            Assert.IsTrue(lastTransaction.Complete && lastTransaction.Committed);
        }

        private ISchedulerDataAccessService GetService()
        {
            return new SchedulerDataAccessService(mapper, unitOfWork);
        }
    }
}
