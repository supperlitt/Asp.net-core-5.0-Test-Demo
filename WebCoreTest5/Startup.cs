using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using WebCoreTest5.Db;
using WebCoreTest5.Http;
using WebCoreTest5.Model;
using System.Text.Json;
using WebCoreTest5.Converter;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Net;
using System.IO;
using AngleSharp;
using OpenIddict.Abstractions;
using WebCoreTest5.Handler;
using WebCoreTest5.Tool;
using WebCoreTest5.Filters;
using WebCoreTest5.Middleware;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.FileProviders;
using WebCoreTest5.Provider;
using Microsoft.AspNetCore.Builder.Extensions;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using WebCoreTest5.Controllers;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Logging;
using WebCoreTest5.Data;

namespace WebCoreTest5
{
    public class Startup
    {
        public Microsoft.Extensions.Configuration.IConfiguration _Configuration
        {
            get; set;
        }

        public Startup(Microsoft.Extensions.Configuration.IConfiguration Configuration)
        {
            this._Configuration = Configuration;
        }


        public static readonly ILoggerFactory MyLoggerFactory = LoggerFactory.Create(builder => { builder.AddConsole(); });

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // System.Text.Json不支持全局配置 Newtonsoft.Json 可以支持全局配置
            services.AddControllersWithViews().AddJsonOptions(options =>
            {
                // 格式化日期时间格式
                options.JsonSerializerOptions.Converters.Add(new DatetimeJsonConverter());

                // 数据格式原样输出
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
            });

            services.AddRazorPages();

            services.AddMvc(options => { options.EnableEndpointRouting = false; });
            services.AddMvc().SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Latest);

            // System.Text.Json不支持全局配置 Newtonsoft.Json 可以支持全局配置
            services.AddMvc().AddJsonOptions(options =>
            {
                // 格式化日期时间格式
                options.JsonSerializerOptions.Converters.Add(new DatetimeJsonConverter());

                // 数据格式原样输出
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
            });

            // 过滤，哪些？
            services.AddMvc(o =>
            {
                o.Filters.Add(new XFrameOptionsAttribute("DENY"));
                o.Filters.Add(new XContentTypeOptionsAttribute("nosniff"));
                o.Filters.Add(new XXSSProtectionAttribute());
                o.Filters.Add(new ReferrerPolicyAttribute("same-origin"));
            });

            // services.AddDefaultIdentity<MyUser>(options => options.SignIn.RequireConfirmedAccount = false);

            // options.UseSqlServer(  Configuration.GetConnectionString("DefaultConnection"))
            services.AddDbContext<ApplicationDbContext>(options =>
                {
                    var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
                    var configuration = builder.Build();
                    options.UseMySql(configuration.GetConnectionString("TestDb"), ServerVersion.AutoDetect(configuration.GetConnectionString("TestDb")));

                    // 打印 EFCore 执行的真实SQL
                    options.UseLoggerFactory(MyLoggerFactory);
                });

            services.AddHttpClient();
            services.AddHttpClient(nameof(MyHttpProvider), httpClient =>
            {
                httpClient.Timeout = Timeout.InfiniteTimeSpan;
            });
            services.TryAddSingleton<MyHttpProvider>();

            services.AddScoped<IFactory, MyFactory>();

            // xss 漏洞处理 
            services.TryAddSingleton<Ganss.XSS.HtmlSanitizer>(o =>
            {
                var htmlSanitizer = new Ganss.XSS.HtmlSanitizer();
                //htmlSanitizer.RemovingAtRule += (sender, args) =>
                //{
                //};
                //htmlSanitizer.RemovingTag += (sender, args) =>
                //{
                //    if (args.Tag.TagName.Equals("img", StringComparison.InvariantCultureIgnoreCase))
                //    {
                //        if (!args.Tag.ClassList.Contains("img-fluid"))
                //        {
                //            args.Tag.ClassList.Add("img-fluid");
                //        }

                //        args.Cancel = true;
                //    }
                //};

                //htmlSanitizer.RemovingAttribute += (sender, args) =>
                //{
                //    if (args.Tag.TagName.Equals("img", StringComparison.InvariantCultureIgnoreCase) &&
                //        args.Attribute.Name.Equals("src", StringComparison.InvariantCultureIgnoreCase) &&
                //        args.Reason == Ganss.XSS.RemoveReason.NotAllowedUrlValue)
                //    {
                //        args.Cancel = true;
                //    }
                //};
                //htmlSanitizer.RemovingStyle += (sender, args) => { args.Cancel = true; };
                //htmlSanitizer.AllowedAttributes.Add("class");
                //htmlSanitizer.AllowedTags.Add("iframe");
                //htmlSanitizer.AllowedTags.Remove("img");
                //htmlSanitizer.AllowedAttributes.Add("webkitallowfullscreen");
                //htmlSanitizer.AllowedAttributes.Add("allowfullscreen");
                return htmlSanitizer;
            });

