using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using PaymentGateway.Domain;
using PaymentGateway.ApplicationService.Models;
using PaymentGateway.ApplicationService.PaymentProcess.Services;
using PaymentGateway.ApplicationService.PaymentHistory.Services;

namespace PaymentGateway.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<PaymentGatewayDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("PaymentGatewayDbContext") ?? throw new InvalidOperationException("Connection string 'PaymentGatewayDbContext' not found.")));

            // Add services to the container.
            builder.Services.AddOptions();
            builder.Services.Configure<AcquiringBankSettings>(builder.Configuration.GetSection("AcquiringBankSettings").Bind);

            builder.Services.AddScoped<IPaymentProcessService, PaymentProcessService>();
            builder.Services.AddScoped<IPaymentHistoryService, PaymentHistoryService>();

            builder.Services.AddControllers().AddJsonOptions(opt =>
            {
                opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();
            app.UseRouting();
            app.MapControllers();

            app.Run();
        }
    }
}