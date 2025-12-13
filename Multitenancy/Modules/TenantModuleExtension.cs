using HappyTools.Multitenancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyTools.Multitenancy.Modules
{
    public static class TenantModuleExtension
    {
        public static TenantModulesResult CheckModules(this List<TenantModuleSimpleDto> tenantModules)
        {
            return new TenantModulesResult()
            {
                HasSmsModule = tenantModules.Any(m => m.ModuleName == ModuleNameSetting.SmsPanel),
                HasAccountingModule = tenantModules.Any(m => m.ModuleName == ModuleNameSetting.Accounting),
                HasNotificationModule = tenantModules.Any(m => m.ModuleName == ModuleNameSetting.Notification),
                HasBuyBuyRequestModule = tenantModules.Any(m => m.ModuleName == ModuleNameSetting.BuyRequest),
                HasReturnBuyRequestModule = tenantModules.Any(m => m.ModuleName == ModuleNameSetting.ReturnBuyRequest),
                HasTargetModule = tenantModules.Any(m => m.ModuleName == ModuleNameSetting.Target),
                HasReportModule = tenantModules.Any(m => m.ModuleName == ModuleNameSetting.Report)
            };

        }
    }

    public class TenantModulesResult
    {
        public List<TenantModuleSimpleDto>? TenantModules { get; set; }

        public bool HasAccountingModule { get; set; }
        public bool HasNotificationModule { get; set; }
        public bool HasBuyBuyRequestModule { get; set; }
        public bool HasReturnBuyRequestModule { get; set; }
        public bool HasTargetModule { get; set; }
        public bool HasReportModule { get; set; }
        public bool HasSmsModule { get; set; }
    }

}
