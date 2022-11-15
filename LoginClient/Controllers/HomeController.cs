using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using lab10.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using LoginClient.Models;
using MongoDB.Driver;
using MongoDB.Bson.IO;

namespace lab10.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    // Un solo client per controller
    private readonly IHttpClientFactory _httpClientFactory;

    public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    public IActionResult Index(int Page = 1)
    {
        var client = _httpClientFactory.CreateClient(name: "LoginClient.api");
        HttpRequestMessage req = new HttpRequestMessage()
        {

            Method = HttpMethod.Get
            
        };
        client.BaseAddress = new Uri($"{client.BaseAddress}/GetAllUsers");
        //int PageNumber = GetPageNumber();
        var res = client.SendAsync(req).Result;
        var dati = res.Content.ReadFromJsonAsync<IEnumerable<User>>().Result;
        return View(dati);
    }

    public async Task<IActionResult> Login(string username, string password)
    {
        var client = _httpClientFactory.CreateClient(name: "LoginClient.api");
        var dic = new Dictionary<string, string>();
        dic["username"] = username;
        dic["password"] = password;
        var uri= Microsoft.AspNetCore.WebUtilities.QueryHelpers.AddQueryString($"{client.BaseAddress}/TestLoginHashed", dic);
        var res = await client.PostAsync(uri, null);
        return Ok();
    }

    public IActionResult Details(string Id)
    {
        var client = _httpClientFactory.CreateClient(name: "lab10.api");
        HttpRequestMessage req = new HttpRequestMessage()
        {
            Method = HttpMethod.Get
            //,RequestUri = new Uri("")
        };
        client.BaseAddress = new Uri($"{client.BaseAddress}/Get/{Id}");
        var res = client.SendAsync(req).Result;
        try
        {
            res.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine(e.Message);
            return View(null);
        }
        var log = res.Content.ReadFromJsonAsync<Log>().Result;

        return View(log);
    }

    //public int GetPageNumber()
    //{
    //    var client = _httpClientFactory.CreateClient(name: "LoginClient.api");
    //    HttpRequestMessage req = new HttpRequestMessage()
    //    {
    //        Method = HttpMethod.Get

    //        //,RequestUri = new Uri("http://localhost:5134")
    //    };
    //    client.BaseAddress = new Uri($"{client.BaseAddress}/Get/Page/Number");
    //    var res = client.SendAsync(req).Result;
    //    return  res.Content.ReadFromJsonAsync<int>().Result;
    //}


    public IActionResult AddNew(User user)
    {
        var client = _httpClientFactory.CreateClient(name: "LoginClient.api");
        client.BaseAddress = new Uri($"{client.BaseAddress}/AddNewUser");
        //Log log = new()
        //{
        //    Data = DateTime.Today,
        //    NominativoRX = "RX Proxy",
        //    NominativoTX = "TX Proxy",
        //    Status = 1,
        //    ProgressivoSessione = 14
        //};
        var res = client.PutAsJsonAsync(client.BaseAddress, user).Result;
        try
        {
            res.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine(e.Message);
            return View("Details", null);
        }
        //return View("Details", log);
        return RedirectToAction("Index");
    }

    //public async Task<IActionResult> AddMany()
    //{
    //    var client = _httpClientFactory.CreateClient(name: "LoginClient.api");
    //    client.BaseAddress = new Uri($"{client.BaseAddress}/Put");
    //    for (int i = 101; i<10000; i++)
    //    {
    //        Log log = new()
    //        {
    //            Data = DateTime.Today,
    //            NominativoRX = $"RX {i}",
    //            NominativoTX = $"TX {i}",
    //            Status = i % 2,
    //            ProgressivoSessione = i
    //        };
    //        var res = await client.PutAsJsonAsync(client.BaseAddress, log);
    //        try
    //        {
    //            res.EnsureSuccessStatusCode();
    //        }
    //        catch (HttpRequestException e)
    //        {
    //            Console.WriteLine(e.Message);
    //            return RedirectToAction("Index");
    //        }
    //    }
    //    //return View("Details", log);
    //    return RedirectToAction("Index");
    //}

    public IActionResult Modify(User user)
    {
        var client = _httpClientFactory.CreateClient(name: "LoginClient.api");
        client.BaseAddress = new Uri($"{client.BaseAddress}/UpdateUser/{user.id}");
        //Log log = new()
        //{
        //    id = Id,
        //    Data = DateTime.Today,
        //    NominativoRX = "RX Modified",
        //    NominativoTX = "TX Modified",
        //    Status = 0,
        //    ProgressivoSessione = 10
        //};
        var res = client.PostAsJsonAsync(client.BaseAddress, user).Result;
        try
        {
            res.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine(e.Message);
            return View("Details", null);
        }
        //return View("Details", log);
        return RedirectToAction("Index");
    }

    public IActionResult Delete(string Id)
    {
        var client = _httpClientFactory.CreateClient(name: "LoginClient.api");
        HttpRequestMessage req = new HttpRequestMessage()
        {
            Method = HttpMethod.Delete
            //,RequestUri = new Uri("")
        };
        client.BaseAddress = new Uri($"{client.BaseAddress}/Delete/{Id}");
        var res = client.SendAsync(req).Result;
        try
        {
            res.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine(e.Message);
            return RedirectToAction("Index");
        }
        //return View("Index", null);
        return RedirectToAction("Index");
    }

    public IActionResult Edit(string? Id, int Flag = 0)
    {
        var client = _httpClientFactory.CreateClient(name: "LoginClient.api");
        HttpRequestMessage req = new HttpRequestMessage()
        {
            Method = HttpMethod.Get
            //,RequestUri = new Uri("")
        };
        if (Flag == 0)
        {
            client.BaseAddress = new Uri($"{client.BaseAddress}/UpdateUser/{Id}");
            var res = client.SendAsync(req).Result;
            try
            {
                res.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine(e.Message);
                return RedirectToAction("Index");
            }
            var log = res.Content.ReadFromJsonAsync<User>().Result;
            return View(log);
        }
        return View(null);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
