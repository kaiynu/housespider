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
[Daily(TriggerId = "trigger_CommunityReportJob", Description = "小区抓取服务")]
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
				var pageData = JsonConvert.DeserializeObject<Hashtable>(pageNode.GetAttributeValue("page-data", ""));
				var maxPage = pageData["totalPage"].ToInt();
				var pageUrl = pageNode.GetAttributeValue("page-url", "");
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
					cont = WebUtils.Get(host, pageUrl.Replace("{page}", curPage.ToString()));
					html = new HtmlDocument();
					html.LoadHtml(cont);
					document = html.DocumentNode;

				}
				while (maxPage > curPage);
				if (comlist.Count > 0)
				{
					rep.InsertRange(comlist);
					DoHouse(comlist, host, batchid);
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
				var cont = WebUtils.Get(host, string.Format(url, comId));
				HtmlDocument html = new HtmlDocument();
				html.LoadHtml(cont);
				var document = html.DocumentNode;
				var pageNode = document.QuerySelector(".page-box .house-lst-page-box");
				var pageData = JsonConvert.DeserializeObject<Hashtable>(pageNode.GetAttributeValue("page-data", ""));
				var maxPage = pageData["totalPage"].ToInt();
				var pageUrl = pageNode.GetAttributeValue("page-url", "");
				var curPage = 1;
				var list = new List<HouseReport>();
				do
				{
					var nodes = document.QuerySelectorAll("ul.sellListContent li");
					foreach (var node in nodes)
					{
						var a = node.QuerySelector("a");
						var t = DoHouseDetail(item, a.GetAttributeValue("href", ""), batchid);
						if (t != null)
						{
							list.Add(t);
						}
					}
					curPage++;
					cont = WebUtils.Get(host, pageUrl.Replace("{page}", curPage.ToString()));
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
			obj.Name = document.QuerySelector(".detailHeader .main").GetAttributeValue("title", "");
			obj.BatchId = batchid;
			obj.AvgPrive = document.QuerySelector(".price-container .unitPriceValue").InnerText.ToDecimal();
			obj.TotalPrice = document.QuerySelector(".price-container .total").InnerText.ToDecimal();
			obj.CommunityName = communitie.Name;
			obj.ReportDate = DateTime.Now.Date;
			obj.HotArea = communitie.HotArea;
			var introContent = document.QuerySelectorAll(".introContent .base li .label").ToList();
			obj.HuXing = introContent[0].NextSibling.InnerText.TrimAll();
			obj.Size = introContent[1].NextSibling.InnerText.TrimAll();
			obj.HuXingJieGou = introContent[2].NextSibling.InnerText.TrimAll();
			obj.BuildType = introContent[3].NextSibling.InnerText.TrimAll();
			obj.BuildLevel = introContent[4].NextSibling.InnerText.TrimAll();
			obj.InnerSize = introContent[5].NextSibling.InnerText.TrimAll();
			obj.BuildJieGou = introContent[6].NextSibling.InnerText.TrimAll();
			obj.ZhuangXiu = introContent[7].NextSibling.InnerText.TrimAll();
			obj.TiHuBiLi = introContent[8].NextSibling.InnerText.TrimAll();
			obj.DianTi = introContent[9].NextSibling.InnerText.TrimAll();
			var introContent2 = document.QuerySelectorAll(".introContent .transaction li .label").ToList();
			obj.GuaPaiTime = introContent2[0].NextSibling.InnerText.TrimAll().ToDateTime();
			obj.JiaoYiQuanShu = introContent2[1].NextSibling.InnerText.TrimAll();
			obj.ShangCiJiaoYi = introContent2[2].NextSibling.InnerText.TrimAll();
			obj.FangWuYongTu = introContent2[3].NextSibling.InnerText.TrimAll();
			obj.FangYuanHeYanMa = introContent2.Last().NextSibling.InnerText.TrimAll();
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