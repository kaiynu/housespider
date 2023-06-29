using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using NewLife;
using OBS.Model;
using static SKIT.FlurlHttpClient.Wechat.Api.Models.ChannelsECMerchantAddFreightTemplateRequest.Types.FreightTemplate.Types;

namespace Admin.NET.Core;

/// <summary>
/// 小区抓取服务
/// </summary>
[JobDetail("HomeReportJob", Description = "房产抓取服务", GroupName = "default", Concurrent = false)]
[Weekly(TriggerId = "trigger_HomeReportJob", Description = "房产抓取服务")]
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
			foreach (var item in communities)
			{
				if (item.Url.IsNullOrWhiteSpace())
				{
					continue;
				}
				var comId = item.Url.Split('/', StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
				if (comId.IsNullOrWhiteSpace())
				{
					continue;
				}
				if (url.Contains("goodhouse"))
				{
					Console.WriteLine("=========c===============:"+item.Id);
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
						var href = a?.GetAttributeValue("href", "");
						if (href.IsNullOrEmpty() || href.Contains("goodhouse"))
						{
							continue;	
						}
						var t = DoHouseDetail(item,href , batchid);
						if (t != null)
						{
							list.Add(t);
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
				}

			}
		}
		catch (Exception e)
		{
			Log.Error("DoHouse:" + e);

		}
	}
	private HouseReport DoHouseDetail(CommunityReport communitie, string url, DateTime batchid)
	{
		try
		{


			var obj = new HouseReport();

			var cont = WebUtils.Get(url, "");
			HtmlDocument html = new HtmlDocument();
			html.LoadHtml(cont);
			var document = html.DocumentNode;
			obj.Url= url;
			var nameNode = document.QuerySelector(".detailHeader .main");
			if (nameNode == null)
			{
				return null;
			}
			obj.Name = nameNode.GetAttributeValue("title", "");
			obj.BatchId = batchid;
			obj.AvgPrive = document.QuerySelector(".price-container .unitPriceValue").InnerText.ToDecimal();
			obj.TotalPrice = document.QuerySelector(".price-container .total").InnerText.ToDecimal();
			obj.CommunityName = communitie.Name;
			obj.ReportDate = DateTime.Now.Date;
			obj.HotArea = communitie.HotArea;
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
			obj.ThumbImgUrl = smallpics.First().Src;
			obj.AllImgUrl = JsonConvert.SerializeObject(smallpics);
			return obj;
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