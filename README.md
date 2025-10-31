# Login and Register using ASP.Net Core Cookies

This is a example that uses cookie authentication and no aspnet identity framework.

## Project Walkthrough

`Models/User.cs`: This is the model that stores user account details, EF Core can be used with it but for now it it is user are initialised at startup and stored memory there is no user persistance.

`ViewModels/{LoginViewModel.cs,RegisterViewModel.cs}`: Instead of passing a instance of `User` to a view two viewmodels are used one for login and one for register. Notice how the `User` stores a password hash but the view models recive a raw password. This is the reason for creating seperate view models as it makes it easier to access the password in the view and recieve it from the form, without it the password would need to be hashed in the browser, which may actually be better or not ¯\_(ツ)_/¯.

## An important difference

One very important difference/detail between existing examples for setting up frameworkless identity is the proper use of `ReturnUrl` handling and signin.

Most examples will show that to sigin you must use `await HttpContext.SignInAsync` and pass `ReturnUrl` explicitly to the View and use hidden fields in the from and use `LocalRedirect(Url.GetLocalUrl(returnUrl))`. My question is why, when Cookie Authentication is setup, you can configure `options.ReturnUrlParameter = "ReturnUrl";` which is added to a login redirect when a user tries to access a `Authorize` annotated route. If this return url is being set automatically then why do I need to handle it manually.

I think it stems from the fact that with a default html form template and no special handling the `ReturnUrl` gets overriden which makes it impossible to for `HttpPost` handler to access the return url.


Take a look at this cshtml example:

```html
<form asp-controller="Account" asp-action="Login" method="post">
</form>
```

This is render to html as:

```html
<form action="/Account/Login" method="post">
</form>
```

The browser will use this action explicily override any query parameters set in a redirect so you have to add a hidden form field like: 

```html
<input type="hidden" name="ReturnUrl" value="/Dashboard">
```

An example http session:

```http
GET /Dashboard

HTTP/1.1 302 Found
Location: /Account/Login?ReturnUrl=%2FDashboard

---
GET /Account/Login?ReturnUrl=%2FDashboard

HTTP/1.1 200 Ok

<form action="/Account/Login" method="post">
    <input type="hidden" name="ReturnUrl" value="/Dashboard">
    <input type="text" name="username">
    <input type="password" name="password">
</form>

---
POST /Account/Login
Content-Type: application/x-www-form-urlencoded

username=henry&password=danger&ReturnUrl=%2FDashboard

HTTP/1.1 302 Found
Location: /Dashboard
Cookies: ...

GET /Dashboard
```

But ASP can automatically handle this but the default form template is wrong.

There is a function that is part of the controller that called `SignIn` which returns a `SignInResult` which is an implementation of `IActionResult`. It does the same thing as `await HttpContext.SignInAsync` but can automatically redirect based on `ReturnUrl`. The problem is it only looks at a request query parameters not at any form fiels so frustration that `ReturnUrl` is not being set, examples turn to manually passing `ReturnUrl`

The simple fix for this is to change the cshtml template to not include the asp-action or any action.

```html
<form method="post">
</form>
```

This now means that whatever url the browser has as the current location is used to send back the post request which almost always what you want, unless of cause the post request needs a different route.

What this looks like:

```http
GET /Dashboard

HTTP/1.1 302 Found
Location: /Account/Login?ReturnUrl=%2FDashboard

---
GET /Account/Login?ReturnUrl=%2FDashboard

HTTP/1.1 200 Ok

<form method="post">
    <input type="text" name="username">
    <input type="password" name="password">
</form>

---
POST /Account/Login?ReturnUrl=%2FDashboard
Content-Type: application/x-www-form-urlencoded

username=henry&password=danger

HTTP/1.1 302 Found
Location: /Dashboard
Cookies: ...

GET /Dashboard
```