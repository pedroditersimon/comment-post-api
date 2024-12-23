﻿using CommentPost.Application.Repositories;
using CommentPost.Application.Services;
using CommentPost.Infrastructure.Configurations;
using CommentPost.Infrastructure.EntityFramework;
using CommentPost.Infrastructure.EntityFramework.Repositories;
using CommentPost.Infrastructure.Services;
using CommentPost.Infrastructure.Services.Auth;
using Microsoft.Extensions.Options;

namespace CommentPost.API.Extensions;

public static class ServiceExtensions
{
	public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
	{
		// Repositories
		services.AddScoped<IUserRepository, UserRepository>();
		services.AddScoped<ICommentRepository, CommentRepository>();

		// unit of work
		services.AddScoped<IUnitOfWork, UnitOfWork>();

		// services
		services.AddScoped<IUserService, UserService>();
		services.AddScoped<ICommentService, CommentService>();

		services.AddScoped<JwtService>(provider =>
		{
			JwtSettings settings = provider.GetRequiredService<IOptions<JwtSettings>>().Value;
			return new JwtService(settings);
		});

		services.AddScoped<AuthService>();

		services.AddScoped<LocalAuthService>();

		services.AddScoped<Auth0Service>(provider =>
		{
			Auth0Settings settings = provider.GetRequiredService<IOptions<Auth0Settings>>().Value;
			IHttpClientFactory clientFactory = provider.GetRequiredService<IHttpClientFactory>();

			IUserService userService = provider.GetRequiredService<IUserService>();
			JwtService jwtService = provider.GetRequiredService<JwtService>();
			AuthService authService = provider.GetRequiredService<AuthService>();

			return new Auth0Service(clientFactory, settings, userService, jwtService, authService);
		});

		return services;
	}
}
