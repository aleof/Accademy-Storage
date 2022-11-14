using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MongoDB.Bson;

using Models;

namespace lab08.Controllers;

[ApiController]
[Route("[controller]")]
public class LogController : ControllerBase
{
    /*private readonly IOptions<Models.MongoSettings> _mydbset;

    public LogController(IOptions<Models.MongoSettings> dbSetup) {
        _mydbset = dbSetup;
    }

    [HttpGet(Name = "GetLogBook")]
    public IEnumerable<LogEntry> Get()
    {
        MongoClientSettings settings = 
            MongoClientSettings.FromConnectionString(
                _mydbset.Value.ConnectionString
            );
        
        settings.LinqProvider= LinqProvider.V3;
        MongoClient client = new MongoClient(settings);

        var db = client.GetDatabase(_mydbset.Value.DataBaseName);

        var cls = db.GetCollection<LogEntry>(_mydbset.Value.CollectionName);

        var lsl = cls.Find(_ => true).ToList();

        return lsl;
    }
    [HttpGet("page{page}",Name = "GetPaged")]
    public IEnumerable<LogEntry> GetPage(int? page)
    {
        //My 
        const int PageLen=20;

        MongoClientSettings settings = 
            MongoClientSettings.FromConnectionString(
                _mydbset.Value.ConnectionString
            );
        
        settings.LinqProvider= LinqProvider.V3;
        MongoClient client = new MongoClient(settings);

        var db = client.GetDatabase(_mydbset.Value.DataBaseName);

        var cls = db.GetCollection<LogEntry>(_mydbset.Value.CollectionName);

        //Added Lmit & skip
        var lsl = cls.Find(_ => true)
                        .Skip(page*PageLen)
                        .Limit(PageLen)
                        .ToList();

        return lsl;
    }

    [HttpGet("PopolaDB", Name = "PopolaDB")]
    public IEnumerable<LogEntry> PopolaDB(){
        MongoClientSettings settings = 
            MongoClientSettings.FromConnectionString(
                _mydbset.Value.ConnectionString
            );
        
        settings.LinqProvider= LinqProvider.V3;
        MongoClient client = new MongoClient(settings);

        var db = client.GetDatabase(_mydbset.Value.DataBaseName);
        
        var cls = db.GetCollection<LogEntry>(_mydbset.Value.CollectionName);
        
        List<LogEntry> LogBook= new List<LogEntry>();
        for (int i =0;i<10;i++) {
            LogEntry tmp = new LogEntry(){
                RST="599",
                ToQRZ=$"ik{i}tmp",
                FromQRZ="iz2vto",
                DATAQSO=DateTime.UtcNow,
                Status=0,
                Progressivo=i
            };
            LogBook.Add(tmp);
        }
        cls.InsertMany(LogBook);
        return Get();
    }

    [HttpGet("{id}",Name=nameof(GetLogEntryById) )]
    [ProducesResponseType(200,Type=typeof(LogEntry))]
    [ProducesResponseType(404)] 
    [ProducesResponseType(400)] 
    public IActionResult GetLogEntryById(string id) {
        MongoClientSettings settings = 
            MongoClientSettings.FromConnectionString(
                _mydbset.Value.ConnectionString
            );
        
        settings.LinqProvider= LinqProvider.V3;
        MongoClient client = new MongoClient(settings);

        var db = client.GetDatabase(_mydbset.Value.DataBaseName);
        
        var cls = db.GetCollection<LogEntry>(_mydbset.Value.CollectionName);

        try {
            var Log= cls.Find(x=>x.id==id).FirstOrDefault();
            if(Log==null)  return NotFound($"{id} not found!");       //404 todo: move to async! 
            return Ok(Log);                         //200 
        } catch {
            return BadRequest($"{id} invalid");                    //400 
        }
        
    }

    //ByQrz
    [HttpGet("qrz/{qrz}",Name=nameof(GetLogEntryByQrz) )]
    [ProducesResponseType(200,Type=typeof(List<LogEntry>))]
    [ProducesResponseType(404)] 
    [ProducesResponseType(400)] 
    public IActionResult GetLogEntryByQrz(string qrz) {
        MongoClientSettings settings = 
            MongoClientSettings.FromConnectionString(
                _mydbset.Value.ConnectionString
            );
        
        settings.LinqProvider= LinqProvider.V3;
        MongoClient client = new MongoClient(settings);

        var db = client.GetDatabase(_mydbset.Value.DataBaseName);
        
        var cls = db.GetCollection<LogEntry>(_mydbset.Value.CollectionName);

        try {
            var Log= cls.Find(x=>x.ToQRZ==qrz).ToList();
            if(Log==null)  return NotFound($"{qrz} not found!");       //404 todo: move to async! 
            if(Log.Count==0) return NotFound($"{qrz} not found");
            return Ok(Log);                         //200 
        } catch {
            return BadRequest($"{qrz} invalid");                    //400 
        }
        
    }
    [HttpPut()]
    //[HttpPut("{id}")]
    [ProducesResponseType(400)]
    [ProducesResponseType(204)]
    //public IActionResult PutLogEntry(string id,[FromBody] LogEntry log) {
    public IActionResult PutLogEntry([FromBody] LogEntry log) {
        MongoClientSettings settings = 
            MongoClientSettings.FromConnectionString(
                _mydbset.Value.ConnectionString
            );
        
        settings.LinqProvider= LinqProvider.V3;
        MongoClient client = new MongoClient(settings);

        var db = client.GetDatabase(_mydbset.Value.DataBaseName);
        
        var cls = db.GetCollection<LogEntry>(_mydbset.Value.CollectionName);

        cls.InsertOne(log);

        return Ok();
    }

    [HttpPost("{id}")]
    //[HttpPut("{id}")]
    [ProducesResponseType(400)]
    [ProducesResponseType(204)]
    //public IActionResult PutLogEntry(string id,[FromBody] LogEntry log) {
    public IActionResult UpdateLogEntry(string id,[FromBody] LogEntry log) {
        MongoClientSettings settings = 
            MongoClientSettings.FromConnectionString(
                _mydbset.Value.ConnectionString
            );
        
        settings.LinqProvider= LinqProvider.V3;
        MongoClient client = new MongoClient(settings);

        var db = client.GetDatabase(_mydbset.Value.DataBaseName);
        
        var cls = db.GetCollection<LogEntry>(_mydbset.Value.CollectionName);

        try {
            //cls.Find(_=>true).Skip().Limit();
            var buco = cls.Find(x=>x.id==id).SingleOrDefault();
            if (buco==null) return NotFound();
            //var filter = Builders<BsonDocument>.Filter.Eq("_id", id);
            //cls.DeleteOne(x=>x.id==id);
            //cls.InsertOne(log);
            cls.ReplaceOne(x=>x.id==id,log);
            return Ok(log);
        } catch(Exception ops){
            return BadRequest(ops.Message);
        }
    } 

    [HttpDelete("delete/{id}",Name="Delete")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)] 
    [ProducesResponseType(400)] 
    public IActionResult Delete(string id) {
         MongoClientSettings settings = 
            MongoClientSettings.FromConnectionString(
                _mydbset.Value.ConnectionString
            );
        
        settings.LinqProvider= LinqProvider.V3;
        //Connection
        MongoClient client = new MongoClient(settings);
        //use database 
        var db = client.GetDatabase(_mydbset.Value.DataBaseName);
        //Select collection
        var cls = db.GetCollection<LogEntry>(_mydbset.Value.CollectionName);
        //Do the job
        try {
            //Check Exists
            if( cls.Find(x=>x.id==id).CountDocuments()<1 )  return NotFound();
            //Delete
            cls.DeleteOne(x=>x.id==id);
            //200: OK 
            return Ok();
        }catch(Exception ops){
            return BadRequest(ops.Message);
        }
    }
    */
}
