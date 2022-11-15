using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LoginClient.Models;

public class User {

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? id { get; set; }
    public string? username { get; set; }
    public string? password { get; set; }
    public string? firstName { get; set; }
    public string? lastName { get; set; }
    public string? email { get; set; }
    public bool isAdmin { get; set; }
    public string? fullName { get { return this.firstName + " " + this.lastName; } } 

}