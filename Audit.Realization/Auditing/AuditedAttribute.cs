namespace Audit.Realization.Auditing
{
    /// <summary>
    /// 需要审计
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
    public class AuditedAttribute : Attribute
    {

    }
    /// <summary>
    /// 跳过审计，不做审计
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
    public class DisableAuditingAttribute : Attribute
    {

    }
}
