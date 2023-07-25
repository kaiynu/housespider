using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using Minio.DataModel;
using Nest;
using NewLife;
using OBS.Model;
using System.Collections.Generic;
using static SKIT.FlurlHttpClient.Wechat.Api.Models.ChannelsECMerchantAddFreightTemplateRequest.Types.FreightTemplate.Types;

namespace Admin.NET.Core;

/// <summary>
/// 房产抓取服务
/// </summary>
[JobDetail("HomeReportJob", Description = "房产抓取服务", GroupName = "default", Concurrent = false)]
[Cron("0 0 7 1/3 * ? *", Furion.TimeCrontab.CronStringFormat.WithSecondsAndYears, TriggerId = "trigger_HomeReportJob", Description = "房产抓取服务")]
public class HomeReportJob : IJob
{
	private readonly IServiceProvider _serviceProvider;
	private  SqlSugarRepository<Community> repCom;
	private SqlSugarRepository<HouseReport> rep;
	public HomeReportJob(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider;
	}

	public async Task ExecuteAsync(JobExecutingContext context, CancellationToken stoppingToken)
	{

		using var serviceScope = _serviceProvider.CreateScope() ;
		repCom = serviceScope.ServiceProvider.GetService<SqlSugarRepository<Community>>();
		rep = serviceScope.ServiceProvider.GetService<SqlSugarRepository<HouseReport>>();
		var pageIndex = 1;
		var pageSize = 2000;

		var batchId = DateTime.Now;

		var tasks = new List<Task<List<HouseReport>>>();

		do
		{
			var comList = new List<Community>();
			
			
				
				comList = repCom.GetPageList(i => 1 == 1, new PageModel() { PageIndex = pageIndex, PageSize = pageSize }, i => i.Id);
				//var comList = repCom.GetList(i => i.BatchId == batTime.Value);

			
			if (comList.Count() == 0)
			{
				break;
			}
			tasks.Add(Task.Run(() => { return DoHouse(comList, batchId); }));
			pageIndex++;
		} while (true);
		Task.WaitAll(tasks.ToArray());
		foreach (var item in tasks)
		{
			try
			{
				if(item.Result!=null&& item.Result.Count > 0)
				{
					rep.InsertRange(item.Result);
				}
				
			}
			catch (Exception)
			{

				
			}
		}
		Console.WriteLine("==========totaltime:" + (DateTime.Now - batchId).TotalMinutes);
	}

	private List<HouseReport> DoHouse(List<Community> communities, DateTime batchid)
	{
		var list = new List<HouseReport>();
		try
		{


			var url = "/ershoufang/c{0}/";
			

			foreach (var communitie in communities)
			{




				try
				{
					var host = communitie.Url.Substring(0, communitie.Url.IndexOf("/xiaoqu"));
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

					var cont = WebUtils.Get(host, string.Format(url, comId) + "?t=" + DateTime.Now.Ticks);
					HtmlDocument html = new HtmlDocument();
					html.LoadHtml(cont);
					var document = html.DocumentNode;
					var noresult = document.QuerySelector(".leftContent .m-noresult");
					if (noresult != null)
					{
						continue;
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
					
					do
					{
						var nodes = document.QuerySelectorAll("ul.sellListContent li");

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
			
				
				}
				catch (Exception e)
				{
					Log.Error("DoHouse2222:" + e);

				}
				Log.Information("comm:" + communitie.Name);

			


			}


		}
		catch (Exception e)
		{
			Log.Error("DoHouse:" + e);

		}
		return list;
	}





}