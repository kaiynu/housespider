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
[Cron("0 15 20 L * ? *",Furion.TimeCrontab.CronStringFormat.WithSecondsAndYears, TriggerId = "trigger_CommunityReportJob", Description = "小区抓取服务")]
public class CommunityReportJob : IJob
{
	private readonly IServiceProvider _serviceProvider;
	private List<AreaDto> citys = new List<AreaDto>();

	public CommunityReportJob(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider;
		citys.Add(new AreaDto() { Host= "https://cd.ke.com",Provice="四川",City="成都" });
		citys.Add(new AreaDto() { Host = "https://nc.ke.com", Provice = "江西", City = "南昌" });
		citys.Add(new AreaDto() { Host = "https://sh.ke.com", Provice = "上海", City = "上海" });
		citys.Add(new AreaDto() { Host = "https://bj.ke.com", Provice = "北京", City = "北京" });
		citys.Add(new AreaDto() { Host = "https://sz.ke.com", Provice = "深圳", City = "深圳" });
	}

	public async Task ExecuteAsync(JobExecutingContext context, CancellationToken stoppingToken)
	{
		var batchId = DateTime.Now;
		var tasks = new List<Task>();
		Func<object?, bool> fuc = (object? dto) =>
		{
			var area = dto as AreaDto;
			return DoCommunty(area, batchId, area.Host);
		};
		foreach (var item in citys)
		{
			var host = item.Host;
			var city = item.City;
			var province = item.Provice;
			var cont = WebUtils.Get(host, "/xiaoqu");
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
					Url = host + node.GetAttributeValue("href", ""),
					City = city,
					Provice = province,
					Host=item.Host
				}
				);
			}
			
			foreach (var area in areas)
			{
				tasks.Add(Task<bool>.Factory.StartNew(fuc, area));
			}			
		}
		Task.WaitAll(tasks.ToArray());
		using (var serviceScope = _serviceProvider.CreateScope()) {
			var rep2 = serviceScope.ServiceProvider.GetService<SqlSugarRepository<Community>>();
			rep2.Context.Ado.ExecuteCommand(@"insert into house.community(id,Name,url,region,city,province,address,hotarea,thumbimgurl,avgprice,buildyear,IsDelete)

select c.id,c.Name,c.url,c.region,c.city,c.province,c.address,c.hotarea,c.thumbimgurl,c.avgprice,c.buildyear,0 isdelete #,batchid
FROM house.communityReport c
left join house.community a on  c.url=a.url 
where a.id is null 
and c.batchid=@bid", new { bid = batchId.ToString("yyyy-MM-dd HH:mm:ss") });
		}
			Console.WriteLine("==========totaltime:" + (DateTime.Now - batchId).TotalMinutes);
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
					if (nodes == null || nodes.Count() == 0)
					{
						int rety = 0;
						while (rety<10)
						{
							Thread.Sleep(2*1000);
							cont = WebUtils.Get(host, pageUrl.Replace("{page}", curPage.ToString())+"?t="+DateTime.Now.Ticks);
							html = new HtmlDocument();
							html.LoadHtml(cont);
							document = html.DocumentNode;
							nodes = document.QuerySelectorAll("ul.listContent li");
							if (nodes != null && nodes.Count() > 0)
							{
								break;
							}
							rety++;
						}
					}
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
					rep.InsertRange(comlist.DistinctBy(i=>i.Url).ToList());
					var list = new List<Community>();					
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