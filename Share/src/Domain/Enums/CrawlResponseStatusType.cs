using System;
namespace SharedDomain.Enums
{
	public enum CrawlResponseStatusType
	{
		NoAction=0,
		Ok=1,
		Timeout=2,
		LimitAccess=3,
		Exception=4
	}
}

