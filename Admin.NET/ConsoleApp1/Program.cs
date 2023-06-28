using ConsoleApp1;
using HtmlAgilityPack;
using Fizzler.Systems.HtmlAgilityPack;
var cont = Class1.Get("https://cd.ke.com/xiaoqu/","");



HtmlDocument html = new HtmlDocument();
html.LoadHtml(cont);
var document = html.DocumentNode;

var nodes=document.QuerySelectorAll(".position a.CLICKDATA");
Console.WriteLine(nodes.Count());

var nodes2 = document.QuerySelectorAll("ul.listContent li");
Console.WriteLine(nodes2.Count());


Console.ReadLine();


