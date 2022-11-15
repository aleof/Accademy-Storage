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
    [ApiController]
    [Route("[controller]")]
    public class StorageController : ControllerBase
    {
        private readonly IOptions<MongoSettings> _mydbset;
        private MD5 md5sum = MD5.Create();
        private string psk = "psk";
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
                        UserName = $"UserID{i}",
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
    public IActionResult Get()
    {
       //if (Request.Headers["_token"] != psk)
       //     return null;
        MongoClientSettings settings = 
            MongoClientSettings.FromConnectionString(
                _mydbset.Value.ConnectionString
            );
        
        settings.LinqProvider= LinqProvider.V3;
        MongoClient client = new MongoClient(settings);

        var db = client.GetDatabase(_mydbset.Value.DataBaseName);

        var cls = db.GetCollection<Questionario>(_mydbset.Value.CollectionName);

        var lsl = cls.Find(_ => true).ToList();

        return Ok(lsl);
    }

    [HttpGet("{QuestionarioID}",Name=nameof(GetQuestionarioById) )]
    [ProducesResponseType(200,Type=typeof(Questionario))]
    [ProducesResponseType(404)] 
    [ProducesResponseType(400)] 
    public IActionResult GetQuestionarioById(string QuestionarioID) {
            if (Request.Headers["_token"] != psk)
                return BadRequest($"Invalid token:{Request.Headers["_token"]}" );

            MongoClientSettings settings = 
            MongoClientSettings.FromConnectionString(
                _mydbset.Value.ConnectionString
            );
        
        settings.LinqProvider= LinqProvider.V3;
        MongoClient client = new MongoClient(settings);

        var db = client.GetDatabase(_mydbset.Value.DataBaseName);
        
        var cls = db.GetCollection<Questionario>(_mydbset.Value.CollectionName);

        try {
            var questionario= cls.Find(x=>x.id==QuestionarioID).FirstOrDefault();
            if(questionario==null)  return NotFound($"{QuestionarioID} not found!");       //404 todo: move to async! 
            return Ok(questionario);                         //200 
        } catch {
            return BadRequest($"{QuestionarioID} invalid");                    //400 
        }
        
    }

        [HttpGet("User/{UserName}", Name = nameof(GetQuestionariByUserName))]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Questionario>))]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public IActionResult GetQuestionariByUserName(string UserName)
        {
            //if (Request.Headers["_token"] != psk)
            //    return BadRequest($"Invalid token:{Request.Headers["_token"]}");

            MongoClientSettings settings =
            MongoClientSettings.FromConnectionString(
                _mydbset.Value.ConnectionString
            );

            settings.LinqProvider = LinqProvider.V3;
            MongoClient client = new MongoClient(settings);

            var db = client.GetDatabase(_mydbset.Value.DataBaseName);

            var cls = db.GetCollection<Questionario>(_mydbset.Value.CollectionName);

            try
            {
                var questionario = cls.Find(x => x.UserName == UserName).ToList();
                if (questionario == null) return NotFound($"{UserName} not found!");       //404 todo: move to async! 
                return Ok(questionario);                         //200 
            }
            catch
            {
                return BadRequest($"{UserName} invalid");                    //400 
            }

        }

        [HttpPut("Put")]
    [ProducesResponseType(400)]
    [ProducesResponseType(204)]   
    public IActionResult PutQuestionario([FromBody] Questionario q) {
            if (Request.Headers["_token"] != psk)
                return BadRequest($"Invalid token:{Request.Headers["_token"]}");
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

    [HttpDelete("delete/{QuestionarioID}",Name="Delete")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)] 
    [ProducesResponseType(400)] 
    public IActionResult Delete(string QuestionarioID) {
            if (Request.Headers["_token"] != psk)
                return BadRequest($"Invalid token:{Request.Headers["_token"]}");
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
            if( cls.Find(x=>x.id== QuestionarioID).CountDocuments()<1 )  return NotFound();
            //Delete
            cls.DeleteOne(x=>x.id==QuestionarioID);
            //200: OK 
            return Ok();
        }catch(Exception ops){
            return BadRequest(ops.Message);
        }

    }

    [HttpPost("{QuestionarioID}")]
    [ProducesResponseType(400)]
    [ProducesResponseType(204)]

    public IActionResult UpdateQuestionario(string QuestionarioID,[FromBody] Questionario q) {
            if (Request.Headers["_token"] != psk)
                return BadRequest($"Invalid token:{Request.Headers["_token"]}");
            MongoClientSettings settings = 
            MongoClientSettings.FromConnectionString(
                _mydbset.Value.ConnectionString
            );
        
        settings.LinqProvider= LinqProvider.V3;
        MongoClient client = new MongoClient(settings);

        var db = client.GetDatabase(_mydbset.Value.DataBaseName);
        
        var cls = db.GetCollection<Questionario>(_mydbset.Value.CollectionName);

        try {
            var quest = cls.Find(x=>x.id==QuestionarioID).SingleOrDefault();
            if (quest == null) return NotFound();
            cls.ReplaceOne(x=>x.id==QuestionarioID,q);
            return Ok(q);
        } catch(Exception ops){
            return BadRequest(ops.Message);
        }
    } 

    }
}
