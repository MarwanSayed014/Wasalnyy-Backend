namespace EmployeeCrud.BLL.Common
{
    public static class ModularBussinessLogicLayer
    {
        public static IServiceCollection AddBussinessInPL(this IServiceCollection services, IConfiguration configuration)
        {
			services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
			var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();
			var key = Encoding.UTF8.GetBytes(jwtSettings!.Key);
			services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
			.AddJwtBearer(options =>
			{
				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidateLifetime = true,
					ValidateIssuerSigningKey = true,
					ValidIssuer = jwtSettings.Issuer,
					ValidAudience = jwtSettings.Audience,
					IssuerSigningKey = new SymmetricSecurityKey(key)
				};
			});
			services.AddScoped<JwtHandler>();
			services.AddScoped<IAuthService, AuthService>();
			return services;
        }
    }
}
