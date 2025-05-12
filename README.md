
# SilkierQuartzEasyConfig

Easy Why To Config SilkierQuartz


## Why This Created
 + SilkierQuartz Document Is Out Of Date
 + I Cant Find Why To Configration Quartz Other Than Use quartz.config
+ When I Try Use Other Auth With SilkierQuartz Auth A Bug Happen
---
# Setting Up Quartz with SilkierQuartz and RecentHistory Plugin

## 1. Configure Quartz with Persistent Storage
First, add Quartz with persistent storage. This example uses PostgreSQL along with the **SilkierQuartz RecentHistory Plugin**:

```csharp
services.AddQuartz(opt =>
{
    opt.UsePersistentStore(o =>
    {
        o.UsePostgres(Database.Database.Configuration?.GetConnectionString("DefaultConnection")!);
        o.UseSystemTextJsonSerializer();
    });

    // Configure RecentHistory Plugin
    opt.Properties.Add("quartz.plugin.recentHistory.type",
        "Quartz.Plugins.RecentHistory.ExecutionHistoryPlugin, Quartz.Plugins.RecentHistory");
    opt.Properties.Add("quartz.plugin.recentHistory.storeType",
        "Quartz.Plugins.RecentHistory.Impl.InProcExecutionHistoryStore, Quartz.Plugins.RecentHistory");
});
```

---

## 2. Configure SilkierQuartz
> **⚠️ Note:** The order of method calls is important.

### Step 1: Add Quartz Host Configuration
Place this **directly after** `builder` initialization:

```csharp
builder.Host.ConfigureSilkierQuartzHost();
```

### Step 2: Add SilkierQuartz Services
Place this **before** `services.AddControllers();`:

```csharp
services.AddSilkierQuartzEasy(new SilkierQuartzOptions
{
    VirtualPathRoot = "/quartz",
    UseLocalTime = false
});
```

### Step 3: Configure Authentication
> **⚠️ Must be added before `AddAuthorization` or `AddAuthorizationBuilder`.**

```csharp
builder.Services.AddAuthentication()
    .AddSilkierQuartzEasyCookies(new SilkierQuartzAuthenticationOptions
    {
        AuthScheme = CookieAuthenticationDefaults.AuthenticationScheme,
        SilkierQuartzClaim = "Silkier",
        SilkierQuartzClaimValue = "Quartz",
        UserName = "admin",
        UserPassword = "Input Password",
        AccessRequirement = SilkierQuartzAuthenticationOptions.SimpleAccessRequirement.AllowOnlyUsersWithClaim,
    });
```

### Step 4: Configure Authorization
> **⚠️ Must be added after `AddAuthentication`.**

```csharp
builder.Services.AddAuthorization(options =>
{
    options.AddSilkierQuartzEasyPolicy();
});
```

### Step 5: Finalize Setup
> **⚠️ This must be added before `app.Run()`.**

```csharp
app.UseSilkierQuartzEasy();
```

## License

This project is licensed under the [MIT License](LICENSE).

© Amr Mostafa, 2025. See the LICENSE file for details.