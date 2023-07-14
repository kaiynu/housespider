using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using Minio.DataModel;
using NewLife;
using OBS.Model;
using static SKIT.FlurlHttpClient.Wechat.Api.Models.ChannelsECMerchantAddFreightTemplateRequest.Types.FreightTemplate.Types;

namespace Admin.NET.Core;

/// <summary>
/// 房产抓取服务
/// </summary>
[JobDetail("HomeReportJob", Description = "房产抓取服务", GroupName = "default", Concurrent = false)]
[Cron("0 0 12 /2 * ? *", Furion.TimeCrontab.CronStringFormat.WithSecondsAndYears, TriggerId = "trigger_HomeReportJob", Description = "房产抓取服务", RunOnStart = true)]
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
		var repCom = serviceScope.ServiceProvider.GetService<SqlSugarRepository<Community>>();


		var pageIndex = 1;
		var pageSize = 2000;

		var batchId = DateTime.Now;

		var tasks = new List<Task>();
		Func<object?, bool> fuc = (object? dto) =>
		{
			var areas = dto as List<Community>;
			DoHouse(areas, batchId);
			return true;
		};
		do
		{
			var comList = repCom.GetPageList(i => true , new PageModel() { PageIndex = pageIndex, PageSize = pageSize }, i => i.Region);
			//var comList = repCom.GetList(i => i.BatchId == batTime.Value);
			if (comList.Count() == 0)
			{
				break;
			}
			tasks.Add(Task<bool>.Factory.StartNew(fuc, comList));

			pageIndex++;
		} while (true);


		Task.WaitAll(tasks.ToArray());
		Console.WriteLine("==========totaltime:" + (DateTime.Now - batchId).TotalMinutes);
	}

	private void DoHouse(List<Community> communities, DateTime batchid)
	{
		try
		{


			var url = "/ershoufang/c{0}/";
			
			var tasks = new List<Task>();
			foreach (var communitie in communities)
			{

				tasks.Add(Task.Factory.StartNew(() =>
				{

					using var serviceScope = _serviceProvider.CreateScope();
					var rep = serviceScope.ServiceProvider.GetService<SqlSugarRepository<HouseReport>>();
					try
					{
						var host = communitie.Url.Substring(0, communitie.Url.IndexOf("/xiaoqu"));
						if (communitie.Url.IsNullOrWhiteSpace())
						{
							return;
						}
						var comId = communitie.Url.Split('/', StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
						if (comId.IsNullOrWhiteSpace())
						{
							return;
						}
						if (url.Contains("goodhouse"))
						{
							return;
						}

						var cont = WebUtils.Get(host, string.Format(url, comId) + "?t=" + DateTime.Now.Ticks);
						HtmlDocument html = new HtmlDocument();
						html.LoadHtml(cont);
						var document = html.DocumentNode;
						var noresult = document.QuerySelector(".leftContent .m-noresult");
						if (noresult != null)
						{
							return;
						}
						var pageNode = document.QuerySelector(".page-box .house-lst-page-box");
						var maxPage = 1;
						if (pageNode != null)
						{
							var pageData = JsonConvert.DeserializeObject<Hashtable>(pageNode.GetAttributeValue("page-data", ""));
							maxPage = pageData["totalPage"].ToInt();
						}

						var pageUrl = (pageNode?.GetAttributeValue("page-url", "")) ?? "";
						var curPage = 1;
						var list = new List<HouseReport>();
						do
						{
							var nodes = document.QuerySelectorAll("ul.sellListContent li");
							if (nodes == null || nodes.Count() == 0)
							{
								int rety = 0;
								while (rety < 10)
								{
									Thread.Sleep(2 * 1000);
									cont = WebUtils.Get(host, pageUrl.Replace("{page}", curPage.ToString()));
									html = new HtmlDocument();
									html.LoadHtml(cont);
									document = html.DocumentNode;
									nodes = document.QuerySelectorAll("ul.sellListContent li");
									if (nodes != null && nodes.Count() > 0)
									{
										break;
									}
									rety++;
								}
							}
							if (nodes == null || nodes.Count() == 0)
							{
								break;
							}
							foreach (var node in nodes)
							{
								try
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
									obj.ThumbImgUrl = a.QuerySelector(".lj-lazy")?.GetAttributeValue("data-original", "").TrimAll();
									obj.TotalPrice = info?.QuerySelector(".totalPrice span")?.InnerText.TrimAll().ToDecimal();
									obj.AvgPrive = info?.QuerySelector(".unitPrice span")?.InnerText.Replace("元/平", "").Replace(",", "").TrimAll().ToDecimal();
									obj.BatchId = batchid;
									obj.ReportDate = DateTime.Now.Date;
									obj.BuildYear = communitie.BuildYear;
									if (obj != null)
									{
										list.Add(obj);
									}
								}
								catch (Exception ex)
								{

									Log.Error("DoHouse333:" + ex);
								}


							}
							curPage++;
							if (pageUrl.IsNullOrEmpty())
							{
								break;
							}
							cont = WebUtils.Get(host, pageUrl?.Replace("{page}", curPage.ToString()) + "?t=" + DateTime.Now.Ticks);
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
								Task.Run(() =>
								{
									DoHouseDetail(item);
								});
							}
						}
					}
					catch (Exception e)
					{
						Log.Error("DoHouse2222:" + e);

					}
					Log.Information("comm:" + communitie.Name);

				}));


			}
			Task.WaitAll(tasks.ToArray());

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
			obj.HuXing = GetBaseInfo(introContent, "房屋户型");
			obj.Size = GetBaseInfo(introContent, "建筑面积");
			obj.DSize=obj.Size.Replace("㎡","").ToDecimal();
			obj.HuXingJieGou = GetBaseInfo(introContent, "户型结构"); ;
			obj.BuildType = GetBaseInfo(introContent, "建筑类型");
			obj.BuildLevel = GetBaseInfo(introContent, "所在楼层");
			var match = new Regex("共(\\d+)层").Match(obj.BuildLevel);
			if (match.Success)
			{
				obj.TotalLevel = match.Groups[1].Value.ToInt();
			}
			obj.InnerSize = GetBaseInfo(introContent, "套内面积");
			obj.Direction = GetBaseInfo(introContent, "房屋朝向");
			obj.BuildJieGou = GetBaseInfo(introContent, "建筑结构");
			obj.ZhuangXiu = GetBaseInfo(introContent, "装修情况");
			obj.TiHuBiLi = GetBaseInfo(introContent, "梯户比例");
			obj.DianTi = GetBaseInfo(introContent, "配备电梯");
			obj.GuaPaiTime = GetBaseInfo(introContent, "挂牌时间").ToDateTime();
			obj.JiaoYiQuanShu = GetBaseInfo(introContent, "交易权属");
			obj.ShangCiJiaoYi = GetBaseInfo(introContent, "上次交易");
			obj.DShangCiJiaoYi = obj.ShangCiJiaoYi.ToDateTime();
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
	private string GetBaseInfo(List<HtmlNode> nodes, string name)
	{
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