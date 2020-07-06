# API Gateway Custom Authorizer

> A .NET Core OAuth2 implementation of a custom authorizer Lambda function for AWS API Gateway

### Overview

If you want to put auth in front of your API Gateway using a user management service other than Amazon Cognito (e.g. Auth0, Okta, etc.), then your only option is to implement a custom authorizer Lambda function. This is a .NET Core OAuth2 implementation.

## Auth Strategies

### All-or-Nothing

Out of the box this authorizer takes a simple all-or-nothing strategy. If a caller supplies a valid auth token then they will be **allowed** access to all methods on all resources, but if the caller fails to supply a valid auth token then they will be **denied** access to all methods on all resources.

### Claims-Based

However, this authorizer gives you the ability to easily implement a more granular, claims-based approach where you can allow or deny access on a per resource/method basis. You would just need to implement the `AllowMethod` and `DenyMethod` methods of the PolicyBuilder class for each resource/method you desire.

## Logging Strategy

This authorizer has its own `Logger` class. It implements structured logging to make querying and aggregating your log data much easier, as well as a customizable `LogLevel` that gives you real-time control over the level of detail in your logs.

### Structured Logging

The first thing the function handler does is set the `Request` and `Context` properties of the `EnvironmentWrapper`. These are then referenced inside any call to any of the `Log{X}` methods of the Logger, giving you things such as the AWS request ID, the method ARN, and the remaining allowed execution time of the Lambda function.

### Customizable Log Level

This authorizer has an environment variable called `LOG_LEVEL` that should be set to one of the Microsoft.Extensions.Logging.LogLevel enum values: `Trace`, `Debug`, `Information`, `Warning`, `Error`, `Critical`, or `None`. It is recommended that you set it to `Information` under normal operating conditions. However, if you start to encounter issues and are having trouble diagnosing you can simply go into the AWS Lambda Console and temporarily change the log level to `Debug` or even `Trace`, and you will immediately begin getting more detailed logs. **_Note:_** _This obviously only works for future invocations of the Lambda, and doesn't somehow miraculously dump logs from previous invocations._

