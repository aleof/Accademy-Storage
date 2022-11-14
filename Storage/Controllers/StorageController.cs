using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using MongoDB.Driver.Linq;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using Models;
using System.Security.Cryptography;
using System.Text;

namespace Controllers
{
    public class StorageController : ControllerBase
    {
        private readonly IOptions<MongoSettings> _mydbset;
        private MD5 md5sum = MD5.Create();
        public StorageController(IOptions<MongoSettings> dbSetup)
        {
            _mydbset = dbSetup;
        }

        [HttpGet("PopolaDB", Name = "PopolaDB")]
        public IEnumerable<Questionario> PopolaDB()
        {
            MongoClientSettings settings =
                MongoClientSettings.FromConnectionString(
                    _mydbset.Value.ConnectionString
                );

            try
            {
                settings.LinqProvider = LinqProvider.V3;
                MongoClient client = new MongoClient(settings);

                var db = client.GetDatabase(_mydbset.Value.DataBaseName);

                var cls = db.GetCollection<Questionario>(_mydbset.Value.CollectionName);

                List<Questionario> Questionari = new List<Questionario>();
                for (int i = 0; i < 10; i++)
                {
                    Questionario tmp = new Questionario()
                    {
                        FirstName = $"Nome{i}",
                        LastName = $"Cognome{i}"   
                    };
                    for (int j=0; j<5; j++) {
                        tmp.Risposte[$"domanda{j}"] = $"risposta{j}";
                    }
                    Questionari.Add(tmp);
                };
                cls.InsertMany(Questionari);
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

    [HttpGet("Get", Name = "GetQuestionari")]
    public IEnumerable<Questionario> Get()
    {
        MongoClientSettings settings = 
            MongoClientSettings.FromConnectionString(
                _mydbset.Value.ConnectionString
            );
        
        settings.LinqProvider= LinqProvider.V3;
        MongoClient client = new MongoClient(settings);

        var db = client.GetDatabase(_mydbset.Value.DataBaseName);

        var cls = db.GetCollection<Questionario>(_mydbset.Value.CollectionName);

        var lsl = cls.Find(_ => true).ToList();

        return lsl;
    }

    [HttpGet("{id}",Name=nameof(GetQuestionarioById) )]
    [ProducesResponseType(200,Type=typeof(Questionario))]
    [ProducesResponseType(404)] 
    [ProducesResponseType(400)] 
    public IActionResult GetQuestionarioById(string id) {
        MongoClientSettings settings = 
            MongoClientSettings.FromConnectionString(
                _mydbset.Value.ConnectionString
            );
        
        settings.LinqProvider= LinqProvider.V3;
        MongoClient client = new MongoClient(settings);

        var db = client.GetDatabase(_mydbset.Value.DataBaseName);
        
        var cls = db.GetCollection<Questionario>(_mydbset.Value.CollectionName);

        try {
            var questionario= cls.Find(x=>x.id==id).FirstOrDefault();
            if(questionario==null)  return NotFound($"{id} not found!");       //404 todo: move to async! 
            return Ok(questionario);                         //200 
        } catch {
            return BadRequest($"{id} invalid");                    //400 
        }
        
    }

    [HttpPut("Put")]
    [ProducesResponseType(400)]
    [ProducesResponseType(204)]   
    public IActionResult PutQuestionario([FromBody] Questionario q) {
        MongoClientSettings settings = 
            MongoClientSettings.FromConnectionString(
                _mydbset.Value.ConnectionString
            );
        
        settings.LinqProvider= LinqProvider.V3;
        MongoClient client = new MongoClient(settings);

        var db = client.GetDatabase(_mydbset.Value.DataBaseName);
        
        var cls = db.GetCollection<Questionario>(_mydbset.Value.CollectionName);

        cls.InsertOne(q);

        return Ok();
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
        var cls = db.GetCollection<Questionario>(_mydbset.Value.CollectionName);
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

    [HttpPost("{id}")]
    [ProducesResponseType(400)]
    [ProducesResponseType(204)]

    public IActionResult UpdateLogEntry(string id,[FromBody] Questionario q) {
        MongoClientSettings settings = 
            MongoClientSettings.FromConnectionString(
                _mydbset.Value.ConnectionString
            );
        
        settings.LinqProvider= LinqProvider.V3;
        MongoClient client = new MongoClient(settings);

        var db = client.GetDatabase(_mydbset.Value.DataBaseName);
        
        var cls = db.GetCollection<Questionario>(_mydbset.Value.CollectionName);

        try {
            //cls.Find(_=>true).Skip().Limit();
            var buco = cls.Find(x=>x.id==id).SingleOrDefault();
            if (buco==null) return NotFound();
            //var filter = Builders<BsonDocument>.Filter.Eq("_id", id);
            //cls.DeleteOne(x=>x.id==id);
            //cls.InsertOne(log);
            cls.ReplaceOne(x=>x.id==id,q);
            return Ok(q);
        } catch(Exception ops){
            return BadRequest(ops.Message);
        }
    } 

    }
}
