// MIT License
//
// Copyright (c) 2021-present zuohuaijun, Daming Co.,Ltd and Contributors
//
// 电话/微信：18020030720 QQ群1：87333204 QQ群2：252381476

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
namespace ConsoleApp1;
public class Class1
{
	public static string Get(string host, string action)
	{
		var client = new RestClient(host);
		var request = new RestRequest(action, Method.Get);
		request.AddHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
		var response = client.Execute(request);
		var content = response.Content;
		return content;
	}
}
