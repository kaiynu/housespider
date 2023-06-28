namespace Admin.NET.Core;

[SugarTable(null, "小区")]
[SystemTable]
public class CommunityReport : EntityBase
{

	[Required, MaxLength(64)]
	public virtual string Name { get; set; }


	[MaxLength(512)]
	[SugarColumn(Length = 512)]
	public string? Url { get; set; }

	[MaxLength(32)]
	public string? Region { get; set; }
	public string? City { get; set; }
	public string? Province { get; set; }

	public string? Street { get; set; }
	public string? Address { get; set; }

	public string? HotArea { get; set; }

	[SugarColumn(Length = 512)]
	public string? ThumbImgUrl { get; set; }
	public decimal? AvgPrice { get; set; }

	public int? SellCount { get; set; }

	public int? BuildYear { get; set; }

	public DateTime? ReportDate { get; set; }
	public int? SellCountIn90 { get; set; }
	public DateTime? BatchId { get; set; }

}
[SugarTable(null, "小区日报")]
[SystemTable]
public class HouseReport : EntityBase
{
	/// <summary>
	/// 名称
	/// </summary>
	[SugarColumn(Length = 255)]
	[Required, MaxLength(64)]
	public virtual string Name { get; set; }

	/// <summary>
	/// 链接
	/// </summary>
	[SugarColumn(Length = 512)]
	[MaxLength(512)]
	public string? Url { get; set; }

	[SugarColumn(Length = 500)]
	[MaxLength(500)]
	public string? ThumbImgUrl { get; set; }

	[SugarColumn(Length = 5000)]
	[MaxLength(5000)]
	public string? AllImgUrl { get; set; }


	public string? HotArea { get; set; }
	public string? CommunityName { get; set; }
	public decimal? TotalPrice { get; set; }
	public decimal? AvgPrive { get; set; }
	public string? HuXing { get; set; }
	public string? Size { get; set; }
	public string? HuXingJieGou { get; set; }
	public string? BuildType { get; set; }
	public string? BuildLevel { get; set; }
	public string? InnerSize { get; set; }
	public string? Direction { get; set; }
	public string? BuildJieGou { get; set; }
	public string? ZhuangXiu { get; set; }
	public string? DianTi { get; set; }

	public string? TiHuBiLi { get; set; }

	public DateTime? GuaPaiTime { get; set; }
	public string? JiaoYiQuanShu { get; set; }

	public string? ShangCiJiaoYi { get; set; }
	public string? FangWuYongTu { get; set; }

	public string? FangYuanHeYanMa { get; set; }

	public DateTime? BatchId { get; set; }

	public DateTime? ReportDate { get; set; }

}