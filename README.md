# API Gateway Custom Authorizer

> A .NET Core OAuth2 implementation of a custom authorizer Lambda function for AWS API Gateway

### Overview

If you want to put auth in front of your API Gateway using a user management service other than Amazon Cognito (e.g. Auth0, Okta, etc.), then your only option is to implement a custom authorizer Lambda function. This is a .NET Core OAuth2 implementation that provides various strategies for how to configure your auth, as well as helpful logging strategies that will help you diagnose issues and offer the ability to create robust dashboards for observability.

## Auth Strategies

### JSON Web Key Sets (JWKS)

This authorizer offers three methods for acquiring the JSON web key sets used to validate a caller's token:

1. Environment variable
   - This is the preferred method. It's very quick and therefore has minimal impact on latency. These keys are public by definition so this shouldn't pose a security threat. To implement, you would set the `JWKS` environment variable. For example, if you use Auth0 you would copy the results of a call to `https://YOUR_DOMAIN/.well-known/jwks.json`.
2. SSM parameter
   - If you feel you need more security then you may choose to use an SSM parameter. A stubbed out method is provided that you would need to implement yourself.
3. HTTP request
   - This is a horrible option as it will add a ton of unnecessary latency to each invocation. To implement, you would set the `JWKS_URI` environment variable. For example, if you use Auth0 you would set the value to `https://YOUR_DOMAIN/.well-known/jwks.json`.

### All-or-Nothing vs Claims-Based

Out of the box this authorizer takes a simple all-or-nothing strategy. If a caller supplies a valid auth token then they will be **allowed** access to all methods on all resources, but if the caller fails to supply a valid auth token then they will be **denied** access to all methods on all resources.

However, this authorizer also gives you the ability to easily implement a more granular, claims-based approach where you can allow or deny access on a per resource/method basis. You would just need to implement the `AllowMethod` and `DenyMethod` methods of the PolicyBuilder class for each resource/method you desire.

## Logging Strategies

This authorizer has its own `Logger` class. It implements structured logging to make querying and aggregating your log data much easier, as well as a customizable `LogLevel` that gives you real-time control over the level of detail in your logs.

### Customizable Log Level

This authorizer has an environment variable called `LOG_LEVEL` that should be set to one of the `LogLevel` enum values found in the `Microsoft.Extensions.Logging` package (i.e. `Trace`, `Debug`, `Information`, `Warning`, `Error`, `Critical`, or `None`). The `Logger` will actively filter out any logs with a type less than this value.

```
if (CurrentLogLevel.Value > logLevel)
{
    return;
}
```

It is recommended that you set it to `Information` under normal operating conditions. However, if you're having trouble diagnosing an issue you can simply change the log level to `Debug` or `Trace` (via the AWS CLI or the AWS Lambda Console), and you will immediately begin seeing more detailed logs.

**_Note:_** _This obviously only works for future invocations of the Lambda, and doesn't somehow miraculously dump logs from previous invocations._

### Structured Logging

The first thing the function handler does is set the `Request` and `Context` properties of the `EnvironmentWrapper`. These are then referenced inside any call to any of the `Log{X}` methods of the Logger, giving you things such as the AWS request ID, the method ARN, and the remaining allowed execution time of the Lambda function.

```
var logDynamic = new
{
    Time = DateTime.UtcNow.ToString("o"),
    Level = $"{logLevel}",
    Logger = _name,
    Message = message,
    Exception = e?.ToString(),
    Data = logData,
    _environmentWrapper.Version,
    _environmentWrapper.Context?.AwsRequestId,
    _environmentWrapper.Context?.FunctionName,
    _environmentWrapper.Context?.RemainingTime,
    _environmentWrapper.Request?.MethodArn,
};
```

The dynamic object is then serialized as JSON and passed into the `LambdaLogger` provided by the `Amazon.Lambda.Core` dependency.

```
var log = JsonSerializer.Serialize(logDynamic, JsonConfig.SerializerOptions);

LambdaLogger.Log(log);
```

Ensuring all of your logs are structured like this buys you the ability to query them much easier. This will obviously come in handy if you're ever trying to diagnose an issue, but it also makes it much easier to aggregate your log data so that you can create robust dashboards (e.g. AWS, Splunk, DataDog, etc.) that give you observability into your serverless architecture.
