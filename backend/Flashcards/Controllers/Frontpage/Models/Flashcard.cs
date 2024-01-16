using Amazon.DynamoDBv2.DataModel;

namespace Flashcards.Controllers.Frontpage.Models;

[DynamoDBTable("Flashcards")]
public class Flashcard
{
    [DynamoDBHashKey("card_id")]
    public Guid CardId { get; set; }

    [DynamoDBProperty("user_id")]
    public Guid UserId { get; set; }

    [DynamoDBProperty("word")]
    public string Word { get; set; }

    [DynamoDBProperty("definition")]
    public string Definition { get; set; }

    [DynamoDBRangeKey("bucket_id")]
    public Guid BucketId { get; set; }

    [DynamoDBProperty("bucket_name")]
    public string BucketName { get; set; }

    [DynamoDBProperty("image_url")]
    public string ImageUrl { get; set; }

public List<TestChild> Children { get; set; }
}

public class TestChild
{
    [DynamoDBProperty("kkey")]
    public string Key { get; set; }
    [DynamoDBProperty("vvalue")]
    public string Value { get; set; }
}

[DynamoDBTable("FlashcardBins")]
public class Bucket
{
    [DynamoDBHashKey("user_id")]
    public Guid UserId { get; set; }

    [DynamoDBProperty("bucket_id")]
    public Guid BucketId { get; set; }

    [DynamoDBProperty("bucket_name")]
    public string BucketName { get; set; }
}