            // 允许访问
            services.AddCors(options =>
            {
                options.AddPolicy("TestPolicy", p => p.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
            });

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;
                options.Password.RequireUppercase = false;
                // Configure Identity to use the same JWT claims as OpenIddict instead
                // of the legacy WS-Federation claims it uses by default (ClaimTypes),
                // which saves you from doing the mapping in your authorization controller.
                //options.ClaimsIdentity.UserNameClaimType = OpenIddictConstants.Claims.Name;
                //options.ClaimsIdentity.UserIdClaimType = OpenIddictConstants.Claims.Subject;
                //options.ClaimsIdentity.RoleClaimType = OpenIddictConstants.Claims.Role;
            });

            // If the HTTPS certificate path is not set this logic will NOT be used and the default Kestrel binding logic will be.
            services.Configure<KestrelServerOptions>(kestrel =>
            {
                kestrel.Limits.MaxRequestLineSize = 8_192 * 10 * 5; // Around 500K, transactions passed in URI should not be bigger than this
            });

            bool is_443 = false;
            if (is_443)
            {
                var bindAddress = IPAddress.Any;
                int bindPort = 443;
                services.Configure<KestrelServerOptions>(kestrel =>
                {
                    kestrel.Listen(bindAddress, bindPort, l =>
                    {
                        l.UseHttps();
                        // l.UseHttps(httpsCertificateFilePath, Configuration.GetOrDefault<string>("HttpsCertificateFilePassword", null));
                    });
                });
            }

            // OIDC 身份认证
            services.AddOpenIddict()
                .AddCore(options =>
                {
                    // Configure OpenIddict to use the Entity Framework Core stores and entities.
                    options.UseEntityFrameworkCore().UseDbContext<ApplicationDbContext>();
                })
                .AddServer(options =>
                {
                    options.UseAspNetCore()
                        .EnableStatusCodePagesIntegration()
                        .EnableAuthorizationEndpointPassthrough()
                        .EnableLogoutEndpointPassthrough()
                        .EnableTokenEndpointPassthrough()
                        .EnableAuthorizationRequestCaching()
                        .DisableTransportSecurityRequirement();

                    // Enable the token endpoint (required to use the password flow).
                    options.SetTokenEndpointUris("/connect/token");
                    options.SetAuthorizationEndpointUris("/connect/authorize");
                    options.SetLogoutEndpointUris("/connect/logout");

                    //we do not care about these granular controls for now
                    options.IgnoreScopePermissions();
                    options.IgnoreEndpointPermissions();
                    // Allow client applications various flows
                    options.AllowImplicitFlow();
                    options.AllowClientCredentialsFlow();
                    options.AllowRefreshTokenFlow();
                    options.AllowPasswordFlow();
                    options.AllowAuthorizationCodeFlow();
                    // options.UseRollingTokens();

                    options.RegisterScopes(
                        OpenIddictConstants.Scopes.OpenId
                    );
                    //options.AddEventHandler(PasswordGrantTypeEventHandler.Descriptor);
                    //options.AddEventHandler(OpenIdGrantHandlerCheckCanSignIn.Descriptor);
                    //options.AddEventHandler(ClientCredentialsGrantTypeEventHandler.Descriptor);
                    //options.AddEventHandler(LogoutEventHandler.Descriptor);

                    options.AddSigningKey(SignKey.GetSigningKey(this._Configuration, "signing.rsaparams"))
                            .AddEncryptionKey(SignKey.GetSigningKey(this._Configuration, "encrypting.rsaparams"));
                })
                .AddValidation(options =>
                {
                    options.UseLocalServer();
                    options.UseAspNetCore();
                });

            services.AddSession();
            services.AddSignalR();

            services.AddMemoryCache();

            // 压缩响应流
            services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    new[] { "application/octet-stream" });
            });

            //添加jwt验证：
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,//是否验证Issuer
                        ValidateAudience = true,//是否验证Audience
                        ValidateLifetime = true,//是否验证失效时间
                        ClockSkew = TimeSpan.FromSeconds(30),
                        ValidateIssuerSigningKey = true,//是否验证SecurityKey
                        // ValidAudience = Const.Domain,//Audience
                        ValidIssuer = Const.Domain,//Issuer，这两项和前面签发jwt的设置一致
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Const.SecurityKey)), // 拿到SecurityKey
                        AudienceValidator = (m, n, z) =>
                        {
                            var token = (n as JwtSecurityToken);
                            var caudience = token.Audiences.FirstOrDefault();
                            Console.WriteLine("check caudience " + caudience);
                            Console.WriteLine(m != null ? ("m token " + m.FirstOrDefault()) : "m token null");
                            return m != null && m.FirstOrDefault().Equals(caudience);
                        }
                    };
                });

            //注册Swagger生成器，定义一个和多个Swagger 文档
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "My API", Version = "v1" });

                c.AddSecurityDefinition("Bearer",
                    new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                    {
                        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                        Description = "请输入OAuth接口返回的Token，前置Bearer。示例：Bearer {Roken}",
                        Name = "Authorization",
                        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey
                    });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });
            });

            // http上下文 包含 jwt获取用户信息
            services.AddHttpContextAccessor();

            // 自定义用户认证
            services.AddAuthentication(options =>
            {
                options.AddScheme<MyAuthHandler>(MyAuthHandler.SchemeName, "token add in auth");
                options.DefaultAuthenticateScheme = MyAuthHandler.SchemeName;
                options.DefaultChallengeScheme = MyAuthHandler.SchemeName;
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            ServiceLocator.Instance = app.ApplicationServices;
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error");

                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }


            //启用中间件服务生成Swagger作为JSON终结点
            app.UseSwagger();
            //启用中间件服务对swagger-ui，指定Swagger JSON终结点
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                c.RoutePrefix = string.Empty;
            });

            // 这里必须用上。 jwt等使用他进行验证
            app.UseAuthentication();

            //app.UseMiddleware<HeadersOverrideMiddleware>();
            //var forwardingOptions = new ForwardedHeadersOptions()
            //{
            //    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            //};
            //forwardingOptions.KnownNetworks.Clear();
            //forwardingOptions.KnownProxies.Clear();
            //forwardingOptions.ForwardedHeaders = ForwardedHeaders.All;
            //app.UseForwardedHeaders(forwardingOptions);

            app.UseRouting();
            app.UseCors();

            //app.UseStaticFiles();

            //string dir = Path.Combine(AppContext.BaseDirectory, "local");
            //DirectoryInfo dirInfo;
            //if (!Directory.Exists(dir))
            //{
            //    dirInfo = Directory.CreateDirectory(dir);
            //}
            //else
            //{
            //    dirInfo = new DirectoryInfo(dir);
            //}

            //app.UseStaticFiles(new StaticFileOptions()
            //{
            //    ServeUnknownFileTypes = true,
            //    RequestPath = new PathString($"/LocalStorage"),
            //    FileProvider = new PhysicalFileProvider(dirInfo.FullName),
            //    OnPrepareResponse = ExtendMethod.HandleStaticFileResponse()
            //});

            //string tmpdir = Path.Combine(AppContext.BaseDirectory, "localtmp");
            //DirectoryInfo tmpdirInfo;
            //if (!Directory.Exists(tmpdir))
            //{
            //    tmpdirInfo = Directory.CreateDirectory(tmpdir);
            //}
            //else
            //{
            //    tmpdirInfo = new DirectoryInfo(tmpdir);
            //}
            //app.UseStaticFiles(new StaticFileOptions()
            //{
            //    ServeUnknownFileTypes = true,
            //    RequestPath = new PathString($"/LocalStoragetmp"),
            //    FileProvider = new TemporaryLocalFileProvider(tmpdirInfo, dirInfo),
            //    OnPrepareResponse = ExtendMethod.HandleStaticFileResponse()
            //});

            app.UseSession();
            //app.UseSignalR(route =>
            //{
            //    AppHub.Register(route);
            //    PaymentRequestHub.Register(route);
            //});
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<TestHub>("/path");
            });

            app.UseWebSockets();
            app.UseStatusCodePages();

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
            });

            app.UseMvc();
            app.UseResponseCompression();
            app.UseAuthorization();
        }
    }
}
