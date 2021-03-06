
# Microsoft Identity UI with blazor components.

The aim of this project is to replace Microsoft.AspNetCore.Identity.UI cshtml files and replace them with native blazor
components. Library uses middleware to authenticate user and create Blazor version of Microsoft Identity.




## Usage/Examples

1. Add library to your project.
2. Modify your Program.cs and add this lines:

```c#
using Blazor.Identity.UI.Extensions;
using Blazor.Identity.UI.Interfaces;

builder.Services.AddSingleton<IEmailService, EmailService>();

app.UseBlazorIdentityUI();
```

Remember to implement your IEmailService in executing assembly.

3. Modify your App.razor and modify following line

```razor
@using Blazor.Identity.UI.Common

<Router AppAssembly="@typeof(App).Assembly" AdditionalAssemblies="@SharedComponents.GetAll()">
```

3. Modify or add following blazor component to display login in your app header

``` razor
<AuthorizeView>
    <Authorized>
        <a href="Account/Manage">Hello, @context.User.Identity?.Name!</a>
        <form method="post" action="Account/LogOut">
            <button type="submit" class="nav-link btn btn-link">Log out</button>
        </form>
    </Authorized>
    <NotAuthorized>
        <a href="Account/Register">Register</a>
        <a href="Account/Login">Log in</a>
    </NotAuthorized>
</AuthorizeView>
```

For further details check BlazorIdeintityUIExample project.

## Screenshots

![App Screenshot](https://github.com/Adlorem/Blazor.Identity.UI/blob/master/blazor_register.jpg?text=App+Screenshot+Login)

![App Screenshot](https://github.com/Adlorem/Blazor.Identity.UI/blob/master/blazor_login.jpg?text=App+Screenshot+Login)


