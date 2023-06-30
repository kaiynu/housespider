using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using NewLife;
using OBS.Model;
using static SKIT.FlurlHttpClient.Wechat.Api.Models.ChannelsECMerchantAddFreightTemplateRequest.Types.FreightTemplate.Types;

namespace Admin.NET.Core;

/// <summary>
/// 房产抓取服务
/// </summary>
[JobDetail("HomeReportJob", Description = "房产抓取服务", GroupName = "default", Concurrent = false)]
[Cron("0 0 12 /2 * ? *", Furion.TimeCrontab.CronStringFormat.WithSecondsAndYears, TriggerId = "trigger_HomeReportJob", Description = "房产抓取服务")]
public class HomeReportJob : IJob
{
	private readonly IServiceProvider _serviceProvider;

	public HomeReportJob(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider;
	}

	public async Task ExecuteAsync(JobExecutingContext context, CancellationToken stoppingToken)
	{
		using var serviceScope = _serviceProvider.CreateScope();
		var rep = serviceScope.ServiceProvider.GetService<SqlSugarRepository<HouseReport>>();
		var repCom = serviceScope.ServiceProvider.GetService<SqlSugarRepository<CommunityReport>>();
		var batTime = repCom.AsQueryable().Max<DateTime?>("BatchId");
		if (batTime == null)
		{
			return;
		}
		var pageIndex = 0;
		var pageSize = 2000;
		
		var batchId = DateTime.Now;
		var url = "https://cd.ke.com";
		var tasks = new List<Task>();
		Func<object?, bool> fuc = (object? dto) =>
		{
			var areas = dto as List<CommunityReport>;
			DoHouse(areas,url, batchId);
			return true;
		};
		do
		{
			var comList = repCom.GetPageList(i => i.BatchId == batTime.Value, new PageModel() { PageIndex = pageIndex, PageSize = pageSize });
			if (comList.Count == 0)
			{
				break;
			}
			tasks.Add(Task<bool>.Factory.StartNew(fuc, comList));
			pageIndex++;
		} while (true);
			
		
		Task.WaitAll(tasks.ToArray());
		Console.WriteLine("==========totaltime:" + (DateTime.Now - batchId).TotalMilliseconds);
	}

	private void DoHouse(List<CommunityReport> communities, string host, DateTime batchid)
	{
		try
		{


			var url = "/ershoufang/c{0}/";
			using var serviceScope = _serviceProvider.CreateScope();
			var rep = serviceScope.ServiceProvider.GetService<SqlSugarRepository<HouseReport>>();
			foreach (var communitie in communities)
			{
				if (communitie.Url.IsNullOrWhiteSpace())
				{
					continue;
				}
				var comId = communitie.Url.Split('/', StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
				if (comId.IsNullOrWhiteSpace())
				{
					continue;
				}
				if (url.Contains("goodhouse"))
				{
					continue;
				}

				var cont = WebUtils.Get(host, string.Format(url, comId));
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
				var list = new List<HouseReport>();
				do
				{
					var nodes = document.QuerySelectorAll("ul.sellListContent li");
					foreach (var node in nodes)
					{
						var a = node.QuerySelector("a");
						var info = node.QuerySelector(".info");
						var houseInfo = node.QuerySelector(".houseInfo");
						var href = a?.GetAttributeValue("href", "");
						if (href.Contains("goodhouse"))
						{
							continue;
						}
						var obj = new HouseReport();
						obj.Url = href;
						obj.CommunityName = communitie.Name;
						obj.City = communitie.City;
						obj.Region = communitie.Region;
						obj.Province = communitie.Province;
						obj.ReportDate = DateTime.Now.Date;
						obj.HotArea = communitie.HotArea;
						obj.Name = a.GetAttributeValue("title", "").TrimAll();
						obj.ThumbImgUrl=a.QuerySelector(".lj-lazy")?.GetAttributeValue("data-original","").TrimAll();
						obj.TotalPrice=info?.QuerySelector(".totalPrice span")?.InnerText.TrimAll().ToDecimal();
						obj.AvgPrive = info?.QuerySelector(".unitPrice span")?.InnerText.Replace("元/平","").Replace(",","").TrimAll().ToDecimal();
						obj.BatchId = batchid;
						obj.ReportDate= DateTime.Now.Date;
						if (obj != null)
						{
							list.Add(obj);
						}
					}
					curPage++;
					if (pageUrl.IsNullOrEmpty())
					{
						break;
					}
					cont = WebUtils.Get(host, pageUrl?.Replace("{page}", curPage.ToString()));
					html = new HtmlDocument();
					html.LoadHtml(cont);
					document = html.DocumentNode;

				}
				while (maxPage > curPage);
				if (list.Count > 0)
				{
					rep.InsertRange(list);
					foreach (var item in list)
					{
						Task.Run(() => {
							DoHouseDetail(item);
						});
					}
				}

			}
		}
		catch (Exception e)
		{
			Log.Error("DoHouse:" + e);

		}
	}
	private HouseReport DoHouseDetail(HouseReport obj)
	{
		try
		{
			using var serviceScope = _serviceProvider.CreateScope();
			var rep = serviceScope.ServiceProvider.GetService<SqlSugarRepository<HouseReport>>();
			var cont = WebUtils.Get(obj.Url, "");
			HtmlDocument html = new HtmlDocument();
			html.LoadHtml(cont);
			var document = html.DocumentNode;
			var introContent = document.QuerySelectorAll(".introContent li .label").ToList();
			obj.HuXing = GetBaseInfo(introContent,"房屋户型");
			obj.Size = GetBaseInfo(introContent, "建筑面积");
			obj.HuXingJieGou = GetBaseInfo(introContent, "户型结构"); ;
			obj.BuildType = GetBaseInfo(introContent, "建筑类型");
			obj.BuildLevel = GetBaseInfo(introContent, "所在楼层");
			obj.InnerSize = GetBaseInfo(introContent, "套内面积");
			obj.Direction = GetBaseInfo(introContent, "房屋朝向");
			obj.BuildJieGou = GetBaseInfo(introContent, "建筑结构");
			obj.ZhuangXiu = GetBaseInfo(introContent, "装修情况");
			obj.TiHuBiLi = GetBaseInfo(introContent, "梯户比例");
			obj.DianTi = GetBaseInfo(introContent, "配备电梯");			
			obj.GuaPaiTime = GetBaseInfo(introContent, "挂牌时间").ToDateTime();
			obj.JiaoYiQuanShu = GetBaseInfo(introContent, "交易权属");
			obj.ShangCiJiaoYi = GetBaseInfo(introContent, "上次交易");
			obj.FangWuYongTu = GetBaseInfo(introContent, "房屋用途");
			obj.FangYuanHeYanMa = GetBaseInfo(introContent, "房源核验码");
			var smallpics = document.QuerySelectorAll(".thumbnail .smallpic li").Select(i => new ImgDto { Src = i.QuerySelector("img").GetAttributeValue("src", ""), Desc = i.GetAttributeValue("data-desc", "") }).ToList();
			obj.AllImgUrl = JsonConvert.SerializeObject(smallpics);
			rep.Update(obj);
			return null;
		}
		catch (Exception e)
		{
			Log.Error("DoHouseDetail:" + e);
			return null;
		}
	}
	private string GetBaseInfo(List<HtmlNode> nodes,string name) {
		foreach (var item in nodes)
		{
			if (item.InnerText.TrimAll() == name)
			{
				return item.NextSibling.InnerText.TrimAll();
			}
		}
		return null;
	}
	public class ImgDto
	{
		public string Desc { get; set; }
		public string Src { get; set; }
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