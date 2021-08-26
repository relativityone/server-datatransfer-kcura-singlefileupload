using Relativity.Toggles;

// namespace name is not an error. REL-586137
namespace Relativity.SingleFileUpload.Core.Toggles
{
    [DefaultValue(false)]
    public class ValidateSFUCustomPermissions : IToggle
	{
	}
}
