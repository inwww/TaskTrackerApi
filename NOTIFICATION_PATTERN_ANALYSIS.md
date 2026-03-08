## Task Tracker API - Notification Service Integration Pattern Analysis

### Overview
This document outlines the recommended integration pattern for adding a NotificationService that should send emails when tasks are completed in the Task Service.

### Integration Pattern Recommendation: **Asynchronous Event-Driven Architecture**

#### Why Asynchronous?

1. **Decoupling**: The Task Service should not depend on the success or failure of the Notification Service
2. **Performance**: Task completion should not be blocked by email delivery
3. **Resilience**: Email failures should not cause task completion to fail
4. **Scalability**: Asynchronous processing allows independent scaling of both services
5. **Reliability**: Failed notifications can be retried without affecting the core business logic

#### Recommended Technology Stack

**Message Broker: RabbitMQ**
- Reliable message delivery with acknowledgments
- Message persistence to disk
- Dead letter queues for failed messages
- Support for complex routing patterns
- Easy integration with .NET via `RabbitMQ.Client` NuGet package

**Alternative: Azure Service Bus** (for cloud deployments)
- Enterprise-grade reliability
- Automatic message deduplication
- Session-based message processing
- Integration with other Azure services

### Architecture Pattern: Event-Driven Microservices

```
┌─────────────────────────────────────────────────────────┐
│                    Task Service                          │
│  ┌────────────────────────────────────────────────────┐ │
│  │  When CompleteTask() is called:                     │ │
│  │  1. Task marked as completed                        │ │
│  │  2. OnTaskCompleted event triggered                │ │
│  │  3. Event published to Message Broker              │ │
│  │  4. Response returned to client (no wait)          │ │
│  └────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────┘
                          │
                          │ (Message Queue)
                          │ RabbitMQ / Service Bus
                          ▼
┌─────────────────────────────────────────────────────────┐
│               Notification Service                       │
│  ┌────────────────────────────────────────────────────┐ │
│  │  Consumer subscribes to TaskCompleted events:      │ │
│  │  1. Receives message from queue                    │ │
│  │  2. Prepares email (template rendering)            │ │
│  │  3. Sends email via SMTP (SendGrid, AWS SES etc)   │ │
│  │  4. Logs result and acknowledges message           │ │
│  │  5. If fails: message goes to DLQ for retry        │ │
│  └────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────┘
```

### Implementation Approach

#### 1. Message Definition
```csharp
public record TaskCompletedEvent
{
    public Guid TaskId { get; set; }
    public string Title { get; set; }
    public DateTime CompletedAt { get; set; }
    public string UserEmail { get; set; }
}
```

#### 2. Publishing the Event (Task Service)
```csharp
// In BaseTask.CompleteTask()
public IEventPublisher _eventPublisher;

task.OnTaskCompleted += (sender, args) =>
{
    var @event = new TaskCompletedEvent
    {
        TaskId = args.TaskId,
        Title = args.Title,
        CompletedAt = args.CompletedAt,
        UserEmail = userEmail
    };
    
    await _eventPublisher.PublishAsync("task.completed", @event);
};
```

#### 3. Consuming the Event (Notification Service)
```csharp
public class TaskCompletedEventConsumer : IEventConsumer
{
    private readonly IEmailService _emailService;
    
    public async Task HandleAsync(TaskCompletedEvent @event)
    {
        try
        {
            await _emailService.SendTaskCompletionEmailAsync(
                @event.UserEmail,
                @event.Title,
                @event.CompletedAt
            );
        }
        catch (Exception ex)
        {
            // Log error - message will go to DLQ if not acknowledged
            _logger.LogError(ex, "Failed to send notification for task {@Task}", @event);
            throw; // Cause NACK for retry
        }
    }
}
```

### Why NOT Synchronous (HTTP/REST)?

❌ **HTTP/REST Synchronous Pattern**: 
- Task completion would be blocked until email is sent
- If Notification Service is down, task completion fails
- Creates hard dependency between services
- Difficult to handle timeouts and retries
- Poor user experience (long wait times)

### Implementation Technologies

| Component | Technology | Package |
|-----------|-----------|---------|
| Message Broker | RabbitMQ | `RabbitMQ.Client` v6.x |
| Event Publisher | Custom or MassTransit | `MassTransit` / `EasyNetQ` |
| Email Service | SendGrid / AWS SES | `SendGrid.Extensions.Mail` |
| Serialization | JSON | System.Text.Json (built-in) |
| Retry Policy | Polly | `Polly` |

### Deployment Considerations

**docker-compose.yml** would include:
```yaml
services:
  task-service:
    # ASP.NET Core app
  
  notification-service:
    # Email notification consumer
  
  rabbitmq:
    image: rabbitmq:management
    # RabbitMQ message broker
```

### Reliability Guarantees

1. **At-Least-Once Delivery**: Messages are persisted and retried
2. **Dead Letter Queue**: Failed messages after max retries go to DLQ for inspection
3. **Idempotency**: Notification Service should handle duplicate events gracefully
4. **Message TTL**: Set appropriate time-to-live for message queues
5. **Monitoring**: Track queue depth, consumer lag, and DLQ messages

### Future Enhancements

- **Webhook Support**: Allow clients to subscribe to task events via webhooks (HTTP callbacks)
- **Multiple Notification Channels**: SMS, Slack, Teams in addition to email
- **Event Sourcing**: Full audit trail of all task events
- **Saga Pattern**: Orchestrate complex workflows across multiple services
- **Event Store**: Use EventStoreDB for event persistence and history

### Conclusion

The asynchronous event-driven pattern using RabbitMQ provides:
- ✅ Loose coupling between services
- ✅ Improved resilience and fault tolerance
- ✅ Better performance and scalability
- ✅ Support for future extensions (multiple notification channels)
- ✅ Ability to replay events for auditing

This is the industry-standard approach for microservice-based distributed systems.
