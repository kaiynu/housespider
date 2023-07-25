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
[JobDetail("HomeDetailReportJob", Description = "房产详情抓取服务", GroupName = "default", Concurrent = false)]
[Hourly(TriggerId = "trigger_HomeDetailReportJob", Description = "房产抓取服务")]
public class HomeDetailReportJob : IJob
{
	private readonly IServiceProvider _serviceProvider;

	public HomeDetailReportJob(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider;
	}

	public async Task ExecuteAsync(JobExecutingContext context, CancellationToken stoppingToken)
	{
		using var serviceScope = _serviceProvider.CreateScope();
		{
			var rep = serviceScope.ServiceProvider.GetService<SqlSugarRepository<HouseReport>>();


			var pageIndex = 1;
			var pageSize = 100;

			var batchId = DateTime.Now;

			var tasks = new List<Task>();
			Func<object?, bool> fuc = (object? dto) =>
			{
				var item = dto as HouseReport;
			
				DoHouseDetail(item);
	
				return true;
			};
			do
			{
				var list = rep.AsQueryable().Where(i => i.UpdateTime == null).Take(pageSize).ToList();
				if (list.Count() == 0)
				{
					break;
				}
				foreach (var item in list)
				{
					tasks.Add(Task<bool>.Factory.StartNew(fuc, item));
				}
				Task.WaitAll(tasks.ToArray());
				rep.AsUpdateable(list).ExecuteCommand();

			} while (true);
			Console.WriteLine("==========totaltime:" + (DateTime.Now - batchId).TotalMinutes);
		}
		

		
	}


	private HouseReport DoHouseDetail(HouseReport obj)
	{
		try
		{

			var cont = WebUtils.Get(obj.Url, "");
			HtmlDocument html = new HtmlDocument();
			html.LoadHtml(cont);
			var document = html.DocumentNode;
			var introContent = document.QuerySelectorAll(".introContent li .label").ToList();
			if (introContent == null)
			{
				return obj;
			}
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

			obj.AllImgUrl = JsonConvert.SerializeObject(smallpics);;
			return obj;
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