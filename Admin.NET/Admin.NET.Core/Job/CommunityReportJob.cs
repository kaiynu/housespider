using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using NewLife;
using OBS.Model;
using static SKIT.FlurlHttpClient.Wechat.Api.Models.ChannelsECMerchantAddFreightTemplateRequest.Types.FreightTemplate.Types;

namespace Admin.NET.Core;

/// <summary>
/// 小区抓取服务
/// </summary>
[JobDetail("CommunityReportJob", Description = "小区抓取服务", GroupName = "default", Concurrent = false)]
[Monthly(TriggerId = "trigger_CommunityReportJob", Description = "小区抓取服务")]
public class CommunityReportJob : IJob
{
	private readonly IServiceProvider _serviceProvider;

	public CommunityReportJob(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider;
	}

	public async Task ExecuteAsync(JobExecutingContext context, CancellationToken stoppingToken)
	{
		var batchId = DateTime.Now;
		var url = "https://cd.ke.com";
		var city = "成都";
		var province = "四川";
		var cont = WebUtils.Get(url, "/xiaoqu");
		HtmlDocument html = new HtmlDocument();
		html.LoadHtml(cont);
		var document = html.DocumentNode;
		var areas = new List<AreaDto>();
		var nodes = document.QuerySelectorAll(".position a.CLICKDATA");
		foreach (var node in nodes)
		{
			areas.Add(new AreaDto()
			{
				Name = node.InnerText.TrimAll(),
				Url = url + node.GetAttributeValue("href", ""),
				City = city,
				Provice = province,
			}
			);
		}
		var tasks = new List<Task>();
		Func<object?, bool> fuc = (object? dto) =>
		{
			var area = dto as AreaDto;
			return DoCommunty(area, batchId, url);
		};
		foreach (var area in areas)
		{
			tasks.Add(Task<bool>.Factory.StartNew(fuc, area));
		}
		Task.WaitAll(tasks.ToArray());
		Console.WriteLine("==========totaltime:" + (DateTime.Now - batchId).TotalMilliseconds);
	}
	private bool DoCommunty(AreaDto area, DateTime batchid, string host)
	{
		try
		{


			if (area != null)
			{
				using var serviceScope = _serviceProvider.CreateScope();
				var rep = serviceScope.ServiceProvider.GetService<SqlSugarRepository<CommunityReport>>();

				var cont = WebUtils.Get(area.Url, "");
				HtmlDocument html = new HtmlDocument();
				html.LoadHtml(cont);
				var document = html.DocumentNode;
				var pageNode = document.QuerySelector(".page-box .house-lst-page-box");
				var maxPage = 1;
				if (pageNode != null)
				{
					var pageData = JsonConvert.DeserializeObject<Hashtable>(pageNode.GetAttributeValue("page-data", ""));
					 maxPage = pageData["totalPage"].ToInt();
				}
				var pageUrl = pageNode?.GetAttributeValue("page-url", "");
				var curPage = 1;
				var comlist = new List<CommunityReport>();
				do
				{

					var nodes = document.QuerySelectorAll("ul.listContent li");
					foreach (var node in nodes)
					{
						var a = node.QuerySelector(".info .title .maidian-detail");
						var item = new CommunityReport()
						{
							Name = a.InnerText,
							Url = a.GetAttributeValue("href", ""),
						};
						var img = node.QuerySelector(".maidian-detail img.lj-lazy");
						var xiaoquListItemPrice = node.QuerySelector(".xiaoquListItemPrice .totalPrice span");
						var totalSellCount = node.QuerySelector(".xiaoquListItemRight .totalSellCount span");
						var positionInfo = node.QuerySelector(".info .positionInfo");
						var houseInfo = node.QuerySelector(".info .houseInfo a");
						var bizcircle = positionInfo.QuerySelector(".bizcircle");

						item.ThumbImgUrl = img.GetAttributeValue("data-original", "");

						item.Region = area.Name;
						item.Province = area.Provice;

						item.SellCountIn90 = houseInfo.InnerText.TrimAll().Replace("90天成交", "").Replace("套", "").ToInt();
						item.BuildYear = (positionInfo.LastChild.InnerText.TrimAll().Replace("&nbsp;", "").Replace("/", "").Replace("年建成", "")).ToInt();
						item.HotArea = bizcircle.InnerText.TrimAll();
						item.City = area.City;
						item.Province = area.Provice;
						item.SellCount = totalSellCount.InnerText.TrimAll().ToInt();
						item.ReportDate = DateTime.Today;
						item.BatchId = batchid;
						item.AvgPrice = xiaoquListItemPrice.InnerText.TrimAll().ToDecimal();
						comlist.Add(item);
					}
					curPage++;
					if (pageUrl.IsNullOrEmpty())
					{
						break;
					}
					cont = WebUtils.Get(host, pageUrl.Replace("{page}", curPage.ToString()));
					html = new HtmlDocument();
					html.LoadHtml(cont);
					document = html.DocumentNode;

				}
				while (maxPage > curPage);
				if (comlist.Count > 0)
				{
					rep.InsertRange(comlist);
				}
			}


			return true;
		}
		catch (Exception e)
		{
			Log.Error("DoCommunty:" + e);
			return false;

		}
	}

	
	public class AreaDto
	{
		public string Name { get; set; }
		public string Url { get; set; }
		public string Host { get; set; }
		public string City { get; set; }
		public string Provice { get; set; }
	}


}