using System;
namespace SharedDomain.Enums
{
	public enum CrawlRequestCommandType
	{
		LoadUrlAsync = 0,
		WaitForSelector = 1,
		ExecuteScript = 2,
		WaitForNavigation = 3,
		GetContent= 4,
		GetAddress=5,
		ScrollToBotttom=6,
		EvaluateScriptAsync=7,
	}
}

