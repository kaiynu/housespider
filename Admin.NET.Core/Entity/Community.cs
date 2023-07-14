namespace Admin.NET.Core;

[SugarTable(null, "小区")]
[SystemTable]
public class Community : EntityBase
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



}
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

	[SugarColumn(Length = 500)]
	[MaxLength(500)]
	public string? HotArea { get; set; }
	[SugarColumn(Length = 500)]
	[MaxLength(500)]
	public string? CommunityName { get; set; }
	public decimal? TotalPrice { get; set; }
	public decimal? AvgPrive { get; set; }
	[SugarColumn(Length = 100)]
	[MaxLength(100)]
	public string? HuXing { get; set; }

	[SugarColumn(Length = 100)]
	[MaxLength(100)]
	public string? Region { get; set; }
	[SugarColumn(Length = 100)]
	[MaxLength(100)]
	public string? City { get; set; }

	[SugarColumn(Length = 100)]
	[MaxLength(100)]
	public string? Province { get; set; }
	[SugarColumn(Length = 100)]
	[MaxLength(100)]
	public string? Size { get; set; }

	public decimal? DSize { get; set; }
	[SugarColumn(Length = 100)]
	[MaxLength(100)]
	public string? HuXingJieGou { get; set; }
	[SugarColumn(Length = 100)]
	[MaxLength(100)]
	public string? BuildType { get; set; }
	[SugarColumn(Length = 100)]
	[MaxLength(100)]
	public string? BuildLevel { get; set; }
	[SugarColumn(Length = 100)]
	[MaxLength(100)]
	public int? BuildYear { get; set; }
	public int? TotalLevel { get; set; }
	[SugarColumn(Length = 100)]
	[MaxLength(100)]
	public string? InnerSize { get; set; }
	[SugarColumn(Length = 100)]
	[MaxLength(100)]
	public string? Direction { get; set; }
	[SugarColumn(Length = 100)]
	[MaxLength(100)]
	public string? BuildJieGou { get; set; }
	[SugarColumn(Length = 100)]
	[MaxLength(100)]
	public string? ZhuangXiu { get; set; }
	[SugarColumn(Length = 100)]
	[MaxLength(100)]
	public string? DianTi { get; set; }
	[SugarColumn(Length = 100)]
	[MaxLength(100)]
	public string? TiHuBiLi { get; set; }

	public DateTime? GuaPaiTime { get; set; }
	public string? JiaoYiQuanShu { get; set; }
	[SugarColumn(Length = 100)]
	[MaxLength(100)]
	public string? ShangCiJiaoYi { get; set; }
	public DateTime? DShangCiJiaoYi { get; set; }
	[SugarColumn(Length = 100)]
	[MaxLength(100)]
	public string? FangWuYongTu { get; set; }
	[SugarColumn(Length = 100)]
	[MaxLength(100)]

	public string? FangYuanHeYanMa { get; set; }

	public DateTime? BatchId { get; set; }

	public DateTime? ReportDate { get; set; }

}