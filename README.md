IT-Decision Telecom .NET SDK
===============================

Convenient .NET client for IT-Decision Telecom messaging API.

[![test](https://github.com/IT-DecisionTelecom/DecisionTelecom-CSharp/actions/workflows/test.yml/badge.svg)](https://github.com/IT-DecisionTelecom/DecisionTelecom-CSharp/actions/workflows/test.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

Requirements
-----

- [Sign up](https://web.it-decision.com/site/signup) for a free IT-Decision Telecom account
- Request login and password to send SMS messages and access key to send Viber messages
- IT-Decision Telecom client for C# requires **.NET Framework >= 4** or **.NET Standard >= 2.0**

Installation
-----

IT-Decision Telecom C# client could be added to your project as a **DecisionTelecom** Nuget package.
For more information on how to install Nuget package, see [Official documentation](https://docs.microsoft.com/en-us/nuget/quickstart/install-and-use-a-package-in-visual-studio).

Usage
-----

We have put some self-explanatory usage examples in the *DecisionTelecom.Examples* project,
but here is a quick reference on how IT-Decision Telecom clients work.
First, you need to create a required client. Be sure to use real login, password and access key.

```csharp
using DecisionTelecom;

SmsClient smsClient = new SmsClient("<YOUR_LOGIN>", "<YOUR_PASSWORD>");
ViberClient viberClient = new ViberClient("<YOUR_ACCESS_KEY>");
ViberPlusSmsClient viberPlusSmsClient = new ViberPlusSmsClient("<YOUR_ACCESS_KEY>");
```

Now you can use created clients to perform some operations. For example, this is how you can get your SMS balance:

```csharp
// Get your SMS balance
var balanceResult = await smsClient.GetBalanceAsync()
```

Please see other examples in the *DecisionTelecom.Examples* project for a complete overview of all available SDK calls.

### Error handling
All client methods return special `Result` object, which has flags to determine whether the operation was executed successfully or not.
If operation was successful, `Value` property contains the result of the operation. Otherwise, if operation was not successful
(if error appeared during the operation execution), `Error` property contains the corresponding error.

See provided examples on how to process `Result` object returned by the client methods.

#### SMS errors
SmsClient methods return errors in form of the error code. Here are all possible error codes:

- 40 - Invalid number
- 41 - Incorrect sender
- 42 - Invalid message ID
- 43 - Incorrect JSON
- 44 - Invalid login or password
- 45 - User locked
- 46 - Empty text
- 47 - Empty login
- 48 - Empty password
- 49 - Not enough money to send a message
- 50 - Authentication error
- 51 - Invalid phone number

#### Viber errors
ViberClient and ViberPlusSmsClient methods return errors in form of a class with the `Name`, `Message`, `Code` and `Status` properties.

If underlying API request returns unsuccessful status code (like 401 Unauthorized),
then client methods will return error with only `Name` and `Status` properties set:

```json
{
  "name": "Unauthorized",
  "status": 401
}
```

Known Viber errors are:

```json
{
  "name": "Too Many Requests",
  "message": " Rate limit exceeded",
  "code": 0,
  "status": 429
}
```

```json
{
  "name": "Invalid Parameter: [param_name]",
  "message": "Empty parameter or parameter validation error",
  "code": 1,
  "status": 400
}
```

```json
{
  "name": "Internal server error",
  "message": "The server encountered an unexpected condition which prevented it from fulfilling the request",
  "code": 2,
  "status": 500
}
```

```json
{
  "name": "Topup balance is required",
  "message": "User balance is empty",
  "code": 3,
  "status": 402
}
```
