using Agentic_Rentify.Application.Specifications;
using Agentic_Rentify.Core.Entities;
using Agentic_Rentify.Core.Enums;

namespace Agentic_Rentify.Application.Features.SystemSettings.Specifications;

public sealed class SystemSettingByPageSpecification(SystemPage page)
    : BaseSpecification<SystemSetting>(s => s.PageName == page && !s.IsDeleted)
{
}

public sealed class AllSystemSettingsSpecification() : BaseSpecification<SystemSetting>(s => !s.IsDeleted)
{
}
