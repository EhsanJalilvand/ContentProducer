using DataIntegrationProvider.Domain.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DataIntegrationProvider.Application.Application.Common.Interfaces;
using DataIntegrationProvider.Domain.ConfigEntities;
using Quartz;
using Share.Domain.Extensions;
using Marten;
using EnigmaDataProvider.Domain.Models;

namespace DataIntegrationProvider.Application.Application.Common.Abstractions
{
    public abstract class RecieverCommandAbstraction<T> : IRecieverService where T : ModelBase
    {
        private readonly IDocumentSession _documentSession;
        private readonly ILogger<RecieverCommandAbstraction<T>> _logger;
        private static bool _flag = false;
        public RecieverCommandAbstraction(IDocumentSession documentSession, ILogger<RecieverCommandAbstraction<T>> logger)
        {
            _logger = logger;
            _documentSession = documentSession;
        }
        public abstract ServiceCategoryId ServiceCategoryId { get; }
        protected abstract Task<T> GetData(PlanningInfo serviceInfo);
        protected virtual async Task<bool> SaveData(T response, PlanningInfo planningInfo)
        {
            response.CreateTime = DateTime.Now;
            response.ServiceCategoryName=ServiceCategoryId.GetDisplayName();
            DocumentSession.Store(response);
            await DocumentSession.SaveChangesAsync();
            return true;
        }
        protected virtual async Task<bool> DeleteData(T response, PlanningInfo planningInfo)
        {
            DocumentSession.HardDeleteWhere<T>(x => x.ID > 0);
            await DocumentSession.SaveChangesAsync();
            return true;
        }
        protected abstract void Dispose();
        public IDocumentSession DocumentSession { get { return _documentSession; } }
        public async Task Run(PlanningInfo plan)
        {
            if (_flag)
                return;
            _flag = true;
            T allData = null;
            var dtime = DateTime.Now;
            string _planLog = string.Empty;
            try
            {
                await Task.Delay(200);
                //plan = _documentSession.Query<PlanningInfo>().FirstOrDefault(p => p.PlanningInfoId == plan.PlanningInfoId);


                if (plan == null)
                {
                    _logger.LogWarning($"PlanningInfo For Id:{plan.PlanningInfoId}  Is Null");
                    return;
                }

                _planLog = $"Plan Name: {plan.PlanningInfoId.GetDisplayName()}  ";
                _logger.LogInformation($"Start Call Api For Recieve Data {_planLog}");


                _logger.LogInformation($"Start GetData.Plan: {_planLog}");
                try
                {
                    allData = await GetData(plan);
                    if (allData == null )
                    {
                        _logger.LogWarning($"Null Data Response in Calling Data {_planLog}");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(ex, $"Error Raised In Get Data {_planLog}");
                    return;
                }



                try
                {
                    if (plan.CanDelete)
                    {
                        _logger.LogWarning($"Start Delete OldData For Data {_planLog}");
                        await DeleteData(allData, plan);
                    }
                    _logger.LogInformation($"Start Save Response  For Data {_planLog}");
                    await SaveData(allData, plan);

                    _logger.LogInformation($"Saved Successfully For Data {_planLog}");
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(ex, $"Exception Raised in Saving Data {_planLog}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, $"Exception Raised in Calling Data {_planLog}");
            }
            finally
            {
                var dtime2 = DateTime.Now;
                _logger.LogInformation($"ProceeId {plan.PlanningInfoId} Run In {dtime2.Subtract(dtime).TotalSeconds} Sec");
                await Task.Delay(plan.Interval * 1000);
                allData = null;
                Dispose();
                plan = null;
                _flag = false;
            }
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var serviceInfo = context.JobDetail.JobDataMap.Get("PlanningInfo");
            if (serviceInfo == null)
                return;
            var info = (PlanningInfo)serviceInfo;

            await Run(info);
        }

    }
}
