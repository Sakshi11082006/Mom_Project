//namespace Mom_Project
//{
//    public class Program
//    {
//        public static void Main(string[] args)
//        {
//            var builder = WebApplication.CreateBuilder(args);

//            // Add services to the container.
//            builder.Services.AddControllersWithViews();

//            var app = builder.Build();

//            // Configure the HTTP request pipeline.
//            if (!app.Environment.IsDevelopment())
//            {
//                app.UseExceptionHandler("/Home/Error");
//            }
//            app.UseRouting();

//            app.UseAuthorization();

//            app.MapStaticAssets();
//            app.MapControllerRoute(
//                name: "default",
//                pattern: "{controller=Home}/{action=Index}/{id?}")
//                .WithStaticAssets();

//            app.Run();
//        }
//    }
//}

using Mom_Project.Filters;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
    builder.Services.AddControllersWithViews(options =>
    {
        options.Filters.Add(new CheckAccess());
    });

    builder.Services.AddSession(options =>
    {
        options.IdleTimeout = TimeSpan.FromMinutes(60);
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
    });

    var app = builder.Build();

// Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
    }

    app.UseStaticFiles();

    app.UseRouting();

    app.UseSession();

// Conventional Routing
    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=UserLogin}/{action=Login}/{id?}"
    );

    app.Run();
