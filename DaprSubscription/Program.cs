using Dapr.Client;
using DaprSubscription;
using Microsoft.AspNetCore.Http.HttpResults;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDaprClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.MapGet("/dapr/subscribe", () => {
    var subscriptions = new Subscription[] {
            new Subscription
            {
                Topic = "TestTopic",
                PubsubName = "kubemq",
                Routes = new Routes
                {
                    Rules = new List<Rule>
                    {
                        new Rule
                        {
                            Match = "event.type == 'KubeMQ.Data.Created'",
                            Path = "/dapr/data/created"
                        }
                    }
                },
                DeadLetterTopic = "poisonMessages",
                Metadata = new Metadata
                {
                    { Metadata.RawPayload, "false" },
                    // { "clientId", "LicManagerCore" },
                    // { "group", "licmanager" },
                    { "subscriptionType", "StartFromFirstEvent" }
                }
            }
        };
    return subscriptions;

}).ExcludeFromDescription();

app.MapPost("/dapr/data/created", (Data data) =>
{
    Console.WriteLine($"Data event : {data.Id} with name {data.Name} has been received");
}).ExcludeFromDescription();

app.MapGet("/sendData/{id}/name/{name}", async (Guid id, string name, DaprClient dapr) => {
    await dapr.PublishEventAsync("kubemq", "TestTopic", new Data { Id = id, Name = name }, new Dictionary<string, string> { { "cloudevent.type", "KubeMQ.Data.Created" } });
}).WithName("PublishData").WithOpenApi();



app.UseCloudEvents();

app.Run();

